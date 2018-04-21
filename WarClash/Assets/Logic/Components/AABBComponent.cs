using Lockstep;
using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Logic.Components
{
    class AABBComponent : SceneObjectBaseComponent
    {
        public AABB AABB = new AABB();
        public long Rotation;
        private FixtureProxy fp;
        public int fpId { get; private set; }
        private Vector3d previousPosi;
        private BattleScene bs;
        public AABBComponent()
        {
            ExecuteOrder = ExecuteOrder.Late;
        }
        public override void OnAdd()
        {
            base.OnAdd();
            SceneObject.TransformComp.EventGroup.ListenEvent((int)TransformComponent.Event.OnPositionChange, OnPositionChanged);
            bs = LogicCore.SP.SceneManager.CurrentScene as BattleScene;
            fp = new FixtureProxy();
            fp.AABB = AABB;
            fp.Fixture = new Transform2d() { p = AABB.Center, angle = Rotation };
            var halfw = AABB.Width / 2;
            var halfh = AABB.Height / 2;
            var radius = FixedMath.Sqrt((halfw).Mul(halfw) + halfh.Mul(halfh));
            var outteraabb = new AABB(AABB.Center, radius * 2, radius * 2);
          //  fpId = bs.PhysicsTree.AddProxy(ref outteraabb, fp);
            previousPosi = SceneObject.TransformComp.Position;
        }
        public override void OnFixedUpdate()
        {
            var posi = SceneObject.TransformComp.Position;
            if(posi.x != previousPosi.x || posi.y != previousPosi.y)
            {
                var offset = posi - previousPosi;
                long x = posi.x, y = posi.y;
                var xaabb = new AABB(new Vector2d(previousPosi.x+offset.x, previousPosi.y + FixedMath.Half), FixedMath.One-2, FixedMath.One-2);
                var rst = bs.ObbTest(xaabb, 0);
                if(rst.EndIndex!=0)
                {
                    int index = -1;
                    if (rst.EndIndex == 1)
                    {
                        index = 0;
                    }
                    else
                    {

                        for (int i = 0; i < rst.EndIndex; i++)
                        {
                            var center = rst.HitInfos[i].Proxy.AABB.Center;
                            if (Vector2d.Dot(center - new Vector2d(previousPosi.x, previousPosi.y), new Vector2d(offset.x, 0)) < 0)
                            {
                                i = index;
                                break;
                            }
                        }
                    }
                    var hit = rst.HitInfos[index];
                    if(hit.Proxy.AABB.Center.x > xaabb.Center.x)
                    {
                        x = hit.Proxy.AABB.Center.x - FixedMath.One;
                    }
                    else
                    {
                        x = hit.Proxy.AABB.Center.x + FixedMath.One;
                    }
                }
                var yaabb = new AABB(new Vector2d(previousPosi.x, previousPosi.y + offset.y + FixedMath.Half), FixedMath.One - 2, FixedMath.One - 2);
                yaabb.DrawAABB();
                rst = bs.ObbTest(yaabb, 0);
                if (rst.EndIndex != 0)
                {
                    var hit = rst.HitInfos[0];
                    if (hit.Proxy.AABB.Center.y > yaabb.Center.y)
                    {
                        y = hit.Proxy.AABB.Center.y - FixedMath.Half-FixedMath.One;
                        SceneObject.TransformComp.Velocity.y = 0;
                    }
                    else
                    {
                        y = hit.Proxy.AABB.Center.y + FixedMath.Half;
                        SceneObject.TransformComp.Velocity.y = 0;
                    }
                }
                var correctedPosi = new Vector3d(x, y, 0);
                SceneObject.TransformComp.Position = correctedPosi;
                previousPosi = correctedPosi;
            }
            base.OnFixedUpdate();

        }
        public override void OnRemove()
        {
            base.OnRemove();
        }
        private void OnPositionChanged(object sender, EventMsg e)
        {
            var msg = e as EventTwoArgs<Vector3d, Vector3d>;
            AABB aabb = new AABB(new Vector2d(msg.value2.x, msg.value2.y + FixedMath.Half) , FixedMath.One, FixedMath.One);
            fp.AABB = aabb;
           // bs.PhysicsTree.MoveProxy(fpId, ref aabb, Vector2d.zero);
            //var output = bs.ObbTest(aabb, 0);
            //if(output.HitInfos!=null && output.HitInfos.Length>0)
            //{
            //    for (int i = 0; i < output.EndIndex; i++)
            //    {
            //        var hit = output.HitInfos[i];

            //        DLog.Log(hit.Proxy.AABB.Center.ToString());
            //    }
            //}
        }
    }
}
