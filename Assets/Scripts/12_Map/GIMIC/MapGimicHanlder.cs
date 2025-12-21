using UnityEngine;

public class MapGimicHandler : IP2PMessageHandler
{
    private readonly GameManager gameManager;
    private readonly MapManager mapManager;
    private readonly int myPlayerNumber;

    public MapGimicHandler(GameManager _gameManager, MapManager _mapManager, int _myNumber)
    {
        gameManager = _gameManager;
        mapManager = _mapManager;
        myPlayerNumber = _myNumber;
    }

    // 모든 기믹 메시지는 [GIMIC_ 으로 시작하므로 이를 감지
    public bool CanHandle(string msg) => msg.StartsWith("[GIMIC_");

    public void Handle(string msg)
    {
        // 1. 자(쥐) 기믹
        if (msg.StartsWith("[GIMIC_RAT]"))
        {
            var packet = JsonUtility.FromJson<Packet_1_Rat>(msg.Substring("[GIMIC_RAT]".Length));
            mapManager.ExecuteGimic_Rat(packet);
        }

        // 2. 축(소) 기믹
        //else if (msg.StartsWith("[GIMIC_COW]"))
        //{
        //    var packet = JsonUtility.FromJson<Packet_2_Cow>(msg.Substring("[GIMIC_COW]".Length));
        //    mapManager.ExecuteGimic_Cow(packet);
        //}
    }
}