using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public static class StunClient
{
    // 오버로딩 버전 1: 기존 방식 (자동 소켓 생성)
    public static async Task<StunResult> QueryAsync(string stunServer, int port = 19302)
    {
        using var udp = new UdpClient(0);
        udp.Client.ReceiveTimeout = 5000;
        return await QueryAsync(udp, stunServer, port);
    }

    // 오버로딩 버전 2: 외부에서 생성한 소켓을 재사용
    public static async Task<StunResult> QueryAsync(UdpClient udp, string stunServer, int port = 19302)
    {
        Debug.Log($"[STUN] Query: {stunServer}:{port}");

        var serverEndpoint = new IPEndPoint(Dns.GetHostAddresses(stunServer)[0], port);
        var requestBytes = StunMessage.BuildRequest();

        await udp.SendAsync(requestBytes, requestBytes.Length, serverEndpoint);
        var result = new StunResult();

        try
        {
            var receive = await udp.ReceiveAsync();
            var publicEp = StunMessage.ParseResponse(receive.Buffer);

            if (publicEp == null)
                Debug.LogError("[STUN] XOR-MAPPED-ADDRESS 파싱 실패");
            else
                Debug.Log($"[STUN] 응답: {publicEp.Address}:{publicEp.Port}");

            result.PublicEndPoint = publicEp;
        }
        catch (SocketException ex)
        {
            Debug.LogError($"[STUN] 소켓 예외: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[STUN] 기타 예외: {ex.Message}");
        }

        return result;
    }
}