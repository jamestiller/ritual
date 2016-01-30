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
//	ReverbBakeSettingsValue
//	The underlying values for a specific set of bake settings.
//

[Serializable]
public class ReverbBakeSettingsValue
{

    //
    //	Constructor.
    //
	public ReverbBakeSettingsValue()
    {
    }

    //
    //	Constructor.
    //
	public ReverbBakeSettingsValue(int rays, int bounces, float duration, int samplingRate)
    {
        Rays = rays;
        Bounces = bounces;
        Duration = duration;
    }

    //
    //	Copy constructor.
    //
	public ReverbBakeSettingsValue(ReverbBakeSettingsValue other)
    {
        CopyFrom(other);
    }

    //
    //	Copies data from another object.
    //
	public void CopyFrom(ReverbBakeSettingsValue other)
    {
        Rays = other.Rays;
        Bounces = other.Bounces;
        Duration = other.Duration;
    }

    //
    //	Data members.
    //

    // Number of rays to trace.
    [Range(16384, 65536)]
    public int Rays;

    // Number of bounces to simulate.
    [Range(64, 256)]
    public int Bounces;

    // Duration of IR.
    [Range(0.5f, 5.0f)]
    public float Duration;

}