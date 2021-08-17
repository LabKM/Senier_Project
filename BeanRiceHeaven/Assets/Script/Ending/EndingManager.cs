using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviourPunCallbacks
{
    private BeanRiceHeavenRoomManager _roomManager;

    [Header("- Rank Text")] 
    public Text[] rankTexts = new Text[4];

    private void Awake()
    {
        _roomManager = BeanRiceHeavenRoomManager.Instance;
        rankTexts[0].text = PhotonNetwork.LocalPlayer.NickName;

        int i = 1;
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.NickName == PhotonNetwork.LocalPlayer.NickName)
                continue;
            rankTexts[i++].text = p.NickName;
            Debug.Log(p.NickName + " nick!");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PressQuitButton();
        }
    }

    public void PressQuitButton()
    {
        Application.Quit();
    }
}
