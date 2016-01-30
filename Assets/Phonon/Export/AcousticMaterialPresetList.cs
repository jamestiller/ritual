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

using UnityEngine;


//
//	AcousticMaterialPresetList
//	Represents a list of all available material presets.
//

public static class AcousticMaterialPresetList
{

    //
    //	Checks whether or not the list has been updated.
    //
    static bool IsInitialized()
    {
        return (values != null);
    }

    //
    //	Refreshes the list of presets.
    //
    public static void Initialize()
    {
        // Count the number of presets.
        int numPresets = 12;
        values = new AcousticMaterialValue[numPresets];

        // Create all the built-in presets.
        values[0] = new AcousticMaterialValue(0.10f, 0.20f, 0.30f);
        values[1] = new AcousticMaterialValue(0.03f, 0.04f, 0.07f);
        values[2] = new AcousticMaterialValue(0.05f, 0.07f, 0.08f);
        values[3] = new AcousticMaterialValue(0.01f, 0.02f, 0.02f);
        values[4] = new AcousticMaterialValue(0.60f, 0.70f, 0.80f);
        values[5] = new AcousticMaterialValue(0.24f, 0.69f, 0.73f);
        values[6] = new AcousticMaterialValue(0.06f, 0.03f, 0.02f);
        values[7] = new AcousticMaterialValue(0.12f, 0.06f, 0.04f);
        values[8] = new AcousticMaterialValue(0.11f, 0.07f, 0.06f);
        values[9] = new AcousticMaterialValue(0.20f, 0.07f, 0.06f);
        values[10] = new AcousticMaterialValue(0.13f, 0.20f, 0.24f);
        values[11] = new AcousticMaterialValue();
    }

    //
    //	Returns the values of a material by index.
    //
    public static AcousticMaterialValue PresetValue(int index)
    {
        if (!IsInitialized())
            Initialize();

        return values[index];
    }

    //
    //	Data members.
    //

    // Values of all presets.
    static AcousticMaterialValue[] values;

}