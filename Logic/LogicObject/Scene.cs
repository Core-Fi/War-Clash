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
            ADDSCENEOBJECT,
            REMOVESCENEOBJECT,
            ONCREATE
        }
        public EventGroup EventGroup { get; private set; }
        private float deltaTime = 0;
        private VoidAction<SceneObject> updateAction = null;
        public Scene()
        {
        }

        internal void Init()
        {
            updateAction = delegate(SceneObject so) { so.Update(deltaTime); };
            EventGroup = new EventGroup();


        }
        internal void CreateProjectile(Projectile projectile)
        {
           AddSceneObject(IDManager.SP.GetID(), projectile);
        }

        internal void CreateCharacter(Character character)
        {
            AddSceneObject(IDManager.SP.GetID(), character);
        }
        public void CreateNpc(Npc npc)
        {
            AddSceneObject(IDManager.SP.GetID(), npc);
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
            so.Init();
            so.ListenEvents();
            this.AddObject(id, so);
            EventGroup.FireEvent((int)SceneEvent.ADDSCENEOBJECT, this, EventGroup.NewArg<EventSingleArgs<SceneObject>, SceneObject>(so));
        }
        internal void Update(float deltaTime)
        {
            this.deltaTime = deltaTime;
            ForEachDo<SceneObject>(updateAction);
        }
        internal void Destroy()
        {

        }
    }
}
