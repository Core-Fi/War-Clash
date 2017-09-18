using System;
using System.Collections.Generic;
using UnityEngine;

class ViewManager : Manager
{
    struct  PendingViewInfo
    {
        public string UiName;
        public Type UiType;
        public object Param;
    }
    private readonly List<View> _views = new List<View>();
    private readonly List<PendingViewInfo> _waitingForLoadView = new List<PendingViewInfo>(1);  
    public ViewManager()
    {
        ListenEvent(UIEventList.ShowUI.ToInt(), ShowUI);
        ListenEvent(UIEventList.HideUI.ToInt(), HideUI);
    }

    private void HideUI(object sender, EventMsg e)
    {
        var v = e as EventSingleArgs<View>;
        v.value.OnHide();
        FireEvent((int)UIEventList.OnHideUI, this, EventGroup.NewArg<EventSingleArgs<View>, View>(v.value));
    }

    private void ShowUI(object sender, EventMsg e)
    {
        var msg = e as EventThreeArgs<string, Type, object>;
        string uiName = msg.value1;
        Type t = msg.value2;
        object para = msg.value3;
        _waitingForLoadView.Add(new PendingViewInfo() {UiName = uiName, UiType = t, Param = para});
        AssetResources.LoadAsset(uiName, OnLoadUI);
    }

    private void OnLoadUI(string uiName, UnityEngine.Object obj)
    {
        PendingViewInfo info = default(PendingViewInfo);
        for (int i = _waitingForLoadView.Count-1; i >= 0; i--)
        {
            var pendingViewInfo = _waitingForLoadView[i];
            if (pendingViewInfo.UiName.Equals(uiName))
            {
                info = pendingViewInfo;
                _waitingForLoadView.RemoveAt(i);
            }
        }
        if (!info.Equals(default(PendingViewInfo)) && info.UiName.Equals(uiName))
        {
            GameObject go = UnityEngine.Object.Instantiate(obj) as GameObject;
            go.transform.parent = Main.SP.Uiparent;
            var v = Activator.CreateInstance(info.UiType) as View;
            _views.Add(v);
            v.name = uiName;
            v.Init(go);
            v.Show(info.Param);
            FireEvent((int) UIEventList.OnShowUI, this, EventGroup.NewArg<EventSingleArgs<View>, View>(v));
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        for (int i = 0; i < _views.Count; i++)
        {
            _views[i].Update();
        }
    }

}
