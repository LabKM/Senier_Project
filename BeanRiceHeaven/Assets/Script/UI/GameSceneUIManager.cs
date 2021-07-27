using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUIManager : MonoBehaviour
{
    bool isInputable;
    public GameObject MapUI;
    public GameObject MissionUI;
    public GameObject MissionUI_List;
    Vector3 MissionUI_List_Position;

    public GameObject Prefab_Room;

    void Awake(){
        isInputable = true;
    }

    string NameOfRoom(int x, int y){
        return "Room_" + x.ToString() + "_" + y.ToString();
    }

    public void CreateMapByRoom(Room[,] flagMap){
        Transform rooms = MapUI.transform.Find("Rooms").Find("Room");
        
        if(rooms != null){
            DestroyImmediate(rooms.gameObject);
        }
        rooms = new GameObject("Room").transform;
        rooms.parent = MapUI.transform.Find("Rooms");
        rooms.localPosition = Vector3.zero;

        Rect size_rect = Prefab_Room.GetComponent<RectTransform>().rect; 
        Vector3 StartPoint = Vector3.left * flagMap.GetLength(0) * size_rect.width / 2 + Vector3.up * flagMap.GetLength(1) * size_rect.height / 2;

        for(int i = 0; i < flagMap.GetLength(0); ++i)
            for(int j = 0; j < flagMap.GetLength(1); ++j){
                if(flagMap[i, j] != null){
                    RectTransform rT = Instantiate<GameObject>(Prefab_Room, Vector3.zero, Quaternion.identity, rooms).GetComponent<RectTransform>();
                    rT.localPosition = StartPoint + Vector3.right * i * rT.rect.width + Vector3.down * j * rT.rect.height;
                    rT.name = NameOfRoom(i, j);
                }
            }
    }

    public void noticePlayer(int playerId, Vector2Int rc){
        if(playerId >= 4 || playerId < 0){
            Debug.Log("UI:PLlayer Icon:Player ID::player id is wrong");
        }
        Transform aRoom = MapUI.transform.Find("Rooms").Find("Room").Find(NameOfRoom(rc.x, rc.y));
        Transform playerIcon =  MapUI.transform.Find("Icon").Find("PlayerIcon").GetChild(playerId);
        Debug.Log(playerIcon.name);
        playerIcon.position = aRoom.position;

    }

    public void noticeGuard(int guardId, Vector2Int rc){
        
    }

    void Update(){
        if(isInputable){
            if(MissionUI_List != null && Input.GetKeyDown(KeyCode.Tab)){
                RectTransform rectTransform = MissionUI_List.GetComponent<RectTransform>();
                MissionUI_List_Position = rectTransform.localPosition;
                rectTransform.localPosition = Vector3.zero;
            }else if(Input.GetKeyUp(KeyCode.Tab)){
                RectTransform rectTransform = MissionUI_List.GetComponent<RectTransform>();
                rectTransform.localPosition = MissionUI_List_Position;
            }
            if(Input.GetKeyDown(KeyCode.M)){
                MapUI.SetActive(!MapUI.activeSelf); 
            }
        }
    }
}
