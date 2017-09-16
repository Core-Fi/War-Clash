using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(MapItemBehaviour))]
public class MapItemInspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var t = target as MapItemBehaviour;
        if (t.MapItem is Logic.Map.MapBuildingItem)
        {
            var bItem = t.MapItem as Logic.Map.MapBuildingItem;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("建筑ID");
            bItem.BuildingId = EditorGUILayout.IntField(bItem.BuildingId);
            EditorGUILayout.EndHorizontal();
        }
    }
}
