using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    BeanController owner;
    
    Transform liftUp;
    Transform liftDown;

    LiftableObject handItem;
    public bool isHandEmpty{ get{ return handItem == null; } }
    List<ItemObject> itemInPocket;

   public bool isTreasure{ get; private set; }

    List<LiftableObject> itemsOutOfPocket;
    
    int nowChosenItem;
    LiftableObject nowItemOut { get{
        if(itemsOutOfPocket.Count > nowChosenItem)
            return itemsOutOfPocket[nowChosenItem];
        else if(itemsOutOfPocket.Count > 0){
            Debug.LogWarning("nowChosenItem is missed...");
            nowChosenItem = 0;
            return itemsOutOfPocket[nowChosenItem];
        }else{
            return null;
        }
    } }
    public int MaxItem;

    public Inventory(BeanController _owner, Transform _up, Transform _down){
        owner = _owner;
        liftUp = _up;
        liftDown = _down;
        itemInPocket = new List<ItemObject>();
        itemsOutOfPocket = new List<LiftableObject>();
        nowChosenItem = 0;
        isTreasure = false;
    }

    public void OnCloseObejct(LiftableObject liftable){
        if(itemsOutOfPocket.IndexOf(liftable) < 0){
            itemsOutOfPocket.Add(liftable);
            if(nowItemOut == liftable){
                ChagneChosenItem(0);
            }
        }
    }

    public void ChagneChosenItem(int delta){
        if(itemsOutOfPocket.Count > 0){
            nowItemOut.OffEffectOfOutline();
            Debug.Log("Change chosen item : " + nowChosenItem.ToString() + " + " + delta.ToString()
             + " % " + itemsOutOfPocket.Count.ToString() + " = " + (Mathf.Abs(nowChosenItem + delta) % itemsOutOfPocket.Count).ToString());
            nowChosenItem = Mathf.Abs(nowChosenItem + delta) % itemsOutOfPocket.Count;
            nowItemOut.OnEffectOfOutline();
        }
    }

    public void OutofObject(LiftableObject liftable){
        if(itemsOutOfPocket.Remove(liftable)){
            if(liftable.isOutline && itemsOutOfPocket.Count > 0){
                nowChosenItem = 0;
                nowItemOut.OnEffectOfOutline();
            }
            liftable.OffEffectOfOutline();
        }
        Debug.Log("Remove item to out of pocket list");
    }

    public void GetItem(){
        // 주위에 아무 물건도 없는데 주우려한 경우
        if(itemsOutOfPocket.Count < 1)
            return;
        LiftableObject target = nowItemOut;
        OutofObject(target); 
        ItemObject item = target.GetComponent<ItemObject>();
        if(item != null){ // 주머니 아이템
            if(handItem == null && itemInPocket.Count >= MaxItem){
                // 주머니 아이템이 넘치는 경우 
                PutPocketItem(0);    
            }else{// 이미 손에 물건 들고 있는 경우 물건 놓기
                PutHandItem();
            }
            itemInPocket.Add(item);
            owner.hand = false;
        }else{  // 들 것
            if(handItem == null){
            // 주머니에 있는 물건 토하기
                PutPocketItem(0);
                PutPocketItem(0);
            }else{
            // 쥐고 있던 다른 아이템 버리기
                PutHandItem();
            }
            if(target.GetComponent<Treasure>())
                isTreasure = true;
            owner.hand = true;
            handItem = target;
        }
        target.LeftShift(liftUp);
    }

    public void UseItem(int target){
        
    }

    public void PutPocketItem(int target){
        if(target >= 0 && itemInPocket.Count > target){
            itemInPocket[target].LeftShift(liftDown);
            itemInPocket.RemoveAt(target);
        }
    }

    public void PutHandItem(){
        if(handItem){
            owner.hand = false;
            handItem.LeftShift(liftDown);
            handItem = null;
            isTreasure = false;
        }        
    }
}
