using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Test_06_11 : MonoBehaviour
{

    public SkinnedMeshRenderer mf;

    public Dictionary<int, List<int>> dic = new Dictionary<int, List<int>>();

    // Use this for initialization
    void Start()
    {

        var mesh = mf.sharedMesh;
        for (int i = 0; i < mesh.boneWeights.Length; i++)
        {
            var weight = mesh.boneWeights[i];
            if (!dic.ContainsKey(weight.boneIndex0))
            {
                dic[weight.boneIndex0] = new List<int>();
            }
            dic[weight.boneIndex0].Add(i);
        }

    }

    // Update is called once per frame
    void Update()
    {



    }
}