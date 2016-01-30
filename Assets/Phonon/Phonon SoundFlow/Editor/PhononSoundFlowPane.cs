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
using System.Runtime.InteropServices;
using System.Threading;

using UnityEditor;
using UnityEngine;


namespace Phonon
{

	public enum SoundFlowBakeStatus
	{
		Ready,
        Preprocessing,
		InProgress,
		Complete
	}


	public class PhononSoundFlowPane
	{
		public static void DrawPane()
		{
            if (targetObject == null || editor == null)
            {
                targetObject = SoundFlowGlobalSettings.GetObject();
                editor = Editor.CreateEditor(targetObject.GetComponent<SoundFlowBakeSettings>());
            }

            GUIEnabled = (Status == SoundFlowBakeStatus.Ready && !EditorApplication.isPlayingOrWillChangePlaymode);
            bool guiWasEnabled = GUI.enabled;
            GUI.enabled = GUIEnabled;

            editor.OnInspectorGUI();

			Phonon.PhononGUI.SectionHeader("Bake SoundFlow");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(" ");
			if (GUILayout.Button("Bake"))
			{
                if (GameObject.FindObjectsOfType<AcousticGeometry>().Length == 0)
                    EditorUtility.DisplayDialog("Phonon SoundFlow", "No GameObjects have been marked as Phonon Geometry, so baking cannot continue. Please add Phonon Geometry components to one or more objects and try again.", "OK");
                else if (GameObject.FindObjectOfType<AcousticGrid>() == null)
                    EditorUtility.DisplayDialog("Phonon SoundFlow", "An Acoustic Grid has not been created. Please create an Acoustic Grid first and try again.", "OK");
                else if (GameObject.FindObjectsOfType<PhononSoundFlowSource>() == null)
                    EditorUtility.DisplayDialog("Phonon SoundFlow", "No AudioSources have been marked as Phonon SoundFlow Sources, so baking cannot continue. Please add Phonon SoundFlow Source components to one or more AudioSource objects and try again.", "OK");
                else
                    BeginBake();
			}
			EditorGUILayout.EndHorizontal();

			GUI.enabled = guiWasEnabled;

			DisplayProgressBar();

			if (Status == SoundFlowBakeStatus.Complete)
				EndBake();
		}

