using System;
using System.Collections;
using System.Collections.Generic;
using FastCollections;
using Lockstep;
using Logic.Map;
using UnityEngine;

public class MapItemBehaviour : MonoBehaviour
{

    public MapItem MapItem;
    private static readonly Dictionary<Type, Color> ColorDic = new BiDictionary<Type, Color>{{typeof(MapBuildingItem), Color.blue}};

    void OnDrawGizmos()
    {
        if(MapItem == null)return;
        MapItem.Position = new Vector3d(transform.position);
        MapItem.Forward = new Vector3d(transform.forward);
        if (MapItem is MapBuildingItem)
        {
            Gizmos.color = ColorDic[typeof(MapBuildingItem)];
            Gizmos.DrawSphere(transform.position +Vector3.up/2, 1);
        }
    }
}
