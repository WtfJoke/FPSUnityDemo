using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkLauncher : Photon.PunBehaviour
{

    #region Public Variables

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    public GameObject controlPanel;

    [Tooltip("The Ui Text to inform the user about the connection progress")]
    public Text feedbackText;

    [Tooltip("The maximum number of players per room")]
    public byte maxPlayersPerRoom = 4;

    [Tooltip("The UI Loader Anime")]
    public LoaderAnime loaderAnime;

    #endregion

    #region Private Variables
    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;


    /// <summary>
    /// This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
    /// </summary>
    string _gameVersion = "1";


    #endregion


    #region MonoBehaviour CallBacks


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {


        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;


        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;
    }


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        feedbackText.gameObject.SetActive(false);
        controlPanel.SetActive(true);
    }


    #endregion


    #region Public Methods


    /// <summary>
    /// Start the connection process. 
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        feedbackText.gameObject.SetActive(true);
        controlPanel.SetActive(false);
        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        isConnecting = true;
        if (loaderAnime != null)
        {
            loaderAnime.StartLoaderAnimation();
        }

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.connected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            PhotonNetwork.JoinOrCreateRoom("MasterDisasterHL4", new RoomOptions() { MaxPlayers = 4 }, null);
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    #region Photon.PunBehaviour CallBacks


    public override void OnConnectedToMaster()
    {
        Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
        if (isConnecting)
        {

            bool joined = PhotonNetwork.JoinOrCreateRoom("MasterDisasterHL4", new RoomOptions() { MaxPlayers = 4 }, null);

            string joinText = "join:" + joined;
            Debug.Log("blabla");
        }
    }


    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");
        feedbackText.gameObject.SetActive(false);
        controlPanel.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("created room");
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");


            // #Critical
            // Load the Room Level. 
            PhotonNetwork.LoadLevel("main");
        }
        Debug.Log("joined room");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("left room");
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
        }
    }


    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
        }
    }

    #endregion


    #endregion
}
