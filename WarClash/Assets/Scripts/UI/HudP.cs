using UnityEngine;
using UnityEngine.UI;
public class HudP : UITemplate
{
public HudP(GameObject go) : base(go)
{
    m_template_player_go = Go.transform.Find("m_template_player").gameObject;
}
public GameObject m_template_player_go;
}
