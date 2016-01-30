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


//
//	AcousticMaterialInspector
//	Custom inspector for AcousticMaterial components.
//

[CustomEditor(typeof(AcousticMaterial))]
[CanEditMultipleObjects]
public class AcousticMaterialInspector : Editor
{
    //
    //	Draws the inspector.
    //
    public override void OnInspectorGUI()
    {
		serializedObject.Update();
		
        bool guiWasEnabled = GUI.enabled;
        GUI.enabled = !AcousticSceneExporter.Exporting;

		Phonon.PhononGUI.SectionHeader("Material Preset");
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Preset"));

		if (serializedObject.FindProperty("Preset").enumValueIndex < 11)
        {
            AcousticMaterialValue actualValue = ((AcousticMaterial)target).Value;
			actualValue.CopyFrom(AcousticMaterialPresetList.PresetValue(serializedObject.FindProperty("Preset").enumValueIndex));
        }

        else
        {
			Phonon.PhononGUI.SectionHeader("Custom Material");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Value"));
        }

		EditorGUILayout.Space();
		
		// Re-enable the GUI.
		GUI.enabled = guiWasEnabled;

        // Save changes.
		serializedObject.ApplyModifiedProperties();
    }
}