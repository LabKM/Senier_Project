using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RotateMainBackgroundCircle : MonoBehaviour
{
    private Sequence _sequence;
    
    [Range(0, 20)]
    public float turnMoveTime = 15f;
        
    private void Start()
    {
        _sequence = DOTween.Sequence();
        Vector3 endValue = new Vector3(0f, 0f, 360f);
        _sequence.Append(transform.DOLocalRotate(endValue, turnMoveTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        
        //_sequence.PrependInterval(turnMoveTime / 10.0f);
        _sequence.SetLoops(-1);
        
        _sequence.Play();
    }

    private void OnEnable()
    {
        _sequence.Restart();
    }

    private void OnDisable()
    {
        _sequence.Pause();
    }
}
