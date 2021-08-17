using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPanelAnimation : MonoBehaviour
{
    private Animator _animator;
    public GameObject insideRoomPanel;
    public GameObject lobbyPanel;
    public Image backgroundBlur;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.anyKeyDown)
        {
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1)  || Input.GetKey(KeyCode.Mouse2) )
            {
                return;
            }
            _animator.SetBool("IsKeyDown", true);
            backgroundBlur.color = new Color(0, 0, 0, 0);
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Nickname Entered Animation") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.999f)
        {
            // 닉네임 입력 완료시
            //SceneManager.LoadScene("GeneratedMap");
            if (PhotonNetwork.InLobby)
            {
                BeanRiceHeavenLobbyManager.Instance.OnRoomJoinButtonClicked();
                insideRoomPanel.SetActive(true);
                lobbyPanel.SetActive(false);
            }
        }
        
        
    }
}
