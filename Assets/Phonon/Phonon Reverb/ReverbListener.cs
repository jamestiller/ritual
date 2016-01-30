/************************************************************************/
/* Copyright (C) 2011-2015 Impulsonic Inc. All Rights Reserved.         */
/*                                                                      */
/* The source code, information  and  material ("Material") contained   */
/* herein is owned  by Impulsonic Inc. or its suppliers or licensors,   */
/* and title to such  Material remains  with Impulsonic  Inc.  or its   */
/* suppliers or licensors. The Material contains proprietary informa-   */
/* tion  of  Impulsonic or  its  suppliers and licensors. No  part of   */
/* the Material may be used, copied, reproduced, modified, published,   */
/* uploaded, posted, transmitted, distributed or disclosed in any way   */
/* without Impulsonic's prior express written permission. No  license   */
/* under  any patent, copyright or other intellectual property rights   */
/* in the Material is  granted  to  or  conferred  upon  you,  either   */
/* expressly, by implication, inducement, estoppel or otherwise.  Any   */
/* license  under  such intellectual property rights must  be express   */
/* and approved by Impulsonic in writing.                               */
/*                                                                      */
/* Third Party trademarks are the property of their respective owners.  */
/*                                                                      */
/* Unless otherwise  agreed upon by Impulsonic  in  writing, you  may   */
/* not remove or  alter this  notice or any other  notice embedded in   */
/* Materials by Impulsonic or Impulsonic's  suppliers or licensors in   */
/* any way.                                                             */
/************************************************************************/

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using UnityEngine;


//
// PhononReverbListener
// Component that interpolates baked reverb or calculates real-time reverb.
//

[AddComponentMenu("Phonon/Phonon Reverb Listener")]
public class ReverbListener : MonoBehaviour
{
    //
    // Exports the scene, initializes the simulator, and
    // launches the real-time thread.
    //
    void Awake()
    {
        // Export the scene.
        AcousticSceneExporter.ExportScene();

		// Export the grid.
		Grid = GameObject.FindObjectOfType<AcousticGrid>();
		if (Grid != null)
		{
			grid = Grid.LoadGrid();

			BakedData = Grid.gameObject.GetComponent<ReverbBakedData>();
			if (BakedData != null)
				BakedData.LoadBakedReverb(grid);
		}

        ReverbZones = GameObject.FindObjectsOfType<ReverbZone>();

        // Initialize the propagator.
        InitializePropagator();

        // Initialize the runtime.
        InitializeRuntime();

		audioEngine = Phonon.AudioEngineComponent.GetAudioEngine();

        // Launch the simulation thread.
        thread = new Thread(new ThreadStart(this.SimulateReverb));
        thread.Start();

		effectEnabled = true;
    }

    //
    // Initializes the propagator.
    //
    void InitializePropagator()
    {
		Phonon.ReverbSettings settings = new Phonon.ReverbSettings();
		settings.rays = ReverbGlobalSettings.GetBakeSettings().Value.Rays;
		settings.bounces = ReverbGlobalSettings.GetBakeSettings().Value.Bounces;
		settings.samplingRate = AudioSettings.outputSampleRate;
		settings.duration = ReverbGlobalSettings.GetBakeSettings().Value.Duration;

		Phonon.Reverb.iplCreateReverbEstimator(AcousticSceneExporter.Scene, settings, (int) (Phonon.ReverbType.Parametric) | (int) (Phonon.ReverbType.Convolution), ref estimator);
    }

    //
    // Initializes the runtime.
    //
    void InitializeRuntime()
    {
		Phonon.ReverbSettings settings = new Phonon.ReverbSettings();
		settings.rays = ReverbGlobalSettings.GetBakeSettings().Value.Rays;
		settings.bounces = ReverbGlobalSettings.GetBakeSettings().Value.Bounces;
		settings.samplingRate = AudioSettings.outputSampleRate;
		settings.duration = ReverbGlobalSettings.GetBakeSettings().Value.Duration;

        if (Phonon.AudioEngineComponent.GetAudioEngine() == Phonon.AudioEngine.Unity)
        {
            int frameSize, numBuffers;
            AudioSettings.GetDSPBufferSize(out frameSize, out numBuffers);
            Phonon.Reverb.iplCreateReverbContext((uint) AudioSettings.outputSampleRate, (uint) frameSize, (uint) 2, settings.duration, ref context);
            Phonon.Reverb.iplCreateReverbRenderer(context, ref convolutionRenderer);
        }

        // Add a reverb filter if needed.
		if (Phonon.AudioEngineComponent.GetAudioEngine() == Phonon.AudioEngine.Unity)
		{
            reverbFilter = gameObject.GetComponent<AudioReverbFilter>();
	        if (reverbFilter == null)
	            reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
	        reverbFilter.reverbPreset = AudioReverbPreset.User;
		}

		if (Phonon.AudioEngineComponent.GetAudioEngine() == Phonon.AudioEngine.Wwise)
		{
			Phonon.ReverbWwise.iplWwiseSetReverbSettings(settings);
		}
		else if (Phonon.AudioEngineComponent.GetAudioEngine() == Phonon.AudioEngine.FMODStudio)
		{
            Phonon.ReverbFMODStudio.iplFMODInitialize(settings);
		}
    }

