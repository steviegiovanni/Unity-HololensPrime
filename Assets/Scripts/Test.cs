using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    public GameObject toFollow;
    private Quaternion offsetRot;
    private Vector3 offsetPos;

	// Use this for initialization
	void Start () {
        offsetPos = toFollow.transform.InverseTransformPoint(this.transform.position);
        offsetRot = Quaternion.Inverse(toFollow.transform.rotation) * this.transform.rotation;

    }
	
	// Update is called once per frame
	void Update () {
        this.transform.SetPositionAndRotation(toFollow.transform.TransformPoint(offsetPos),toFollow.transform.rotation * offsetRot);
	}
}
