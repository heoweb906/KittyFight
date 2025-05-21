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
                    Debug.LogError("Ƽ���� �������� ����");
                    return false;
                }

                if (ticket.Status == MatchmakingConfigurationStatus.COMPLETED)
                {
                    Debug.Log("��Ī �Ϸ�");
                    return true;
                }
                else if (ticket.Status == MatchmakingConfigurationStatus.FAILED)
                {
                    Debug.LogError("��Ī ����");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("��Ī ���� Ȯ�� ����: " + ex.Message);
            }
        }
    }
}