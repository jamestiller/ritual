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
using System.Runtime.InteropServices;
using System.Threading;

using UnityEditor;
using UnityEngine;


//
//	BakeStatus
//	Possible states the bake process can be in.
//
public enum ReverbBakeStatus
{
    Ready,
    InProgress,
    Complete
}


//
//	MainWindow
//	Main Phonon Reverb window.
//

public class PhononReverbPane
{
    //
    //	Draws the window.
    //
    public static void DrawPane()
    {
		if (targetObject == null || editor == null)
		{
			targetObject = ReverbGlobalSettings.GetObject();
			editor = Editor.CreateEditor(targetObject.GetComponent<ReverbBakeSettings>());
		}

        // Enable the GUI only if the bake process is not running or finalizing.
        GUIEnabled = (Status == ReverbBakeStatus.Ready && !EditorApplication.isPlayingOrWillChangePlaymode);
        bool guiWasEnabled = GUI.enabled;
        GUI.enabled = GUIEnabled;

		editor.OnInspectorGUI();

		Phonon.PhononGUI.SectionHeader("Bake Reverb");

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button("Bake"))
        {
            // If no objects have been marked static, stop.
            if (NoStaticObjects())
            {
                EditorUtility.DisplayDialog("Phonon Reverb", "No GameObjects have been marked as Phonon Geometry, so baking cannot continue. Please add Phonon Geometry components to one or more objects and try again.", "OK");
            }

            // If a grid doesn't exist, stop.
            else if (GameObject.FindObjectOfType<AcousticGrid>() == null)
            {
                EditorUtility.DisplayDialog("Phonon Reverb", "An Acoustic Grid has not been created. Please create an Acoustic Grid first and try again.", "OK");
            }

            else if (!ReverbGlobalSettings.GetBakeSettings().BakeParametricReverb && !ReverbGlobalSettings.GetBakeSettings().BakeConvolutionReverb)
            {
                EditorUtility.DisplayDialog("Phonon Reverb", "No Output Settings selected. Please select Bake Convolution Reverb or Bake Parametric Reverb.", "OK");
            }

            // Otherwise, if there is at least one unlocked zone, bake.
            else
            {
                BeginBake();
            }
        }
		EditorGUILayout.EndHorizontal();

        // Re-enable the GUI.
        GUI.enabled = guiWasEnabled;

        // Display a progress bar if necessary.
        DisplayProgressBar();

        // When baking completes, update all reverb zones.
        if (Status == ReverbBakeStatus.Complete)
        {
            EndBake();
        }
    }

    //
    //      Returns true if no GameObjects have been marked static.
    //
    static bool NoStaticObjects()
    {
        return (GameObject.FindObjectsOfType<AcousticGeometry>().Length == 0);
    }

    //
    //      Displays a progress bar when baking is in progress.
    //
    static void DisplayProgressBar()
    {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(" ");
        if (Status == ReverbBakeStatus.InProgress)
        {
            float progress = (float)(ZonesBaked) / (float)ZonesToBake;
            int progressPercent = Mathf.FloorToInt(progress * 100.0f);
            string progressString = "Baking reverb: " + progressPercent.ToString() + "% complete";
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), progress, progressString);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ");
            EditorGUILayout.Space();
            if (GUILayout.Button("Cancel"))
            {
                Phonon.Reverb.iplCancelReverbBake();
                BakeCanceled = true;
                Status = ReverbBakeStatus.Complete;
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), 0.0f, "");
            EditorGUILayout.EndHorizontal();
        }
    }

    //
    //      Begins the bake process.
    //
    static void BeginBake()
    {
        // Update the status.
        Status = ReverbBakeStatus.InProgress;

        // Update zone counters.
		ZonesBaked = 0;
		ZonesToBake = 1;

		AcousticSceneExporter.ExportScene();
        
		AcousticGrid acousticGrid = GameObject.FindObjectOfType<AcousticGrid>();
		bakeGrid = acousticGrid.LoadGrid();

		bakeSettings = new Phonon.ReverbSettings();
		bakeSettings.rays = ReverbGlobalSettings.GetBakeSettings().Value.Rays;
		bakeSettings.bounces = ReverbGlobalSettings.GetBakeSettings().Value.Bounces;
		bakeSettings.samplingRate = AudioSettings.outputSampleRate;
		bakeSettings.duration = ReverbGlobalSettings.GetBakeSettings().Value.Duration;

        bool bakeParametric = ReverbGlobalSettings.GetBakeSettings().BakeParametricReverb;
        bool bakeConvolution = ReverbGlobalSettings.GetBakeSettings().BakeConvolutionReverb;
        bakeTypeFlags = 0;
        if (bakeParametric)
            bakeTypeFlags |= (int)Phonon.ReverbType.Parametric;
        if (bakeConvolution)
            bakeTypeFlags |= (int)Phonon.ReverbType.Convolution;
        
        bakeCallback = new Phonon.BakeCallback(AdvanceProgress);
        
		// Start the actual baking process.
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
        bakeCallbackPointer = Marshal.GetFunctionPointerForDelegate(bakeCallback);
        bakeCallbackHandle = GCHandle.Alloc(bakeCallbackPointer);
        GC.Collect();
#endif
        bakeThread = new Thread(new ThreadStart(BakeThread));
		bakeThread.Start();
    }

	//
	// Bake thread function.
	//
	static void BakeThread()
	{
		Phonon.Reverb.iplBakeReverb(AcousticSceneExporter.Scene, bakeGrid, bakeSettings, bakeTypeFlags, bakeCallback);
    }

    //
    //      Advances the progress bar.
    //
    static void AdvanceProgress(int numProcessed, int numTotal)
    {
		ZonesBaked = numProcessed;
		ZonesToBake = numTotal;

        if (ZonesBaked >= ZonesToBake)
        {
            Status = ReverbBakeStatus.Complete;
        }
    }

    //
    //      Ends the bake process.
    //
    static void EndBake()
    {
        bakeThread.Join();
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        if (bakeCallbackHandle.IsAllocated)
            bakeCallbackHandle.Free();
#endif

		AcousticGrid acousticGrid = GameObject.FindObjectOfType<AcousticGrid>();

		if (BakeCanceled)
		{
			BakeCanceled = false;
		}
		else
		{
			ReverbBakedData bakedData = acousticGrid.gameObject.GetComponent<ReverbBakedData>();
			if (bakedData == null)
			{
				bakedData = acousticGrid.gameObject.AddComponent<ReverbBakedData>();
			}
			bakedData.SaveBakedReverb();

			Phonon.Reverb.iplUnloadBakedReverb();
		}

		acousticGrid.UnloadGrid();

		AcousticSceneExporter.Destroy();
        
        // Reset the status.
        Status = ReverbBakeStatus.Ready;
    }

    //
    //      Data members.
    //

	static GameObject targetObject = null;
	static Editor editor = null;

    // Progress and status.
    static ReverbBakeStatus Status = ReverbBakeStatus.Ready;
    static int ZonesBaked = 0;
    static int ZonesToBake = 0;
	static bool BakeCanceled = false;

    // GUI enable/disable flags.
    public static bool GUIEnabled = true;

	private static IntPtr bakeGrid;
	private static Phonon.ReverbSettings bakeSettings;
	private static int bakeTypeFlags;
	private static Phonon.BakeCallback bakeCallback;
	private static Thread bakeThread;

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
    private static IntPtr bakeCallbackPointer;
    private static GCHandle bakeCallbackHandle;
#endif
}