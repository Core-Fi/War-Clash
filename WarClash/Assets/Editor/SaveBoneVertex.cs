using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Test_06_11))]
public class SaveBoneVertex : Editor {
    
    void Float2Byte(float v, List<byte> list)
    {
        var bytes = BitConverter.GetBytes(v);
        for (int i = 0; i < bytes.Length; i++)
        {
            list.Add(bytes[i]);
        }
    }
  
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            VAData va = new VAData();
            var t = target as Test_06_11;
            var smr = t.GetComponent<SkinnedMeshRenderer>();
            Mesh new_m = new Mesh();
            smr.BakeMesh(new_m);
            List<byte> data = new List<byte>();
            Float2Byte(new_m.vertexCount, data);
            for (int i = 0; i < new_m.vertexCount; i++)
            {
                Float2Byte(new_m.vertices[i].x, data);
                Float2Byte(new_m.vertices[i].y, data);
                Float2Byte(new_m.vertices[i].z, data);

                Float2Byte(new_m.uv[i].x, data);
                Float2Byte(new_m.uv[i].y, data);
            }
            for (int i = 0; i < new_m.triangles.Length; i++)
            {
                Float2Byte(new_m.triangles[i], data);
            }

            Mesh t_m = new Mesh();
            t_m.vertices = new_m.vertices;
            t_m.uv = new_m.uv;
            t_m.triangles = new_m.triangles;
            AssetDatabase.CreateAsset(t_m, "Assets/mesh.asset");
            File.WriteAllBytes(Application.dataPath + "/meshData.txt", data.ToArray());
            var mesh = smr.sharedMesh;
            Matrix4x4 l2w =  t.transform.localToWorldMatrix;
            for (int i = 0; i < mesh.boneWeights.Length; i++)
            {
                var weight = mesh.boneWeights[i];
                if (!va.dic.ContainsKey(smr.bones[weight.boneIndex0].name))
                {
                    va.dic[smr.bones[weight.boneIndex0].name] = new List<int>();
                }
                va.dic[smr.bones[weight.boneIndex0].name].Add(i);
                if (!va.offset.ContainsKey(smr.bones[weight.boneIndex0].name))
                {
                    va.offset[smr.bones[weight.boneIndex0].name] = new List<VAData.V>();
                }
                Vector3 offset = smr.bones[weight.boneIndex0].transform.worldToLocalMatrix.MultiplyPoint(l2w.MultiplyPoint(mesh.vertices[i]));
                va.offset[smr.bones[weight.boneIndex0].name].Add(new VAData.V() {x = offset.x, y = offset.y, z = offset.z });
            }
            string j = Newtonsoft.Json.JsonConvert.SerializeObject(va);
            File.WriteAllText(Application.dataPath + "/" + t.gameObject.name + ".json", j);
        }
        base.OnInspectorGUI();
    }
}
