using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.LogicObject
{
    public interface IScene
    {
        void Init();
        void OnUpdate(float deltaTime);
        void OnFixedUpdate(long deltaTime);
        void Destroy();
    }
}
