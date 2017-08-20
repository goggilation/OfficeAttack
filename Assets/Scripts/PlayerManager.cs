using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Photon.PunBehaviour {

    /// <summary>
    /// Boolean that tells the game if the player succeded or not.
    /// </summary>
    public bool PlayerSucceded;
    public int attemps;

	// Use this for initialization
	void Start () {
        PlayerSucceded = false;
	}
	
	// Update is called once per frame
	void Update () {
        attemps = GameManager._attempts;
        if(GameManager._attempts == 3){
            Debug.Log("Three attempts done");
        }
	}
}
