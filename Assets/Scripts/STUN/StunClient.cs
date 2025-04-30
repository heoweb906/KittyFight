using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public static class StunClient
{
    // �����ε� ���� 1: ���� ��� (�ڵ� ���� ����)
    public static async Task<StunResult> QueryAsync(string stunServer, int port = 19302)
    {
        using var udp = new UdpClient(0);
        udp.Client.ReceiveTimeout = 5000;
        return await QueryAsync(udp, stunServer, port);
    }

    // �����ε� ���� 2: �ܺο��� ������ ������ ����
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
                Debug.LogError("[STUN] XOR-MAPPED-ADDRESS �Ľ� ����");
            else
                Debug.Log($"[STUN] ����: {publicEp.Address}:{publicEp.Port}");

            result.PublicEndPoint = publicEp;
        }
        catch (SocketException ex)
        {
            Debug.LogError($"[STUN] ���� ����: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[STUN] ��Ÿ ����: {ex.Message}");
        }

        return result;
    }
}