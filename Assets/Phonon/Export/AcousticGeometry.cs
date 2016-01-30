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


[AddComponentMenu("Phonon/Phonon Geometry")]
public class AcousticGeometry : MonoBehaviour
{

    //
    // Returns the attached triangle mesh, if any.
    //
    public MeshFilter AttachedMesh { get { return GetComponent<MeshFilter>(); } }

    //
    // Returns the attached terrain, if any.
    //
    public Terrain AttachedTerrain { get { return GetComponent<Terrain>(); } }

    //
    // Number of vertices in this geometry.
    //
    public int NumVertices
    {
        get
        {
            if (AttachedMesh != null)
            {
                return AttachedMesh.sharedMesh.vertices.Length;
            }
            else if (AttachedTerrain != null)
            {
                int w = AttachedTerrain.terrainData.heightmapWidth;
                int h = AttachedTerrain.terrainData.heightmapHeight;
                int s = Mathf.Min(w - 1, Mathf.Min(h - 1, (int) Mathf.Pow(2.0f, (float) TerrainSimplificationLevel)));

                if (s == 0)
                    s = 1;

                w = ((w - 1) / s) + 1;
                h = ((h - 1) / s) + 1;

                return (w * h);
            }
            else
            {
                return 0;
            }
        }
    }

    //
    // Number of triangles in this geometry.
    //
    public int NumTriangles
    {
        get
        {
            if (AttachedMesh != null)
            {
                return (AttachedMesh.sharedMesh.triangles.Length / 3);
            }
            else if (AttachedTerrain != null)
            {
                int w = AttachedTerrain.terrainData.heightmapWidth;
                int h = AttachedTerrain.terrainData.heightmapHeight;
                int s = Mathf.Min(w - 1, Mathf.Min(h - 1, (int)Mathf.Pow(2.0f, (float)TerrainSimplificationLevel)));

                if (s == 0)
                    s = 1;

                w = ((w - 1) / s) + 1;
                h = ((h - 1) / s) + 1;

                return ((w - 1) * (h - 1) * 2);
            }
            else
            {
                return 0;
            }
        }
    }

    //
    // Simplification level for terrain mesh.
    //
    [Range(0, 10)]
    public int TerrainSimplificationLevel = 0;

    //
    // Exports a triangle mesh.
    //
    void ExportMesh(Phonon.Vector3[] vertices, Phonon.Triangle[] triangles)
    {
        for (int i = 0; i < vertices.Length; ++i)
            vertices[i] = Phonon.Common.ConvertVector(gameObject.transform.TransformPoint(AttachedMesh.sharedMesh.vertices[i]));

        for (int i = 0; i < triangles.Length; ++i)
        {
            triangles[i].index0 = AttachedMesh.sharedMesh.triangles[3 * i + 0];
            triangles[i].index1 = AttachedMesh.sharedMesh.triangles[3 * i + 1];
            triangles[i].index2 = AttachedMesh.sharedMesh.triangles[3 * i + 2];
        }
    }

    //
    // Exports a terrain.
    //
    void ExportTerrain(Phonon.Vector3[] vertices, Phonon.Triangle[] triangles)
    {
        int w = AttachedTerrain.terrainData.heightmapWidth;
        int h = AttachedTerrain.terrainData.heightmapHeight;
        int s = Mathf.Min(w - 1, Mathf.Min(h - 1, (int)Mathf.Pow(2.0f, (float)TerrainSimplificationLevel)));

        if (s == 0)
            s = 1;

        w = ((w - 1) / s) + 1;
        h = ((h - 1) / s) + 1;

        Vector3 position = AttachedTerrain.transform.position;
        float[,] heights = AttachedTerrain.terrainData.GetHeights(0, 0, AttachedTerrain.terrainData.heightmapWidth, AttachedTerrain.terrainData.heightmapHeight);

        int vertexIndex = 0;
        for (int v = 0; v < AttachedTerrain.terrainData.heightmapHeight; v += s)
        {
            for (int u = 0; u < AttachedTerrain.terrainData.heightmapWidth; u += s)
            {
                float height = heights[v, u];

                float x = position.x + (((float) u / AttachedTerrain.terrainData.heightmapWidth) * AttachedTerrain.terrainData.size.x);
                float y = position.y + (height * AttachedTerrain.terrainData.size.y);
                float z = position.z + (((float) v / AttachedTerrain.terrainData.heightmapHeight) * AttachedTerrain.terrainData.size.z);

                vertices[vertexIndex++] = Phonon.Common.ConvertVector(new Vector3 { x = x, y = y, z = z });
            }
        }

        int triangleIndex = 0;
        for (int v = 0; v < h - 1; ++v)
        {
            for (int u = 0; u < w - 1; ++u)
            {
                int i0 = v * w + u;
                int i1 = (v + 1) * w + u;
                int i2 = v * w + (u + 1);

                triangles[triangleIndex++] = new Phonon.Triangle { index0 = i0, index1 = i1, index2 = i2 };

                i0 = v * w + (u + 1);
                i1 = (v + 1) * w + u;
                i2 = (v + 1) * w + (u + 1);

                triangles[triangleIndex++] = new Phonon.Triangle { index0 = i0, index1 = i1, index2 = i2 };
            }
        }
    }

    //
    // Exports this object.
    //
    public bool ExportGeometry(IntPtr scene)
    {
        if (NumVertices == 0 || NumTriangles == 0)
            return false;

        Phonon.Vector3[] vertices = new Phonon.Vector3[NumVertices];
        Phonon.Triangle[] triangles = new Phonon.Triangle[NumTriangles];

        if (AttachedMesh != null)
            ExportMesh(vertices, triangles);
        else if (AttachedTerrain != null)
            ExportTerrain(vertices, triangles);
        else
            return false;

        Phonon.Export.iplSetObjectVertices(scene, vertices.Length, vertices);
        Phonon.Export.iplSetObjectTriangles(scene, triangles.Length, triangles);
        return true;
    }

}