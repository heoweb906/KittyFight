using System.Net.Sockets;
using UnityEngine;

public class StunTest : MonoBehaviour
{
    async void Start()
    {
        Debug.Log("[STUN TEST] ����");

        var udp = new UdpClient(0); // ���� ��Ʈ�� ��û
        udp.Client.ReceiveTimeout = 5000;

        var result1 = await StunClient.QueryAsync(udp, "stun.l.google.com", 19302);
        var result2 = await StunClient.QueryAsync(udp, "stun1.l.google.com", 19302);

        udp.Close();

        if (result1.PublicEndPoint == null || result2.PublicEndPoint == null)
        {
            Debug.LogError("[STUN TEST] ���� ����");
            return;
        }

        Debug.Log($"[STUN TEST] [1��] {result1.PublicEndPoint}");
        Debug.Log($"[STUN TEST] [2��] {result2.PublicEndPoint}");

        if (result1.PublicEndPoint.Port != result2.PublicEndPoint.Port)
        {
            Debug.LogWarning("[STUN TEST] Symmetric NAT ������");
        }
        else
        {
            Debug.Log("[STUN TEST] Non-Symmetric NAT");
        }
    }
}