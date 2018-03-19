using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Components
{
    class SpeedUpComponent : SceneObjectBaseComponent
    {
        public Vector3d Acceleration;
        private long maxSpeed;
        private long sqlMaxSpeed;
        private TransformComponent transformComp;
        public override void OnAdd()
        {
            transformComp = SceneObject.GetComponent<TransformComponent>();
            maxSpeed = SceneObject.AttributeManager[AttributeType.MaxSpeed];
            sqlMaxSpeed = maxSpeed.Mul(maxSpeed);
            base.OnAdd();
        }
        public override void OnRemove()
        {
            base.OnRemove();
            transformComp = null;
        }
        public override void OnFixedUpdate()
        {
            var newV = transformComp.Velocity + Acceleration; 
            if (newV.sqrMagnitude < sqlMaxSpeed)
                transformComp.Velocity = newV;
            base.OnFixedUpdate();
        }
    }
}
