using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brainiac;
using Logic.LogicObject;
using Action = Brainiac.Action;
using Lockstep;
using Logic;
using UnityEngine;

[AddNodeMenu("Action/FollowPlayer")]
class FollowTargetAction : Action
{
    private PathFollowSteering _pathFollowSteering;
    private List<Vector3d> _path = new List<Vector3d>();
    private byte _pathIndex;
    public override void OnStart(AIAgent agent)
    {
        base.OnStart(agent);
    }
    
    protected override void OnEnter(AIAgent agent)
    {
        base.OnEnter(agent);
        Move();
        MainPlayer.SP.EventGroup.ListenEvent(SceneObject.SceneObjectEvent.Positionchange.ToInt(),
            OnPosiChange);
    }

    private void OnPosiChange(object sender, EventMsg e)
    {
        Move();
    }

    private void Move()
    {
        if (Vector3d.SqrDistance(MainPlayer.SP.Position, SceneObject.Position) > 5.ToLong())
        {
            SceneObject.AttributeManager.SetBase(AttributeType.Speed, SceneObject.GetAttributeValue(Logic.AttributeType.MaxSpeed));
            //Vector3d offset = new Vector3d(_npc.AlignmentX.ToLong(), 0, _npc.AlignmentY.ToLong());
            JPSAStar.active.GetPath(SceneObject.Position, MainPlayer.SP.Position , _path);
            if (_path.Count > 1)
            {
                _pathIndex = 1;
                Vector3d moveDirection = (_path[_pathIndex] - SceneObject.Position).Normalize();
                SceneObject.Forward = moveDirection;
            }

        }
    }
    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
    {
        var speed = agent.SceneObject.GetAttributeValue(Logic.AttributeType.Speed);
        long moveDistance = speed.Mul(FixedMath.One / 15);
        while (moveDistance > 0 && _pathIndex < _path.Count)
        {
            long nextCornerDistance = Vector3d.SqrDistance(_path[_pathIndex],
                agent.SceneObject.Position);
            if (nextCornerDistance < moveDistance.Mul(moveDistance))
            {
                agent.SceneObject.Position = _path[_pathIndex];
                moveDistance -= FixedMath.Sqrt(nextCornerDistance);
                _pathIndex++;
                if (_pathIndex == _path.Count)
                {
                    break;
                }
                else
                {
                    Vector3d moveDirection = (_path[_pathIndex] - agent.SceneObject.Position).Normalize();
                    SceneObject.Forward = moveDirection;
                }
            }
            else
            {
                agent.SceneObject.Position += agent.SceneObject.Forward * moveDistance;
                break;
            }
        }
        return BehaviourNodeStatus.Running;
    }
}

