using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

class GameNetManager : Manager
{
    private NetManager _netManger;
    
    public GameNetManager()  
    {
        this.ListenEvent(UIEventList.SendNetMsg.ToInt(), SendMsg);
        _netManger = new NetManager(new ClientListenner(), "myapp1");
        _netManger.Start();
        string local = "127.0.0.1";
        string outside = "47.94.204.158";
        _netManger.Connect(outside, 9050);
    }

    public void SendMsg(object sender, EventMsg e)
    {
        var msg = e as EventSingleArgs<NetDataWriter>;
        _netManger.SendToAll(msg.value, SendOptions.ReliableOrdered);
    }
    public override void OnUpdate()
    {
        _netManger.PollEvents();
        
        base.OnUpdate();
    }

    public override void OnDispose()
    {
        base.OnDispose();
        _netManger.Stop();
    }
}

class ClientListenner : INetEventListener
{
    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
        DLog.LogError("Network Error");
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        var msgId = reader.GetShort();
        EventDispatcher.FireEvent(msgId, this, EventGroup.NewArg< EventSingleArgs < NetDataReader > ,NetDataReader >(reader));
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
    {
       
    }

    public void OnPeerConnected(NetPeer peer)
    {
        DLog.Log("Successfully Connected");
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        DLog.LogError("Discoonnected");
    }
}