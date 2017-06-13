using System;
using System.Collections.Generic;

namespace Logic.Skill
{
    public enum UIControlType
    {
        Default,
        Range,
        MutiSelection
    }
    public class DisplayAttribute : Attribute
    {
        public string DisplayName;
        public string GroupName;
        public UIControlType ControlType;
        public object data;
        public DisplayAttribute(string _DisplayName, string _GroupName, UIControlType _ControType, object _data)
        {
            DisplayName = _DisplayName;
            GroupName = _GroupName;
            ControlType = _ControType;
            data = _data;
        }
        public DisplayAttribute(string _DisplayName, string _GroupName, UIControlType _ControType)
        {
            DisplayName = _DisplayName;
            GroupName = _GroupName;
            ControlType = _ControType;
        }
        public DisplayAttribute(string _DisplayName, UIControlType _ControType)
        {
            DisplayName = _DisplayName;
            ControlType = _ControType;
            GroupName = "数据";
        }
        public DisplayAttribute(string _DisplayName, string _GroupName)
        {
            DisplayName = _DisplayName;
            GroupName = _GroupName;
        }
        public DisplayAttribute(string _DisplayName)
        {
            DisplayName = _DisplayName;
            GroupName = "数据";
        }
    }
}
