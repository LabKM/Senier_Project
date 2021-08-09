using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUImanager : MonoBehaviour
{
    public Image image1;
    public Image image2;

    private List<Sprite> itemImage = null;

    private Sprite GetImage(ItemObject.HandItem item){
        if(itemImage == null){
            itemImage = new List<Sprite>(Resources.LoadAll<Sprite>("UI/Ingame/Item"));

            Debug.Log(itemImage.Count);
            itemImage.RemoveAt(0);
            itemImage.RemoveAt(0);
            itemImage.RemoveAt(0);            
        }
        return itemImage[(int)item];
    }

    public void GetItemUI(ItemObject.HandItem item){
        if(!image1.enabled){
            image1.enabled = true;
            image1.sprite = GetImage(item);
        }else if(!image2.enabled){
            image2.enabled = true;
            image2.sprite = GetImage(item);
        }else{
            PutItemUI();
            GetItemUI(item);
        }
    }

    public void PutItemUI(){
        if(image2.enabled){
            image1.sprite = image2.sprite;
            image2.enabled = false;
        }else if(image1.enabled){
            image1.enabled = false;
        }
    }

    public void PutItemUI(ItemObject.HandItem item){
        if(image2.enabled && image2.sprite == GetImage(item)){
            image2.enabled = false;
        }else if(image1.enabled && image1.sprite == GetImage(item)){
            image1.enabled = false;
            if(image2.enabled){
                image2.enabled = false;
                image1.sprite = image2.sprite;
                image1.enabled = true;
            }
        }
    }
}
