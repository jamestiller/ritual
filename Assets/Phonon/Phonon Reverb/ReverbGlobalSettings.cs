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
//	ReverbGlobalSettings
//	Represents global Phonon Reverb settings.
//

public static class ReverbGlobalSettings
{
    //
    //	Returns the global settings object, creating it if necessary.
    //
    public static GameObject GetObject()
    {
        // The name of the object.
        string name = "Phonon Reverb Settings";

        // If the reference is null, we need to point it to the object.
        if (settingsObject == null)
        {
            // Try finding the object.
            GameObject existingObject = GameObject.Find(name);

            // If the object couldn't be found, we need to create it.
            if (existingObject == null)
            {
                // Create the object.
                existingObject = new GameObject(name);

                // Add a bake settings component.
                existingObject.AddComponent<ReverbBakeSettings>();

				ReverbBakeSettings settingsComponent = existingObject.GetComponent<ReverbBakeSettings>();
				settingsComponent.Preset = ReverbBakeSettingsPreset.Low;
				settingsComponent.Value = new ReverbBakeSettingsValue();
				settingsComponent.Value.Rays = ReverbBakeSettingsPresetList.PresetValue(0).Rays;
				settingsComponent.Value.Bounces = ReverbBakeSettingsPresetList.PresetValue(0).Bounces;
				settingsComponent.Value.Duration = ReverbBakeSettingsPresetList.PresetValue(0).Duration;
			}

            // Point our reference to the object.
            settingsObject = existingObject;
        }

        // Return the object.
        return settingsObject;
    }

    //
    //	Returns the bake settings component.
    //
    public static ReverbBakeSettings GetBakeSettings()
    {
        return GetObject().GetComponent<ReverbBakeSettings>();
    }

    //
    //	Data members.
    //

    // The GameObject in which the global settings are stored.
    static GameObject settingsObject;

}