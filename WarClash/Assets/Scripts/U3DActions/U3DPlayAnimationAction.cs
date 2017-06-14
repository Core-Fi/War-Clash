using Logic.Skill.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class U3DPlayAnimationAction : U3DDisplayAction
{
    private PlayAnimationAction animationAction;
    public override void Execute(U3DCharacter sender, U3DCharacter receiver, object data)
    {
        animationAction = this.action as PlayAnimationAction;
        sender.animator.Play(animationAction.animaitonName);
    }
    public override void Stop()
    {
        animationAction = null;
    }
}
