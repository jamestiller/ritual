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
//	ReverbBakeSettingsDrawer
//	Custom property drawer for ReverbBakeSettingsValue.
//

[CustomPropertyDrawer(typeof(ReverbBakeSettingsValue))]
public class ReverbBakeSettingsDrawer : PropertyDrawer
{
    //
    //	Returns the overall height of the drawing area.
    //
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 48f;
    }

    //
    //	Draws the property.
    //
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = 16f;

        if (position.x <= 0)
        {
            position.x += 4f;
            position.width -= 8f;
        }

        EditorGUI.PropertyField(position, property.FindPropertyRelative("Rays"));
        position.y += 16f;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("Bounces"));
        position.y += 16f;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("Duration"), new GUIContent("Duration (s)"));
    }
}