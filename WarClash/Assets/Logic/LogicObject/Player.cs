using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic;
using Logic.LogicObject;
using UnityEngine;

class Player : Character
{
    internal override void ListenEvents()
    {
        base.ListenEvents();
        LogicCore.SP.EventGroup.ListenEvent((int)LogicCore.LogicCoreEvent.OnJoystickStart, OnJoystickStart);
        LogicCore.SP.EventGroup.ListenEvent((int)LogicCore.LogicCoreEvent.OnJoystickMove, OnJoystickMove);
        LogicCore.SP.EventGroup.ListenEvent((int)LogicCore.LogicCoreEvent.OnJoystickEnd, OnJoystickEnd);

    }

    private void OnJoystickEnd(object sender, EventMsg e)
    {
        
    }

    private void OnJoystickMove(object sender, EventMsg e)
    {
        var msg = e as EventSingleArgs<Vector2>;
        Vector3 direction = new Vector3(msg.value.x, 0, msg.value.y);
        var p = Camera.main.transform.parent;
        var r = p.rotation* direction;
        Debug.LogError(direction+"   "+r);
    }

    private void OnJoystickStart(object sender, EventMsg e)
    {
       
    }
}
