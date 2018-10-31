// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
    /// <summary>
    /// represents a player in a multiplayer session
    /// </summary>
    public class Player : Photon.MonoBehaviour
    {
        /// <summary>
        /// save an instance of ourself
        /// </summary>
        public static GameObject LocalPlayerInstance;

        private void Awake()
        {
            if (photonView.isMine)
            {
                Player.LocalPlayerInstance = this.gameObject;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
