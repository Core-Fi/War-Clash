using Lockstep;
using Logic;
using Logic.LogicObject;
using UnityEngine;
using Object = UnityEngine.Object;

public class U3DPlayer : U3DCharacter
{
    private GameObject logicGo;
    public Player Player;
    protected U3DState U3DState;
    public override void OnInit()
    {
        base.OnInit();
        Player = Character as Player;
        AssetResources.LoadAsset("WK_heavy_infantry.prefab", OnLoadedRes);
    }

    public override void ListenEvents()
    {
        Player.EventGroup.ListenEvent(Character.CharacterEvent.ExecuteState.ToInt(), OnExecState);
        Player.EventGroup.ListenEvent(Character.CharacterEvent.StopState.ToInt(), OnStopState);
        base.ListenEvents();
    }

    private void OnExecState(object sender, EventMsg e)
    {
        var msg = e as EventSingleArgs<State>;
        var stateType = msg.value.GetType();
        U3DState = Pool.SP.Get(U3DState.CorrospondingTypes[stateType]) as U3DState;
        U3DState.State = msg.value;
        U3DState.U3DCharacter = this;
        U3DState.Start();
    }
    private void OnStopState(object sender, EventMsg e)
    {
        if (U3DState != null)
        {
            U3DState.Stop();
        }
    }

    public override void OnLoadedRes(string name, Object obj)
    {
        base.OnLoadedRes(name, obj);
    }

    protected override void OnAttributeChange(object sender, EventMsg e)
    {
        base.OnAttributeChange(sender, e);
        var msg = e as EventSingleArgs<AttributeMsg>;
        if (msg.value.At == AttributeType.IsVisible)
        {
            if (Player.GetStatus(AttributeType.IsVisible))
            {
                if (Player is MainPlayer)
                {
                    Go.SetActive(true);
                }
                else
                {
                    Go.SetActive(true);
                }
            }
            else
            {
                if (Player is MainPlayer)
                {
                    Go.SetActive(false);
                }
                else
                {
                    Go.SetActive(false);
                }
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (U3DState != null)
        {
            U3DState.Update();
        }
        if (Go != null)
        {
            var logicPosi = Character.Position.ToVector3();
            var logicForward = Character.Forward.ToVector3();
           // logicGo.transform.position = logicPosi;
            //float speed = this.Character.GetAttributeValue(AttributeType.MaxSpeed).ToFloat();
            //float distance = Vector3.Distance(logicPosi, Transform.position);
            OuterTransform.position = Vector3.Lerp(OuterTransform.position, logicPosi, Time.deltaTime*10);

            //if (distance < 0.1f)
            //{
            //   // Transform.position = logicPosi;
            //}
            //else
            //{
            //    var additive = (logicPosi - Transform.position).normalized*Time.deltaTime*speed;

            //    if (additive.magnitude > distance)
            //    {
            //        Transform.position = logicPosi;
            //        Debug.LogError("on dest point");
            //    }
            //    else
            //    {
            //        Transform.position += additive;
            //    }
            //}
            Transform.forward = Vector3.Lerp(Transform.forward, logicForward, Time.deltaTime * 6);
            //if (Vector3.Distance(logicPosi, Transform.position) > 0.1f)
            //{
            //    var tempForward = (logicPosi - Transform.position).normalized;
            //    if (Transform.forward != tempForward)
            //    {
            //           Transform.forward = tempForward;
            //    }
            //}
            
          
        }
    }
}
