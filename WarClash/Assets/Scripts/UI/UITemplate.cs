using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class UITemplate
{
    public GameObject Go;
    public int Id;
    public UITemplate(GameObject go)
    {
        Go = go;
     
    }
    public void Init()
    {
        OnInit();
    }
    protected virtual void OnInit()
    {
        
    }

    public void Dispose()
    {
        OnDispose();
    }

    protected virtual void OnDispose()
    {
        
    }
}
