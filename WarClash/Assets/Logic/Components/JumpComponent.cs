using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Components
{
    class JumpComponent : SceneObjectBaseComponent
    {
        public long InitSpeed;
        TransformComponent transComp;
        public override void OnAdd()
        {
            transComp = SceneObject.GetComponent<TransformComponent>();
            transComp.Velocity.y = InitSpeed;
            base.OnAdd();
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

    }
}
