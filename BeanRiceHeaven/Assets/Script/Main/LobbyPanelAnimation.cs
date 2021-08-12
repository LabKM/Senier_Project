using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
