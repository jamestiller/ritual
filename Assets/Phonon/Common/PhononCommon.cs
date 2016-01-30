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
using System.Runtime.InteropServices;

using UnityEngine;


namespace Phonon
{

    //
    // Basic types.
    //

    // Boolean values.
    public enum Bool
    {
        FALSE,
        TRUE
    }

    // Error codes.
    public enum Error
    {
        NONE,
        MEMORY,
        FILEIO,
        UNSUPPORTED_FORMAT,
        INVALID_INPUT,
        INITIALIZATION,
        UNKNOWN
    }


    //
    // Geometric types.
    //

    // Point in 3D space.
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
    }


    //
    // Audio pipeline.
    //

    // Supported speaker layouts.
    public enum SpeakerLayout
    {
        MONO,
        STEREO,
        QUADRAPHONIC,
        FIVE_POINT_ONE,
        SEVEN_POINT_ONE
    }

    
    //
    // Common API functions.
    //
    public static class Common
    {
        public static Vector3 ConvertVector(UnityEngine.Vector3 point)
        {
            Vector3 convertedPoint;
            convertedPoint.x = point.x;
            convertedPoint.y = point.y;
            convertedPoint.z = -point.z;

            return convertedPoint;
        }
    }
}