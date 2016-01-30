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
//	ReverbBakeSettingsEditor
//	In-line editor for bake settings in the main window.
//

public static class ReverbBakeSettingsEditor
{
    //
    //	Returns the editor.
    //
    public static Editor GetEditor()
    {
        if (editor == null || editor.target == null)
        {
            editor = Editor.CreateEditor(ReverbGlobalSettings.GetBakeSettings());
        }

        return editor;
    }

    //
    //	Draws the editor.
    //
    public static void Draw()
    {
//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Quality Settings", EditorStyles.boldLabel);
//        EditorGUILayout.Space();
//        EditorGUILayout.Space();
        GetEditor().OnInspectorGUI();
//        EditorGUILayout.Space();
    }

    //
    //	Data members.
    //

    // Editor for the global bake settings.
    static Editor editor;
}