using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {
    public GameObject ToFollow;
	
	// Update is called once per frame
	void Update () {
        if (ToFollow == null)
            Debug.LogWarning("No object to follow...");
        else
            this.transform.SetPositionAndRotation(ToFollow.transform.position, ToFollow.transform.rotation);
	}
}
