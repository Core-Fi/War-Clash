using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Test_06_11))]
public class SaveBoneVertex : Editor {

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            VAData va = new VAData();
            var t = target as Test_06_11;
            var smr = t.GetComponent<SkinnedMeshRenderer>();
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
