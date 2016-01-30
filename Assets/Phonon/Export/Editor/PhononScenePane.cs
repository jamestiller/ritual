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


public static class PhononScenePane
{
	public static void DrawPane()
	{
		if (targetObject == null || editor == null)
		{
			targetObject = AcousticMaterialSettings.GetObject();
			editor = Editor.CreateEditor(targetObject.GetComponent<AcousticMaterial>());
		}
		
		editor.OnInspectorGUI();

		Phonon.PhononGUI.SectionHeader("Export Phonon Geometry");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(" ");
		if (GUILayout.Button("Export to OBJ"))
		{
			string fileName = EditorUtility.SaveFilePanel("Export Phonon Geometry", "", Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + ".obj", "obj");
			if (fileName.Length > 0)
			{
				AcousticSceneExporter.ExportScene();
				AcousticSceneExporter.DumpScene(fileName);
                AcousticSceneExporter.Destroy();
            }
        }
		EditorGUILayout.EndHorizontal();
    }

	static GameObject targetObject = null;
	static Editor editor = null;
}