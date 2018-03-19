using Logic.Skill.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using UnityEngine;

public class U3DPlayAnimationAction : U3DDisplayAction
{
    private PlayAnimationAction _animationAction;
 
    public override void Execute(U3DSceneObject sender, U3DSceneObject receiver, object data)
    {
        _animationAction = this.Action as PlayAnimationAction;
        var transformComp = sender.GetComponent<U3DModelComponent>();
        var animator = transformComp.Go.GetComponent<Animator>();
        if(animator != null)
            animator.Play(_animationAction.animaitonName, -1, 0);
    }
    public override void Stop()
    {
        _animationAction = null;
    }
}
