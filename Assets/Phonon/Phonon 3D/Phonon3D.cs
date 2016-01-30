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

    //
    // Settings for automatic prioritization.
    //
    [StructLayout(LayoutKind.Sequential)]
    public struct ListenerSettings
    {
        public int maxSources;
        public float maxDistance;
        public float minAttenuation;
    }


    //
    // Phonon 3D API functions.
    //
    public static class Phonon3D
    {
        public const int DefaultSourcePriority = 128;

        [DllImport("phonon3d")]
        public static extern Error iplCreate3DContext(uint samplingRate, uint frameSize, uint numChannels, string hrtfFile, ListenerSettings listenerSettings, [In, Out] ref IntPtr context);

        [DllImport("phonon3d")]
        public static extern void iplDestroy3DContext([In, Out] ref IntPtr context);

        [DllImport("phonon3d")]
        public static extern Error iplCreateSource(IntPtr context, int priority, [In, Out] ref IntPtr source);

        [DllImport("phonon3d")]
        public static extern void iplDestroySource([In, Out] ref IntPtr source);

        [DllImport("phonon3d")]
        public static extern void iplUpdateSource(IntPtr source, Vector3 position);

        [DllImport("phonon3d")]
        public static extern void iplProcessSource(IntPtr source, float[] inBuffer, float[] outBuffer);

        [DllImport("phonon3d")]
        public static extern void iplUpdateListener(IntPtr context, Vector3 position, Vector3 ahead, Vector3 up);
    }

	//
	// Phonon 3D API functions.
	//
	public static class Phonon3DUnity5
	{
		[DllImport("AudioPlugin_phonon3d_unity5")]
		public static extern Error iplUnity5InitializePhonon3D(int samplingRate, int frameSize, string hrtfFile, ListenerSettings listenerSettings);

		[DllImport("AudioPlugin_phonon3d_unity5")]
		public static extern void iplUnity5UpdateListener();
	}

	//
	// Phonon 3D API functions.
	//
	public static class Phonon3DFMOD
	{
		[DllImport("phonon3d_fmod")]
		public static extern void iplFMODInitialize(string hrtfFileName, ListenerSettings settings);
		
		[DllImport("phonon3d_fmod")]
		public static extern void iplFMODUpdateListener();
	}

    //
    // Phonon 3D Wwise Integration API functions.
    //
    public static class Phonon3DWwise
    {
        [DllImport("AkSoundEngine")]
        public static extern Error iplWwiseCreateListener(string hrtfFileName, ListenerSettings settings);
    }
}