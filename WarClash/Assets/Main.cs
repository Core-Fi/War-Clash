using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DG.Tweening;
using LiteNetLib.Utils;
using Lockstep;
using Logic;
using Logic.LogicObject;
using Logic.Skill;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
public class Main : MonoBehaviour
{
//#if UNITY_EDITOR
    public bool ShowShortCut = true;
    public bool ShowDebug;

    //#endif
#if  UNITY_ANDROID
    private bool _showLogWindow = false;
    private List<string> log = new List<string>();
#endif
    public Transform CameraParent;
    public Transform UIParent;

    public Camera MainCamera;
    public Camera UICamera;
  
    public static Main SP;
    private U3DSceneManager u3dSceneManager;
    private ManagerDriver managerDriver;

    void Awake()
    {
        SP = this;
        managerDriver = new ManagerDriver();
#if UNITY_ANDROID
        Application.logMessageReceived += OnLogReceive;
#endif
    }
#if UNITY_ANDROID
    private void OnLogReceive(string condition, string stackTrace, LogType type)
    {
        log.Add(condition + " " + stackTrace + " " + type);
    }
#endif
    // Use this for initialization
    void Start ()
	{
        DonotDestroy();
	    Logic.LogicCore.SP.Init();
	    u3dSceneManager = new U3DSceneManager();
	    Logic.LogicCore.SP.SceneManager.SwitchScene(new HotFixScene());
        //AssetResources.LoadAsset("skill_1.bytes", (s, o) =>
        //{
        //    var ta = o as TextAsset;
        //    Debug.LogError(ta.text);
        //});
	    //   EventDispatcher.FireEvent(UIEventList.ShowUI.ToInt(), this, EventGroup.NewArg<EventThreeArgs<string, Type, object>, string, Type, object>("BattleUI.prefab", typeof(BattleUI), null));
	    //EventDispatcher.FireEvent(UIEventList.ShowUI.ToInt(), this, EventGroup.NewArg<EventThreeArgs<string, Type, object>, string, Type, object>("Hud.prefab", typeof(HudView), null));
#if UNITY_ANDROID
        FingerGestures.OnTap += CheckOpenLogWindow;
#endif

	}
#if UNITY_ANDROID
    void CheckOpenLogWindow(Vector2 fingerPos, int tapCount)
    {
        if (tapCount == 3)
        {
            _showLogWindow = !_showLogWindow;
        }
    }
#endif
 
    private Vector2 _offset;
    void OnGUI()
    {
        return;
        var bs = LogicCore.SP.SceneManager.CurrentScene as BattleScene;
        GUILayout.BeginHorizontal();
        if (ShowShortCut && bs!=null)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(LogicCore.SP.LockFrameMgr.LocalFrameCount + "  " +
                            LogicCore.SP.LockFrameMgr.ServerFrameCount);
            if (GUILayout.Button("NewMainPlayer", GUILayout.Width(150), GUILayout.Height(70)))
            {
                LogicCore.SP.LockFrameMgr.SendCommand(new CreateMainPlayerCommand());
            }
            if (GUILayout.Button("NewP"))
            {
                LogicCore.SP.LockFrameMgr.SendCommand(new CreatePlayerCommand());
            }
            if (GUILayout.Button("NewNpc"))
            {
                if (MainPlayer.SP != null)
                {
                    LogicCore.SP.LockFrameMgr.SendCommand(new CreateNpcCommand() { NpcId = 1001, Sender = MainPlayer.SP.Id});
                }
                else
                    LogicCore.SP.LockFrameMgr.SendCommand(new CreateNpcCommand() {NpcId = 1001});

            }
           
            if (GUILayout.Button("CreateBarack1"))
            {
                var mp = bs.GetObject<MainPlayer>();
                // var mapItem = LogicCore.SP.SceneManager.CurrentScene.MapConfig.MapDic[MapItemId] as MapBuildingItem;
                LogicCore.SP.LockFrameMgr.SendCommand(new CreateBuildingCommand {BuildingId = 1002, Sender = mp.Id});
            }
            if (GUILayout.Button("CreateBarack2"))
            {
                var mp = bs.GetObject<MainPlayer>();
                if (mp != null)
                    LogicCore.SP.LockFrameMgr.SendCommand(new CreateBuildingCommand
                    {
                        BuildingId = 1002,
                        Sender = mp.Id
                    });
                else
                {
                    LogicCore.SP.LockFrameMgr.SendCommand(new CreateBuildingCommand {BuildingId = 1002});
                }
            }
            if (GUILayout.Button("SaveLog"))
            {
                NetDataWriter w = new NetDataWriter();
                w.Put((short) 1);
                EventDispatcher.FireEvent(UIEventList.SendNetMsg.ToInt(), this,
                    EventGroup.NewArg<EventSingleArgs<NetDataWriter>, NetDataWriter>(w));
            }
#if UNITY_ANDROID
            if (GUILayout.Button("OpenLogWindow", GUILayout.Width(150), GUILayout.Height(70)))
            {
                _showLogWindow = !_showLogWindow;
            }
#endif
            GUILayout.EndVertical();
        }
#if UNITY_ANDROID
        if(_showLogWindow)
        {
            _offset = GUILayout.BeginScrollView(_offset, GUILayout.Width(400), GUILayout.Height(500));
            for (int i = 0; i < log.Count; i++)
            {
                GUILayout.Label(log[log.Count-i-1]);
            }
            GUILayout.EndScrollView();
        }

#endif
        GUILayout.EndHorizontal();
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
        //managerDriver.Update();
        //u3dSceneManager.Update();
        //Logic.LogicCore.SP.Update(Time.deltaTime);
	}
    void FixedUpdate()
    {
        //Logic.LogicCore.SP.FixedUpdate();
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
