using System;
using System.Threading.Tasks;
using Amazon.GameLift.Model;
using UnityEngine;

public class GameLiftCancelMatch : GameLiftManager
{
    public static async Task<bool> CancelMatchmaking(string ticketId)
    {
        var client = GetClient();

        var request = new StopMatchmakingRequest
        {
            TicketId = ticketId
        };

        try
        {
            await client.StopMatchmakingAsync(request);
            Debug.Log("GameLift 매칭 취소 성공 - TicketID: " + ticketId);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("GameLift 매칭 취소 실패: " + ex.Message);
            return false;
        }
    }
}