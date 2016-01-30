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
using System.Text;

using UnityEngine;


//
// SceneExporter
// Exports a Unity scene to the Phonon backend.
//

public class AcousticSceneExporter
{
    //
    // Main export function.
    //
    public static void ExportScene()
    {
        if (Exported)
            return;

        Exporting = true;

        Initialize();
        ExportSceneMesh();
		Phonon.Export.iplFinalizeScene(scene);
        
        Exporting = false;
		Exported = true;
    }

    //
    //  Initializes the exporter.
    //
    static void Initialize()
    {
    }

    //
    //  Cleans up backend handles.
    //
    public static void Destroy()
    {
		if (scene != IntPtr.Zero)
		{
			Phonon.Export.iplDestroyScene(scene);
			scene = IntPtr.Zero;
			Exported = false;
		}
    }

    //
    // Exports the scene data.
    //
    static void ExportSceneMesh()
    {
        if (scene != IntPtr.Zero)
            return;

        AcousticGeometry[] geometry = GameObject.FindObjectsOfType<AcousticGeometry>();
        if (geometry.Length == 0)
        {
            Debug.LogError("No Acoustic Geometry found; aborting scene export.");
            return;
        }

        Phonon.Export.iplCreateScene(ref scene);

        for (int i = 0; i < geometry.Length; ++i)
        {
            if (geometry[i].NumVertices == 0 || geometry[i].NumTriangles == 0)
                continue;

            Phonon.Export.iplAddObject(scene, geometry[i].gameObject.name + "-" + i.ToString());

            if (geometry[i].ExportGeometry(scene))
            {
                // Get the attached material.
                AcousticMaterial attachedMaterial = geometry[i].GetComponent<AcousticMaterial>();
                if (attachedMaterial == null)
                    attachedMaterial = AcousticMaterialSettings.GetDefaultMaterial();
                AcousticMaterialValue materialValue = attachedMaterial.Value;

                // Pack the material value in API-friendly format.
                Phonon.Material material = new Phonon.Material();
                material.absorptionLow = materialValue.LowFreqAbsorption;
                material.absorptionMid = materialValue.MidFreqAbsorption;
                material.absorptionHigh = materialValue.HighFreqAbsorption;
                material.scattering = materialValue.Scattering;

                // Export the material.
                Phonon.Export.iplSetObjectMaterial(scene, material);
            }
        }
    }

	//
	// Dumps the scene to an .obj file.
	//
	public static void DumpScene(string fileName)
	{
		if (scene == IntPtr.Zero)
			return;

		Phonon.Export.iplDumpScene(scene, fileName);
	}

    public static IntPtr Scene
    {
        get { return scene; }
    }

    // Status indicator.
    public static bool Exporting = false;
    public static bool Exported = false;

    // Backend handles.
	static IntPtr scene = IntPtr.Zero;
}