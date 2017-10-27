using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Objects;
using System.IO;
using Lockstep;
using UnityEngine;

namespace Logic.LogicObject
{
    public class Scene : ObjectCollection<int, SceneObject>
    {
        public string Name;
        public Map.Map MapConfig;
        public FixedQuadTree<Character> FixedQuadTree;
        public FixedQuadTree<Building> FixedQuadTreeForBuilding;
        public bool CanEnd()
        {
            return false;
        }
        public enum SceneEvent
        {
            Addsceneobject,
            Removesceneobject,
            Oncreate
        }
        public EventGroup EventGroup { get; private set; }
        public Scene()
        {
        }

        public Scene(string name)
        {
            Name = name;
        }
        internal void Init()
        {
            EventGroup = new EventGroup();
            MapConfig = Logic.Map.Map.Deserialize(Name);
            FixedQuadTree = new FixedQuadTree<Character>();
            FixedQuadTreeForBuilding = new FixedQuadTree<Building>();
            FixedQuadTree.SetBounds(new Utility.FixedRect(-FixedMath.One * 50, -FixedMath.One * 50, FixedMath.One * 100, FixedMath.One * 100));
            FixedQuadTreeForBuilding.SetBounds(new Utility.FixedRect(-FixedMath.One * 50, -FixedMath.One * 50, FixedMath.One * 100, FixedMath.One * 100));
        }

        public Building CreateBuilding(BuildingCreateInfo createInfo)
        {
            var bType = Building.BuildingIdType[createInfo.BuildingId];
            var b = CreateSceneObject(bType, createInfo);
            return b as Building;
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
            EventGroup.FireEvent((int)SceneEvent.Removesceneobject, this, EventGroup.NewArg<EventSingleArgs<int>, int>(id));
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
            so.ListenEvents();
            this.AddObject(so.Id, so);
            if (so is Character)
            {
                FixedQuadTree.Insert<Character>((Character) so);
            }
            else if (so is Building)
            {
                FixedQuadTreeForBuilding.Insert<Building>((Building) so);
            }
            EventGroup.FireEvent((int)SceneEvent.Addsceneobject, this, EventGroup.NewArg<EventSingleArgs<SceneObject>, SceneObject>(so));
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            FixedQuadTree.DebugDraw();
            FixedQuadTreeForBuilding.DebugDraw();
        }

        public override void OnFixedUpdate(long deltaTime)
        {
            base.OnFixedUpdate(deltaTime);
        }

        internal void Destroy()
        {

        }
    }
}
