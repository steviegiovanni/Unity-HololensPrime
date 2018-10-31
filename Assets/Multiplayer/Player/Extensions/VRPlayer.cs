// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRUtilities;

namespace Multiplayer
{
    /// <summary>
    /// represents a player in a VR multiplayer session
    /// </summary>
    public class VRPlayer : Player
    {
        public List<DeviceTracker> _deviceTrackers;

        // Use this for initialization
        void Start()
        {
            if (photonView.isMine == true && PhotonNetwork.connected == true)
            {
                foreach (var deviceTracker in _deviceTrackers)
                    deviceTracker.TrackDevice();
            }
        }
    }
}
