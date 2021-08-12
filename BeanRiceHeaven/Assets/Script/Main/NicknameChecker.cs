using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NicknameChecker : MonoBehaviour
{
    public Animator animator;
    private InputField nicknameField;

    private void Awake()
    {
        nicknameField = GetComponent<InputField>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nicknameField.text.Length == 0)
        {
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            animator.SetBool("IsNicknameEntered", true);
        }
    }
}
