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
public class ReverbBakedConvolution
{
	public float[] GetImpulseResponse()
	{
		if (ImpulseResponse == null)
		{
			Debug.LogWarning("No IR specified for convolution reverb override!");
			return null;
		}
		else if (ImpulseResponse.frequency != AudioSettings.outputSampleRate)
		{
			Debug.LogWarning("IR specified for convolution reverb override has incorrect sampling rate; ignoring.");
			return null;
		}
		else if (ImpulseResponse.channels != 1)
		{
			Debug.LogWarning("IR specified for convolution reverb override has more than 1 channel; ignoring.");
			return null;
		}
		else
		{
			if (irData == null)
			{
				irData = new float[ImpulseResponse.samples];
				ImpulseResponse.GetData(irData, 0);
			}

			if (irDataAdjusted == null)
			{
				int samplingRate = AudioSettings.outputSampleRate;
				float duration = ReverbGlobalSettings.GetBakeSettings().Value.Duration;
				int numSamples = Mathf.CeilToInt(samplingRate * duration);

				irDataAdjusted = new float[numSamples];
				Array.Clear(irDataAdjusted, 0, irDataAdjusted.Length);

				int numSamplesToCopy = Mathf.Min(irData.Length, irDataAdjusted.Length);
				Array.Copy(irData, irDataAdjusted, numSamplesToCopy);
			}

			return irDataAdjusted;
		}
	}

	public AudioClip ImpulseResponse = null;

	float[] irData = null;
	float[] irDataAdjusted = null;
}