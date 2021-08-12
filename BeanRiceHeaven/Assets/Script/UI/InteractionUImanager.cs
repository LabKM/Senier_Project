using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUImanager : MonoBehaviour
{
    public GameObject BarPrefab;
    public Transform BarHolder;
    public Image Arrow;
    [SerializeField]
    List<Sprite> ResourceImage;

    public enum Interaction{
        Hide, Open, Save, Search, Use
    }

    List<Transform> ListOfBar;
    int NowBar;

    void Awake(){
        ListOfBar = new List<Transform>();
        if(ListOfBar.Count == 0){
            Arrow.enabled = false;
        }
    }

    public void AddInteraction(Interaction type){
        GameObject instance = Instantiate<GameObject>(BarPrefab, BarHolder);
        instance.GetComponent<Image>().sprite = ResourceImage[(int)type];
        ListOfBar.Add(instance.transform);
        instance.transform.localPosition = Vector3.up * ListOfBar.IndexOf(instance.transform) * 67;

        if(!Arrow.enabled){
            Arrow.enabled = true;
        }
    }

    public void RemoveInteraction(){
        Destroy(ListOfBar[NowBar].gameObject);
        ListOfBar.RemoveAt(NowBar);
        for(int i = NowBar; i < ListOfBar.Count; ++i){
            ListOfBar[i].localPosition = Vector3.up * i * 67;
        }
        if(ListOfBar.Count == 0){
            Arrow.enabled = false;
        }
    }

    public void Change(int _change)
    {
        NowBar += _change;
        NowBar = Mathf.Clamp(NowBar, 0, ListOfBar.Count - 1);
        BarHolder.localPosition = Vector3.down * NowBar * 67;
    }
    
}