    //
    // Cleans up resources.
    //
    void OnDestroy()
    {
        // Don't do anything if the effect was never enabled.
        if (!effectEnabled)
            return;

		effectEnabled = false;

		// Terminate the simulation thread.
        thread.Abort();
        thread.Join();

        // Destroy the runtime.
        DestroyRuntime();

        // Destroy the propagator.
        DestroyPropagator();

        // Shut down the effect.
		Phonon.Reverb.iplUnloadBakedReverb();
		Grid.UnloadGrid();
        AcousticSceneExporter.Destroy();
    }

    //
    // Destroys the runtime.
    //
    void DestroyRuntime()
    {
        if (audioEngine == Phonon.AudioEngine.Unity)
        {
            Phonon.Reverb.iplDestroyReverbRenderer(ref convolutionRenderer);
            Phonon.Reverb.iplDestroyReverbContext(ref context);
        }
    }

    //
    // Destroys the propagator.
    //
    void DestroyPropagator()
    {
		Phonon.Reverb.iplDestroyReverbEstimator(estimator);
    }

    ReverbZone CurrentReverbZone()
    {
        ReverbZone zone = null;

        if (ReverbZones == null)
            return zone;

        for (int i = 0; i < ReverbZones.Length; ++i)
        {
            if (ReverbZones[i].Contains(gameObject.transform.position))
            {
                zone = ReverbZones[i];
                break;
            }
        }

        return zone;
    }

