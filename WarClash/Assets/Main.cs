using System;
using DG.Tweening;
using LiteNetLib.Utils;
using Lockstep;
using Logic;
using Logic.LogicObject;
using Logic.Skill;
using UnityEngine;
using Random = UnityEngine.Random;
public class Main : MonoBehaviour
{
//#if UNITY_EDITOR
    public bool ShowShortCut = true;
    public bool ShowDebug;
//#endif
    public Transform Uiparent;
    public Transform CameraParent;
    public static Main SP;
    private U3DSceneManager u3dSceneManager;
    private ManagerDriver managerDriver;
    void Awake()
    {
        SP = this;
        managerDriver = new ManagerDriver();
    }
	// Use this for initialization
	void Start ()
	{
      
        DonotDestroy();
        Logic.LogicCore.SP.Init();
        u3dSceneManager = new U3DSceneManager();
        Logic.LogicCore.SP.SceneManager.SwitchScene(new Scene("scene03"));
        EventDispatcher.FireEvent(UIEventList.ShowUI.ToInt(), this, EventGroup.NewArg<EventThreeArgs<string, Type, object>, string, Type, object>("UI-JoyStick.prefab", typeof(BattleView), null));
        EventDispatcher.FireEvent(UIEventList.ShowUI.ToInt(), this, EventGroup.NewArg<EventThreeArgs<string, Type, object>, string, Type, object>("BattleUI.prefab", typeof(BattleView), null));

	    FingerGestures.OnDragMove += (a, b) =>
	    {
	        b /= 2;
	       // CameraParent.transform.position += new Vector3(b.x, 0, b.y);
        };
    }
#if UNITY_EDITOR
    void OnGUI()
    {
        if (!ShowShortCut) return;
        GUILayout.BeginVertical();
        GUILayout.Label(LogicCore.SP.LockFrameMgr.LocalFrameCount+"  "+ LogicCore.SP.LockFrameMgr.ServerFrameCount);
        if (GUILayout.Button("NewMainPlayer"))
        {
            LogicCore.SP.LockFrameMgr.SendCommand(new CreateMainPlayerCommand() );
        }
        if (GUILayout.Button( "NewP"))
        {
            LogicCore.SP.LockFrameMgr.SendCommand(new CreatePlayerCommand());
        }
        if (GUILayout.Button("NewNpc"))
        {
            LogicCore.SP.LockFrameMgr.SendCommand(new CreateNpcCommand(){NpcId = 1001});
  
        }
        if (GUILayout.Button("CreateBarack1"))
        {
            var mp = LogicCore.SP.SceneManager.CurrentScene.GetObject<MainPlayer>();
           // var mapItem = LogicCore.SP.SceneManager.CurrentScene.MapConfig.MapDic[MapItemId] as MapBuildingItem;
            LogicCore.SP.LockFrameMgr.SendCommand(new CreateBuildingCommand{ BuildingId = 1002, Sender = mp.Id});
        }
        if (GUILayout.Button("CreateBarack2"))
        {
            var mp = LogicCore.SP.SceneManager.CurrentScene.GetObject<MainPlayer>();
            if(mp!=null)
                LogicCore.SP.LockFrameMgr.SendCommand(new CreateBuildingCommand { BuildingId = 1002, Sender = mp.Id });
            else
            {
                LogicCore.SP.LockFrameMgr.SendCommand(new CreateBuildingCommand { BuildingId = 1002});
            }
        }
        if (GUILayout.Button("SaveLog"))
        {
            NetDataWriter w = new NetDataWriter();
            w.Put((short)1);
            EventDispatcher.FireEvent(UIEventList.SendNetMsg.ToInt(),this, EventGroup.NewArg< EventSingleArgs <NetDataWriter> , NetDataWriter>(w));
        }

        GUILayout.EndVertical();
        //if (GUI.Button(new Rect(0, 0, 300, 50), "GC"))
        //{
        //    System.GC.Collect();
        //    Resources.UnloadUnusedAssets();
        //}
        //if (GUI.Button(new Rect(0, 50, 300, 50), "Unload"))
        //{
        //    Resource.UnloadBundles();
        //}
    }
#endif
    void DonotDestroy()
    {
        var objs = GameObject.FindGameObjectsWithTag("NotDestroy");
        for (var i = 0; i < objs.Length; i++)
        {
            DontDestroyOnLoad(objs[i]);
        }
    }

    private bool a;
	// Update is called once per frame
	void Update ()
    {
        managerDriver.Update();
        u3dSceneManager.Update();
        Logic.LogicCore.SP.Update(Time.deltaTime);
	}
    void FixedUpdate()
    {
        Logic.LogicCore.SP.FixedUpdate();
    }

    void OnApplicationQuit()
    {
        foreach (var manager in managerDriver.managers)
        {
            manager.Dispose();
        }
    }

    void OnDrawGizmos()
    {
        if(Application.isPlaying)
            GridService.DrawGizmos();
    }
}
