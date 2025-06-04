using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class P2PManager
{
    protected static UdpClient udpClient;
    protected static IPEndPoint remoteEndPoint;
    protected static int localPort;

    public static void Init(int port, UdpClient socket = null)
    {
        // 닫힌 소켓 재사용 방지
        if (udpClient != null)
        {
            if (udpClient.Client == null || !udpClient.Client.IsBound)
            {
                Debug.LogWarning("[P2PManager] 이전 소켓이 유효하지 않음. 재초기화합니다.");
                udpClient = null;
            }
            else
            {
                Debug.Log("[P2PManager] Already initialized and active. Skipping Init.");
                return;
            }
        }

        if (socket != null)
        {
            udpClient = socket; // 기존 소켓 재사용
            localPort = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
            Debug.Log("Socket Reuse");
        }
        else
        {
            udpClient = new UdpClient(port); // 새로 만들기
            localPort = port;
            Debug.Log("New socket");
        }

        udpClient.BeginReceive(OnReceive, null);
    }

    public static void ConnectToOpponent(string ip, int port)
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        SendRaw("[HOLEPUNCH]HELLO");
        Debug.Log("Holepunching");
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

        P2PMessageDispatcher.Dispatch(msg);
        udpClient.BeginReceive(OnReceive, null);
        Debug.Log("Receive");
    }

    public static void Dispose()
    {
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
            //Debug.Log("[P2PManager] UDP 소켓 닫힘");
        }
    }
}