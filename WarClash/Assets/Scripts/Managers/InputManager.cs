using Logic;
using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class InputManager : Manager
{
    public override void OnUpdate()
    {
        base.OnUpdate();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            var cmd = Pool.SP.Get<JumpCommand>();
            cmd.Sender = SceneObject.MainPlayer.Id;
            LogicCore.SP.LockFrameMgr.SendCommand(cmd);
        }
    }
}
