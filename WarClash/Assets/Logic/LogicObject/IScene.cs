using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.LogicObject
{
    public interface IScene
    {
        void Init();
        void Update(float deltaTime);
        void FixedUpdate(long deltaTime);
        void Destroy();
    }
}
