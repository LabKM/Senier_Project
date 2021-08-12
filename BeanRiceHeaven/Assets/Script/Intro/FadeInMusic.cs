using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInMusic : MonoBehaviour
{
    private AudioSource audioSource;
    public float fadeTime = 1f;
    private bool isFadeIn = true;
    private float deltaFadeTime = 0.0f;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isFadeIn)
        {
            deltaFadeTime += Time.deltaTime;
            if (deltaFadeTime > fadeTime)
            {
                isFadeIn = false;
            }
            audioSource.volume = (deltaFadeTime / fadeTime);
        }
    }
}
