using UnityEngine;
using UnityEngine.UI;
public class HudP : UITemplate
{
public HudP(GameObject go) : base(go)
{}
public GameObject m_template_player_go;
protected override void OnInit()
{
m_template_player_go = Go.transform.Find("m_template_player").gameObject;
}
}
