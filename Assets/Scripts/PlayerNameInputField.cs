using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour {

    static string playerNamePrefKey = "PlayerName";
	// Use this for initialization
	void Start () 
    {
        string defaultName = "";
        InputField _inputField = this.GetComponent<InputField>();
        if(_inputField != null)
        {
            if(PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }

        PhotonNetwork.playerName = defaultName;
	}
	
    /// <summary>
    /// Sets the name of the player.
    /// </summary>
    /// <param name="value">Name of player.</param>
    public void SetPlayerName(string value)
    {
        PhotonNetwork.playerName = value + " "; //Ett blanksteg bakom ifall userName är tomt. En tom sträng uppdaterar inte fältet i prefs

        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
}
