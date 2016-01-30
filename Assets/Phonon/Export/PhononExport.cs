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
	// Scene-related types.
	//

	// Indexed triangle.
	[StructLayout(LayoutKind.Sequential)]
	public struct Triangle
	{
		public int		index0;
		public int		index1;
		public int		index2;
	}

	// Material.
	[StructLayout(LayoutKind.Sequential)]
	public struct Material
	{
		public float	absorptionLow;
		public float	absorptionMid;
		public float	absorptionHigh;
		public float	scattering;
	}


	//
	// Scene export functions.
	//

	public static class Export
	{
		[DllImport("phononxp")]
		public static extern Error	iplCreateScene([In, Out] ref IntPtr scene);
		
		[DllImport("phononxp")]
		public static extern void	iplDestroyScene(IntPtr scene);
		
		[DllImport("phononxp")]
		public static extern Error	iplAddObject(IntPtr scene, string name);
		
		[DllImport("phononxp")]
		public static extern Error	iplSetObjectVertices(IntPtr scene, int numVertices, Vector3[] vertices);
		
		[DllImport("phononxp")]
		public static extern Error	iplSetObjectTriangles(IntPtr scene, int numTriangles, Triangle[] triangles);
		
		[DllImport("phononxp")]
		public static extern Error	iplSetObjectMaterial(IntPtr scene, Material material);
		
		[DllImport("phononxp")]
		public static extern Error	iplFinalizeScene(IntPtr scene);
		
		[DllImport("phononxp")]
		public static extern void	iplDumpScene(IntPtr scene, string fileName);
	}


	//
	// Grid functions.
	//

	public static class Grid
	{
		[DllImport("phononxp")]
		public static extern Error	iplCreateGrid(IntPtr scene, Vector3 down, float spacing, [In, Out] ref IntPtr grid);
		
		[DllImport("phononxp")]
		public static extern void	iplDestroyGrid(IntPtr grid);
		
		[DllImport("phononxp")]
		public static extern Error	iplLoadGrid(IntPtr scene, Vector3 down, byte[] data, [In, Out] ref IntPtr grid);
		
		[DllImport("phononxp")]
		public static extern int	iplSaveGrid(IntPtr grid, byte[] data);
		
		[DllImport("phononxp")]
		public static extern int 	iplGetGridPoints(IntPtr grid, Vector3[] points);
	}

}