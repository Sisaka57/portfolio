using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class PhotonConnection : MonoBehaviourPunCallbacks
{

    public static PhotonConnection instance;

    public InputField PlayerNameInput;

    public Button StartGameButton;
    public GameObject PlayerListEntryPrefab;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;


    public PhotonView photonView;

    public void Awake()
    {

        photonView = GetComponent<PhotonView>();
        instance = this;

        PhotonNetwork.AutomaticallySyncScene = true;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        //PlayerNameInput.text = "Player " + Random.Range(1000, 10000);


    }

    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
    }

    public void TwoPlayersClicked()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        //SetActivePanel(JoinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions { MaxPlayers = 2 };

        PhotonNetwork.CreateRoom(roomName, options, null);


        Debug.Log("Joining Random room failed creating a new room with name " + roomName);
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {

            photonView.RPC("startBackGammonScene", RpcTarget.AllViaServer, null);
        }

    }

    [PunRPC]
    public void startBackGammonScene()
    {
        Debug.Log("Room Full");
        DataManager.DataManagerClass.two_playerOnline();
    }


    //public override void 
    // Update is called once per frame
    void Update()
    {
        
    }
}
