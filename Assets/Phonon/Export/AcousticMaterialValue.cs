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
//	AcousticMaterialValue
//	Represents the values of a specific material.
//

[Serializable]
public class AcousticMaterialValue
{

    //
    //	Constructor.
    //
    public AcousticMaterialValue()
    {
    }

    //
    //	Constructor.
    //
    public AcousticMaterialValue(float aLow, float aMid, float aHigh)
    {
        LowFreqAbsorption = aLow;
        MidFreqAbsorption = aMid;
        HighFreqAbsorption = aHigh;

        Scattering = 0.05f;
    }

    //
    //	Constructor.
    //
    public AcousticMaterialValue(float aLow, float aMid, float aHigh, float scattering)
    {
        LowFreqAbsorption = aLow;
        MidFreqAbsorption = aMid;
        HighFreqAbsorption = aHigh;

        Scattering = scattering;
    }

    //
    //	Copy constructor.
    //
    public AcousticMaterialValue(AcousticMaterialValue other)
    {
        CopyFrom(other);
    }

    //
    //	Copies data from another object.
    //
    public void CopyFrom(AcousticMaterialValue other)
    {
        LowFreqAbsorption = other.LowFreqAbsorption;
        MidFreqAbsorption = other.MidFreqAbsorption;
        HighFreqAbsorption = other.HighFreqAbsorption;

        Scattering = other.Scattering;
    }

    //
    //	Data members.
    //

    // Absorption coefficients.
    [Range(0.0f, 1.0f)]
    public float LowFreqAbsorption;
    [Range(0.0f, 1.0f)]
    public float MidFreqAbsorption;
    [Range(0.0f, 1.0f)]
    public float HighFreqAbsorption;

    // Scattering coefficients.
    [Range(0.0f, 1.0f)]
    public float Scattering;

}