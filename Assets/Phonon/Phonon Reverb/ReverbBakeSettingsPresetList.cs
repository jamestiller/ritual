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


//
//	RevervBakeSettingsPresetList
//	A statically-available list of all bake settings presets and their values.
//

public static class ReverbBakeSettingsPresetList
{

    //
    //	Returns whether or not the list has been initialized.
    //
    static bool IsInitialized()
    {
        return (values != null);
    }

    //
    //	Initializes the preset list.
    //
    static void Initialize()
    {
        int numPresets = 4;
        values = new ReverbBakeSettingsValue[numPresets];

        values[0] = new ReverbBakeSettingsValue(16384, 64, 1.0f, 44100);
        values[1] = new ReverbBakeSettingsValue(16384, 128, 1.0f, 44100);
        values[2] = new ReverbBakeSettingsValue(32768, 128, 1.0f, 44100);
        values[3] = new ReverbBakeSettingsValue();
    }

    //
    //	Returns the value of a given preset by index.
    //
    public static ReverbBakeSettingsValue PresetValue(int index)
    {
        if (!IsInitialized())
            Initialize();

        return values[index];
    }

    //
    //	Data members.
    //

    // Array of preset values.
    static ReverbBakeSettingsValue[] values;

}