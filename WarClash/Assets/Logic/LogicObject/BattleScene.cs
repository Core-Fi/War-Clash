using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Objects;
using System.IO;
using Lockstep;
using UnityEngine;
using Logic.Components;

namespace Logic.LogicObject
{
    public class BattleScene : ObjectCollection<int, SceneObject>, IScene
    {
        public string Name;
        public Map.Map MapConfig;
        public FixedQuadTree<SceneObject> FixedQuadTree;
        public DynamicTree<FixtureProxy> PhysicsTree;
        private bool _hasInited;
        private PhysicsOutput output = new PhysicsOutput();
        private int _outputIndex;
        public bool CanEnd()
        {
            return false;
        }
        public enum SceneEvent
        {
            AddSceneObject,
            RemoveSceneObject,
            OnCreate,
            OnLoaded,
            OnRaycastCallback,
        }
        public EventGroup EventGroup { get; private set; }
        public BattleScene()
        {
        }

        public BattleScene(string name)
        {
            Name = name;
        }
        public void Init()
        {
            _hasInited = false;
            EventGroup = new EventGroup();
            MapConfig = Logic.Map.Map.Deserialize(Name);
            FixedQuadTree = new FixedQuadTree<SceneObject>();
            FixedQuadTree.SetBounds(new Utility.FixedRect(-FixedMath.One * MapConfig.Width/2, -FixedMath.One * MapConfig.Height / 2, FixedMath.One * MapConfig.Width, FixedMath.One * MapConfig.Height));
            PhysicsTree = new DynamicTree<FixtureProxy>();
            for (int i = 0; i < MapConfig.Data.Data.Count; i++)
            {
                var stageData = MapConfig.Data.Data[i];
                var aabb = new AABB(new Vector2d(stageData.X * FixedMath.One + FixedMath.Half, stageData.Y * FixedMath.One + FixedMath.Half), FixedMath.One, FixedMath.One);
                var fp = new FixtureProxy();
                fp.AABB = aabb;
                fp.Fixture = new Transform2d() { p = aabb.Center, angle = 0 };
                var nodeid = PhysicsTree.AddProxy(ref aabb, fp);
            }
            EventGroup.ListenEvent(SceneEvent.OnLoaded.ToInt(), OnLoded);
        }
        private void OnLoded(object sender, EventMsg msg)
        {
            _hasInited = true;
            EventGroup.DelEvent(SceneEvent.OnLoaded.ToInt(), OnLoded);
            var so = CreateSceneObject();
            var modelComp = new ModelComponent();
            modelComp.RePath = "kachujin.prefab";
            so.AddComponent(modelComp);
            so.AddComponent<MainPlayerComponent>();
            so.AddComponent<GravityComponent>();
            so.AddComponent<StateMachine>();
            so.AddComponent<AttributeManager>();
            so.AttributeManager.SetBase(AttributeType.MaxSpeed, FixedMath.One*2);
            so.Position = new Vector3d(FixedMath.Create(16.5f), FixedMath.One * 12, 0);
            var aabb = new AABBComponent();
            aabb.AABB = new AABB(new Vector2d(FixedMath.One * 15, FixedMath.One * 15+FixedMath.Half), FixedMath.One, FixedMath.One);
            so.AddComponent(aabb);

        }
        public SceneObject CreateSceneObject()
        {
            var id = IDManager.SP.GetID();
            SceneObject so = new SceneObject();
            so.Id = id;
            so.Init(new CreateInfo());
            so.ListenEvent((int)SceneObject.SceneObjectEvent.OnAddComponent, OnSceneObjectAddComponent);
            so.ListenEvent((int)SceneObject.SceneObjectEvent.OnRemoveComponent, OnSceneObjectRemoveComponent);
            so.AddComponent<TransformComponent>();
            this.AddObject(so.Id, so);
            EventGroup.FireEvent(SceneEvent.AddSceneObject.ToInt(), this, EventGroup.NewArg<EventSingleArgs<SceneObject>, SceneObject>(so));
            return so;
        }
       

