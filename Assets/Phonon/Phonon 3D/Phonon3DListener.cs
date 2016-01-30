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
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Phonon;


//
// Phonon3DListener
// Represents a binaural listener and its HRTF.
//

[AddComponentMenu("Phonon/Phonon 3D Listener")]
public class Phonon3DListener : MonoBehaviour
{
	//
	// Initializes the listener.
	//
	void Awake()
	{
		if (effectEnabled)
			return;

        // Construct the full path to the HRTF file.
#if UNITY_ANDROID && !UNITY_EDITOR
        string hrtfAssetFile = Path.Combine(Application.streamingAssetsPath, Phonon3DListener.hrtfFileName);
        Debug.Log(hrtfAssetFile);
        WWW streamingAssetLoader = new WWW(hrtfAssetFile);
        while (!streamingAssetLoader.isDone) ;
        byte[] assetData = streamingAssetLoader.bytes;
        string hrtfPath = Path.Combine(Application.temporaryCachePath, Phonon3DListener.hrtfFileName);
        try
        {
            using (BinaryWriter dataWriter = new BinaryWriter(new FileStream(hrtfPath, FileMode.Create)))
            {
                dataWriter.Write(assetData);
                dataWriter.Close();
            }
        }
        catch (IOException)
        {
            Debug.Log("HRTF file already opened once. Ignoring.");
        }
#else
        string hrtfPath = Path.Combine(Application.streamingAssetsPath, hrtfFileName);
#endif
		
		// Copy the listener settings.
		ListenerSettings listenerSettings;
		listenerSettings.maxSources = MaxSources;
		listenerSettings.maxDistance = MaxDistance;
		listenerSettings.minAttenuation = 0.02f;

		if (AudioEngineComponent.GetAudioEngine() == AudioEngine.Unity)
		{
			int numBuffers;
			AudioSettings.GetDSPBufferSize(out frameSize, out numBuffers);
            if (Phonon3D.iplCreate3DContext((uint) AudioSettings.outputSampleRate, (uint) frameSize, (uint) 2, hrtfPath, listenerSettings, ref context) != Error.NONE)
			{
				Debug.Log("Unable to create Phonon 3D Context. Please check the log file for details.");
				return;
			}
		}
		else if (AudioEngineComponent.GetAudioEngine() == AudioEngine.Unity5)
		{
			int numBuffers;
			AudioSettings.GetDSPBufferSize(out frameSize, out numBuffers);
			Phonon3DUnity5.iplUnity5InitializePhonon3D(AudioSettings.outputSampleRate, frameSize, hrtfPath, listenerSettings);
		}
		else if (AudioEngineComponent.GetAudioEngine() == AudioEngine.FMODStudio)
		{
			Phonon3DFMOD.iplFMODInitialize(hrtfPath, listenerSettings);
		}
        else if (AudioEngineComponent.GetAudioEngine() == AudioEngine.Wwise)
        {
            Phonon3DWwise.iplWwiseCreateListener(hrtfPath, listenerSettings);
        }

		audioEngine = AudioEngineComponent.GetAudioEngine();

		effectEnabled = true;
	}
	
	//
	// Destroys the listener.
	//
	void OnDestroy()
	{
		if (!effectEnabled)
			return;
		
		effectEnabled = false;
		
		if (audioEngine == AudioEngine.Unity)
		{
            Phonon3D.iplDestroy3DContext(ref context);
		}
	}
	
	//
	// Updates the listener position and orientation.
	//
	void Update()
	{
		if (!effectEnabled)
			return;

		Phonon.Vector3 zero;
		zero.x = zero.y = zero.z = 0;
		
		if (audioEngine == AudioEngine.Unity)
		{
			Phonon3D.iplUpdateListener(context, zero, Common.ConvertVector(transform.forward), Common.ConvertVector(transform.up));
		}
		else if (audioEngine == AudioEngine.Unity5)
		{
			Phonon3DUnity5.iplUnity5UpdateListener();
		}
		else if (audioEngine == AudioEngine.FMODStudio)
		{
			Phonon3DFMOD.iplFMODUpdateListener();
		}
	}
	
	//
	// Returns the frame size.
	//
	public static int GetFrameSize()
	{
		return frameSize;
	}
	
	public static bool IsEffectEnabled()
	{
		return effectEnabled;
	}
	
	//
	// Data members.
	//
	
	// Is the effect enabled?
	static bool effectEnabled = false;
	
	// Default HRTF file name.
	public static string hrtfFileName = "cipic_124.hrtf";
	
	// Audio pipeline settings.
	static int frameSize = 0;
	
	// Audio Engine.
	Phonon.AudioEngine audioEngine;

    IntPtr context = IntPtr.Zero;
	
	//
	// Public properties.
	//
	
	[Range(1, 64)]
	public int MaxSources = 32;
	
	[Range(0.0f, 500.0f)]
	public float MaxDistance = 100.0f;
}