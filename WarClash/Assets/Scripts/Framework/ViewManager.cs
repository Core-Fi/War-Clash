using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ViewManager : Manager
{
    public List<View> views = new List<View>();
    public ViewManager()
    {
        ListenEvent((int)EventList.ShowUI, ShowUI);
        ListenEvent((int)EventList.HideUI, HideUI);
    }

    private void HideUI(object sender, EventMsg e)
    {
        var v = e as EventSingleArgs<View>;
        v.value.OnHide();
        FireEvent((int)EventList.OnHideUI, this, EventGroup.NewArg<EventSingleArgs<View>, View>(v.value));
    }

    private void ShowUI(object sender, EventMsg e)
    {
        var msg = e as EventThreeArgs<string, Type, object>;
        string ui_name = msg.value1;
        Type t = msg.value2;
        object para = msg.value3;

        var v = Activator.CreateInstance(t) as View;
        views.Add(v);
        v.name = ui_name;
        v.Init(null);
        v.Show(para);

        FireEvent((int)EventList.OnShowUI, this, EventGroup.NewArg<EventSingleArgs<View>, View>(v));
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        for (int i = 0; i < views.Count; i++)
        {
            views[i].Update();
        }
    }

}
