using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ERuntimeOperatePanel : IEElement
{
    private GameObject testPlayer;
    private string playerID = "";
    public void Draw()
    {
        playerID = GUILayout.TextField(playerID, GUILayout.MinWidth(60));


    }
}
