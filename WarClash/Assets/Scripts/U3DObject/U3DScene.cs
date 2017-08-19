using Logic.LogicObject;
using Logic.Objects;
using System.Collections;
using UnityEngine;
using System;

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
    IEnumerator LoadScene()
    {
        AsyncOperation asyn = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("scene01");
        yield return asyn;
        OnInit();
        //NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
        //Mesh mesh = new Mesh();
        //mesh.vertices = triangles.vertices;
        //mesh.triangles = triangles.indices;
        //AssetDatabase.CreateAsset(mesh, "Assets/navmesh.asset");
        // ListenEvents();
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
