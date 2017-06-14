using Logic;
using Logic.Skill.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class U3DDisplayAction : IPool{
    public static Dictionary<Type, Type> _Logic_Display_Actions = new Dictionary<Type, Type>()
    {
        { typeof(PlayAnimationAction), typeof(U3DPlayAnimationAction) },
        { typeof(PlayFXAction), typeof(U3DPlayFXAction) },

    };

    public DisplayAction action;

    public abstract void Execute(U3DCharacter sender, U3DCharacter receiver, object data);

    public void Reset() { }

    public abstract void Stop();
}

public class U3DDisplayActionManager
{
    private U3DCharacter u3dCharacter;
    private List<U3DDisplayAction> displayActions = new List<U3DDisplayAction>();

    public U3DDisplayActionManager(U3DCharacter u3dCharacter)
    {
        this.u3dCharacter = u3dCharacter;
    }
    public void Play(DisplayAction action)
    {
        Type targetType = U3DDisplayAction._Logic_Display_Actions[action.GetType()];
        U3DDisplayAction u3dDisplayAction = PoolManager.SP.Get(targetType) as U3DDisplayAction;
        u3dDisplayAction.action = action;
        u3dDisplayAction.Execute(u3dCharacter, null, null);
    }
    public void Stop(DisplayAction action)
    {
        for (int i = 0; i < displayActions.Count; i++)
        {
            if(displayActions[i].action == action)
            {
                displayActions[i].Stop();
                PoolManager.SP.Recycle(displayActions[i]);
                displayActions.RemoveAt(i);
                i--;
            }
        }
    }
}
