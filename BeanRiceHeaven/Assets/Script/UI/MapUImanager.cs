using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUImanager : MonoBehaviour
{
    public bool nowVisible{ get; private set; }
    Transform holder;
    public Transform MapHolder{ get{  
        if(holder == null)
            holder = Rooms.Find("Room");
        return holder; 
        } 
    }
    public Transform PlayerIcon{ get{
        return MapIcon.Find("Player Icon").GetChild(0);
        }
    }

    public Transform MapIcon;
    public Transform Background;
    public Transform Rooms;
    public Transform MiniMap;
    public Transform MiniMapMask;

    Transform[,] minimap_Rooms;

    public void Awake(){
        Invisible();
    }

    public void RecreateMapHolder(){
        if(MapHolder != null){
            DestroyImmediate(MapHolder.gameObject);
        }
        
        holder = new GameObject("Room").transform;
        holder.parent = Rooms;
        holder.localPosition = Vector3.zero;
    }

    public void OnButton(){
        if(nowVisible)
            Invisible();
        else
            Visible();
    }

    public void Invisible(){
        nowVisible = false;
        Background.gameObject.SetActive(nowVisible);
        MiniMap.gameObject.SetActive(!nowVisible);

        Rooms.parent = MiniMapMask;
        Rooms.localPosition = PlayerIcon.localPosition / -2; // 임시 미니맵 중심을 플레이어로 바꾸어야 함
        Rooms.localScale = Vector3.one / 2;
    }

    public void Visible(){
        nowVisible = true;
        Background.gameObject.SetActive(nowVisible);
        MiniMap.gameObject.SetActive(!nowVisible);

        Rooms.parent = transform;
        Rooms.localPosition = Vector3.zero; 
        Rooms.localScale = Vector3.one;
    }
}
