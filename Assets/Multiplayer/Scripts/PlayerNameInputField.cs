using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Variables

        static string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour Callbacks
        

        // Use this for initialization
        void Start()
        {
            string defaultName = "";
            InputField _inputField = this.GetComponent<InputField>();
            if(_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.playerName = defaultName;
        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

        #region Public Methods

        public void SetPlayerName(string value)
        {
            PhotonNetwork.playerName = value + " ";
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}
