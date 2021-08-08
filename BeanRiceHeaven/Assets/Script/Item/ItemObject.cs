using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : LiftableObject
{
    public enum HandItem{
        Cloth, Guardtracker, Healkit, Key, Speedup, Treasuretracker
    }
    [SerializeField]
    HandItem Type;
    public HandItem type{ get{ return Type; } }

    public override void LeftShift(Transform _transform){
        onHand = !onHand;
        if(onHand){
            base.PhysicsOff();
            mesh.enabled = false;
            GameSceneUIManager.Instance.itemUI.GetItemUI(type);
        }else{
            base.PhysicsOn();
            mesh.enabled = true;
            GameSceneUIManager.Instance.itemUI.PutItemUI(Type);
            transform.position = _transform.position;
        }
    }
}
