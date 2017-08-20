using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : Photon.PunBehaviour
{

    /// <summary>
    /// This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
    /// </summary>
    string _gameVersion = "1";

	[Tooltip("The Ui Panel to let the user enter name, connect and play")]
	public GameObject controlPanel;
	[Tooltip("The UI Label to inform the user that the connection is in progress")]
	public GameObject progressLabel;

	/// <summary>
	/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
	/// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
	/// Typically this is used for the OnConnectedToMaster() callback.
	/// </summary>
	bool isConnecting;

#region Public variables
    public PhotonLogLevel LogLevel = PhotonLogLevel.Informational;

	[Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
	public byte MaxPlayersPerRoom = 5;
#endregion

    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Awake()
    {
        progressLabel.SetActive(false);
        //#NotImportant
        //Force LogLevel
        PhotonNetwork.logLevel = LogLevel;

        //#Critical
        //we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        //#Critical
        //this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;
    }
    // Use this for initialization
    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt to join a random room
    /// - If not yet connected, connect this application instance to Photon Cloud
    /// </summary>
    public void Connect()
    {
        isConnecting = true;
        progressLabel.GetComponent<Text>().text = "Connecting...";
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        if (PhotonNetwork.connected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    #region Photon.PunBehaviour Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Office Attack/Launcher: OnConnectedToMaster() was called by PUN");
		progressLabel.GetComponent<Text>().text = "Connected!";
        if (isConnecting)
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
            PhotonNetwork.JoinRandomRoom();
        }

    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("Office Attack/Launcher: OnDisconnectedFromServer() was called by PUN");
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        base.OnPhotonRandomJoinFailed(codeAndMsg);
        Debug.Log("Office Attack/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");

		// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
		PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom}, null);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Office Attack / Launcher: OnJoinedRoom was called by PUN");

        if(PhotonNetwork.room.PlayerCount == 1)
        {
            Debug.Log("First person here! Loading level");

			PhotonNetwork.LoadLevel(1);
			progressLabel.SetActive(false);
        }
    }

    #endregion
}
