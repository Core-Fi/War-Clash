using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;

class GameNetManager : Manager
{
    private NetManager _netManger;
    
    public GameNetManager()
    {
        this.ListenEvent(UIEventList.SendNetMsg.ToInt(), SendMsg);
        _netManger = new NetManager(new ClientListenner(), "myapp1");
        _netManger.Start();
        _netManger.Connect("127.0.0.1", 9050);
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
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
       
    }
}