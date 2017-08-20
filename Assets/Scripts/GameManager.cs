using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour
{
    public static GameManager Instance;
    public static int _attempts, correctAttempts;

    public void Start()
    {
        Instance = this;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void LoadArena()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("Trying to load level, but this client is not master.");
        }

        Debug.Log("PhotonNetwork : Loading Level With current amount of players : " + PhotonNetwork.room.PlayerCount);
        PhotonNetwork.LoadLevel(1);
    }

    #region Photon Callbacks
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        base.OnPhotonPlayerConnected(newPlayer);
        Debug.Log("OnPhotonPlayerConnected() called. " + newPlayer.NickName);

        if(PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected is master client: " + PhotonNetwork.isMasterClient);

            LoadArena();
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);
		Debug.Log("OnPhotonPlayerDisconnected() called. " + otherPlayer.NickName);

		if (PhotonNetwork.isMasterClient)
		{
			Debug.Log("OnPhotonPlayerDisonnected is master client: " + PhotonNetwork.isMasterClient);

			LoadArena();
		}
    }

	/// <summary>
	/// Called when the local player left the room. We need to load the launcher scene.
	/// </summary>
	public override void OnLeftRoom()
	{
		Debug.Log("Left a room");
		SceneManager.LoadScene(0);
	}


    #endregion
}
