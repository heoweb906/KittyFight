using System.Net.Sockets;
using UnityEngine;

public class StunTest : MonoBehaviour
{
    async void Start()
    {
        Debug.Log("[STUN TEST] 시작");

        var udp = new UdpClient(0); // 같은 포트로 요청
        udp.Client.ReceiveTimeout = 5000;

        var result1 = await StunClient.QueryAsync(udp, "stun.l.google.com", 19302);
        var result2 = await StunClient.QueryAsync(udp, "stun1.l.google.com", 19302);

        udp.Close();

        if (result1.PublicEndPoint == null || result2.PublicEndPoint == null)
        {
            Debug.LogError("[STUN TEST] 응답 실패");
            return;
        }

        Debug.Log($"[STUN TEST] [1차] {result1.PublicEndPoint}");
        Debug.Log($"[STUN TEST] [2차] {result2.PublicEndPoint}");

        if (result1.PublicEndPoint.Port != result2.PublicEndPoint.Port)
        {
            Debug.LogWarning("[STUN TEST] Symmetric NAT 감지됨");
        }
        else
        {
            Debug.Log("[STUN TEST] Non-Symmetric NAT");
        }
    }
}