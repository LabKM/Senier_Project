using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUIManager : MonoBehaviour
{
    bool isInputable;
    public MapUImanager mapUI;
    public ItemUImanager itemUI;
    Vector3 MissionUI_List_Position;
    Transform[,] miniRoom;
    public GameObject Prefab_Room;

    public Option option;

    void Awake(){
        isInputable = true;
    }

    string NameOfRoom(int x, int y){
        return "Room_" + x.ToString() + "_" + y.ToString();
    }

    public void CreateMiniMapByRoom(Vector2Int widthHeight){
        Transform roomHolder = mapUI.MapHolder;
        
        miniRoom = new Transform[widthHeight.x, widthHeight.y];

        Rect size_rect = Prefab_Room.GetComponent<RectTransform>().rect; 
        Vector3 StartPoint = Vector3.left * (widthHeight.x - 1) * size_rect.width / 2 + Vector3.up * (widthHeight.y - 1)* size_rect.height / 2;

        for(int i = 0; i < widthHeight.x; ++i)
            for(int j = 0; j < widthHeight.y; ++j){
                miniRoom[i,j] = Instantiate<GameObject>(Prefab_Room, Vector3.zero, Quaternion.identity, roomHolder).transform;
                RectTransform rT = miniRoom[i,j].GetComponent<RectTransform>();
                rT.localPosition = StartPoint + Vector3.right * i * rT.rect.width + Vector3.down * j * rT.rect.height;
                rT.name = NameOfRoom(i, j);
            }
    }

    public void FlipMiniMapRoom(Room room){
        Vector2Int rc = room.section;
        Transform mRoom = miniRoom[rc.x, rc.y];
        Image image = mRoom.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("UI/Ingame/Map2/Room/Room" + (room.hallway ? "1" : "0") + ((int)room.style).ToString());
        mRoom.localRotation = Quaternion.Euler(0, 0, -room.minimap_rotation);
    }

    public void noticePlayer(int playerId, Vector2Int rc){
        if(playerId >= 4 || playerId < 0){
            Debug.Log("UI:PLlayer Icon:Player ID::player id is wrong");
        }
        Transform aRoom = miniRoom[rc.x, rc.y];
        Transform playerIcon =  mapUI.MapIcon.Find("Player Icon").GetChild(playerId);
        playerIcon.position = aRoom.position;
    }

    public void noticeGuard(int guardId, Vector2Int rc){
        
    }

    void Update(){
        if(isInputable){
            // if(MissionUI_List != null && Input.GetKeyDown(KeyCode.Tab)){
            //     RectTransform rectTransform = MissionUI_List.GetComponent<RectTransform>();
            //     MissionUI_List_Position = rectTransform.localPosition;
            //     rectTransform.localPosition = Vector3.zero;
            // }else if(Input.GetKeyUp(KeyCode.Tab)){
            //     RectTransform rectTransform = MissionUI_List.GetComponent<RectTransform>();
            //     rectTransform.localPosition = MissionUI_List_Position;
            // }
            if(Input.GetKeyDown(KeyCode.M)){
                mapUI.OnButton();
            }
        }
    }
}
