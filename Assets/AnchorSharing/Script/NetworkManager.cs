// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// a class that will handle the connecting to photon
    /// </summary>
    public class NetworkManager : Photon.PunBehaviour
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

        public GameObject ConnectionIndicator;

        /// <summary>
        /// connect UI panel
        /// </summary>
        public GameObject controlPanel;

        /// <summary>
        /// the name of the room to join
        /// </summary>
        public string roomName = "DefaultRoom";

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
            ConnectionIndicator.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
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
            ConnectionIndicator.GetComponent<Renderer>().material.color = new Color(1, 1, 0);
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
            if(isConnecting)
                PhotonNetwork.JoinRoom(roomName);
        }

        /// <summary>
        /// called if we're disconnected
        /// </summary>
        public override void OnDisconnectedFromPhoton()
        {
            ConnectionIndicator.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        }


        /// <summary>
        /// called when we fail to join a random room
        /// </summary>
        public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
            // try to create a new room
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        }

        /// <summary>
        /// called when we successfully join a room
        /// </summary>
        public override void OnJoinedRoom()
        {
            ConnectionIndicator.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
        }

        #endregion
    }
