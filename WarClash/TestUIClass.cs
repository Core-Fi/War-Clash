using UnityEngine;
using UnityEngine.UI;
public class test : UITemplate
{
GameObject m_txt_go;
UnityEngine.UI.Text m_txt_text;
protected override void OnInit()
{
m_txt_go = MainGo.transform.Find('m_txt').gameObject;
m_txt_text = m_txt_go.GetComponent<UnityEngine.UI.Text>();
}
}
