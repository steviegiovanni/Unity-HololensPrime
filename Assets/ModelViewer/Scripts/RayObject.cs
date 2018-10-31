// author: Stevie Giovanni

using ModelViewer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// simple ray object that implements IHasRay interface
/// return the ray coming from the origin this script is attached to and looking in the forward direction
/// </summary>
public class RayObject : MonoBehaviour, IHasRay
{
    public Ray GetRay()
    {
        return new Ray(this.transform.position, this.transform.forward);
    }
}
