/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour {
	[SerializeField]
	private bool _locked = true;
	public bool Locked{
		get{ return _locked; }
		set{ _locked = value; }
	}
		
	// Update is called once per frame
	void LateUpdate () {
		if(Locked)
			this.transform.rotation = Quaternion.identity;
	}
}
