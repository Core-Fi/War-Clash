using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using Logic.LogicObject;
using UnityEngine.UI;
using System;
using Lockstep;
using Logic.Skill;

public class UIBattle : View {

    private List<ArmyItem> _armyItems = new List<ArmyItem>();
    public GameObject m_followme_go;
    public UnityEngine.UI.Button m_followme_button;
    public GameObject m_atk_go;
    public UnityEngine.UI.Button m_atk_button;
    public GameObject m_scrollRect_go;
    public GameObject m_layout_go;
    public UnityEngine.UI.HorizontalLayoutGroup m_layout_horizontallayoutgroup;
    public GameObject m_template_cell_go;



    public override void OnInit(GameObject go)
    {
        base.OnInit(go);
        m_followme_go = Go.transform.Find("Strategy/m_followme").gameObject;
        m_followme_button = m_followme_go.GetComponent<UnityEngine.UI.Button>();
        m_atk_go = Go.transform.Find("m_atk").gameObject;
        m_atk_button = m_atk_go.GetComponent<UnityEngine.UI.Button>();
        m_scrollRect_go = Go.transform.Find("m_scrollRect").gameObject;
        m_layout_go = Go.transform.Find("m_scrollRect/m_layout").gameObject;
        m_layout_horizontallayoutgroup = m_layout_go.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        m_template_cell_go = Go.transform.Find("m_scrollRect/m_layout/m_template_cell").gameObject;

        m_template_cell_go.SetActive(false);
        m_atk_button.onClick.AddListener(OnAtkBtnClick);
    }

    public override void OnShow(object para)
    {
        base.OnShow(para);
        
        for (int i = 0; i < 5; i++)
        {
            var newGo = CreateNew(m_template_cell_go);
            var army = new ArmyItem(newGo);
            army.Init();
            _armyItems.Add(army);
        }
    }
    private GameObject CreateNew(GameObject go)
    {
        var newgo = UnityEngine.Object.Instantiate(go) as GameObject;
        newgo.transform.SetParent(go.transform.parent);
        newgo.SetActive(true);
        newgo.transform.position = go.transform.position;
        newgo.transform.rotation = go.transform.rotation;
        newgo.transform.localScale = go.transform.localScale;
        return newgo;
    }
    public override void OnDispose()
    {
        base.OnDispose();
        m_atk_button.onClick.RemoveListener(OnAtkBtnClick);
    }

    private void OnFollowMeClick()
    {
        var cmd = new ChangeStrategyCommand{Strategy = (byte)LockFrameMgr.Strategy.FollowPlayer, Sender = SceneObject.MainPlayer.Id};
        LogicCore.SP.LockFrameMgr.SendCommand(cmd);
    }

    private void OnAtkBtnClick()
    {
        var sm = SceneObject.MainPlayer.GetComponent<SkillManager>();
        if (sm.IsRunningSkill)
        {
            EventDispatcher.FireEvent(UIEventList.OnSkillBtnClick.ToInt(), this, null);
        }
        else
        {
            var _battleScene = LogicCore.SP.SceneManager.CurrentScene as BattleScene;
            var cmd = Pool.SP.Get<ReleaseSkillCommand>();
            cmd.Id = 4;
            cmd.Sender = SceneObject.MainPlayer.Id;
            LogicCore.SP.LockFrameMgr.SendCommand(cmd);
            var buffcmd = Pool.SP.Get<ReleaseBuffCommand>();
            buffcmd.Id = 1;
            buffcmd.Sender = SceneObject.MainPlayer.Id;
            LogicCore.SP.LockFrameMgr.SendCommand(buffcmd);
        }
    }

}

public class ArmyItem : UITemplate
{
    public ArmyItem(GameObject go) : base(go)
    { }
    public GameObject m_icon_go;
    public UnityEngine.UI.Image m_icon_image;
    private DragAndDropItem _dragAndDropItem;
    protected override void OnInit()
    {
        m_icon_go = Go.transform.Find("bg/m_icon").gameObject;
        m_icon_image = m_icon_go.GetComponent<UnityEngine.UI.Image>();
        _dragAndDropItem = m_icon_go.GetComponent<DragAndDropItem>();
        _dragAndDropItem.OnItemDragEndEvent += OnDragEnd;
    }

    private bool CanDrag()
    {
        return true;
    }
    private void OnDragEnd(DragAndDropItem item)
    {
        Ray ray = Main.SP.MainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 intersection;
        Math3D.LinePlaneIntersection(out intersection, ray.origin, ray.direction, Vector3.up, Vector3.zero);
        Debug.DrawRay(ray.origin, ray.direction*1000, Color.blue, 20);
        //var cmd = new CreateNpcCommand {NpcId = 1001, PosiX = FixedMath.Create(intersection.x), PosiZ = FixedMath.Create(intersection.z)};
        //LogicCore.SP.LockFrameMgr.SendCommand(cmd);
    }

    protected override void OnDispose()
    {
        _dragAndDropItem.OnItemDragEndEvent -= OnDragEnd;
        base.OnDispose();
    }
}
