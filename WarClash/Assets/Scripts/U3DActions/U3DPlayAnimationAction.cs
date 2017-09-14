using Logic.Skill.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;

public class U3DPlayAnimationAction : U3DDisplayAction
{
    private PlayAnimationAction _animationAction;
 
    public override void Execute(U3DCharacter sender, U3DCharacter receiver, object data)
    {
        _animationAction = this.Action as PlayAnimationAction;
        UnityEngine.Debug.Log(sender+" "+_animationAction.animaitonName);
        sender.animator.Play(_animationAction.animaitonName, -1, 0);
    }
    public override void Stop()
    {
        _animationAction = null;
    }
}
