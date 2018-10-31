// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer { 
    /// <summary>
    /// a class that will handle the connect screen 
    /// </summary>
    public class Launcher : Photon.PunBehaviour
    {
        #region Public Variables

        /// <summary>
        /// what photon related stuff to log
        /// </summary>
        public PhotonLogLevel LogLevel = PhotonLogLevel.Informational;

        /// <summary>
        /// maximum number of players per room
        /// </summary>
        public byte MaxPlayersPerRoom = 4;

        /// <summary>
        /// connect UI panel
        /// </summary>
        public GameObject controlPanel;

        /// <summary>
        /// progress label when trying to connect
        /// </summary>
        public GameObject progressLabel;

        #endregion

        #region Private Variables

        /// <summary>
        /// instances can only join the same session if they have the same gameversion
        /// </summary>
        string _gameVersion = "1";

        /// <summary>
        /// whether we're trying to connect
        /// </summary>
        bool isConnecting;

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            // set default  photon parameters
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true; // when master client switches scene, other clients will switch as well
            PhotonNetwork.logLevel = LogLevel;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// interface to connect to photon server
        /// </summary>
        public void Connect()
        {
            isConnecting = true;
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            // try to join a random room if we're already connected
            if (PhotonNetwork.connected)
                PhotonNetwork.JoinRandomRoom();
            else // connect using the default setting and gameversion if we're not connected
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }

        #endregion

        #region Photon.PunBehaviour Callbacks

        /// <summary>
        /// called when we're connected to master
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("Launcher: OnConnectedToMaster() was called by PUN");
            if(isConnecting)
                PhotonNetwork.JoinRandomRoom();
        }

        /// <summary>
        /// called if we're disconnected
        /// </summary>
        public override void OnDisconnectedFromPhoton()
        {
            // bring back the connect UI panel
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            Debug.LogWarning("Launcher: OnDisconnectedFromPhoton() was called by PUN");
        }

        /// <summary>
        /// called when we fail to join a random room
        /// </summary>
        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            // try to create a new room
            Debug.Log("Launcher: OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCaling: PhotonNetwork.CreateRoom(nukk, new RoomOptions(){maxPlayers = 4}, null);");
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        }

        /// <summary>
        /// called when we successfully join a room
        /// </summary>
        public override void OnJoinedRoom()
        {
            // load a room for 1 client
            Debug.Log("We load the 'Room for 1'");
            PhotonNetwork.LoadLevel(1);
        }

        #endregion
    }
}
