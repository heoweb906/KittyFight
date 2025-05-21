using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class P2PManager
{
    protected static UdpClient udpClient;
    protected static IPEndPoint remoteEndPoint;
    protected static int localPort;

    //public static Action<string> OnRawMessageReceived;

    public static void Init(int port, UdpClient socket = null)
    {
        localPort = port;

        if (socket != null)
        {
            udpClient = socket; // 기존 소켓 재사용
        }
        else
        {
            udpClient = new UdpClient(localPort); // 새로 만들기
        }

        udpClient.BeginReceive(OnReceive, null);
    }

    public static void ConnectToOpponent(string ip, int port)
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        SendRaw("[HOLEPUNCH]HELLO");
    }

    public static void SendRaw(string msg)
    {
        if (udpClient == null || remoteEndPoint == null) return;

        byte[] data = Encoding.UTF8.GetBytes(msg);
        udpClient.Send(data, data.Length, remoteEndPoint);
    }

    private static void OnReceive(IAsyncResult result)
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = udpClient.EndReceive(result, ref sender);
        string msg = Encoding.UTF8.GetString(data);

        //OnRawMessageReceived?.Invoke(msg);
        P2PMessageDispatcher.Dispatch(msg);
        udpClient.BeginReceive(OnReceive, null);
    }
}