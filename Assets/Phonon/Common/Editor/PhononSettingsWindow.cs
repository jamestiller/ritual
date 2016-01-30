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
using System.Reflection;
using System.Threading;

using UnityEngine;
using UnityEditor;


namespace Phonon
{
	enum SettingsTab
	{
		General,
		SceneExport,
		Phonon3D,
		PhononReverb,
		PhononSoundFlow
	}

	public class PhononSettingsWindow : EditorWindow
	{

		//
		// Mapping from tabs to tab pane classes.
		//
		static Dictionary<SettingsTab, string> 	SettingsTabNames;
		static Dictionary<SettingsTab, string> 	SettingsTabClasses;
		static Dictionary<SettingsTab, string> 	SettingsTabProducts;
		static string[]							SettingsTabNamesValues;

		static PhononSettingsWindow()
		{
			SettingsTabNames = new Dictionary<SettingsTab, string>();
			SettingsTabNames.Add(SettingsTab.General, "General");
			SettingsTabNames.Add(SettingsTab.SceneExport, "Scene");
			SettingsTabNames.Add(SettingsTab.Phonon3D, "3D Audio");
			SettingsTabNames.Add(SettingsTab.PhononReverb, "Reverb");
			SettingsTabNames.Add(SettingsTab.PhononSoundFlow, "Occlusion");

			SettingsTabClasses = new Dictionary<SettingsTab, string>();
			SettingsTabClasses.Add(SettingsTab.General, "PhononCommonPane");
			SettingsTabClasses.Add(SettingsTab.SceneExport, "PhononScenePane");
			SettingsTabClasses.Add(SettingsTab.Phonon3D, "Phonon3DPane");
			SettingsTabClasses.Add(SettingsTab.PhononReverb, "PhononReverbPane");
			SettingsTabClasses.Add(SettingsTab.PhononSoundFlow, "Phonon.PhononSoundFlowPane");

			SettingsTabProducts = new Dictionary<SettingsTab, string>();
			SettingsTabProducts.Add(SettingsTab.SceneExport, "Phonon Reverb or Phonon SoundFlow");
			SettingsTabProducts.Add(SettingsTab.Phonon3D, "Phonon 3D");
			SettingsTabProducts.Add(SettingsTab.PhononReverb, "Phonon Reverb");
			SettingsTabProducts.Add(SettingsTab.PhononSoundFlow, "Phonon SoundFlow");

			SettingsTabNamesValues = new string[SettingsTabNames.Count];
			SettingsTabNames.Values.CopyTo(SettingsTabNamesValues, 0);
		}

		[MenuItem("Window/Phonon")]
		static void Init()
		{
#pragma warning disable 618
			PhononSettingsWindow window = EditorWindow.GetWindow<PhononSettingsWindow>();
			window.title = "Phonon";
			window.Show();
#pragma warning restore 618
		}
		
		void OnEnable()
		{
			autoRepaintOnSceneChange = true;
		}
		
		void OnInspectorUpdate()
		{
			Repaint();
		}
		
		void OnGUI()
		{
			EditorGUILayout.Space();
			selectedTab = (SettingsTab) GUILayout.Toolbar((int) selectedTab, SettingsTabNamesValues);		
			EditorGUILayout.Space();

			if (!SettingsTabClasses.ContainsKey(selectedTab))
			{
			}
			else if (Type.GetType(SettingsTabClasses[selectedTab]) == null)
			{
				if (SettingsTabProducts.ContainsKey(selectedTab))
					EditorGUILayout.HelpBox("This feature requires " + SettingsTabProducts[selectedTab] + ".", MessageType.Info);
				else
					EditorGUILayout.HelpBox("Phonon has not been installed correctly.", MessageType.Error);
			}
			else
			{
				Type.GetType(SettingsTabClasses[selectedTab]).GetMethod("DrawPane").Invoke(null, null);
			}
		}

		SettingsTab		selectedTab			= SettingsTab.General;
	}
}