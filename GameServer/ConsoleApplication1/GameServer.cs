using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

public class MsgSending
{
    
}
internal class GameServer
{
    public NetManager NetManager;
    private Queue<byte[]> readers = new Queue<byte[]>();
    private short frameCount;
    private bool started = false;
    private bool accessing;
    private StringBuilder builder = new StringBuilder();
    public GameServer()
    {
        var msgSending = new MsgSending();
        Thread t = new Thread(IntervalSendMsg);
        t.Start();
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
        builder.Clear();
        if (readers.Count > 0)
        {
            toSendBytes.AddRange(BitConverter.GetBytes((short) 0));
            toSendBytes.AddRange(frameCountBytes);
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
            toSendBytes.AddRange(BitConverter.GetBytes((short)0));
            toSendBytes.AddRange(frameCountBytes);
            builder.Append(0 + " at " + frameCount);
           // Console.WriteLine(builder.ToString());
            this.NetManager.SendToAll(toSendBytes.ToArray(), SendOptions.ReliableOrdered);
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

    internal void OnPeerConnect()
    {
        //Start();
        //return;
        if (this.NetManager.PeersCount == 2 && !started)
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
        byte[] cmd = BitConverter.GetBytes((short) 2);
        readers.Enqueue(cmd);
    }
}