    //
    // Per-frame update. Makes sure the correct mode
    // is used, and performs interpolation in baked mode.
    //
    void Update()
    {
        vListener = Phonon.Common.ConvertVector(gameObject.transform.position);

        if (Mode == ReverbMode.Off)
        {
        }
        else if (Mode == ReverbMode.Baked)
        {
			ReverbZone currentZone = CurrentReverbZone();

			if (Type == Phonon.ReverbType.Parametric && currentZone != null)
			{
				parametricParams.room = currentZone.ParametricReverbOverride.Room;
				parametricParams.roomHigh = currentZone.ParametricReverbOverride.RoomHigh;
				parametricParams.roomLow = currentZone.ParametricReverbOverride.RoomLow;
				parametricParams.decayTime = currentZone.ParametricReverbOverride.DecayTime;
				parametricParams.decayTimeHighRatio = currentZone.ParametricReverbOverride.DecayHighRatio;
				parametricParams.reflections = currentZone.ParametricReverbOverride.Reflections;
				parametricParams.reflectionsDelay = currentZone.ParametricReverbOverride.ReflectionsDelay;
				parametricParams.reverb = currentZone.ParametricReverbOverride.Reverb;
				parametricParams.reverbDelay = currentZone.ParametricReverbOverride.ReverbDelay;
				parametricParams.hfReference = currentZone.ParametricReverbOverride.HFReference;
				parametricParams.lfReference = currentZone.ParametricReverbOverride.LFReference;
				parametricParams.roomRolloff = currentZone.ParametricReverbOverride.RoomRolloff;
				parametricParams.diffusion = currentZone.ParametricReverbOverride.Diffusion;
				parametricParams.density = currentZone.ParametricReverbOverride.Density;
			}
			else if (Type == Phonon.ReverbType.Convolution && currentZone != null)
			{
				float[] zoneImpulseResponse = currentZone.ConvolutionReverbOverride.GetImpulseResponse();
				if (zoneImpulseResponse != null)
				{
					if (audioEngine == Phonon.AudioEngine.Unity)
						Phonon.Reverb.iplUpdateReverbRaw(convolutionRenderer, zoneImpulseResponse);
					else if (audioEngine == Phonon.AudioEngine.Wwise)
						Phonon.ReverbWwise.iplWwiseSetImpulseResponseRaw(zoneImpulseResponse);
					else if (audioEngine == Phonon.AudioEngine.FMODStudio)
						Phonon.ReverbFMODStudio.iplFMODSetImpulseResponseRaw(zoneImpulseResponse);
				}
			}
			else
			{
				if (Grid != null && BakedData != null && grid != IntPtr.Zero)
					thread.Interrupt();
			}
        }
        else if (Mode == ReverbMode.Preview)
        {
			if (updateReverbPreview)
			{
				thread.Interrupt();
				updateReverbPreview = false;
			}
        }

        if (Mode != ReverbMode.Off && Type == Phonon.ReverbType.Parametric)
        {
			if (audioEngine == Phonon.AudioEngine.Unity)
			{
				reverbFilter.enabled = parametricReverbEnabled;
				reverbFilter.reverbPreset = AudioReverbPreset.User;
				reverbFilter.room = parametricParams.room;
				reverbFilter.roomHF = parametricParams.roomHigh;
				reverbFilter.roomLF = parametricParams.roomLow;
				reverbFilter.decayTime = parametricParams.decayTime;
				reverbFilter.decayHFRatio = parametricParams.decayTimeHighRatio;
				reverbFilter.reflectionsLevel = parametricParams.reflections;
				reverbFilter.reflectionsDelay = parametricParams.reflectionsDelay;
				reverbFilter.reverbLevel = parametricParams.reverb;
				reverbFilter.reverbDelay = parametricParams.reverbDelay;
				reverbFilter.hfReference = parametricParams.hfReference;
#if UNITY_5
				reverbFilter.lfReference = parametricParams.lfReference;
#else
				reverbFilter.lFReference = parametricParams.lfReference;
#endif
                reverbFilter.roomRolloff = parametricParams.roomRolloff;
                reverbFilter.dryLevel = ParametricDryLevel;
            }
			else if (audioEngine == Phonon.AudioEngine.Wwise)
			{
				if (parametricReverbEnabled || Mode == ReverbMode.Preview)
				{
					// Phonon Reverb's Wwise integration does not support reflections level
					// and reverb level. So, if the reverb level is too low, set the decay
					// time to minimum.
					if (parametricParams.reverb <= -1000.0f)
                    	parametricParams.decayTime = 0.1f;

					float rtpcDecayTime = RTPCValue(parametricParams.decayTime, 0.2f, 10.0f);
					float rtpcDecayTimeHF = RTPCValue(1.0f / parametricParams.decayTimeHighRatio, 0.5f, 10.0f);

                    System.Type type = System.Type.GetType("AkSoundEngine");
                    if (type != null)
                    {
                        Type[] paramTypes = new Type[2];
                        paramTypes[0] = typeof(string);
                        paramTypes[1] = typeof(float);

                        MethodInfo method = type.GetMethod("SetRTPCValue", paramTypes);
                        if (method != null)
                        {
                            object[] parameters = new object[2];
                            
                            parameters[0] = DecayTimeRTPC;
                            parameters[1] = rtpcDecayTime;
                            method.Invoke(null, parameters);

                            parameters[0] = DecayTimeHFRatioRTPC;
                            parameters[1] = rtpcDecayTimeHF;
                            method.Invoke(null, parameters);
                        }
                    }
				}
			}
            else if (audioEngine == Phonon.AudioEngine.FMODStudio)
            {
                Debug.LogWarning("Phonon Reverb Parametric Mode is not supported for FMOD Studio.");
            }
        }
        else
        {
            if (audioEngine == Phonon.AudioEngine.Unity)
                reverbFilter.enabled = (Mode == ReverbMode.Off && reverbEnabledWhenOff);
        }
    }

	//
	// Converts an absolute parameter value to an RTPC percentage value.
	//
	float RTPCValue(float x, float xMin, float xMax)
	{
		return 100.0f * ((x - xMin) / (xMax - xMin));
	}

	//
	// Updates the real-time preview.
	//
	public void UpdatePreview()
	{
		if (Mode != ReverbMode.Preview)
			return;

		updateReverbPreview = true;
	}

    //
    // Specifies whether the parametric reverb filter should be left on
    // when Phonon Reverb is off.
    //
    public void SetReverbEnabledWhenOff(bool enabled)
    {
        reverbEnabledWhenOff = enabled;
    }

