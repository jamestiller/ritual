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


namespace Phonon
{

	[StructLayout(LayoutKind.Sequential)]
	public struct IPLSoundPath
	{
		public Vector3	direction;
		public float	eqLow;
		public float	eqMid;
		public float	eqHigh;
		public float	delay;
		public bool		valid;
	}

    [StructLayout(LayoutKind.Sequential)]
    public struct IPLSoundFlowSettings
    {
        public int rays;
        public int bounces;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SoundFlowPreprocessCallback([MarshalAs(UnmanagedType.I8)] Int64 processedVoxels, [MarshalAs(UnmanagedType.I8)] Int64 totalVoxels);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void SoundFlowBakeCallback(int processedSources, int totalSources, int processedPoints, int totalPoints);

	public static class SoundFlow
	{
        [DllImport("phononsf")]
        public static extern Error iplCreateSoundFlowBaker(IntPtr scene, IntPtr grid, IPLSoundFlowSettings settings, Bool enableReflection, Bool enableDiffraction, int numSources, Vector3[] sources, SoundFlowPreprocessCallback callback, [In, Out] ref IntPtr baker);

        [DllImport("phononsf")]
        public static extern Error iplLoadSoundFlow(IntPtr scene, IntPtr grid, IPLSoundFlowSettings settings, int numSources, Vector3[] sources, byte[] data, [In, Out] ref IntPtr baker);

        [DllImport("phononsf")]
		public static extern void iplBakeSoundFlow(IntPtr baker, SoundFlowBakeCallback callback);

		[DllImport("phononsf")]
		public static extern void iplCancelSoundFlowBake(IntPtr baker);

		[DllImport("phononsf")]
		public static extern int iplSaveSoundFlow(IntPtr baker, byte[] data);

		[DllImport("phononsf")]
		public static extern void iplDestroySoundFlowBaker([In, Out] ref IntPtr baker);

		[DllImport("phononsf")]
		public static extern IntPtr iplGetBakedSoundFlow(IntPtr baker, int sourceIndex, Vector3 position);

        [DllImport("phononsf")]
        public static extern IntPtr iplGetDirectSoundPath(IntPtr baker, int sourceIndex, Vector3 source, Vector3 listener);

        [DllImport("phononsf")]
        public static extern IPLSoundPath iplGetBakedSoundPath(byte[] buffer, int numGridPoints, int sourceIndex, int gridIndex, int pathIndex);

		[DllImport("phononsf")]
		public static extern Error iplCreateSoundFlowEstimator(IntPtr scene, IPLSoundFlowSettings settings, Bool enableReflection, Bool enableDiffraction, [In, Out] ref IntPtr estimator);

		[DllImport("phononsf")]
		public static extern void iplDestroySoundFlowEstimator(IntPtr estimator);

		[DllImport("phononsf")]
		public static extern IntPtr iplEstimateSoundFlow(IntPtr estimator, Vector3 source, Vector3 listener, Vector3 ahead, Vector3 up);

        [DllImport("phononsf")]
        public static extern Error iplCreateSoundFlowContext(uint samplingRate, uint frameSize, uint numChannels, [In, Out] ref IntPtr context);

        [DllImport("phononsf")]
        public static extern void iplDestroySoundFlowContext([In, Out] ref IntPtr context);

		[DllImport("phononsf")]
		public static extern Error iplCreateSoundFlowRenderer(IntPtr context, int numPaths, [In, Out] ref IntPtr renderer);

		[DllImport("phononsf")]
		public static extern void iplDestroySoundFlowRenderer([In, Out] ref IntPtr renderer);

		[DllImport("phononsf")]
		public static extern void iplUpdateSoundFlow(IntPtr renderer, IntPtr soundFlow, IntPtr directPath, Vector3 ahead, Vector3 up);

		[DllImport("phononsf")]
		public static extern void iplProcessSoundFlow(IntPtr renderer, float[] inBuffer, float[] outBuffer);
	}

}