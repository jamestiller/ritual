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

using UnityEngine;
using UnityEditor;


//
// ReverbZoneInspector
// Custom inspector for ReverbZone.
//

[CustomEditor(typeof(ReverbZone))]
public class ReverbZoneInspector : Editor
{
	//
	// Draws the inspector GUI.
	//
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		Phonon.PhononGUI.SectionHeader("Zone Shape");
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Type"));

		ReverbZoneType type = (ReverbZoneType) serializedObject.FindProperty("Type").enumValueIndex;
		if (type == ReverbZoneType.Sphere)
		{
			Phonon.PhononGUI.SectionHeader("Sphere Shape");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"));
		}
		else
		{
			Phonon.PhononGUI.SectionHeader("Box Shape");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dimensions"));
		}

		Phonon.PhononGUI.SectionHeader("Parametric Reverb Override");
		EditorGUILayout.PropertyField(serializedObject.FindProperty("ParametricReverbOverride"));

		Phonon.PhononGUI.SectionHeader("Convolution Reverb Override");
		EditorGUILayout.PropertyField(serializedObject.FindProperty("ConvolutionReverbOverride"));

		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties();
	}

	//
	// Draws the scene GUI.
	//
	void OnSceneGUI()
	{
		ReverbZone targetZone = target as ReverbZone;
		
		if (targetZone.Type == ReverbZoneType.Sphere)
			targetZone.radius = Handles.RadiusHandle(Quaternion.identity, targetZone.gameObject.transform.position, targetZone.radius);
		else if (targetZone.Type == ReverbZoneType.Box)
			targetZone.dimensions = Handles.ScaleHandle(targetZone.dimensions, targetZone.gameObject.transform.position, Quaternion.identity, HandleUtility.GetHandleSize(targetZone.gameObject.transform.position));
	}
}