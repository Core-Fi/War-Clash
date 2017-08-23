using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

class UIJoyStick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [System.Serializable] public class OnMoveStartHandler : UnityEvent { }
    [System.Serializable] public class OnMoveSpeedHandler : UnityEvent<Vector2> { }
    [System.Serializable] public class OnMoveHandler : UnityEvent<Vector2> { }
    [System.Serializable] public class OnMoveEndHandler : UnityEvent { }
    [SerializeField]
    public OnMoveStartHandler OnMoveStart;
    [SerializeField]
    public OnMoveHandler OnMove;
    [SerializeField]
    public OnMoveSpeedHandler OnMoveSpeed;
    [SerializeField]
    public OnMoveEndHandler OnMoveEnd;
    public RectTransform Thumb;
    [SerializeField]
    private RectTransform cachedRectTransform;
    private Vector2 thumbPosition;
    private Vector2 tmpAxis;

    void Awake()
    {
        this.OnMoveStart.AddListener(() => { LogicCore.SP.EventGroup.FireEvent((int)LogicCore.LogicCoreEvent.OnJoystickStart, this, null); });
        this.OnMove.AddListener((p) => { LogicCore.SP.EventGroup.FireEvent((int)LogicCore.LogicCoreEvent.OnJoystickMove, this, EventGroup.NewArg<EventSingleArgs<Vector2>, Vector2>(p)); });
        this.OnMoveEnd.AddListener(() => { LogicCore.SP.EventGroup.FireEvent((int)LogicCore.LogicCoreEvent.OnJoystickEnd, this, null); });
        cachedRectTransform = transform as RectTransform;
    }

    private float GetRadius()
    {
        return cachedRectTransform.sizeDelta.x*0.5f;
    }

    void Update()
    {
        var oldAxis = tmpAxis;
        tmpAxis = thumbPosition / GetRadius();
        if (!tmpAxis.x.RoundEquals(0) || !tmpAxis.y.RoundEquals(0))
        {
            if (oldAxis == Vector2.zero)
            {
                OnMoveStart.Invoke();
            }
            OnMove.Invoke(tmpAxis);
        }
        else if (tmpAxis.x.RoundEquals(0) && tmpAxis.y.RoundEquals(0) && oldAxis != Vector2.zero)
        {
            OnMoveEnd.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float radius = GetRadius();
        thumbPosition = (eventData.position - eventData.pressPosition);// / (cachedRootCanvas.rectTransform().localScale.x  ) ;
        thumbPosition.x = Mathf.FloorToInt(thumbPosition.x);
        thumbPosition.y = Mathf.FloorToInt(thumbPosition.y);
        if (thumbPosition.magnitude > radius)
        {
            thumbPosition = thumbPosition.normalized * radius;
        }
        Thumb.anchoredPosition = thumbPosition;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        thumbPosition = Vector2.zero;
        Thumb.anchoredPosition = Vector2.zero;
    }

}
