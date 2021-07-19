using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerCharacter;

    public Transform StartPoint;
    public Transform GoalPoint;

    void Start(){
        GameStart();
    }

    public void GameStart(){ // 이걸 실행하면 게임 시작 세팅을 갖추도록 만듦
        PlayerCharacter.transform.position = StartPoint.position;
    }
}
