using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.GameLift.Model;
using UnityEngine;

public class GameLiftStartMatch : GameLiftManager
{
    public static async Task<string> StartMatchmaking(string playerId)
    {
        var request = new StartMatchmakingRequest
        {
            ConfigurationName = "RandomMatch1v1",
            Players = new List<Player>
            {
                new Player
                {
                    PlayerId = playerId,
                    Team = "team"
                }
            }
        };

        var client = GetClient();

        try
        {
            var response = await client.StartMatchmakingAsync(request);
            Debug.Log("GameLift ��Ī ��û �Ϸ� - TicketID: " + response.MatchmakingTicket.TicketId);
            return response.MatchmakingTicket.TicketId;
        }
        catch (Exception ex)
        {
            Debug.LogError("��Ī ��û ����: " + ex.Message);
            return null;
        }
    }
}