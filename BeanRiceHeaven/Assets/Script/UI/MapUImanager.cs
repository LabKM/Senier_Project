using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUImanager : MonoBehaviour
{
    bool nowVisible;
    Transform holder;
    public Transform MapHolder{ get{  
        if(holder == null)
            holder = transform.Find("Rooms").Find("Room");
        return holder; 
        } 
    }
    public Transform MapIcon;
    public Transform Background;

    Transform[,] minimap_Rooms;

    public void Awake(){
        Invisible();
    }

    public void RecreateMapHolder(){
        if(MapHolder != null){
            DestroyImmediate(MapHolder.gameObject);
        }
        
        holder = new GameObject("Room").transform;
        holder.parent = transform.Find("Rooms");
        holder.localPosition = Vector3.zero;
        holder.gameObject.SetActive(nowVisible);
    }

    public void OnButton(){
        if(nowVisible)
            Invisible();
        else
            visible();
    }

    public void Invisible(){
        nowVisible = false;
        MapHolder.gameObject.SetActive(nowVisible);
        MapIcon.gameObject.SetActive(nowVisible);
        Background.gameObject.SetActive(nowVisible);
    }

    public void visible(){
        nowVisible = true;
        MapHolder.gameObject.SetActive(nowVisible);
        MapIcon.gameObject.SetActive(nowVisible);
        Background.gameObject.SetActive(nowVisible);
    }

}
