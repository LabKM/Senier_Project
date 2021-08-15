using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEditor.AI;

using Coord = UnityEngine.Vector2Int;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{ get; private set;}
    public GameSceneUIManager uiManager;
    public GameObject PlayerCharacter;
    public GameObject Treasure;

    public Coord StartPoint;
    public Coord GoalPoint;

    List<LivingEntity> hpList;
    [HideInInspector]
    public Room[,] roomMap;
    public Vector3 StartPosition { get; set; }
    public Vector3 TileSize { get; set; }

    public Text timer;
    public float limitTime = 900;

    private Coord lastNowRoomCoord;

    NavMeshSurface surface;

    void Awake()
    {
        hpList = new List<LivingEntity>();
        MapGenerator map = GetComponent<MapGenerator>();
        map.gameManager = this;
        map.GenerateMap();
        if(GameManager.Instance != null && GameManager.Instance != this){
            DestroyImmediate(GameManager.Instance);    
        }
        Instance = this;
    }

    void Start()
    {
        GameStart();
    }

    public void GameStart()
    { // 이걸 실행하면 게임 시작 세팅을 갖추도록 만듦
        PlayerCharacter.transform.position = CoordToVector(StartPoint.x, StartPoint.y);
        StartCoroutine("TimeOver", limitTime);
    }

    void Update()
    {
        // 시간 화긴
        limitTime -= Time.deltaTime;
        int limitTime_min = (int)(limitTime / 60);
        int limitTime_sec = (int)Mathf.Round(limitTime % 60);

        string timeStr;
        timeStr = limitTime_min.ToString() + ":" + limitTime_sec.ToString();
        timer.text = timeStr;

        //플레이어의 현재 위치 추적
        Coord nowRoomCoord_player = VectorToCoord(PlayerCharacter.transform.position);
        if (lastNowRoomCoord != nowRoomCoord_player)
        {
            uiManager.FlipMiniMapRoom(roomMap[nowRoomCoord_player.x, nowRoomCoord_player.y]);
            uiManager.noticePlayer(0, nowRoomCoord_player);

            // 시작 지점에 있을 때 보물을 가지고 있는가 확인
            BeanController PlayerBean = PlayerCharacter.GetComponent<BeanController>();
            if (!PlayerBean.inventory.isHandEmpty)
            {
                Debug.Log("Get Item");
                if (nowRoomCoord_player == StartPoint && PlayerBean.inventory.isTreasure){
                    Debug.Log("Get Item in goal");
                    //uiManager.option.GameClear();
                }
         }

            lastNowRoomCoord = nowRoomCoord_player;
        }
    }

    public Coord VectorToCoord(Vector3 position)
    {
        Vector3 temp = (position - StartPosition + TileSize / 2);
        Coord result = new Coord(Mathf.FloorToInt(temp.x / TileSize.x), -1 * Mathf.FloorToInt(temp.z / TileSize.z));
        return result;
    }

    public Vector3 CoordToVector(int x, int y)
    {
        return StartPosition + TileSize.x * x * Vector3.right + Vector3.back * TileSize.z * y;
    }

    IEnumerator TimeOver(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        uiManager.option.GameOver();
        yield return null;
    }
}
