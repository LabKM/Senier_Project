using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InsideRoomPlayerInfo : MonoBehaviour
{
    [Header("- UI")] 
    public Text playerNickName;
    public Text playerReadyText;
    public GameObject playerObject;
    public Button playerReadyButton;

    private int _clientID;
    private bool _isPlayerReady = false;
    [HideInInspector] public int playerNum;

    private void Awake()
    {
        //PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
    }

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != _clientID)
        {
            playerReadyButton.gameObject.SetActive(false);
            // playerNickName.color = new Color32(176, 59, 69, 255);
        }
        Hashtable playerStatus = new Hashtable() {{"ReadyStatus", _isPlayerReady}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerStatus);
        
        playerReadyButton.onClick.AddListener(PressReadyButton);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != _clientID)
        {
            playerReadyButton.gameObject.SetActive(false);
            // playerNickName.color = new Color32(176, 59, 69, 255);
        }
        Hashtable playerStatus = new Hashtable() {{"ReadyStatus", _isPlayerReady}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerStatus);
        
        playerReadyButton.onClick.AddListener(PressReadyButton);
    }
    
    private void OnDestroy()
    {
        //PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
    }
    
    public void InitRoomPlayer(int playerID, string playerName)
    {
        _clientID = playerID;
        playerNickName.text = playerName;
    }

    public void SetPlayerReady(bool isPlayerReady)
    {
        _isPlayerReady = isPlayerReady;
        playerReadyText.text = _isPlayerReady ? "Ready" : "Not Ready";
        
        //Color32 readyColor = new Color32(66, 236, 41, 255);
        //Color32 notReadyColor = new Color32(219, 219, 219, 200);
        //playerReadyText.color = _isPlayerReady ? readyColor : notReadyColor;
    }

    public void PressReadyButton()
    {
        _isPlayerReady = !_isPlayerReady;
        SetPlayerReady(_isPlayerReady);
        Hashtable readyStatus = new Hashtable() {{"ReadyStatus", _isPlayerReady}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(readyStatus);
    }

    
    
    // void OnPlayerNumberingChanged()
    // {
    //     if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == -1)
    //         return;
    //     foreach (Player p in PhotonNetwork.PlayerList)
    //     {
    //         Debug.Log("Player: " + p.NickName + " and " + p.GetPlayerNumber());
    //         if (BeanRiceHeavenLobbyManager.Instance.InsideRoomPlayerEntries.ContainsKey(p.ActorNumber))
    //         {
    //             BeanRiceHeavenLobbyManager.Instance.InsideRoomPlayerEntries[p.ActorNumber].transform.localPosition =
    //                 new Vector3(-1000 + (p.GetPlayerNumber() + 1) * 400, 0, 0);  
    //         }
    //     }
    // }
}
