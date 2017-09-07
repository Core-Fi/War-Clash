using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum UIEventList
{
    PreLoadResource = 10000,
    //5000开始
    ShowUI = 50000,
    HideUI,
    OnShowUI,
    OnHideUI,

    SendNetMsg = 60000,
}

public enum NetEventList
{
    LockStepMsg,
    LockStepFrame,
    SaveToLog,
    BattleStart,
    PlayerMoveMsg,
    PlayerStopMsg,
    PlayerRotateMsg,
    CreatePlayer,
    CreateNpc
}
