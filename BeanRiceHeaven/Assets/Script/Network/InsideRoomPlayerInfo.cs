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
    public GameObject playerObject;
    public GameObject readyImage;
    public GameObject notReadyImage;

    private int _clientID;
    public bool isReady = false;
    [HideInInspector] public int playerNum;

    private void Awake()
    {
        //PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
    }

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != _clientID)
        {
            //playerReadyButton.gameObject.SetActive(false);
            // playerNickName.color = new Color32(176, 59, 69, 255);
            return;
        }
        Hashtable playerStatus = new Hashtable() {{"ReadyStatus", isReady}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerStatus);
        
        //playerReadyButton.onClick.AddListener(PressReadyButton);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != _clientID)
        {
            return;
        }
        Hashtable playerStatus = new Hashtable() {{"ReadyStatus", isReady}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerStatus);
        
        //playerReadyButton.onClick.AddListener(PressReadyButton);
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
        Debug.Log(gameObject + " [" + playerNickName.text + "] SetPlayerReady()");
        isReady = isPlayerReady;

        if (isReady)
        {
            readyImage.SetActive(true);
            notReadyImage.SetActive(false);
        }
        else
        {
            readyImage.SetActive(false);
            notReadyImage.SetActive(true);
        }
    }

    public void PressReadyButton()
    {
        SetPlayerReady(!isReady);
        Hashtable readyStatus = new Hashtable() {{"ReadyStatus", isReady}};
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
