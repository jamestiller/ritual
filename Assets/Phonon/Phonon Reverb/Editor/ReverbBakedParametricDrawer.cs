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


[CustomPropertyDrawer(typeof(ReverbBakedParametric))]
public class ReverbBakedParametricDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 14f * 16f;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("Room"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("RoomHigh"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("RoomLow"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("DecayTime"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("DecayHighRatio"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("Reflections"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("ReflectionsDelay"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("Reverb"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("ReverbDelay"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("HFReference"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("LFReference"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("RoomRolloff"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("Diffusion"));
		position.y += 16f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("Density"));
	}
}