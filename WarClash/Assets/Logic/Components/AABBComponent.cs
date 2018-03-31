using Lockstep;
using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Components
{
    class AABBComponent : SceneObjectBaseComponent
    {
        public AABB AABB;
        public long Rotation;
        private FixtureProxy fp;
        public int fpId { get; private set; }
        private Vector2d previousPosi;
        public override void OnAdd()
        {
            base.OnAdd();
            var bs = LogicCore.SP.SceneManager.CurrentScene as BattleScene;
            fp = new FixtureProxy();
            fp.AABB = AABB;
            fp.Fixture = new Transform2d() { p = AABB.Center, angle = Rotation };
            var halfw = AABB.Width / 2;
            var halfh = AABB.Height / 2;
            var radius = FixedMath.Sqrt((halfw).Mul(halfw) + halfh.Mul(halfh));
            var outteraabb = new AABB(AABB.Center, radius * 2, radius * 2);
            fpId = bs.PhysicsTree.AddProxy(ref outteraabb, fp);
            previousPosi = AABB.Center;
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }
        public override void OnRemove()
        {
            base.OnRemove();
        }
    }
}
