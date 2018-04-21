using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Logic.Components
{
    public class GravityComponent : SceneObjectBaseComponent
    {
        private long gravity = FixedMath.One * 20;
        public override void OnAdd()
        {
            base.OnAdd();
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            base.SceneObject.TransformComp.Velocity += Vector3d.down * gravity * LockFrameMgr.FixedFrameTime;
        }
         
    }
}
