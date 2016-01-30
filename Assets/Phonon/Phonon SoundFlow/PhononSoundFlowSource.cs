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
using System.Threading;
using UnityEngine;


namespace Phonon
{

	[AddComponentMenu("Phonon/Phonon SoundFlow Source")]
	public class PhononSoundFlowSource : MonoBehaviour
	{

		void Awake()
		{
            updateStarted = false;
            updateComplete = false;

            audioEngine = AudioEngineComponent.GetAudioEngine();

            if (audioEngine == AudioEngine.Unity)
            {
                listenerObject = GameObject.FindObjectOfType<AudioListener>();

                int frameSize = 0, numBuffers = 0;
                AudioSettings.GetDSPBufferSize(out frameSize, out numBuffers);
                SoundFlow.iplCreateSoundFlowContext((uint)AudioSettings.outputSampleRate, (uint)frameSize, (uint)2, ref context);
                SoundFlow.iplCreateSoundFlowRenderer(context, 4, ref sfRenderer);

                monoAudio = new float[frameSize];
                
                effectEnabled = true;
            }

			AcousticSceneExporter.ExportScene();

            if (!bakedDataLoaded)
            {
                Grid = GameObject.FindObjectOfType<AcousticGrid>();
                if (Grid == null)
                    return;
                grid = Grid.LoadGrid();
                BakedData = Grid.gameObject.GetComponent<PhononSoundFlowBakedData>();
                if (BakedData == null)
                    return;

                PhononSoundFlowSource[] sfSources = GameObject.FindObjectsOfType<PhononSoundFlowSource>();
                if (sfSources == null)
                    return;
                sources = new Vector3[sfSources.Length];
                for (int i = 0; i < sources.Length; ++i)
                {
                    sources[i] = Common.ConvertVector(sfSources[i].transform.position);
                }

                BakedData.LoadBakedSoundFlow(ref baker, sources, AcousticSceneExporter.Scene, grid);
                bakedDataLoaded = true;
            }

            IPLSoundFlowSettings settings;
            settings.rays = SoundFlowGlobalSettings.GetBakeSettings().Value.Rays;
            settings.bounces = SoundFlowGlobalSettings.GetBakeSettings().Value.Bounces;
            SoundFlow.iplLoadSoundFlow(AcousticSceneExporter.Scene, grid, settings, sources.Length, sources, BakedData.Data, ref baker);

            simulateThread = new Thread(new ThreadStart(this.SimulateThread));
            simulateThread.Start();
		}
		
		void OnDestroy()
		{
            simulateThread.Abort();
            simulateThread.Join();

			effectEnabled = false;
            SoundFlow.iplDestroySoundFlowRenderer(ref sfRenderer);
            SoundFlow.iplDestroySoundFlowContext(ref context);
            SoundFlow.iplDestroySoundFlowBaker(ref baker);
            AcousticSceneExporter.Destroy();
		}

		void Update()
		{
			if (!effectEnabled)
				return;

			sourcePosition = Common.ConvertVector(transform.position);				
			listenerPosition = Common.ConvertVector(listenerObject.transform.position);
			listenerAhead = Common.ConvertVector(listenerObject.transform.forward);
			listenerUp = Common.ConvertVector(listenerObject.transform.up);
            updateStarted = true;

            simulateThread.Interrupt();
		}

		void SimulateThread()
		{
			while (true)
			{
				try
				{
                    if (baker != IntPtr.Zero && BakedSourceIndex >= 0)
                    {
                        IntPtr soundFlow = SoundFlow.iplGetBakedSoundFlow(baker, BakedSourceIndex, listenerPosition);
                        IntPtr directPath = SoundFlow.iplGetDirectSoundPath(baker, BakedSourceIndex, sourcePosition, listenerPosition);
                        SoundFlow.iplUpdateSoundFlow(sfRenderer, soundFlow, directPath, listenerAhead, listenerUp);
                        updateComplete = true;
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

		void OnAudioFilterRead(float[] data, int channels)
		{
            if (data == null)
                return;

			if (!effectEnabled || !updateStarted || !updateComplete)
            {
                Array.Clear(data, 0, data.Length);
                return;
            }

            int index = 0;
            for (int i = 0; i < monoAudio.Length; ++i)
            {
                monoAudio[i] = 0.0f;
                for (int j = 0; j < channels; ++j)
                {
                    monoAudio[i] += data[index];
                    ++index;
                }
                monoAudio[i] /= channels;
            }

            if (baker != IntPtr.Zero)
                SoundFlow.iplProcessSoundFlow(sfRenderer, monoAudio, data);
		}

        [HideInInspector]
        public int BakedSourceIndex = -1;

        bool effectEnabled = false;
        AudioEngine audioEngine = AudioEngine.Unity;
        IntPtr baker = IntPtr.Zero;
        IntPtr context = IntPtr.Zero;
        IntPtr sfRenderer = IntPtr.Zero;
        Vector3 sourcePosition;
        Vector3 listenerPosition;
        Vector3 listenerAhead;
        Vector3 listenerUp;
        AudioListener listenerObject = null;
        Thread simulateThread = null;
        float[] monoAudio = null;
        bool updateStarted = false;
        bool updateComplete = false;

        static bool bakedDataLoaded = false;
        static AcousticGrid Grid = null;
        static IntPtr grid = IntPtr.Zero;
        static PhononSoundFlowBakedData BakedData = null;
        static Vector3[] sources = null;

	}

}