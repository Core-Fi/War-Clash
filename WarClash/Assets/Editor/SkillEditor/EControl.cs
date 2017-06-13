using System;
using UnityEngine;
using System.Collections;
using Logic.Skill.Actions;
using UnityEditor;

public class EControl : IEElement
{
    public int width = 20;
    public int height = 20;
    public bool isDraging;
    private bool isSelected;
    public Action<EControl> OnSelected;
    public Action<EControl> OnDrag;
    public Vector2 startDragMousePosi;

    public EControl()
    {
        
    }
    public virtual void Draw()
    {
        #region MouseEvent
        if (Event.current != null)
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        isSelected = true;
                        if (OnSelected != null)
                        {
                            OnSelected.Invoke(this);
                        }
                        startDragMousePosi =  Event.current.mousePosition;
                    }
                    break;
                case EventType.MouseDrag:
                    if (isSelected)
                    {
                        isDraging = true;
                        if (OnDrag != null)
                        {
                            OnDrag.Invoke(this);
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (isSelected)
                    {
                        isSelected = false;
                        isDraging = false;
                    }
                    break;
            }
        }
        #endregion
    }

}
