using System;
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
        var rotateRate = FixedQuaternion.AngleAxis(FixedMath.One * 250, new Vector3d(UnityEngine.Vector3.up));
        Vector3d dir = new Vector3d(Vector3.forward);
        dir = rotateRate * dir;
    }
	// Use this for initialization
	void Start ()
	{
	    Debug.LogError((FixedMath.One*1000).ToInt());
        DonotDestroy();
        Logic.LogicCore.SP.Init();
        u3dSceneManager = new U3DSceneManager();
        Logic.LogicCore.SP.SceneManager.SwitchScene(new Scene());

        var b = LogicCore.SP.SceneManager.currentScene.CreateSceneObject<BarackBuilding>();
        b.Team = Team.Team1; 
        b.Position = new Lockstep.Vector3d(new Vector3(0, 0, 0));

        var p = LogicCore.SP.SceneManager.currentScene.CreateSceneObject<Player>();
        p.Team = Team.Team1;
        p.Position = new Lockstep.Vector3d(new Vector3(-1, 0, 0));

        EventDispatcher.FireEvent((int)EventList.ShowUI, this, EventGroup.NewArg<EventThreeArgs<string, Type, object>, string, Type, object>("UI-JoyStick.prefab", typeof(BattleView), null));
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 300, 50), "GC"))
    //    {
    //        System.GC.Collect();
    //        Resources.UnloadUnusedAssets();
    //    }
    //    if (GUI.Button(new Rect(0,50, 300, 50),"Unload"))
    //    {
    //        Resource.UnloadBundles();
    //    }
    //}
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
            EventDispatcher.FireEvent((int)EventList.ShowUI, this, EventGroup.NewArg<EventThreeArgs<string, Type, object>, string, Type, object>("NextBattleUI.prefab", typeof(BattleView), null));
	        a = true;
	    }
        managerDriver.Update();
        u3dSceneManager.Update();
        Logic.LogicCore.SP.Update(Time.deltaTime);
	}
    void FixedUpdate()
    {
        for (int i = 0; i < 1; i++)
        {
            Logic.LogicCore.SP.FixedUpdate();
        }
    }
}
