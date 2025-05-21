using System;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using UnityEngine;

[Serializable]
public class OpponentInfo
{
    public string ip;
    public int port;
    public string localIp;
    public int localPort;
    public string nickname;
    public int myPlayerNumber;
}

[Serializable]
public class LambdaRawWrapper
{
    public string body;
}

public class LambdaGet : LambdaManager
{
    public static async Task<OpponentInfo> GetOpponentInfo(string myPlayerId)
    {
        string jsonBody = $"{{\"playerId\":\"{myPlayerId}\"}}";

        var request = new InvokeRequest
        {
            FunctionName = "GetPlayerInfo",
            InvocationType = InvocationType.RequestResponse,
            Payload = jsonBody
        };

        var client = GetClient();
        var response = await client.InvokeAsync(request);
        string result = Encoding.UTF8.GetString(response.Payload.ToArray());

        Debug.Log("Lambda 응답: " + result);

        // 1차 파싱: 전체 래퍼 구조
        var wrapper = JsonUtility.FromJson<LambdaRawWrapper>(result);

        // 2차 파싱: 실제 body 내용을 OpponentInfo로
        var opponent = JsonUtility.FromJson<OpponentInfo>(wrapper.body);
        return opponent;
    }
}
