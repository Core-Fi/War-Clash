using System;
using LiteNetLib.Utils;
using Lockstep;
using Logic;
using Logic.LogicObject;
using UnityEngine;

public class Main : MonoBehaviour {

    public Transform Uiparent;
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
        Logic.LogicCore.SP.SceneManager.SwitchScene(new Scene());

        //var b = LogicCore.SP.SceneManager.currentScene.CreateSceneObject<BarackBuilding>();
        //b.Team = Team.Team1; 
        //b.Position = new Lockstep.Vector3d(new Vector3(0, 0, 0));

        //var p = LogicCore.SP.SceneManager.currentScene.CreateSceneObject<Player>();
        //p.Team = Team.Team1;
        //p.Position = new Lockstep.Vector3d(new Vector3(-3, 0, 0));

        EventDispatcher.FireEvent((int)UIEventList.ShowUI, this, EventGroup.NewArg<EventThreeArgs<string, Type, object>, string, Type, object>("UI-JoyStick.prefab", typeof(BattleView), null));
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label(LogicCore.SP.fixedtime.ToString("0.00")+"  "+ LogicCore.SP.RealFixedFrame.ToString()+"   "+ (LogicCore.SP.RealFixedFrame/(float)LockFrameMgr.FixedFrameRate).ToString("00.00")+"  "+Time.time.ToString("00.00"));
        if (GUILayout.Button( "NewP"))
        {
            LogicCore.SP.LockFrameMgr.SendCommand(new CreatePlayerCommand{Sender = 100});
        }
        if (GUILayout.Button("SaveLog"))
        {
            NetDataWriter w = new NetDataWriter();
            w.Put((short)1);
            EventDispatcher.FireEvent((int)UIEventList.SendNetMsg,this, EventGroup.NewArg< EventSingleArgs <NetDataWriter> , NetDataWriter>(w));
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
	    if (!a && Time.time > 0.01f)
	    {
            EventDispatcher.FireEvent((int)UIEventList.ShowUI, this, EventGroup.NewArg<EventThreeArgs<string, Type, object>, string, Type, object>("NextBattleUI.prefab", typeof(BattleView), null));
	        a = true;
	    }
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
}
