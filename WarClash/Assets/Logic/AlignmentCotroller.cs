using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic.LogicObject;
using UnityEngine;

namespace Logic
{
    public class AlignmentCotroller
    {
        private Player _commander;
        private List<IAlignmentAgent> _npcs = new List<IAlignmentAgent>(8);
        public AlignmentCotroller(Player c)
        {
            _commander = c;
            LogicCore.SP.SceneManager.CurrentScene.EventGroup.ListenEvent(Scene.SceneEvent.AddSceneObject.ToInt(), OnSceneObjCreate);
            LogicCore.SP.SceneManager.CurrentScene.EventGroup.ListenEvent(Scene.SceneEvent.RemoveSceneObject.ToInt(), OnSceneObjRemove);
        }

        private void OnSceneObjCreate(object sender, EventMsg e)
        {
            var msg = e as EventSingleArgs<SceneObject>;
            if (msg != null && msg.value != null && msg.value.Team == _commander.Team && msg.value is IAlignmentAgent)
            {
                _npcs.Add(msg.value as IAlignmentAgent);
                CaculateLastAlignment();
            }
        }
        private void OnSceneObjRemove(object sender, EventMsg e)
        {
            var msg = e as EventSingleArgs<SceneObject>;
            if (msg != null && msg.value != null && msg.value.Team == _commander.Team  && msg.value is IAlignmentAgent)
            {
                _npcs.Remove(msg.value as IAlignmentAgent);
            }
        }


        public void  CaculateLastAlignment()
        {
            if(_npcs.Count==0)return;
            int x, y;
            GridService.GetCoordinate(_commander.Position, out x, out y);
            int radius = 1;
            int index = _npcs.Count-1;
            while (true)
            {
                for (int i = radius; i >= -radius; i--)
                {
                    for (int j = radius; j >= -radius; j--)
                    {
                        if (i != radius && i != -radius)
                        {
                            if (j == radius || j == -radius)
                            {
                                if (GridService.IsEmpty(j + x, i + y))
                                {
                                    for (int k = 0; k < _npcs.Count; k++)
                                    {
                                        if (_npcs[k].AlignmentX != j || _npcs[k].AlignmentY != i)
                                        {
                                            _npcs[index].AlignmentX = j;
                                            _npcs[index].AlignmentY = i;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (GridService.IsEmpty(j + x, i + y))
                            {
                                for (int k = 0; k < _npcs.Count; k++)
                                {
                                    if (_npcs[k].AlignmentX != j || _npcs[k].AlignmentY != i)
                                    {
                                        _npcs[index].AlignmentX = j;
                                        _npcs[index].AlignmentY = i;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                radius++;
            }
        }
        public void CaculateAlignment()
        {
            var rowCount = FixedMath.Sqrt(_npcs.Count * FixedMath.One).CeilToInt();
            int x, y;
            GridService.GetCoordinate(_commander.Position, out x, out y);
            int radius = 1;
            int index = 0;
            while (true)
            {
                for (int i = radius; i >= -radius; i--)
                {
                    for (int j = radius; j >= -radius; j--)
                    {
                        if (i != radius && i != -radius)
                        {
                            if (j == radius || j == -radius)
                            {
                                if (GridService.IsEmpty(j + x, i + y))
                                {
                                    _npcs[index].AlignmentX = j;
                                    _npcs[index].AlignmentY = i;
                                    index++;
                                    if(index == _npcs.Count)
                                        return;
                                }
                            }
                        }
                        else
                        {
                            if (GridService.IsEmpty(j + x, i + y))
                            {
                                _npcs[index].AlignmentX = j;
                                _npcs[index].AlignmentY = i;
                                index++;
                                if (index == _npcs.Count)
                                    return;
                            }
                        }
                    }
                }
                radius++;
            }
        }
    }
}