// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a class that will lock the transform of the object it is attached to to 
/// always follow the camera's position and reset its orientation once it passes a certain threshold
/// </summary>
public class BodyLock : MonoBehaviour {
    /// <summary>
    /// whether the transform is being rotated
    /// </summary>
    private bool rotating = false;

    /// <summary>
    /// threshold to begin rotating
    /// </summary>
    [SerializeField]
    private float beginRotateThreshold = 15.0f;

    /// <summary>
    /// threshold to stop rotating
    /// </summary>
    [SerializeField]
    private float stopRotateThreshold = 1.0f;
	
	// Update is called once per frame
	void Update () {
        // follow the camera's position
        this.transform.position = Camera.main.transform.position;

        // get delta rotation with the camera's rotation
        Quaternion delta = Quaternion.Inverse(Camera.main.transform.rotation) * this.transform.rotation;
        Vector3 deltaEuler = delta.eulerAngles; 
        float deltaH = Mathf.Abs(deltaEuler.x);
        deltaH = deltaH > 180 ? (360 - deltaH) : deltaH;
        float deltaP = Mathf.Abs(deltaEuler.y);
        deltaP = deltaP > 180 ? (360 - deltaP) : deltaP;
        
        // if delta passes a threshold and it's not rotating, set rotating to true
        if((deltaH >= beginRotateThreshold || deltaP >= beginRotateThreshold) && !rotating)
        {
            rotating = true;
        }

        // if rotating ...
        if (rotating)
        {
            // stop rotating when delta is below a certain threshold
            if(deltaH <= stopRotateThreshold && deltaP <= stopRotateThreshold)
            {
                rotating = false;
            }
            else
            {
                // slerp the rotation
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation,Camera.main.transform.rotation,Time.deltaTime);
            }
        }

	}
}
