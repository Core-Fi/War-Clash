using Lockstep;
using Logic;
using Logic.Components;
using Logic.LogicObject;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : UITemplate
{
    public U3DSceneObject U3DSceneObject;
    private int _maxHp;
    private int _curHp;
    private RectTransform rt;
    #region AutoMaticDeclare
    public GameObject m_Hp_go;
    public Slider m_Hp_slider;
    #endregion
    public PlayerHud(GameObject go, U3DSceneObject player) : base(go)
    {
        U3DSceneObject = player;
    }

    protected override void OnInit()
    {
        #region AutoMaticAssign
        m_Hp_go = Go.transform.Find("m_Hp").gameObject;
        m_Hp_slider = m_Hp_go.GetComponent<Slider>();
        #endregion
        _maxHp = U3DSceneObject.SceneObject.AttributeManager[AttributeType.Maxhp].ToInt();
        _curHp = U3DSceneObject.SceneObject.AttributeManager[AttributeType.Hp].ToInt();
        UpdateSlider();
        U3DSceneObject.SceneObject.TransformComp.EventGroup.ListenEvent((int)TransformComponent.Event.OnPositionChange, OnAttributeChange);
        rt = Go.GetComponent<RectTransform>();
    }

    private void UpdateSlider()
    {
        m_Hp_slider.value = (float)_curHp / _maxHp;
    }
    private void OnAttributeChange(object sender, EventMsg e)
    {
        EventSingleArgs<AttributeMsg> msg = e as EventSingleArgs<AttributeMsg>;
        if (msg == null) return;
        if (msg.value.At == AttributeType.Hp)
        {
            _curHp = msg.value.NewValue.ToInt();
            UpdateSlider();
        }
        else if (msg.value.At == AttributeType.Maxhp)
        {
            _maxHp = msg.value.NewValue.ToInt();
            UpdateSlider();
        }
    }
    public void Update()
    {
        var vp = Main.SP.MainCamera.WorldToViewportPoint(U3DSceneObject.SceneObject.Position.ToVector3() + new Vector3(0,2,0));
        var sp = Main.SP.UICamera.ViewportToScreenPoint(vp);
        sp.x = sp.x - Screen.width / 2f;
        sp.y = sp.y - Screen.height / 2f;
        var np = new Vector2(sp.x, sp.y);
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, np, Time.deltaTime * 6);
    }
}