		static void DisplayProgressBar()
		{
			if (Status != SoundFlowBakeStatus.InProgress && Status != SoundFlowBakeStatus.Preprocessing)
				return;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(" ");
            if (Status == SoundFlowBakeStatus.Preprocessing)
			    EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), PreprocessProgress, "Baking (stage 1 of 2): " + Mathf.FloorToInt(PreprocessProgress * 100.0f).ToString() + "% complete");
            else
                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), BakeProgress, "Baking (stage 2 of 2): " + Mathf.FloorToInt(BakeProgress * 100.0f).ToString() + "% complete");
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(" ");
			if (GUILayout.Button("Cancel"))
			{
                SoundFlow.iplCancelSoundFlowBake(Baker);
				BakeCanceled = true;
				Status = SoundFlowBakeStatus.Complete;
			}
			EditorGUILayout.EndHorizontal();
		}

		static void BeginBake()
		{
			Status = SoundFlowBakeStatus.Preprocessing;

			AcousticSceneExporter.ExportScene();
			Grid = GameObject.FindObjectOfType<AcousticGrid>().LoadGrid();

            bakeSettings = new IPLSoundFlowSettings();
            bakeSettings.rays = SoundFlowGlobalSettings.GetBakeSettings().Value.Rays;
            bakeSettings.bounces = SoundFlowGlobalSettings.GetBakeSettings().Value.Bounces;

            bakeReflection = SoundFlowGlobalSettings.GetBakeSettings().BakeReflection;
            bakeDiffraction = SoundFlowGlobalSettings.GetBakeSettings().BakeDiffraction;

            if (!bakeDiffraction)
                Status = SoundFlowBakeStatus.InProgress;

			PhononSoundFlowSource[] sourceObjects = GameObject.FindObjectsOfType<PhononSoundFlowSource>();
			SourcePositions = new Vector3[sourceObjects.Length];
            for (int i = 0; i < sourceObjects.Length; ++i)
            {
                SourcePositions[i] = Common.ConvertVector(sourceObjects[i].transform.position);
                sourceObjects[i].BakedSourceIndex = i;
            }

            PreprocessCallbackDelegate = new SoundFlowPreprocessCallback(AdvancePreprocessProgress);
            BakeCallbackDelegate = new SoundFlowBakeCallback(AdvanceProgress);
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            PreprocessCallbackPointer = Marshal.GetFunctionPointerForDelegate(PreprocessCallbackDelegate);
            PreprocessCallbackHandle = GCHandle.Alloc(PreprocessCallbackPointer);
            GC.Collect();

			BakeCallbackPointer = Marshal.GetFunctionPointerForDelegate(BakeCallbackDelegate);
			BakeCallbackHandle = GCHandle.Alloc(BakeCallbackPointer);
			GC.Collect();
#endif

			ThreadHandle = new Thread(new ThreadStart(BakeThread));
			ThreadHandle.Start();
		}

		static void EndBake()
		{
			ThreadHandle.Join();

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (PreprocessCallbackHandle.IsAllocated)
                PreprocessCallbackHandle.Free();
			if (BakeCallbackHandle.IsAllocated)
				BakeCallbackHandle.Free();
#endif

			AcousticGrid acousticGrid = GameObject.FindObjectOfType<AcousticGrid>();

			if (BakeCanceled)
			{
				BakeCanceled = false;
			}
			else
			{
				PhononSoundFlowBakedData bakedData = acousticGrid.gameObject.GetComponent<PhononSoundFlowBakedData>();
				if (bakedData == null)
					bakedData = acousticGrid.gameObject.AddComponent<PhononSoundFlowBakedData>();
				bakedData.SaveBakedSoundFlow(Baker);
                SoundFlow.iplDestroySoundFlowBaker(ref Baker);
			}

			acousticGrid.UnloadGrid();
			AcousticSceneExporter.Destroy();

			Status = SoundFlowBakeStatus.Ready;
		}

		static void BakeThread()
		{
            SoundFlow.iplCreateSoundFlowBaker(AcousticSceneExporter.Scene, Grid, bakeSettings, (bakeReflection) ? Bool.TRUE : Bool.FALSE, (bakeDiffraction) ? Bool.TRUE : Bool.FALSE, SourcePositions.Length, SourcePositions, PreprocessCallbackDelegate, ref Baker);
            SoundFlow.iplBakeSoundFlow(Baker, BakeCallbackDelegate);
		}

        static void AdvancePreprocessProgress(Int64 processedVoxels, Int64 totalVoxels)
        {
            PreprocessProgress = (float)processedVoxels / (float)totalVoxels;
            if (processedVoxels >= totalVoxels)
                Status = SoundFlowBakeStatus.InProgress;
        }

		static void AdvanceProgress(int processedSources, int totalSources, int processedPoints, int totalPoints)
		{
			int numComplete = processedSources * totalPoints + processedPoints;
			int numTotal = totalSources * totalPoints - 1;
			BakeProgress = (float) numComplete / (float) numTotal;

			if (numComplete >= numTotal)
				Status = SoundFlowBakeStatus.Complete;
		}

        public static bool GUIEnabled = true;

		static SoundFlowBakeStatus		Status 					= SoundFlowBakeStatus.Ready;
		static float					BakeProgress			= 0.0f;
		static bool						BakeCanceled			= false;
		static IntPtr					Grid					= IntPtr.Zero;
		static Thread					ThreadHandle			= null;
		static IntPtr					BakeCallbackPointer		= IntPtr.Zero;
		static GCHandle					BakeCallbackHandle;
		static SoundFlowBakeCallback	BakeCallbackDelegate;
		static Phonon.Vector3[]			SourcePositions			= null;
        static IntPtr                   Baker                   = IntPtr.Zero;

        static GameObject               targetObject            = null;
        static Editor                   editor                  = null;
        static bool                     bakeReflection;
        static bool                     bakeDiffraction;
        static IPLSoundFlowSettings     bakeSettings;

        static SoundFlowPreprocessCallback PreprocessCallbackDelegate;
        static IntPtr PreprocessCallbackPointer = IntPtr.Zero;
        static GCHandle PreprocessCallbackHandle;
        static float PreprocessProgress = 0.0f;
    }

}