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


namespace Phonon
{

    [Serializable]
    public class SoundFlowBakeSettingsValue
    {

        public SoundFlowBakeSettingsValue()
        {
        }

        public SoundFlowBakeSettingsValue(int rays, int bounces)
        {
            Rays = rays;
            Bounces = bounces;
        }

        public SoundFlowBakeSettingsValue(SoundFlowBakeSettingsValue other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(SoundFlowBakeSettingsValue other)
        {
            Rays = other.Rays;
            Bounces = other.Bounces;
        }

        [Range(128, 16384)]
        public int Rays;

        [Range(0, 4)]
        public int Bounces;

    }

}