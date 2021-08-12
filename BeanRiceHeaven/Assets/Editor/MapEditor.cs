using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapEditor : Editor
{
    static Vector2Int rc;
    static Object vec3;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapGenerator map = target as MapGenerator;
        if (GUILayout.Button("Genarate Map"))
        {
            map.GenerateMap();
        }
        // rc = EditorGUILayout.Vector2IntField("Row & Column", rc);
        // if(GUILayout.Button("FlipMiniMap")){
        //     map.filpMiniMap(rc.x, rc.y);
        // }
        // if(GUILayout.Button("FlipMiniMapAll")){
        //     map.flipAllMiniMap();
        // }
    }
}