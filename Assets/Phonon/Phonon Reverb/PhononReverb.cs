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
	// Reverb data types.
	//

	// Type of reverb.
	public enum ReverbType
	{
		Parametric = 1,
		Convolution = 2
	}

	// Settings for calculating reverb.
	[StructLayout(LayoutKind.Sequential)]
	public struct ReverbSettings
	{
		public int		rays;
		public int		bounces;
		public int		samplingRate;
		public float	duration;
	}

	// I3DL2-compliant parametric reverb.
	[StructLayout(LayoutKind.Sequential)]
	public struct ParametricReverb
	{
		public float	room;
		public float	roomHigh;
		public float	roomLow;
		public float	decayTime;
		public float	decayTimeHighRatio;
		public float	reflections;
		public float	reflectionsDelay;
		public float	reverb;
		public float	reverbDelay;
		public float	hfReference;
		public float	lfReference;
		public float	roomRolloff;
		public float	diffusion;
		public float	density;
	}


	//
	// Bake callback.
	//
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void BakeCallback(int numProcessed, int numTotal);


	//
	// Phonon Reverb API functions.
	//
	public static class Reverb
	{
        [DllImport("phononrv", CallingConvention = CallingConvention.Cdecl)]
		public static extern Error		iplBakeReverb(IntPtr scene, IntPtr grid, ReverbSettings settings, int typeFlags, BakeCallback callback);

		[DllImport("phononrv")]
		public static extern void		iplCancelReverbBake();
		
		[DllImport("phononrv")]
		public static extern Error		iplLoadBakedReverb(int numGridPoints, int dataSize, byte[] data);
		
		[DllImport("phononrv")]
		public static extern int 		iplSaveBakedReverb(byte[] data);
		
		[DllImport("phononrv")]
		public static extern void		iplUnloadBakedReverb();
		
		[DllImport("phononrv")]
		public static extern IntPtr		iplGetBakedReverb(IntPtr grid, ReverbType type, Vector3 position);
		
		[DllImport("phononrv")]
		public static extern Error		iplCreateReverbEstimator(IntPtr scene, ReverbSettings settings, int typeFlags, [In, Out] ref IntPtr estimator);
		
		[DllImport("phononrv")]
		public static extern void		iplDestroyReverbEstimator(IntPtr estimator);
		
		[DllImport("phononrv")]
		public static extern IntPtr		iplEstimateReverb(IntPtr estimator, ReverbType type, Vector3 position);

        [DllImport("phononrv")]
        public static extern Error      iplCreateReverbContext(uint samplingRate, uint frameSize, uint numChannels, float duration, [In, Out] ref IntPtr context);

        [DllImport("phononrv")]
        public static extern void       iplDestroyReverbContext([In, Out] ref IntPtr context);

		[DllImport("phononrv")]
		public static extern Error		iplCreateReverbRenderer(IntPtr context, [In, Out] ref IntPtr renderer);
		
		[DllImport("phononrv")]
		public static extern void		iplDestroyReverbRenderer([In, Out] ref IntPtr renderer);
		
		[DllImport("phononrv")]
		public static extern void		iplUpdateReverb(IntPtr renderer, IntPtr data);

		[DllImport("phononrv")]
		public static extern void		iplUpdateReverbRaw(IntPtr renderer, float[] impulseResponse);
		
		[DllImport("phononrv")]
		public static extern void		iplProcessReverb(IntPtr renderer, float[] inBuffer, float[] outBuffer);
	}

}