using System.Collections;
using System.Collections.Generic;

using Lockstep;
using UnityEngine;

public class Test925 : MonoBehaviour
{
    public int renderOrder;
    public GameObject g;
    public RectTransform rt;
    public Camera UICamera;
    void Update()
    {
        var r = GetComponent<MeshRenderer>();
        r.sortingOrder = renderOrder;
        //var fq = FixedQuaternion.LookRotation(new Vector3d(g.transform.forward), Vector3d.up);
        //transform.rotation = new Quaternion(fq.x.ToFloat(), fq.y.ToFloat(), fq.z.ToFloat(), fq.w.ToFloat());
        //var vp = Camera.main.WorldToViewportPoint(g.transform.position);
        //var sp = UICamera.ViewportToScreenPoint(vp);
        //sp.x =  sp.x - Screen.width/2f;
        //sp.y = sp.y - Screen.height/2f;
        //rt.anchoredPosition = new Vector2(sp.x,sp.y);
    }

}
