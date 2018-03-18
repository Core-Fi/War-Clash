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
        public FixedQuadTree<SceneObject> FixedQuadTreeForBuilding;
        public bool CanEnd()
        {
            return false;
        }
        public enum SceneEvent
        {
            AddSceneObject,
            RemoveSceneObject,
            OnCreate
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
            EventGroup = new EventGroup();
            MapConfig = Logic.Map.Map.Deserialize(Name);
            FixedQuadTree = new FixedQuadTree<SceneObject>();
            FixedQuadTreeForBuilding = new FixedQuadTree<SceneObject>();
            FixedQuadTree.SetBounds(new Utility.FixedRect(-FixedMath.One * 50, -FixedMath.One * 50, FixedMath.One * 100, FixedMath.One * 100));
            FixedQuadTreeForBuilding.SetBounds(new Utility.FixedRect(-FixedMath.One * 50, -FixedMath.One * 50, FixedMath.One * 100, FixedMath.One * 100));
        }

    
        public T CreateSceneObject<T>(int id) where T : SceneObject
        {
            var createInfo = Pool.SP.Get<CreateInfo>();
            createInfo.Id = id;
            var t = CreateSceneObject<T>(createInfo);
            return t;
        }

        public object CreateSceneObject(Type type, CreateInfo createInfo)
        {
            object o = Activator.CreateInstance(type);
            AddSceneObject(createInfo, o as SceneObject);
            return o;
        }
        public T CreateSceneObject<T>(CreateInfo createInfo) where T : SceneObject
        {
            T t = Activator.CreateInstance<T>();
            AddSceneObject(createInfo, t);
            return t;
        }
        public T CreateSceneObject<T>() where T : SceneObject
        {
            var t = CreateSceneObject<T>(IDManager.SP.GetID());
            return t;
        }

        internal void RemoveSceneObject(int id)
        {
            this.RemoveObject(id);
            EventGroup.FireEvent((int)SceneEvent.RemoveSceneObject, this, EventGroup.NewArg<EventSingleArgs<int>, int>(id));
            IDManager.SP.ReturnID(id);
            
        }
        private void AddSceneObject(CreateInfo createInfo, SceneObject so)
        {
            if (createInfo.Id == 0)
            {
                createInfo.Id = IDManager.SP.GetID();
            }
            so.Id = createInfo.Id;
            so.Init(createInfo);
            this.AddObject(so.Id, so);
            FixedQuadTree.Insert<SceneObject>(so);
            EventGroup.FireEvent(SceneEvent.AddSceneObject.ToInt(), this, EventGroup.NewArg<EventSingleArgs<SceneObject>, SceneObject>(so));
            so.ListenEvent((int)SceneObject.SceneObjectEvent.OnAddComponent, OnSceneObjectAddComponent);
            so.ListenEvent((int)SceneObject.SceneObjectEvent.OnRemoveComponent, OnSceneObjectRemoveComponent);
        }
        private void OnSceneObjectAddComponent(object sender, EventMsg msg)
        {
            var so = sender as SceneObject;
            var e = msg as EventSingleArgs<BaseComponent>;
            AddComponent(e.GetType(), so);
        }
        private void OnSceneObjectRemoveComponent(object sender, EventMsg msg)
        {
            var so = sender as SceneObject;
            var e = msg as EventSingleArgs<BaseComponent>;
            AddComponent(e.GetType(), so);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (Main.SP.ShowDebug)
            {
                FixedQuadTreeForBuilding.DebugDraw();
                FixedQuadTree.DebugDraw();
            }
        }

        public override void OnFixedUpdate(long deltaTime)
        {
            base.OnFixedUpdate(deltaTime);
        }

        public void Destroy()
        {

        }
    }
}