        internal void RemoveSceneObject(int id)
        {
            this.RemoveObject(id);
            EventGroup.FireEvent((int)SceneEvent.RemoveSceneObject, this, EventGroup.NewArg<EventSingleArgs<int>, int>(id));
        }
    
        private void OnSceneObjectAddComponent(object sender, EventMsg msg)
        {
            var so = sender as SceneObject;
            var e = msg as EventSingleArgs<SceneObjectBaseComponent>;
            AddComponent(e.value.GetType(), so);
        }
        private void OnSceneObjectRemoveComponent(object sender, EventMsg msg)
        {
            var so = sender as SceneObject;
            var e = msg as EventSingleArgs<SceneObjectBaseComponent>;
            AddComponent(e.value.GetType(), so);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
       //     PhysicsTree.DebugDraw();
            if (Main.SP.ShowDebug)
            {
                FixedQuadTree.DebugDraw();
            }
        }

        public override void OnFixedUpdate(long deltaTime)
        {
           // PhysicsTree.DebugDraw();
            base.OnFixedUpdate(deltaTime);
        }
        public PhysicsOutput ObbTest(AABB aabb, long angle)
        {
            var halfw = aabb.Width / 2;
            var halfh = aabb.Height / 2;
            if (angle == 0)
            {
                return ObbTest(aabb, 0, aabb);
            }
            var radius = FixedMath.Sqrt((halfw).Mul(halfw) + halfh.Mul(halfh));
            var outterAABB = new AABB(aabb.Center, radius * 2, radius * 2);
            return ObbTest(aabb, angle, outterAABB);
        }
        private AABB _toTestAABB;
        private long _toTestAABBAngle;
        public PhysicsOutput ObbTest(AABB aabb, long angle, AABB outterAABB)
        {
            output.EndIndex = 0;
            _toTestAABB = aabb;
            _toTestAABBAngle = angle;
            PhysicsTree.Query(ObbTestCallback, ref outterAABB);
            return output;
        }
        private bool ObbTestCallback(int id)
        {
            var f = PhysicsTree.GetUserData(id);
            f.AABB.DrawAABB(Color.red, 0, 0.01f);
            var hit = AABB.TestObb(f.AABB, _toTestAABB, f.Fixture.angle, _toTestAABBAngle);
            if (hit)
            {
                var hitPosi = (f.AABB.Center + _toTestAABB.Center) / 2;
                output.HitInfos[output.EndIndex] = new HitInfo() {
                    BodyType = f.BodyType,
                    Proxy = f
                };
                output.EndIndex++;
            }
            return true;
        }
        public PhysicsOutput RayCast(RayCastInput input)
        {
            output.EndIndex = 0;
            PhysicsTree.RayCast(RayCastCallback, ref input);
            return output;
        }
       
        private long RayCastCallback(RayCastInput i, int node)
        {
            var proxy = PhysicsTree.GetUserData(node);
            var center = proxy.AABB.Center;
            var aabb = new AABB(-proxy.AABB.Extents, proxy.AABB.Extents);
            var a = i.Point1 - center;
            var b = i.Point2 - center;
            if (proxy.Fixture.angle != 0)
            {
                a.RotateInverse(proxy.Fixture.angle);
                b.RotateInverse(proxy.Fixture.angle);
            }
            i.Point1 = a;
            i.Point2 = b;
            RayCastOutput o;
            if (aabb.RayCast(out o, ref i))
            {
                var hitPosi = i.Point1 + (i.Point2 - i.Point1) * o.Fraction;
                if(proxy.Fixture.angle != 0)
                    hitPosi.Rotate(proxy.Fixture.angle);
                hitPosi = hitPosi + center;
                output.HitInfos[output.EndIndex].BodyType = proxy.BodyType;
                output.HitInfos[output.EndIndex].Proxy = proxy;
                output.EndIndex++;
                //NewSphere(hitPosi.ToVector3(), "hit");
                Debug.Log("hit something at " + hitPosi);
                return FixedMath.One;
            }
            else
            {
                return i.MaxFraction;
            }
        }
        public void Destroy()
        {

        }
    }
}
