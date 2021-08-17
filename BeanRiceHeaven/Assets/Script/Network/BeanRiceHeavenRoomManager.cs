using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BeanRiceHeavenRoomManager : MonoBehaviourPunCallbacks
{
    private static BeanRiceHeavenRoomManager instance = null;

    public static BeanRiceHeavenRoomManager Instance
    {
        get
        {
            if (!instance)
                return null;
            return instance;
        }
    }

    [Header("- Lobby Panel")]
    public GameObject lobbyPanel;
    
    [Header("- Room Panel")] 
    public GameObject roomPanel;
    public GameObject readyButton;
    public GameObject waitButton;
    public Animator readyAnimator;
    
    [Header("- Room Info Text")]
    public Text currRoomNameText;
    public Text readyPlayersText;

    [Header("Players")] 
    public GameObject[] roomPlayers = new GameObject[4];
    private Dictionary<int, GameObject> insideRoomPlayerEntries;

    private bool testCheatKey = false;

    void Awake()
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
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!readyPlayersText)
            return;
        if(readyPlayersText.gameObject.activeSelf && PhotonNetwork.InRoom)
            readyPlayersText.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        if (PhotonNetwork.InRoom && Input.GetKeyDown(KeyCode.F12))
        {
            readyAnimator.SetBool("IsAllReady", true);
            testCheatKey = true;
        }

        if ((PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 4) || (testCheatKey))
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (testCheatKey)
                    break;
                
                object readyData;
                if (p.CustomProperties.TryGetValue("ReadyStatus", out readyData))
                {
                    if (!(bool)readyData)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            
            readyAnimator.SetBool("IsAllReady", true);
            if (readyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Ready Count Animation") &&
                readyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            {
                Debug.Log("AllReady!!");
                LoadingSceneManager.LoadNextScene("GeneratedMap");
            }
        }
    }

    public void LocalPlayerPressReadyButton()
    {
        InsideRoomPlayerInfo localPlayerInfo = insideRoomPlayerEntries[PhotonNetwork.LocalPlayer.ActorNumber]
            .GetComponent<InsideRoomPlayerInfo>();

        localPlayerInfo.PressReadyButton();
        readyButton.SetActive(!localPlayerInfo.isReady);
        waitButton.SetActive(localPlayerInfo.isReady);
    }
    
    #region Photon Function

    public override void OnJoinedRoom()
    {
        roomPanel.SetActive(true);
        currRoomNameText.text = PhotonNetwork.CurrentRoom.Name;
       
        insideRoomPlayerEntries ??= new Dictionary<int, GameObject>();

        int i = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            InsideRoomPlayerInfo entryPlayerInfo = roomPlayers[i].GetComponent<InsideRoomPlayerInfo>();
            roomPlayers[i].SetActive(true);
            entryPlayerInfo.InitRoomPlayer(p.ActorNumber, p.NickName);
            
            // Is Player Ready
            object readyData;
            if (p.CustomProperties.TryGetValue("ReadyStatus", out readyData))
            {
                Debug.Log((bool)readyData + " OnJoinedRoom 닉:" + p.NickName );
                entryPlayerInfo.SetPlayerReady((bool)readyData);
            }

            insideRoomPlayerEntries.Add(p.ActorNumber, roomPlayers[i++]);
        }
        Debug.Log("On Joined Room Function End");
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        foreach (GameObject g in insideRoomPlayerEntries.Values)
        {
            g.SetActive(false);
        }
        
        insideRoomPlayerEntries.Clear();
        insideRoomPlayerEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("다른 플레이어 엔터 룸 업데이트");
        InsideRoomPlayerInfo newEntryPlayerInfo = null;

        int i = -1;
        foreach (GameObject g in roomPlayers)
        {
            i++;
            if (insideRoomPlayerEntries.ContainsValue(g))
            {
                Debug.Log(g.GetComponent<InsideRoomPlayerInfo>().playerNickName.text + " 엔터룸 패스");
                continue;
            }
            
            newEntryPlayerInfo = roomPlayers[i].GetComponent<InsideRoomPlayerInfo>();
            roomPlayers[i].SetActive(true);
            
            break;
        }
        
        if(newEntryPlayerInfo != null)
            newEntryPlayerInfo.InitRoomPlayer(newPlayer.ActorNumber, newPlayer.NickName);
        
        insideRoomPlayerEntries.Add(newPlayer.ActorNumber, roomPlayers[i]);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        insideRoomPlayerEntries[otherPlayer.ActorNumber].gameObject.SetActive(false);
        insideRoomPlayerEntries.Remove(otherPlayer.ActorNumber);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("프로퍼티 업데이트");
        insideRoomPlayerEntries ??= new Dictionary<int, GameObject>();

        GameObject targetPlayerInfo;
        if (!insideRoomPlayerEntries.TryGetValue(targetPlayer.ActorNumber, out targetPlayerInfo)) return;
        
        object isPlayerReady;
        if (changedProps.TryGetValue("ReadyStatus", out isPlayerReady))
        {
            Debug.Log(targetPlayerInfo.GetComponent<InsideRoomPlayerInfo>().playerNickName.text + " " +
                      (bool)isPlayerReady);
            targetPlayerInfo.GetComponent<InsideRoomPlayerInfo>().SetPlayerReady((bool)isPlayerReady);
        }
    }

    #endregion
}
