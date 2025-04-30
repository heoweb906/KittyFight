using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using UnityEngine;

public class GameLiftWait : GameLiftManager
{
    public static async Task<bool> WaitForMatchCompletion(string ticketId)
    {
        var client = GetClient();

        while (true)
        {
            await Task.Delay(2000);

            try
            {
                var response = await client.DescribeMatchmakingAsync(new DescribeMatchmakingRequest
                {
                    TicketIds = new List<string> { ticketId }
                });

                var ticket = response.TicketList.FirstOrDefault();
                if (ticket == null)
                {
                    Debug.LogError("티켓이 존재하지 않음");
                    return false;
                }

                if (ticket.Status == MatchmakingConfigurationStatus.COMPLETED)
                {
                    Debug.Log("매칭 완료");
                    return true;
                }
                else if (ticket.Status == MatchmakingConfigurationStatus.FAILED)
                {
                    Debug.LogError("매칭 실패");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("매칭 상태 확인 실패: " + ex.Message);
            }
        }
    }
}