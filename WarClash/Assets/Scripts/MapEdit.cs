using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[SelectionBase]
public class MapEdit : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public float cell_width = 2f;
    public float cell_height = 1f;
    private int[,] data;
    private List<Box> boxes = new List<Box>();
    public void Clear()
    {
        boxes.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var t = transform.GetChild(i);
            GameObject.DestroyImmediate(t.gameObject);
        }
    }

    public void Load()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            var str = File.ReadAllText(path);
            data = Newtonsoft.Json.JsonConvert.DeserializeObject<int[,]>(str);
        }
        else
        {
            data = new int[width, height];
        }
    }
    public void Save()
    {
        string dataStr = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        File.WriteAllText(GetPath(), dataStr);
    }

    public string GetPath()
    {
        return Application.streamingAssetsPath + "/map.txt";
    }
    public void Generate()
    {
        Load();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject g = new GameObject(i+"_"+j);
              //  g.hideFlags = HideFlags.HideInHierarchy;
                g.transform.parent = transform;
                g.transform.position = new Vector3(j*cell_width,0, i * cell_height) 
                    + new Vector3(cell_width, 0, cell_height)/2 
                    - new Vector3(cell_width *width, 0, height * cell_height)/2;
                var bc = g.AddComponent<BoxCollider>();
                bc.size = new Vector3(cell_width, 0.01f, cell_height);
                var box = g.AddComponent<Box>();
                box.x = j;
                box.y = i;
                box.status = (Box.Status)(data[j, i]);
                box.size = new Vector3(cell_width,0,cell_height);
                boxes.Add(box);
            }
        }
    }
    public void Raycast(Ray ray)
    {
        var hits = Physics.RaycastAll(ray, 1000);
        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            var box = hit.transform.GetComponent<Box>();
            if(box != null)
            {
                data[box.x, box.y] = 1;
                box.status = Box.Status.Reachable;
            }
        }
    }
    void OnMouseDrag()
    {
        Raycast(Camera.main.ScreenPointToRay(Input.mousePosition));
    }
    void OnMouseMove()
    {
        Raycast(Camera.main.ScreenPointToRay(Input.mousePosition));
    }
    void OnDrawGizmos()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].DrawGizmos();
        }
        Gizmos.color = Color.gray;
        for (int i = 0; i <= height; i++)
        {
            Gizmos.DrawLine(transform.position + new Vector3(0,0,i*cell_height), transform.position + new Vector3(cell_width * (width), 0, i * cell_height));
        }
        for (int i = 0; i <= width; i++)
        {
            Gizmos.DrawLine(transform.position + new Vector3(i*cell_width, 0, 0), transform.position + new Vector3(i * cell_width, 0, (height) * cell_height));
        }
    }
}
