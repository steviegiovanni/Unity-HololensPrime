using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : Photon.PunBehaviour {
    public GameObject HUDWelcomePanel;
    public GameObject BodyLockedAnchorSharingPanel;
    public GameObject ControlPanel;
    public GameObject OpenAnchorSharingBtn;
    public GameObject CloseAnchorSharingBtn;
	
	// Update is called once per frame
	void Update () {
        /*if (!PhotonNetwork.inRoom)
        {
            HUDWelcomePanel.SetActive(true);
            ControlPanel.SetActive(false);
            return;
        }
        else
        {
            HUDWelcomePanel.SetActive(false);
            ControlPanel.SetActive(true);
        }*/
	}

    public void Start()
    {
        HUDWelcomePanel.SetActive(true);
        ControlPanel.SetActive(false);
        BodyLockedAnchorSharingPanel.SetActive(false);
        CloseAnchorSharingBtn.SetActive(true);
        OpenAnchorSharingBtn.SetActive(false);
    }

    public void OpenAnchorSharingPanel()
    {
        BodyLockedAnchorSharingPanel.SetActive(true);
        CloseAnchorSharingBtn.SetActive(true);
        OpenAnchorSharingBtn.SetActive(false);
    }

    public void CloseAnchorSharingPanel()
    {
        BodyLockedAnchorSharingPanel.SetActive(false);
        CloseAnchorSharingBtn.SetActive(false);
        OpenAnchorSharingBtn.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        HUDWelcomePanel.SetActive(false);
        ControlPanel.SetActive(true);
        BodyLockedAnchorSharingPanel.SetActive(true);
        CloseAnchorSharingBtn.SetActive(true);
        OpenAnchorSharingBtn.SetActive(false);
    }

    public override void OnDisconnectedFromPhoton()
    {
        HUDWelcomePanel.SetActive(true);
        ControlPanel.SetActive(false);
        BodyLockedAnchorSharingPanel.SetActive(false);
    }
}
