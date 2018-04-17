using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using System;

public abstract class UITemplate
{
    public GameObject Go;
    public UITemplate()
    {
    }
    public UITemplate(GameObject go)
    {
        Go = go;
        Init();
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
        Object.Destroy(Go);
    }
    protected virtual void OnDispose()
    {

    }
}
public class UIContainer<T> : UITemplate where T : UITemplate, new()
{
    public Action<int> OnAddItem;
    //public Action<int> OnRemoveItem;
    private GameObject template;
    private List<T> uiControls = new List<T>();
    public int ChildCount
    {
        get
        {
            return uiControls.Count;
        }
    }
    public UIContainer(GameObject go) : base(go)
    {
        template = go.transform.GetChild(0).gameObject;
        template.SetActive(false);
    }
    public void Resize(int size)
    {
        if(uiControls.Count>size)
        {
            int count = uiControls.Count;
            for (int i = count-1; i >= size; i--)
            {
                var g = uiControls[i];
                uiControls.RemoveAt(i);
                g.Dispose();
            }
        }
        else
        {
            int count = uiControls.Count;
            for (int i = count; i < size; i++)
            {
                var g = GameObject.Instantiate(template.gameObject);
                g.transform.SetParent(Go.transform);
                g.SetActive(true);
                g.transform.localScale = Vector3.one;
                g.transform.localPosition = Vector3.zero;
                var t= new T();
                t.Go = g;
                t.Init();
                uiControls.Add(t);
            }
        }
    }
    public T GetChild(int index)
    {
        return uiControls[index];
    }
}
public static class UIUtility
{
   
}
