using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Logic.Components
{
    class MainPlayerControlComponent : BaseComponent
    {
        bool _isPressing;
        public override void OnAdd()
        {
            base.OnAdd();
            LogicCore.SP.EventGroup.ListenEvent((int)LogicCore.LogicCoreEvent.OnJoystickStart, OnJoystickStart);
            LogicCore.SP.EventGroup.ListenEvent((int)LogicCore.LogicCoreEvent.OnJoystickMove, OnJoystickMove);
            LogicCore.SP.EventGroup.ListenEvent((int)LogicCore.LogicCoreEvent.OnJoystickEnd, OnJoystickEnd);
        }
        private void OnJoystickEnd(object sender, EventMsg e)
        {
            _isPressing = false;
            var cmd = Pool.SP.Get<StopCommand>();
            cmd.Sender = SceneObject.Id;
            LogicCore.SP.LockFrameMgr.SendCommand(cmd);
        }

        private void OnJoystickMove(object sender, EventMsg e)
        {
            var msg = e as EventSingleArgs<Vector2>;
            Vector3 direction = new Vector3(msg.value.x, 0, msg.value.y);
            var p = Camera.main.transform.parent;
            var r = p.rotation * direction;
            var forward = new Vector3d(r);
            var dot = Vector3d.Dot(forward, SceneObject.Forward);
            if (dot < FixedMath.One * 99 / 100)
            {
                var cmd = Pool.SP.Get<ChangeForwardCommand>();
                cmd.Sender = SceneObject.Id;
                cmd.Forward = forward;
                LogicCore.SP.LockFrameMgr.SendCommand(cmd);
            }
        }

        private void OnJoystickStart(object sender, EventMsg e)
        {
            _isPressing = true;
            var cmd = Pool.SP.Get<MoveCommand>();
            cmd.Sender = SceneObject.Id;
            LogicCore.SP.LockFrameMgr.SendCommand(cmd);
        }
    }
}
