using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
}

