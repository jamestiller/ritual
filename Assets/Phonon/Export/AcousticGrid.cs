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


//
// AcousticGrid
// A grid of points on which to bake acoustic data.
//

[AddComponentMenu("Phonon/Phonon Grid")]
public class AcousticGrid : MonoBehaviour
{
	//
	// Creates the grid.
	//
	public void CreateGrid()
	{
		AcousticSceneExporter.ExportScene();

        Phonon.Vector3 downVector;
        downVector.x = .0f; downVector.y = -1.0f; downVector.z = .0f;
        Phonon.Grid.iplCreateGrid(AcousticSceneExporter.Scene, downVector, Spacing, ref grid);

		int gridSize = Phonon.Grid.iplSaveGrid(grid, null);
		GridData = new byte[gridSize];
		Phonon.Grid.iplSaveGrid(grid, GridData);

		int numPoints = Phonon.Grid.iplGetGridPoints(grid, null);
		GridPoints = new float[3 * numPoints];
		Phonon.Vector3[] gridPointsArray = new Phonon.Vector3[numPoints];
		Phonon.Grid.iplGetGridPoints(grid, gridPointsArray);

		for (int i = 0; i < numPoints; ++i)
		{
			GridPoints[3*i + 0] = gridPointsArray[i].x;
			GridPoints[3*i + 1] = gridPointsArray[i].y;
			GridPoints[3*i + 2] = gridPointsArray[i].z;
		}
		
		Phonon.Grid.iplDestroyGrid(grid);

		AcousticSceneExporter.Destroy();
	}

	//
	// Loads the grid from the data array.
	//
	public IntPtr LoadGrid()
	{
        Phonon.Vector3 downVector;
        downVector.x = .0f; downVector.y = -1.0f; downVector.z = .0f;
        
        Phonon.Grid.iplLoadGrid(AcousticSceneExporter.Scene, downVector, GridData, ref grid);
		return grid;
    }

	//
	// Unloads the grid.
	//
	public void UnloadGrid()
	{
		Phonon.Grid.iplDestroyGrid(grid);
	}
    
    //
	// Renders the grid as a set of yellow dots.
	//
	void OnDrawGizmosSelected()
	{
		if (GridData == null)
			return;

		if (GridPoints == null)
			return;

		Color oldColor = Gizmos.color;
		Gizmos.color = Color.yellow;
		
		for (int i = 0; i < GridPoints.Length / 3; ++i)
		{
			UnityEngine.Vector3 center = new UnityEngine.Vector3(GridPoints[3*i + 0], GridPoints[3*i + 1], -GridPoints[3*i + 2]);
			Gizmos.DrawCube(center, new Vector3(0.1f, 0.1f, 0.1f));
		}
		
		Gizmos.color = oldColor;
	}

	public float Spacing = 1.0f;
	public byte[] GridData = null;
	public float[] GridPoints = null;

	IntPtr grid = IntPtr.Zero;
}