using Logic;
using System;

public interface IManager : IEventDispatcher
{
    void Update();
    void Dispose();
}


