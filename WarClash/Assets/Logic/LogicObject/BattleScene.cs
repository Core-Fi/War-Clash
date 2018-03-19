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
            OnCreate,
            OnLoaded
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
            EventGroup.ListenEvent(SceneEvent.OnLoaded.ToInt(), OnLoded);
            FixedQuadTree = new FixedQuadTree<SceneObject>();
            FixedQuadTreeForBuilding = new FixedQuadTree<SceneObject>();
            FixedQuadTree.SetBounds(new Utility.FixedRect(-FixedMath.One * 50, -FixedMath.One * 50, FixedMath.One * 100, FixedMath.One * 100));
            FixedQuadTreeForBuilding.SetBounds(new Utility.FixedRect(-FixedMath.One * 50, -FixedMath.One * 50, FixedMath.One * 100, FixedMath.One * 100));
        }
        private void OnLoded(object sender, EventMsg msg)
        {
            var so = CreateSceneObject();
            var modelComp = new ModelComponent();
            modelComp.RePath = "kachujin.prefab";
            so.AddComponent(modelComp);
            so.AddComponent<MainPlayerComponent>();
            var jumpComp = new JumpComponent();
            jumpComp.InitSpeed = FixedMath.One * 30;
            so.AddComponent(jumpComp);
        }
        public SceneObject CreateSceneObject()
        {
            var id = IDManager.SP.GetID();
            SceneObject so = new SceneObject();
            so.Id = id;
            so.Init(new CreateInfo());
            so.AddComponent<TransformComponent>();
            this.AddObject(so.Id, so);
            EventGroup.FireEvent(SceneEvent.AddSceneObject.ToInt(), this, EventGroup.NewArg<EventSingleArgs<SceneObject>, SceneObject>(so));
            so.ListenEvent((int)SceneObject.SceneObjectEvent.OnAddComponent, OnSceneObjectAddComponent);
            so.ListenEvent((int)SceneObject.SceneObjectEvent.OnRemoveComponent, OnSceneObjectRemoveComponent);
            return so;
        }
       

        internal void RemoveSceneObject(int id)
        {
            this.RemoveObject(id);
            EventGroup.FireEvent((int)SceneEvent.RemoveSceneObject, this, EventGroup.NewArg<EventSingleArgs<int>, int>(id));
            IDManager.SP.ReturnID(id);
            
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
