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

using UnityEngine;


public enum ReverbZoneType
{
    Sphere,
    Box
};

[AddComponentMenu("Phonon/Phonon Reverb Zone")]
public class ReverbZone : MonoBehaviour
{
    void Reset()
    {
        if (ParametricReverbOverride == null)
        {
            ParametricReverbOverride = new ReverbBakedParametric();
			ConvolutionReverbOverride = new ReverbBakedConvolution();
        }
    }

    void OnDrawGizmosSelected()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = Color.magenta;

        if (Type == ReverbZoneType.Sphere)
        {
            Gizmos.DrawWireSphere(gameObject.transform.position, radius);
        }
        else
        {
            Gizmos.DrawWireCube(gameObject.transform.position, dimensions);
        }

        Gizmos.color = oldColor;
    }

    public bool Contains(Vector3 point)
    {
        if (Type == ReverbZoneType.Sphere)
        {
            return (Vector3.Distance(point, transform.position) <= radius);
        }
        else if (Type == ReverbZoneType.Box)
        {
            Vector3 delta = point - transform.position;
            return (Mathf.Abs(delta.x) <= 0.5f * dimensions.x && Mathf.Abs(delta.y) <= 0.5f * dimensions.y && Mathf.Abs(delta.z) <= 0.5f * dimensions.z);
        }
        else
        {
            return false;
        }
    }

    public ReverbZoneType Type = ReverbZoneType.Sphere;

    public float radius = 1.0f;
    public Vector3 dimensions = new Vector3(1.0f, 1.0f, 1.0f);

	public ReverbBakedParametric ParametricReverbOverride = null;
	public ReverbBakedConvolution ConvolutionReverbOverride = null;
}