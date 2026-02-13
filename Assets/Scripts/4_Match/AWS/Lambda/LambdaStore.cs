using System;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using UnityEngine;

public class LambdaStore : LambdaManager
{
    public static async Task StorePlayerInfo(
        string playerId,
        string ip,
        int port,
        string localIp,
        int localPort,
        string nickname,
        string steamId,
        string natType,
        bool relayMarker)
    {
        static string Esc(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }

        string jsonBody =
            "{" +
                $"\"playerId\":\"{Esc(playerId)}\"," +
                $"\"ip\":\"{Esc(ip)}\"," +
                $"\"port\":{port}," +
                $"\"localIp\":\"{Esc(localIp)}\"," +
                $"\"localPort\":{localPort}," +
                $"\"nickname\":\"{Esc(nickname)}\"," +

                $"\"steamId\":\"{Esc(steamId)}\"," +
                $"\"natType\":\"{Esc(natType)}\"," +
                $"\"relayMarker\":{(relayMarker ? "true" : "false")}" +
            "}";

        var request = new InvokeRequest
        {
            FunctionName = "StorePlayerInfo",
            InvocationType = InvocationType.RequestResponse,
            Payload = jsonBody
        };

        var client = GetClient();
        var response = await client.InvokeAsync(request);
        string result = Encoding.UTF8.GetString(response.Payload.ToArray());
        Debug.Log("Lambda Store ¿¿¥‰: " + result);
    }
}