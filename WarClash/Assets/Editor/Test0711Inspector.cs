using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Test918))]
public class Test0711Inspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("AddNew"))
        {
            var t = target as Test918;
            var ani = t.GetComponent<Animator>();
            var info1 = ani.GetCurrentAnimatorStateInfo(0);
            ani.Play(info1.fullPathHash, -1, 1f);
            int length = Mathf.CeilToInt(t.clip.length * 24f);
            float interval = 1 / 24f;
            List<byte> totalBytes= new List<byte>(1024*1024);
            
            for (int i = 0; i < length; i++)
            {
                UnityEditor.AnimationMode.SampleAnimationClip(t.gameObject, t.clip, i*interval);
                Mesh m = new Mesh();
                t.smr.BakeMesh(m);
                if (i == 0)
                {
                    AddToList(totalBytes, m.vertexCount);
                    for (int j = 0; j < m.uv.Length; j++)
                    {
                        AddToList(totalBytes, m.uv[j].x * 10000);
                        AddToList(totalBytes, m.uv[j].y * 10000);
                    }
                }
                for (int j = 0; j < m.vertexCount; j++)
                {
                    AddToList(totalBytes, m.vertices[j].x*10000);
                    AddToList(totalBytes, m.vertices[j].y * 10000);
                    AddToList(totalBytes, m.vertices[j].z * 10000);
                }
                MeshUtility.SetMeshCompression(m, ModelImporterMeshCompression.High);
                AssetDatabase.CreateAsset(m, "Assets/BakeAnimaitons/"+i+".asset");
            }
            var array = totalBytes.ToArray();
            System.IO.File.WriteAllBytes(Application.dataPath + "/BakeAnimaitons/total_byte.byte", array);
        }
    }

    void AddToList(List<byte> bytes, float v)
    {
        short s = (short) v;
        var b=  BitConverter.GetBytes(s);
        for (int i = 0; i < b.Length; i++)
        {
            bytes.Add(b[i]);
        }
    }


}
