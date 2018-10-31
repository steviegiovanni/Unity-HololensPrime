using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceTransformView : Photon.PunBehaviour,IPunObservable {
    [SerializeField]
    public Vector3 position;

    [SerializeField]
    public Quaternion rotation;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(position);
            stream.SendNext(rotation);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
            this.transform.SetPositionAndRotation(position,rotation);
        }
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine)
        {
            position = this.transform.position;
            rotation = this.transform.rotation;
        }
	}
}
