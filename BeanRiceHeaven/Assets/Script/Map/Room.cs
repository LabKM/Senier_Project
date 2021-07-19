using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{    
    public enum DoorPoint{  East, West, South, North };   

    public enum Style{
        Room00, Room01, Room02, Room03, Room04, Room05
    //  다 닫힌, 입구하나, 직선, 입구3개, 다 열림, 입구 2개 위옆, 입구 2개 위옆 
    };

    // room00
    // 000
    // 010
    // 000 

    // room01
    // 000
    // 010
    // 010 

    // room02
    // 010
    // 010
    // 010

    // room03
    // 010
    // 011
    // 010

    // room04
    // 010
    // 111
    // 010

    // room05
    // 010
    // 110
    // 000

    Style m_style;

    public List<Transform> doors; // 동(x+) 서(x-) 남(z-) 북(z+) 순서

    public bool hallway{ set; get; }
    public bool startPoint{ set; get; }
    public bool goalPoint{set; get;}

    Quaternion room_rotation;

    public void SetRoomStyle(Style roomStyle){
        m_style = roomStyle;
        GameObject prefab_roomset = null;

        if(hallway){
            switch(m_style){
                case Style.Room00:
                    break;
                case Style.Room01:
                    prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room01/Room11Set"+((int)Random.Range(0.0f, 2.9f)).ToString());
                    Instantiate<GameObject>(prefab_roomset, transform.position, room_rotation).transform.parent = transform;
                    prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/Room11");  
                    break;
                case Style.Room02:
                    prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room02/Room12Set"+((int)Random.Range(0.0f, 2.9f)).ToString());
                    Instantiate<GameObject>(prefab_roomset, transform.position, room_rotation).transform.parent = transform;
                    prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/Room12");  
                    break;
                case Style.Room03:
                    prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room03/Room13Set"+((int)Random.Range(0.0f, 2.9f)).ToString());
                    Instantiate<GameObject>(prefab_roomset, transform.position, room_rotation).transform.parent = transform;
                    prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/Room13");  
                    break;
                case Style.Room04:
                    prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room04/Room14Set"+((int)Random.Range(0.0f, 2.9f)).ToString());
                    Instantiate<GameObject>(prefab_roomset, transform.position, room_rotation).transform.parent = transform;
                    prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/Room14");  
                    break;
                case Style.Room05:
                    prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room05/Room15Set"+((int)Random.Range(0.0f, 2.9f)).ToString());
                    Instantiate<GameObject>(prefab_roomset, transform.position, room_rotation).transform.parent = transform;
                    prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/Room15");  
                    break;
            }
        }else{
            if(startPoint){

            }else if(goalPoint){
                prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room01/Room11SetTable");
            }else{
                switch(m_style){
                    case Style.Room00:
                        break;
                    case Style.Room01:  
                        prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room01/Room01Set"+((int)Random.Range(0.0f, 2.9f)).ToString());
                        break;
                    case Style.Room02:
                        prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room02/Room02Set"+((int)Random.Range(0.0f, 2.9f)).ToString());
                        break;
                    case Style.Room03:
                        prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room03/Room03Set" + ((int)Random.Range(0.0f, 2.9f)).ToString());
                        break;
                    case Style.Room04:
                        prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room04/Room04Set" + ((int)Random.Range(0.0f, 2.9f)).ToString());
                        break;
                    case Style.Room05:
                        prefab_roomset = Resources.Load<GameObject>("Prefab/Map2/RoomSet/Room05/Room05Set" + ((int)Random.Range(0.0f, 2.9f)).ToString());
                        break;
                }
            }
        }
        if(prefab_roomset == null)
            return;
        GameObject instance_roomset = Instantiate<GameObject>(prefab_roomset, transform.position, room_rotation);
        instance_roomset.transform.parent = transform;
    }

    public void SetDoorStyle(bool east, bool west, bool south, bool north){
        List<bool> offsetRequest = new List<bool>(){ east, west, south, north };
        for(int i = 0; i < doors.Count; ++i){
            doors[i].Find("Wall").gameObject.SetActive(!offsetRequest[i]);
        }
        int numOfDoor = (east ? 1 : 0) + (west ? 1 : 0) + (south ? 1 : 0) + (north ? 1 : 0) ;
        switch(numOfDoor){
            case 0:
                 m_style = Style.Room00;
                break;
            case 1:
                if(!east && !west && !south && north){ // ewsN
                    m_style = Style.Room01;
                    room_rotation = Quaternion.Euler(0, 180, 0);
                }else if (!east && !west && south && !north){ // ewSn
                    m_style = Style.Room01;
                    room_rotation = Quaternion.Euler(0, 0, 0);
                }else if (east && !west && !south && !north) { // Ewsn
                    m_style = Style.Room01;
                    room_rotation = Quaternion.Euler(0, 270, 0);
                }else if (!east && west && !south && !north){
                    m_style = Style.Room01;
                    room_rotation = Quaternion.Euler(0, 90, 0);
                }
                break;
            case 2:
                if(!east && !west && south && north){
                    m_style = Style.Room02;
                }else if(east && west && !south && !north){
                    m_style = Style.Room02;
                    room_rotation = Quaternion.Euler(0, 90, 0);
                }else{
                    if(!east && west && !south && north){
                        m_style = Style.Room05;
                    }
                    else if (!east && west && south && !north){
                        m_style = Style.Room05;
                        room_rotation = Quaternion.Euler(0, 270, 0);
                    }
                    else if (east && !west && !south && north){
                        m_style = Style.Room05;
                        room_rotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (east && !west && south && !north){
                        m_style = Style.Room05;
                        room_rotation = Quaternion.Euler(0, 180, 0);
                    }
                }
                break;
            case 3:
                if(!east && west && south && north){
                    m_style = Style.Room03;
                    room_rotation = Quaternion.Euler(0, 180, 0);
                }else if(east && !west && south && north){
                    m_style = Style.Room03;
                }else if(east && west && !south && north){
                    m_style = Style.Room03;
                    room_rotation = Quaternion.Euler(0, 270, 0);
                }else if(east && west && south && !north){
                    m_style = Style.Room03;
                    room_rotation = Quaternion.Euler(0, 90, 0);
                }
                break;
            case 4:
                m_style = Style.Room04;
                break;
        }
        SetRoomStyle(m_style);
    }
}
