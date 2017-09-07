using Logic.LogicObject;
using Logic.Objects;
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Lockstep;
using Pathfinding;
using UnityEngine.AI;


public class U3DScene : ObjectCollection<int, U3DSceneObject>
{
    public Scene Scene;
    public U3DScene()
    {
    }
    public void Init(Scene scene)
    {
        this.Scene = scene;
        ListenEvents();
        UnityEngine.SceneManagement.SceneManager.LoadScene("scene01");
        Main.SP.StartCoroutine(LoadScene());
    }

    public static bool IsFixed;
    IEnumerator LoadScene()
    {
        AsyncOperation asyn = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("scene01");
        yield return asyn;
        OnInit();
        var graph = AstarPath.active.graphs[0] as RecastGraph;
        //var v = new Vector3(10, 0, 11);
        //var fnode = graph.GetNearestForce(new Vector3d(v), NNConstraint.Default);
        //var node = graph.GetNearestForce(v, NNConstraint.Default);
        //Debug.LogError("----------------- " + v + "  " + fnode.node.NodeIndex + "  " + node.node.NodeIndex);

        int wrong = 0;
        for (int i = 0; i < 100; i++)
        {
            var v = new Vector3(UnityEngine.Random.Range(0, 100), 0, UnityEngine.Random.Range(0, 100));
            var fnode = graph.GetNearestForce(new Vector3d(v), NNConstraint.Default);
            var node = graph.GetNearestForce(v, NNConstraint.Default);
            if (fnode.node != null && node.node != null)
            {
                //   Debug.LogError(fnode.constFixedClampedPosition+"   "+node.constClampedPosition);
                if (fnode.node.NodeIndex != node.node.NodeIndex)
                {
                    Debug.LogError("----------------- " + v + "  " + fnode.node.NodeIndex + "  " + node.node.NodeIndex);
                    wrong++;
                }
            }
        }
        Debug.LogError(wrong);

       
        //Debug.LogError(rawData.vertices.Length);

    }
    protected virtual void OnInit()
    {
        

    }
    protected virtual void ListenEvents()
    {
        Scene.EventGroup.ListenEvent((int)Scene.SceneEvent.Addsceneobject,OnAddSceneObject);
        Scene.EventGroup.ListenEvent((int)Scene.SceneEvent.Removesceneobject, OnRemoveSceneObject);
    }

    private void OnRemoveSceneObject(object sender, EventMsg e)
    {
        EventSingleArgs<SceneObject> msg = e as EventSingleArgs<SceneObject>;
        U3DSceneObject uso = GetObject(msg.value.Id);
        uso.Destroy();
        RemoveObject(msg.value.Id);
    }

    private void OnAddSceneObject(object sender, EventMsg e)
    {
        EventSingleArgs<SceneObject> msg = e as EventSingleArgs<SceneObject>;
        Type t = LogicObjectCorresponding.Corresponding[msg.value.GetType()];
        U3DSceneObject uso = Activator.CreateInstance(t) as U3DSceneObject;
        AddObject(msg.value.Id, uso);
        uso.Init(msg.value);
        uso.ListenEvents();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }
}
