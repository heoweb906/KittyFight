using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

public class P2PManager
{
    protected static UdpClient udpClient;
    protected static IPEndPoint remoteEndPoint;
    protected static int localPort;

    private static bool ackReceived = false;
    private static bool ackSent = false;
    private static bool gameStartTriggered = false;

    private static MonoBehaviour contextForCoroutine;
    public static bool IsReadyToStartGame;

    public static void Init(int port, UdpClient socket = null, MonoBehaviour coroutineContext = null)
    {
        // ���� ���� ���� ����
        if (udpClient != null)
        {
            if (udpClient.Client == null || !udpClient.Client.IsBound)
            {
                Debug.LogWarning("[P2PManager] ���� ������ ��ȿ���� ����. ���ʱ�ȭ�մϴ�.");
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
            udpClient = socket; // ���� ���� ����
            localPort = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;
            Debug.Log("Socket Reuse");
        }
        else
        {
            udpClient = new UdpClient(port); // ���� �����
            localPort = port;
            Debug.Log("New socket");
        }

        udpClient.BeginReceive(OnReceive, null);

        if (coroutineContext != null)
            contextForCoroutine = coroutineContext;
    }

    public static void ConnectToOpponent(string ip, int port)
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        if (contextForCoroutine != null)
            contextForCoroutine.StartCoroutine(HolepunchRoutine());
    }

    private static IEnumerator HolepunchRoutine()
    {
        while (!ackReceived)
        {
            SendRaw("[HOLEPUNCH]");
            Debug.Log("[P2P] Sending HOLEPUNCH...");
            yield return new WaitForSeconds(0.5f);
        }
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

        if (msg.StartsWith("[HOLEPUNCH]"))
        {
            SendRaw("[ACK]");
            ackSent = true;
            Debug.Log("[P2P] Received HOLEPUNCH, sending ACK.");
        }
        else if (msg.StartsWith("[ACK]"))
        {
            ackReceived = true;
            if(ackSent == false) SendRaw("[ACK]");
            ackSent = true;
            Debug.Log("[P2P] Received ACK.");

            if (ackSent && ackReceived && !gameStartTriggered)
            {
                gameStartTriggered = true;
                Debug.Log("[P2P] Both ACKs exchanged. Ready to start game.");

                // ���⼱ �÷��׸� ����� (���� �����忡�� ó��)
                IsReadyToStartGame = true;
            }
        }
        else
        {
            P2PMessageDispatcher.Dispatch(msg);
        }
        
        udpClient.BeginReceive(OnReceive, null);
    }

    public static void Dispose()
    {
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
            ackSent = ackReceived = gameStartTriggered = false;
        }
    }
}