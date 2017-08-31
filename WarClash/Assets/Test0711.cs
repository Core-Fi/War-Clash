using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class Test0711 : MonoBehaviour {
    private RectTransform layout;
    private VerticalLayoutGroup v_layout;
    public ScrollRect scrollRect;
    public RectTransform start;
    public RectTransform end;
    public GameObject Template;
    public Queue<RectTransform> pool = new Queue<RectTransform>();
    private LinkedList<RectTransform> list = new LinkedList<RectTransform>();
    private float scrollRectHight;
    private RectTransform rt;
    private List<int> Data = new List<int>();
    public bool recycle;
    private Vector3[] temp = new Vector3[4];
    private Vector3[] scroll_rect_corner = new Vector3[4];
    private bool moveBottom;
    // Use this for initialization
    void Start()
    {
        Genearte();
    }

    void Update()
    {
        if (moveBottom)
        {
            if (GetValue(list.Last.Value) == Data.Last())
            {
                moveBottom = false;
            }
            var posi = layout.anchoredPosition;
            posi.y += Time.deltaTime * 1000;
            layout.anchoredPosition = posi;
        }
    }
    public void Genearte ()
    {
        for (int i = 0; i < 2; i++)
        {
            Data.Add(i);
        }
        v_layout = scrollRect.GetComponentInChildren<VerticalLayoutGroup>();
        layout = v_layout.GetComponent<RectTransform>();
        rt = scrollRect.GetComponent<RectTransform>();
        rt.GetWorldCorners(scroll_rect_corner);//左下左上右上右下
        scrollRectHight = rt.sizeDelta.y;
        scrollRect.onValueChanged.AddListener(OnValueChange);
        v_layout = layout.GetComponent<VerticalLayoutGroup>();
        if (recycle)
        {
            for (int i = 0; i < 1; i++)
            {
                AddLast(i);
            }
        }
        MoveToBottom();
    }
    private RectTransform AddLast(int i)
    {
        bool valid = Data.Contains(i);
        if (!valid) return null;
        RectTransform r = CreateNew();
        AssignValue(r, i);
        r.SetAsLastSibling();
        list.AddLast(r);
        end.SetAsLastSibling();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
        ResizeLayout();
        return r;
    }
    private RectTransform AddFirst(int i)
    {
        bool valid = Data.Contains(i);
        if (!valid) return null;
        RectTransform r = CreateNew();
        AssignValue(r, i);
        r.SetAsFirstSibling();
        list.AddFirst(r);
        start.SetAsFirstSibling();
        return r;
    }

    private void ResizeLayout()
    {
        var b = RectTransformUtility.CalculateRelativeRectTransformBounds(layout);
        var size = layout.sizeDelta;
        size.y = b.size.y;
        size.y = Mathf.Max(scrollRectHight, size.y);
        layout.sizeDelta = size;

    }
    private RectTransform CreateNew()
    {
        if (pool.Count > 0)
        {
            var r = pool.Dequeue();
            r.gameObject.SetActive(true);
            return r;
        }
        else
        {
            GameObject g = GameObject.Instantiate(Template);
            g.SetActive(true);
            g.transform.parent = Template.transform.parent;
            g.transform.localScale = Vector3.one;
            g.transform.position = Template.transform.position;
            var r = g.GetComponent<RectTransform>();
            return r;
        }
    }
    private void AssignValue(RectTransform r, int i)
    {
        var t = r.transform.Find("Text");
        var image = r.transform.Find("Image");
        image.gameObject.SetActive(i%2 ==0 ? true :false);
        var text = t.GetComponent<Text>();
        text.text = i.ToString();
        r.sizeDelta = new Vector2(r.sizeDelta.x, 0);
        var b = RectTransformUtility.CalculateRelativeRectTransformBounds(r);
        r.sizeDelta = new Vector2(r.sizeDelta.x, b.size.y);
    }
    private int GetValue(RectTransform g)
    {
        var t = g.transform.Find("Text");
        var text = t.GetComponent<Text>();
        string str = text.text;
        return int.Parse(str);
    }
    private float GetRectTransformRelativeBoundsHeight(RectTransform r)
    {
        var b = RectTransformUtility.CalculateRelativeRectTransformBounds(r);
        float h = Mathf.Abs(b.extents.y * 2);
        return h;
    }
   

    public void AddNew()
    {
        Data.Add(Data.Count);
        MoveToBottom();
    }

    private void MoveToBottom()
    {
        moveBottom = true;
    }

    private void OnValueChange(Vector2 arg0)
    {
        if (recycle && list.Count > 0 )
        {
           Refresh();
        }
    }

    private void Refresh()
    {
        do
        {
            float h = GetRectTransformRelativeBoundsHeight(list.First.Value);
            list.First.Value.GetWorldCorners(temp);
            if (temp[0].y > scroll_rect_corner[1].y)
            {
                var b = RectTransformUtility.CalculateRelativeRectTransformBounds(list.First.Value);
                start.sizeDelta += new Vector2(b.size.x, b.size.y);
                list.First.Value.gameObject.SetActive(false);
                pool.Enqueue(list.First.Value);
                list.RemoveFirst();
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            }
            list.First.Value.GetWorldCorners(temp);
            if (temp[1].y < scroll_rect_corner[1].y)
            {
                int v = GetValue(list.First.Value);
                var r = AddFirst(v - 1);
                if (r != null)
                {
                    var b = RectTransformUtility.CalculateRelativeRectTransformBounds(list.First.Value);
                    start.sizeDelta -= new Vector2(b.size.x, b.size.y);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
                }
            }
            list.Last.Value.GetWorldCorners(temp);
            h = GetRectTransformRelativeBoundsHeight(list.Last.Value);
            if (temp[1].y < scroll_rect_corner[0].y)
            {
                var b = RectTransformUtility.CalculateRelativeRectTransformBounds(list.Last.Value);
                end.sizeDelta += new Vector2(b.size.x, b.size.y);
                end.sizeDelta = Vector2.Max(Vector2.zero, end.sizeDelta);
                list.Last.Value.gameObject.SetActive(false);
                pool.Enqueue(list.Last.Value);
                list.RemoveLast();
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            }
            list.Last.Value.GetWorldCorners(temp);
            if (temp[0].y > scroll_rect_corner[0].y)
            {
                int v = GetValue(list.Last.Value);
                var r = AddLast(v + 1);
                if (r != null)
                {
                    var b = RectTransformUtility.CalculateRelativeRectTransformBounds(list.Last.Value);
                    end.sizeDelta -= new Vector2(b.size.x, b.size.y);
                    end.sizeDelta = Vector2.Max(Vector2.zero, end.sizeDelta);
                }
            }
        } while (!AllVisible());
    }
    private bool AllVisible()
    {
        bool allVisible = true;
        var etor = list.GetEnumerator();
        while (etor.MoveNext())
        {
            etor.Current.GetWorldCorners(temp);
            if (temp[1].y < scroll_rect_corner[0].y || temp[0].y > scroll_rect_corner[1].y)
            {
                allVisible = false;
                break;
            }
        }
        etor.Dispose();
        return allVisible;
    }
}
