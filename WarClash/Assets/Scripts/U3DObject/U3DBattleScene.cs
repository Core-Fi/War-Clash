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


public class U3DBattleScene : ObjectCollection<int, U3DSceneObject>, IU3DScene
{
    private BattleScene _battleScene;
    public U3DBattleScene()
    {
    }
    public void Init(IScene battleScene)
    {
        this._battleScene = battleScene as BattleScene;
        ListenEvents();
        UnityEngine.SceneManagement.SceneManager.LoadScene(_battleScene.Name);
        Main.SP.StartCoroutine(LoadScene());
    }

    public static bool IsFixed;
    IEnumerator LoadScene()
    {
        AsyncOperation asyn = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_battleScene.Name);
        yield return asyn;
        GenerateMap();
        _battleScene.EventGroup.FireEvent(BattleScene.SceneEvent.OnLoaded.ToInt(), _battleScene, null);
    }
    protected virtual void ListenEvents()
    {
        _battleScene.EventGroup.ListenEvent(BattleScene.SceneEvent.AddSceneObject.ToInt(),OnAddSceneObject);
        _battleScene.EventGroup.ListenEvent(BattleScene.SceneEvent.RemoveSceneObject.ToInt(), OnRemoveSceneObject);
    }

    private void OnRemoveSceneObject(object sender, EventMsg e)
    {
        EventSingleArgs<int> msg = e as EventSingleArgs<int>;
        U3DSceneObject uso = GetObject(msg.value);
        uso.Destroy();
        RemoveObject(msg.value);
    }

    private void OnAddSceneObject(object sender, EventMsg e)
    {
        EventSingleArgs<SceneObject> msg = e as EventSingleArgs<SceneObject>;
        U3DSceneObject uso = new U3DSceneObject();
        uso.Init(msg.value);
        AddObject(msg.value.Id, uso);
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }

    public void Destroy()
    {
        
    }

    private void GenerateMap()
    {
        Main.SP.CameraParent.position = new Vector3(_battleScene.MapConfig.Width/2, _battleScene.MapConfig.Height / 2, -10);
        for (int i = 0; i < this._battleScene.MapConfig.Data.Data.Count; i++)
        {
            var stageData = _battleScene.MapConfig.Data.Data[i];
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.localScale = Vector3.one;
            quad.transform.position = new Vector3(stageData.X+0.5f, stageData.Y+0.5f, 0);
        }
    }
}
