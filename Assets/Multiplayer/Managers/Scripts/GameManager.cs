// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer
{
    /// <summary>
    /// the game manager
    /// - handles instantiating player
    /// - leaving room
    /// - switching to a room for bigger number of players
    /// </summary>
    public class GameManager : Photon.PunBehaviour
    {
        #region public variables

        /// <summary>
        /// the player prefab to instantiate
        /// </summary>
        public GameObject playerPrefab;

        #endregion

        #region monobehaviour callbacks

        private void Start()
        {
            if (playerPrefab == null)
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            else
            {
                // if we don't have a player representation yet, instantiate a player for us
                if (Player.LocalPlayerInstance == null && PhotonNetwork.connected)
                {
                    Debug.Log("We are Instantiating LocalPlayer from " + SceneManager.GetActiveScene().name);
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
            }
        }

        #endregion

        #region Photon Messages

        /// <summary>
        /// called when a new player join the room room
        /// </summary>
        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            // if we're the master client, call load arena to load an arena that caters to the number of players in the room
            Debug.Log("OnPhotonPlayerConnected() " + newPlayer.NickName);
            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
                LoadArena();
            }
        }

        /// <summary>
        /// called when a player leaves the room
        /// </summary>
        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            // downgrade to a smaller room
            Debug.Log("OnPhotonPlayerDisconnected() " + otherPlayer.NickName);
            if (PhotonNetwork.isMasterClient)
                Debug.Log("OnPhotonPlayerDisconnected isMasterClient " + PhotonNetwork.isMasterClient);
            LoadArena();
        }

        /// <summary>
        /// when we ourself leave the room
        /// </summary>
        public override void OnLeftRoom()
        {
            // load the launcher scene
            SceneManager.LoadScene(0);
        }
        #endregion

        #region Public Methods

        // interface to leave room
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods

        // load a room that accomodates to the number of players
        void LoadArena()
        {
            /*if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork: Trying to load a level but we are not the master Client");
            }
            Debug.Log("PhotonNetwork: Loading Level: " + PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel(1);*/
        }

        #endregion
    }
}
