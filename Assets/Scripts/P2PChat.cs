using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class P2PChat : P2PManager
{
    private static string nickname = "��";
    private static Text logTextUI; // UI�� �α� ��¿�

    public static void Init(int myPort, string nick, UdpClient socket = null, Text logText = null)
    {
        P2PManager.Init(myPort, socket); // ���� ���� ����
        nickname = nick;
        logTextUI = logText;
        OnRawMessageReceived += HandleIncomingMessage;
    }

    public static void Connect(string ip, int port)
    {
        ConnectToOpponent(ip, port);
    }

    public static void SendChat(string msg)
    {
        SendRaw("[CHAT]" + msg);
    }

    private static void HandleIncomingMessage(string msg)
    {
        Log("[���� �޽���] " + msg);
        if (msg.StartsWith("[CHAT]"))
        {
            string chatMsg = msg.Substring(6);
            UnityEngine.Debug.Log("[���] " + chatMsg);
            Log($"[���] {chatMsg}");
        }
    }

    private static void Log(string message)
    {
        UnityEngine.Debug.Log(message);
        if (logTextUI != null)
        {
            logTextUI.text += message + "\n";
        }
    }
}