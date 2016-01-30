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


[Serializable]
public class ReverbBakedParametric
{
    [Range(-10000.0f, 0.0f)]
    public float Room = 0.0f;

    [Range(-10000.0f, 0.0f)]
    public float RoomHigh = 0.0f;

    [Range(-10000.0f, 0.0f)]
    public float RoomLow = 0.0f;

    [Range(0.1f, 20.0f)]
    public float DecayTime = 1.0f;

    [Range(0.1f, 2.0f)]
    public float DecayHighRatio = 0.5f;

    [Range(-10000.0f, 1000.0f)]
    public float Reflections = -10000.0f;

    [Range(0.0f, 0.3f)]
    public float ReflectionsDelay = 0.0f;

    [Range(-10000.0f, 2000.0f)]
    public float Reverb = 0.0f;

    [Range(0.0f, 0.1f)]
    public float ReverbDelay = 0.04f;

    [Range(20.0f, 20000.0f)]
    public float HFReference = 5000.0f;

    [Range(20.0f, 20000.0f)]
    public float LFReference = 250.0f;
    
    [Range(0.0f, 10.0f)]
    public float RoomRolloff = 10.0f;

    [Range(0.0f, 100.0f)]
    public float Diffusion = 100.0f;

    [Range(0.0f, 100.0f)]
    public float Density = 100.0f;
}