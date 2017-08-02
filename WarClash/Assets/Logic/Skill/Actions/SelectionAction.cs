
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;

namespace Logic.Skill.Actions
{
    [Serializable]
    public class BaseSelectionAction : BaseAction
    {
        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            base.Execute(sender, reciever, data);
        }
    }
    [Display("范围选择/矩形")]
    public class RectSelectionAction : BaseSelectionAction
    {
        [Display("事件效果路径")]
        public string path { get; private set; }

        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            LogicCore.SP.sceneManager.currentScene.ForEachDo<Character>((so) =>
            {
                if(so != sender)
                    EventManager.AddEvent(path, new RuntimeData(sender, so, data));
            });
            base.Execute(sender, reciever, data);
        }
    }

}
