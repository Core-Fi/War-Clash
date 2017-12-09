using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public  class ScrollDragAndDropItem : DragAndDropItem
{
    void Start()
    {
    }
    private bool? _canDrag;
    public override void OnBeginDrag(PointerEventData eventData)
    {
        _canDrag = null;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if(_canDrag.Value)
            base.OnEndDrag(eventData);
    }

    public override void OnDrag(PointerEventData data)
    {
        if (_canDrag == null)
        {
            if (data.delta.y >0 && ( Mathf.Abs(data.delta.x) < Mathf.Abs(data.delta.y*3)))
            {
                _canDrag = true;
                base.OnBeginDrag(data);
            }
            else
            {
                _canDrag = false;
            }
        }
        if (_canDrag != null && _canDrag.Value == true)
        {
            base.OnDrag(data);
        }
        else
        {
            var sr = GetComponentInParent<ScrollRect>();
            sr.OnDrag(data);
        }
        
    }

}
