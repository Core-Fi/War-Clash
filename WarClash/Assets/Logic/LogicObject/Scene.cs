using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Objects;

namespace Logic.LogicObject
{
    public class Scene : ObjectCollection<int, SceneObject>
    {
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

        internal void Init()
        {
            EventGroup = new EventGroup();

        }
        public T CreateSceneObject<T>(int id) where T : SceneObject
        {
            T t = Activator.CreateInstance<T>();
            AddSceneObject(id, t);
            return t;
        }
        public T CreateSceneObject<T>() where T : SceneObject
        {
            T t = Activator.CreateInstance<T>();
            AddSceneObject(IDManager.SP.GetID(), t);
            return t;
        }

        internal void RemoveSceneObject(int id)
        {
            this.RemoveObject(id);
            EventGroup.FireEvent((int)SceneEvent.Removesceneobject, this, EventGroup.NewArg<EventSingleArgs<int>, int>(id));
            IDManager.SP.ReturnID(id);
        }
        private void AddSceneObject(int id, SceneObject so)
        {
            so.Id = id;
            so.Init();
            so.ListenEvents();
            this.AddObject(id, so);
            EventGroup.FireEvent((int)SceneEvent.Addsceneobject, this, EventGroup.NewArg<EventSingleArgs<SceneObject>, SceneObject>(so));
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
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
