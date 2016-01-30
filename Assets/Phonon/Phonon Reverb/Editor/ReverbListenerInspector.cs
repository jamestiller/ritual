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

using UnityEditor;
using UnityEngine;

using Phonon;


//
// ReverbListenerInspector
// Custom inspector for the ReverbListener component.
//

[CustomEditor(typeof(ReverbListener))]
public class ReverbListenerInspector : Editor
{

	//
	// Draws the inspector.
	//
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		Phonon.PhononGUI.SectionHeader("Reverb Settings");
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Mode"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Type"));

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(" ");
		if (GUILayout.Button("Update Preview"))
		{
			ReverbListener listener = target as ReverbListener;
			listener.UpdatePreview();
		}
		EditorGUILayout.EndHorizontal();

		if (Phonon.AudioEngineComponent.GetAudioEngine() == AudioEngine.Unity)
		{
			Phonon.PhononGUI.SectionHeader("Reverb Mix");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ParametricDryLevel"), new GUIContent("Parametric Dry Level (mB)"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ConvolutionSendLevel"), new GUIContent("Convolution Send Level (dB)"));
		}
		else if (AudioEngineComponent.GetAudioEngine() == AudioEngine.Wwise)
		{
			Phonon.PhononGUI.SectionHeader("Wwise Integration");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("DecayTimeRTPC"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("DecayTimeHFRatioRTPC"));
		}

		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties();
	}

}