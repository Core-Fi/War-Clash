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
        private long _gravity;
        public override void OnAdd()
        {
            transComp = SceneObject.GetComponent<TransformComponent>();
            transComp.Velocity.y = InitSpeed;
            _gravity = FixedMath.One * 10;
            base.OnAdd();
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            transComp.Velocity.y -= _gravity /15;
        }

    }
}
