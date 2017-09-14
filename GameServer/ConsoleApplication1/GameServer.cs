using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

public class MsgSending
{
    
}

internal class Client
{
    public int id;
    public NetPeer peer;
    public Queue<byte[]> sendToPeerMsgs;
    public Client(NetPeer peer)
    {
        this.peer = peer;
        sendToPeerMsgs = new Queue<byte[]>();
    }
}
internal class GameServer
{
    public NetManager NetManager;
    private Queue<byte[]> readers = new Queue<byte[]>();
    private List<Queue<byte[]>> sendToPeer = new List<Queue<byte[]>>(); 
    private short frameCount;
    private bool started = false;
    private bool accessing;
    private StringBuilder builder = new StringBuilder();
    private List<Client> clients = new List<Client>(2);
    private int ID
    {
        get { return id++; }
    }

    private int id { get; set; }

    public GameServer()
    {
        var msgSending = new MsgSending();
        Thread t = new Thread(IntervalSendMsg);
        t.Start();
        clients.Clear();
    }

    private void IntervalSendMsg()
    {
        while (true)
        {
            SendFrameData();
            Thread.Sleep(1000/15);
        }
    }

    private void SendFrameData()
    {
        if (!started)return;
        while (accessing)
        { }
        accessing = true;
        var frameCountBytes = BitConverter.GetBytes(frameCount);
        List<byte> toSendBytes = new List<byte>();
        toSendBytes.AddRange(BitConverter.GetBytes((short)0));
        toSendBytes.AddRange(frameCountBytes);
        builder.Clear();
        if (readers.Count > 0)
        {
            while (readers.Count > 0)
            {
                var r = readers.Dequeue();
                toSendBytes.AddRange(r);
                var msgid = BitConverter.ToInt16(r, 0);
                builder.Append(msgid+" at "+ frameCount);
            }
            Console.WriteLine(builder.ToString());
            this.NetManager.SendToAll(toSendBytes.ToArray(), SendOptions.ReliableOrdered);
        }
        else
        {
            this.NetManager.SendToAll(toSendBytes.ToArray(), SendOptions.ReliableOrdered);
        }

        foreach (var player in clients)
        {
            if (player.sendToPeerMsgs.Count > 0)
            {
                toSendBytes.Clear();
                toSendBytes.AddRange(BitConverter.GetBytes((short) 0));
                toSendBytes.AddRange(frameCountBytes);
                while (player.sendToPeerMsgs.Count > 0)
                {
                    toSendBytes.AddRange(player.sendToPeerMsgs.Dequeue());
                }
                player.peer.Send(toSendBytes.ToArray(), SendOptions.ReliableOrdered);
            }
        }
        accessing = false;
        frameCount++;
    }

    public void OnReceiveMsg(NetDataReader reader)
    {
        var msgID = BitConverter.ToInt16(reader.Data, 0);
        if (msgID == 1)
        {
            File.WriteAllText("D:\\ServerLog.txt", builder.ToString());
        }
        if (reader.Data != null)
        {
            while (accessing)
            {}
            accessing = true;
                readers.Enqueue(reader.Data);
            accessing = false;
        }
    }

    internal void OnPeerConnect(NetPeer peer)
    {
        clients.Clear();
        var player = new Client(peer);
        player.id = 100 + ID;
        clients.Add(player);

        if (this.NetManager.PeersCount == 1 )//&& !started)
        {
            var timer = new Timer(e => { Start(); }, null, 3000, System.Threading.Timeout.Infinite);
        }
    }

    internal void Start()
    {
        Console.WriteLine("Game Start");

        started = true;
        frameCount = 0;
        readers.Clear();
        byte[] cmd = BitConverter.GetBytes((short) LockFrameEvent.BattleStart);
        readers.Enqueue(cmd);
        for (int i = 0; i < clients.Count; i++)
        {
            var client = clients[i];
            List<byte> willSendBytes = new List<byte>();
            short msgId = (short)LockFrameEvent.CreateMainPlayer;
            willSendBytes.AddRange(BitConverter.GetBytes(msgId));
            willSendBytes.AddRange(BitConverter.GetBytes(client.id));
            client.sendToPeerMsgs.Enqueue(willSendBytes.ToArray());
            Console.WriteLine("Create MainPlayer "+ client.id);
            for (int j = 0; j < clients.Count; j++)
            {
                if (i != j)
                {
                    var remoteClient = clients[j];
                    willSendBytes.Clear();
                    msgId = (short)LockFrameEvent.CreatePlayer;
                    willSendBytes.AddRange(BitConverter.GetBytes(msgId));
                    willSendBytes.AddRange(BitConverter.GetBytes(client.id));
                    remoteClient.sendToPeerMsgs.Enqueue(willSendBytes.ToArray());
                    Console.WriteLine(remoteClient.id+ " Create Remote Player " + client.id);
                }
            }
        }
        
    }
    
    public enum LockFrameEvent
    {
        LockStepFrame,
        SaveToLog,
        BattleStart,
        PlayerMoveMsg,
        PlayerStopMsg,
        PlayerRotateMsg,
        CreateMainPlayer,
        CreatePlayer,
        CreateNpc
    }
}