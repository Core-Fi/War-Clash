
using System;
using UnityEngine;

public interface IView
{
    GameObject go { get; set; }
    void Init(GameObject go);
    void Show(object para);
    void Update();
    void Hide();
    void Dispose();

}

