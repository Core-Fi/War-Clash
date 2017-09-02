using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            Thread.Sleep(100);
        }
    }
    private void SendFrameData()
    {
        if (!started)return;
        while (accessing)
        { }
        accessing = true;
        while (readers.Count > 0)
            {
                var r = readers.Dequeue();
                var bytes = r;
                byte[] newBytes = new byte[bytes.Length + 2];
                var frameCountBytes = BitConverter.GetBytes(frameCount);
                Array.Copy(bytes, 0, newBytes, 0, 2);
                Array.Copy(frameCountBytes, 0, newBytes, 2, 2);
                int msgid = BitConverter.ToInt16(newBytes, 0);
                Array.Copy(bytes, 2, newBytes, 4, bytes.Length - 2);

                this.NetManager.SendToAll(newBytes, SendOptions.ReliableOrdered);
                builder.Append("Send Msg " + msgid + " " + newBytes.Length + " at frame " + frameCount + " ");
                builder.AppendLine();
                Console.WriteLine("Send Msg " + msgid + " " + newBytes.Length + " at frame " + frameCount + " ");
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
        Start();
        return;
        if (this.NetManager.PeersCount == 1 && !started)
        {
            var timer = new Timer(e => { Start(); }, null, 3000, System.Threading.Timeout.Infinite);
        }
    }

    internal void Start()
    {
        started = true;
        frameCount = 0;
        readers.Clear();
        byte[] cmd = BitConverter.GetBytes((short) 2);
        readers.Enqueue(cmd);
    }
}