    //
    // Simulation thread for real-time mode.
    //
    void SimulateReverb()
    {
        while (true)
        {
            try
            {
                if (Mode == ReverbMode.Baked && Type == Phonon.ReverbType.Parametric)
                {
					IntPtr parametricReverb = Phonon.Reverb.iplGetBakedReverb(grid, Phonon.ReverbType.Parametric, vListener);

					if (parametricReverb != IntPtr.Zero)
						parametricParams = (Phonon.ParametricReverb) Marshal.PtrToStructure(parametricReverb, typeof(Phonon.ParametricReverb));
                }
                else if (Mode == ReverbMode.Baked && Type == Phonon.ReverbType.Convolution)
                {
					IntPtr convolutionReverb = Phonon.Reverb.iplGetBakedReverb(grid, Phonon.ReverbType.Convolution, vListener);

					if (convolutionReverb != IntPtr.Zero)
					{
						if (audioEngine == Phonon.AudioEngine.Unity)
							Phonon.Reverb.iplUpdateReverb(convolutionRenderer, convolutionReverb);
						else if (audioEngine == Phonon.AudioEngine.Wwise)
							Phonon.ReverbWwise.iplWwiseSetImpulseResponse(convolutionReverb);
						else if (audioEngine == Phonon.AudioEngine.FMODStudio)
							Phonon.ReverbFMODStudio.iplFMODSetImpulseResponse(convolutionReverb);
					}
                }
                else if (Mode == ReverbMode.Preview && Type == Phonon.ReverbType.Parametric)
                {
					IntPtr parametricReverb = Phonon.Reverb.iplEstimateReverb(estimator, Phonon.ReverbType.Parametric, vListener);
					parametricParams = (Phonon.ParametricReverb) Marshal.PtrToStructure(parametricReverb, typeof(Phonon.ParametricReverb));
					Debug.Log("Phonon Reverb: Preview updated.");
                }
                else if (Mode == ReverbMode.Preview && Type == Phonon.ReverbType.Convolution)
                {
					IntPtr convolutionReverb = Phonon.Reverb.iplEstimateReverb(estimator, Phonon.ReverbType.Convolution, vListener);
					if (audioEngine == Phonon.AudioEngine.Unity)
						Phonon.Reverb.iplUpdateReverb(convolutionRenderer, convolutionReverb);
					else if (audioEngine == Phonon.AudioEngine.Wwise)
						Phonon.ReverbWwise.iplWwiseSetImpulseResponse(convolutionReverb);
					else if (audioEngine == Phonon.AudioEngine.FMODStudio)
						Phonon.ReverbFMODStudio.iplFMODSetImpulseResponse(convolutionReverb);
                    Debug.Log("Phonon Reverb: Preview updated.");
				}

                Thread.Sleep(Timeout.Infinite);
            }
            catch (ThreadInterruptedException)
            {
            }
            catch (ThreadAbortException)
            {
                return;
            }
			catch (ArgumentException e)
			{
				Debug.LogException(e);
			}
        }
    }

    //
    // Applies convolution reverb if needed.
    //
    void OnAudioFilterRead(float[] data, int channels)
    {
		if (!effectEnabled || data == null || convolutionRenderer == IntPtr.Zero)
		{
			Array.Clear(data, 0, data.Length);
			return;
		}

		if (Mode != ReverbMode.Off && Type == Phonon.ReverbType.Convolution && !effectBypassed)
        {
            if (reverbBuffer == null)
                reverbBuffer = new float[data.Length];

            Phonon.Reverb.iplProcessReverb(convolutionRenderer, data, reverbBuffer);

            float sendLevel = Mathf.Pow(10.0f, ConvolutionSendLevel / 20.0f);
            for (int i = 0; i < data.Length; ++i)
                data[i] += sendLevel * reverbBuffer[i];
        }
    }

    //
    // Public properties.
    //

	public ReverbMode Mode = ReverbMode.Baked;
	public Phonon.ReverbType Type = Phonon.ReverbType.Convolution;

    [Range(-10000.0f, 0.0f)]
    public float ParametricDryLevel = 0.0f;

    [Range(-96.0f, 12.0f)]
    public float ConvolutionSendLevel = 0.0f;

	//
	// Data members.
	//

    // Temporary array for mixing dry audio.
    float[] reverbBuffer = null;

	// Is the effect enabled?
	bool effectEnabled = false;

	// Lets the simulation thread tell the main thread whether parametric reverb should be enabled.
	bool parametricReverbEnabled = true;

	// Is the effect being bypassed (because we're outside the grid)?
	bool effectBypassed = false;

	// The simulation thread.
	Thread thread = null;

	// The reverb filter.
	AudioReverbFilter reverbFilter = null;

	// Parametric parameters.
	Phonon.ParametricReverb parametricParams;

    // Controls whether the parametric reverb filter is active even when Phonon Reverb is off.
	bool reverbEnabledWhenOff = false;

	// Controls whether the reverb preview should update.
	bool updateReverbPreview = true;

	// Listener coordinates.
	Phonon.Vector3 vListener;

	// The acoustic grid.
	AcousticGrid Grid = null;
	ReverbBakedData BakedData = null;

	// Backend handles.
	IntPtr estimator = IntPtr.Zero;
    IntPtr context = IntPtr.Zero;
	IntPtr convolutionRenderer = IntPtr.Zero;
	IntPtr grid = IntPtr.Zero;

    ReverbZone[] ReverbZones = null;

	Phonon.AudioEngine audioEngine = Phonon.AudioEngine.Unity;

	public string DecayTimeRTPC;
	public string DecayTimeHFRatioRTPC;
}
