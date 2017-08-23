using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic;
using Logic.LogicObject;
using UnityEngine;

class Player : Character
{
    public StateMachine StateMachine { get; private set; }
    internal override void OnInit()
    {
        base.OnInit();
        StateMachine = new StateMachine(this);
    }

    internal override void ListenEvents()
    {
        base.ListenEvents();
        LogicCore.SP.EventGroup.ListenEvent((int)LogicCore.LogicCoreEvent.OnJoystickStart, OnJoystickStart);
        LogicCore.SP.EventGroup.ListenEvent((int)LogicCore.LogicCoreEvent.OnJoystickMove, OnJoystickMove);
        LogicCore.SP.EventGroup.ListenEvent((int)LogicCore.LogicCoreEvent.OnJoystickEnd, OnJoystickEnd);

    }

    private void OnJoystickEnd(object sender, EventMsg e)
    {
        var cmd = Pool.SP.Get<MoveCommand>();
        cmd.Sender = Id;
        LogicCore.SP.LockFrameMgr.AddFrameCommand(cmd);
     //   StateMachine.Start(new IdleState());
    }

    private void OnJoystickMove(object sender, EventMsg e)
    {
        var msg = e as EventSingleArgs<Vector2>;
        Vector3 direction = new Vector3(msg.value.x, 0, msg.value.y);
        var p = Camera.main.transform.parent;
        var r = p.rotation* direction;
        var forward = new Vector3d(r);
        var dot = Vector3d.Dot(forward, Forward);
        if (dot < FixedMath.One*99/100)
        {
            var cmd = Pool.SP.Get<ChangeForwardCommand>();
            cmd.Sender = Id;
            cmd.Forward = forward;
            LogicCore.SP.LockFrameMgr.AddFrameCommand(cmd);
            //Forward = new Vector3d(r);
        }
    }

    private void OnJoystickStart(object sender, EventMsg e)
    {
        var cmd = Pool.SP.Get<MoveCommand>();
        cmd.Sender = Id;
        LogicCore.SP.LockFrameMgr.AddFrameCommand(cmd);
       //StateMachine.Start(new MoveState());
    }

    internal override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        StateMachine.Update();
    }
}
