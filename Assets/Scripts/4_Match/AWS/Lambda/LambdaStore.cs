using System;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using UnityEngine;

public class LambdaStore : LambdaManager
{
    public static async Task StorePlayerInfo(string playerId, string ip, int port, string localIp, int localPort, string nickname)
    {
        string jsonBody = $"{{" +
            $"\"playerId\":\"{playerId}\"," +
            $"\"ip\":\"{ip}\"," +
            $"\"port\":{port}," +
            $"\"localIp\":\"{localIp}\"," +
            $"\"localPort\":{localPort}," +
            $"\"nickname\":\"{nickname}\"" +
        $"}}";

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