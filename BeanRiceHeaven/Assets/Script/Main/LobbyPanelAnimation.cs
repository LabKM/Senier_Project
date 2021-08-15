using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyPanelAnimation : MonoBehaviour
{
    private Animator _animator;

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
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Nickname Entered Animation") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.999f)
        {
            SceneManager.LoadScene("GeneratedMap");
        }
    }
}
