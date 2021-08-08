using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (GameSceneUIManager))]
public class UIEditor : Editor
{
    static ItemObject.HandItem item;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameSceneUIManager ui = target as GameSceneUIManager;
        item = (ItemObject.HandItem)EditorGUILayout.EnumPopup("Kind of Item", item);
        if (GUILayout.Button("Get Item"))
        {
            ui.itemUI.GetItemUI(item);
        }
        if (GUILayout.Button("Put Item"))
        {
            ui.itemUI.PutItemUI();
        }
    }
}
