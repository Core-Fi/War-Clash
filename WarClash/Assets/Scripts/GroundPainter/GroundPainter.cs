using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class GroundPainter : MonoBehaviour
{
    public static int Type;
    private MeshFilter mf;
    public int width = 9;
    public int height = 9;
    public float cell_width = 2f;
    public float cell_height = 1f;
    public Color[] data;
    private MeshRenderer mr;
    private Material mat;
    private Mesh m;
    void Start()
    {
        if (tex != null)
        {
            data = tex.GetPixels();
            mr = GetComponent<MeshRenderer>();
            mf = GetComponent<MeshFilter>();
            mat = mr.material;
        }
     //   Generate();
    }
    public void Generate()
    {
        tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
        mat = mr.sharedMaterial;
        data = new Color[(width) * (height)];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new Color(0, 0, 0, 0);
        }
        m = new Mesh();
        Vector3[] vs = new Vector3[4]{Vector3.zero, Vector3.right*width*cell_width, Vector3.forward*height*cell_height, new Vector3(width * cell_width, 0,height * cell_height) };
        Vector2[] uvs = new Vector2[4]{Vector2.zero, Vector2.right, Vector2.up, new Vector2(1,1)};
        List<int> ts = new List<int>(){0,2,1,1,2,3};
        //Vector3[] vs = new Vector3[width * height];
        //Vector2[] uvs = new Vector2[width * height];
        //List<int> ts = new List<int>();

        //for (int i = 0; i < height; i++)
        //{
        //    for (int j = 0; j < width; j++)
        //    {
        //        vs[i * width + j] = new Vector3(j * cell_width, 0, i * cell_height);
        //        uvs[i * width + j] = new Vector2(j / (float)(width - 1), i / (float)(height - 1));
        //        if (j < width - 1)
        //        {
        //            if (i == 0)
        //            {
        //                ts.Add(i * width + j);
        //                ts.Add((i + 1) * width + j + 1);
        //                ts.Add((i) * width + j + 1);
        //            }
        //            else if (i == height - 1)
        //            {
        //                ts.Add(i * width + j);
        //                ts.Add((i) * width + j + 1);
        //                ts.Add((i - 1) * width + j);
        //            }
        //            else
        //            {
        //                ts.Add(i * width + j);
        //                ts.Add((i) * width + j + 1);
        //                ts.Add((i - 1) * width + j);

        //                ts.Add(i * width + j);
        //                ts.Add((i + 1) * width + j + 1);
        //                ts.Add((i) * width + j + 1);
        //            }
        //        }
        //    }
        //}
        m.vertices = vs;
        m.triangles = ts.ToArray();
        m.uv = uvs;
        m.RecalculateNormals();
        mf.mesh = m;
        var mc = gameObject.GetComponent<MeshCollider>();
        if (mc)
        {
            GameObject.DestroyImmediate(mc);
        }
        var bc = gameObject.GetComponent<BoxCollider>();
        if (bc != null)
        {
            bc.center = mr.bounds.center - transform.position;
            bc.size = mr.bounds.size;
        }
        if (bc == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
        UpdateImage();
    }
    public void OnHit(Vector3 p)
    {
        if (mat != null)
        {
            Vector3 relative_p = p - gameObject.transform.position;
            int x = Mathf.FloorToInt(relative_p.x / cell_width);
            int z = Mathf.FloorToInt(relative_p.z / cell_height);
            float fmod_x = relative_p.x % cell_width;
            float fmod_z = relative_p.z % cell_height;
            if (fmod_x > cell_width / 2)
                x++;
            if (fmod_z > cell_height / 2)
                z++;
            Vector2 leftup = new Vector2(x - 1, z);
            Vector2 rightup = new Vector2(x, z);
            Vector2 leftdown = new Vector2(x - 1, z - 1);
            Vector2 rightdown = new Vector2(x, z - 1);
            AddWeight(leftdown, 1);
            AddWeight(rightdown, 2);
            AddWeight(leftup, 4);
            AddWeight(rightup, 8);
            UpdateImage();
        }
    }

    private int previousIndex;
    private void AddWeight(Vector2 v, int weight)
    {
        int index = (int) v.x + (int) v.y*(width );
      
        if (index > 0 && index < data.Length)
        {
            Color c = data[index];
            if (Type == 1)
            {
                if((int)(c.r * 256)>=15 && weight == 8)
                {
                    c.r = UnityEngine.Random.Range(15, 31) / 256f;
                }
                else
                {
                    if (((int)(c.r * 256) & weight) == 0)
                    {
                        c.r += weight / 256f;
                    }
                }
                if ((int) (c.g * 256) > 15)
                {
                    c.g = 15;
                }
                if (((int)(c.g * 256) & weight) != 0)
                {
                    c.g -= weight / 256f;
                }

            }
            else if (Type == 2)
            {
                if ((int)(c.g * 256) >= 15 && weight == 8)
                {
                    c.g = UnityEngine.Random.Range(15, 31) / 256f;
                }
                else
                {
                    if (((int)(c.g * 256) & weight) == 0)
                    {
                        c.g += weight / 256f;
                    }
                }
                if (((int)(c.g * 256) & weight) == 0)
                {
                    c.g += weight / 256f;
                }
            }

            data[index] = c;
        }

    }
    // Update is called once per frame
    public void Raycast(bool fromEditor)
    {
        Ray ray = new Ray();// HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (fromEditor)
        {
#if UNITY_EDITOR
            ray = UnityEditor.HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
#endif
        }
        else
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        }
        var hits = Physics.RaycastAll(ray, 100000);
        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            var tran = gameObject.transform;
            if (tran == hit.transform)
            {
                OnHit(hit.point);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Raycast(false);
        }
    }
    void OnMouseDrag()
    {
        Raycast(false);
    }
    void OnMouseMove()
    {
        Raycast(false);
    }

    public Texture2D tex = null;
    void UpdateImage()
    {
        if(tex == null)
            tex = new Texture2D(width , height, TextureFormat.ARGB32, false);
        tex.SetPixels(data);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        mat.SetTexture("_Map", tex);
    }

    public void Save()
    {
        if (tex != null)
        {
            
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i <= height; i++)
        {
            Gizmos.DrawLine(transform.position + new Vector3(0, 0, i * cell_height), transform.position + new Vector3(cell_width * (width ), 0, i * cell_height));
        }
        for (int i = 0; i <= width; i++)
        {
            Gizmos.DrawLine(transform.position + new Vector3(i * cell_width, 0, 0), transform.position + new Vector3(i * cell_width, 0, (height ) * cell_height));
        }
    }
}
