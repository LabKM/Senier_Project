using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class BeanRiceHeavenLobbyManager : MonoBehaviourPunCallbacks
{
    private static BeanRiceHeavenLobbyManager instance = null;

    public static BeanRiceHeavenLobbyManager Instance
    {
        get
        {
            if (!instance)
                return null;
            return instance;
        }
    }
    
    private string _gameVersion = "1.00";
    private RoomInfo[] _rooms;

    [Header("- Lobby Panel")] 
    public GameObject lobbyPanel;
    public InputField nickNameInputField;
    
    [Header("- Inside Room Panel")] 
    public GameObject roomPanel;
    public GameObject backgroundCanvas;
    public GameObject readyButton;
    public GameObject startGameButton;
    public GameObject[] roomPlayerPrefabs = new GameObject[4];
    public GameObject insideRoomPlayerPrefab;

    private Dictionary<int, GameObject> _insideRoomPlayerEntries;
    public Dictionary<int, GameObject> InsideRoomPlayerEntries => _insideRoomPlayerEntries;


    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        if (nickNameInputField.text.Length == 0)
        {
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Return) && PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
        {
            //OnJoinButtonClicked();
        }
    }
    
    public override void OnConnectedToMaster() 
    {     
        Debug.Log("Player has connected to the Photon Master Server"); 
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Join Lobby");
        //joinButton.GetComponent<Button>().interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnJoinButtonClicked()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)
            return;
        PhotonNetwork.LocalPlayer.NickName = nickNameInputField.text;
        
        int randomRoomName = 1;
        RoomOptions roomOptions = new RoomOptions() { IsOpen = true, IsVisible = true, MaxPlayers = 4};

        PhotonNetwork.JoinOrCreateRoom("Room" + randomRoomName, roomOptions, TypedLobby.Default);
    }

    public void OnExitRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    #region ROOM_FUNCTION
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed, there must already be a room with the same name");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions() { IsOpen = true, IsVisible = true, MaxPlayers = 4};
        PhotonNetwork.JoinOrCreateRoom("Room" + randomRoomName, roomOptions, TypedLobby.Default);
    }
    
    public override void OnJoinedRoom()
    {
        roomPanel.SetActive(true);
        backgroundCanvas.SetActive(true);
        lobbyPanel.SetActive(false);

        if (_insideRoomPlayerEntries == null)
        {
            _insideRoomPlayerEntries = new Dictionary<int, GameObject>();
        }

        int i = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            Debug.Log("OnJoinedRoom: " + i + " num Player Add");
            //GameObject enterPlayer = Instantiate(insideRoomPlayerPrefab, roomPanel.transform, false);
            
            InsideRoomPlayerInfo entryPlayerInfo = roomPlayerPrefabs[i].GetComponent<InsideRoomPlayerInfo>();
            
            entryPlayerInfo.playerObject.transform.localPosition = new Vector3(entryPlayerInfo.playerObject.transform.localPosition.x,
                20, entryPlayerInfo.playerObject.transform.localPosition.z);
            
            roomPlayerPrefabs[i].SetActive(true);

            //enterPlayer.transform.localPosition = new Vector3(-1000 + (p.ActorNumber) * 400, 0, 0);  
            entryPlayerInfo.InitRoomPlayer(p.ActorNumber, p.NickName);

            object readyData;
            if (p.CustomProperties.TryGetValue("ReadyStatus", out readyData))
            {
                entryPlayerInfo.SetPlayerReady((bool)readyData);
            }

            _insideRoomPlayerEntries.Add(p.ActorNumber, roomPlayerPrefabs[i++]);
        }
        Debug.Log("On Joined Room Function End");
    }

    public override void OnLeftRoom()
    {

        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        foreach (GameObject g in roomPlayerPrefabs)
        {
            //g.SetActive(false);
        }
        
        foreach (GameObject p in _insideRoomPlayerEntries.Values)
        {
            p.SetActive(false);
        }
        
        _insideRoomPlayerEntries.Clear();
        _insideRoomPlayerEntries = null;
    }
    
    public void PressGameStart()
    {
        SceneManager.LoadScene("BackgroundTest");
    }
    
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //GameObject enterPlayer = Instantiate(insideRoomPlayerPrefab, roomPanel.transform, false);
        //roomPlayerPrefabs[newPlayer.GetPlayerNumber()].SetActive(false);
        InsideRoomPlayerInfo entryPlayerInfo = null;
        int i = -1;
        Debug.Log("Call OnPlayerEnteredRoom");
        foreach (GameObject g in roomPlayerPrefabs)
        {
            i++;
            if (_insideRoomPlayerEntries.ContainsValue(g))
            {
                Debug.Log(i + "num Player Joined");
                continue;
            } 
            entryPlayerInfo = roomPlayerPrefabs[i].GetComponent<InsideRoomPlayerInfo>();
            entryPlayerInfo.playerObject.transform.localPosition = new Vector3(entryPlayerInfo.playerObject.transform.localPosition.x,
                20, entryPlayerInfo.playerObject.transform.localPosition.z);
            roomPlayerPrefabs[i].SetActive(true);
            break;
        }
        Debug.Log("Now " + i + "num Player Join!!");
        //InsideRoomPlayerInfo entryPlayerInfo = roomPlayerPrefabs[newPlayer.GetPlayerNumber()].GetComponent<InsideRoomPlayerInfo>();
        //Debug.Log(newPlayer.NickName + " : " + i);
        if(entryPlayerInfo != null)
            entryPlayerInfo.InitRoomPlayer(newPlayer.ActorNumber, newPlayer.NickName);
        
        _insideRoomPlayerEntries.Add(newPlayer.ActorNumber, roomPlayerPrefabs[i]);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Destroy(_insideRoomPlayerEntries[otherPlayer.ActorNumber].gameObject);
        _insideRoomPlayerEntries[otherPlayer.ActorNumber].gameObject.SetActive(false);
        _insideRoomPlayerEntries.Remove(otherPlayer.ActorNumber);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (_insideRoomPlayerEntries == null)
        {
            _insideRoomPlayerEntries = new Dictionary<int, GameObject>();
        }
        
        GameObject targetPlayerInfo;
        if (_insideRoomPlayerEntries.TryGetValue(targetPlayer.ActorNumber, out targetPlayerInfo))
        {
            object readyData;
            if (changedProps.TryGetValue("ReadyStatus", out readyData))
            {
                targetPlayerInfo.GetComponent<InsideRoomPlayerInfo>().SetPlayerReady((bool)readyData);
            }
        }
    }

    #endregion
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
