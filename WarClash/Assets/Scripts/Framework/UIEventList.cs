using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Logic.LogicObject;

public enum NetEventList
{
    LockStepMsg,
}

public enum UIEventList
{
    SendNetMsg,
    ShowUI,
    HideUI,
    OnShowUI,
    OnHideUI,

    #region Hud

    DrawPlayerHud

    #endregion
}

public static class EventMsgUtility
{
    public static int ToInt(this NetEventList e)
    {
        return (int)(e);
    }
    public static int ToInt(this UIEventList e)
    {
        return (int)e +  10000;
    }

    public static int ToInt(this Player.PlayerEvent playerEvent)
    {
        return (int)playerEvent + 1000;
    }

    public static int ToInt(this Character.CharacterEvent characterEvent)
    {
        return (int)characterEvent + 500;
    }
    public static int ToInt(this SceneObject.SceneObjectEvent soEvent)
    {
        return (int)soEvent +1;
    }

    public static int ToInt(this BattleScene.SceneEvent e)
    {
        return (int) e+1;
    }
}

