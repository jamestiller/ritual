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

using System.IO;
using UnityEditor;
using UnityEngine;


//
//	ReverbBakedDataInspector
//	Custom inspector for a ReverbBakedData component.
//

[CustomEditor(typeof(ReverbBakedData))]
public class ReverbBakedDataInspector : Editor
{
    //
    //	Draws the inspector.
    //
    public override void OnInspectorGUI()
    {
		serializedObject.Update();

		ReverbBakedData data = serializedObject.targetObject as ReverbBakedData;
		int dataSize = (data == null || data.Data == null) ? 0 : data.Data.Length;

		Phonon.PhononGUI.SectionHeader("Baked Data Statistics");
		if (dataSize < 1024)
			EditorGUILayout.LabelField("Data Size", dataSize.ToString() + " bytes");
		else if (dataSize < 1024 * 1024)
			EditorGUILayout.LabelField("Data Size", (dataSize / 1024).ToString() + " kB");
		else
			EditorGUILayout.LabelField("Data Size", (dataSize / (1024 * 1024)).ToString() + " MB");

		// Get name of current scene (without any path information, e.g. "Assets/xxx.unity" -> "xxx.unity")
		string[] currentScenePathStrArr = EditorApplication.currentScene.Split(new string [] {"/"}, System.StringSplitOptions.None);
		string currentSceneNameStr = currentScenePathStrArr [currentScenePathStrArr.Length - 1];

		if (Phonon.AudioEngineComponent.GetAudioEngine() == Phonon.AudioEngine.Wwise)
		{
			Phonon.PhononGUI.SectionHeader("Wwise Integration");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(" ");
			if (GUILayout.Button("Export to Wwise"))
			{
				string fileName = EditorUtility.SaveFilePanel("Export Baked Reverb to Wwise", "", Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + ".ir", "ir");
				if (fileName.Length > 0)
				{
					data.ExportDataForWwise(fileName);
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		else if (Phonon.AudioEngineComponent.GetAudioEngine() == Phonon.AudioEngine.FMODStudio)
		{
			Phonon.PhononGUI.SectionHeader("FMOD Studio Integration");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(" ");
			if (GUILayout.Button("Export to FMOD Studio"))
			{
				string fileName = EditorUtility.SaveFilePanel("Export Baked Reverb to FMOD Studio", "", currentSceneNameStr + ".ir", "ir");
				if (fileName.Length > 0)
				{
					data.ExportDataForWwise(fileName);
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties();
    }
}