using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Objects;

namespace Logic.LogicObject
{
    public class Scene : ObjectCollection<int, SceneObject>
    {
        public enum SceneEvent
        {
            ADDSCENEOBJECT,
            REMOVESCENEOBJECT,
            ONCREATE
        }
        public EventGroup EventGroup { get; private set; }
        public float deltaTime = 0;
        private VoidAction<SceneObject> updateAction = null;
        public Scene()
        {
        }

        public void Init()
        {
            updateAction = delegate(SceneObject so) { so.Update(deltaTime); }; 
        }
        public void ListenEvents()
        {
            EventGroup = new EventGroup();
            
        }
        public void CreateProjectile(Projectile projectile)
        {
           AddSceneObject(IDManager.SP.GetID(), projectile);

        }

        public void CreateCharacter(Character character)
        {
            AddSceneObject(IDManager.SP.GetID(), character);
        }

        internal void RemoveSceneObject(int id)
        {
            this.RemoveObject(id);
            EventGroup.FireEvent((int)SceneEvent.REMOVESCENEOBJECT, this, EventGroup.NewArg<EventSingleArgs<int>, int>(id));
            IDManager.SP.ReturnID(id);
        }
        internal void AddSceneObject(int id, SceneObject so)
        {
            so.ID = id;
            so.ListenEvents();
            so.Init();
            this.AddObject(id, so);
            EventGroup.FireEvent((int)SceneEvent.ADDSCENEOBJECT, this, EventGroup.NewArg<EventSingleArgs<SceneObject>, SceneObject>(so));
        }
        public void Update(float deltaTime)
        {
            this.deltaTime = deltaTime;
            ForEachDo<SceneObject>(updateAction);
        }
    }
}
