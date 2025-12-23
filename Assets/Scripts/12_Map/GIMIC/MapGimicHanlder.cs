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

    public bool CanHandle(string msg) => msg.StartsWith("[GIMIC_");

    public void Handle(string msg)
    {
        // 1. Áã (Rat)
        if (msg.StartsWith("[GIMIC_RAT]"))
        {
            var packet = JsonUtility.FromJson<Packet_1_Rat>(msg.Substring("[GIMIC_RAT]".Length));
            mapManager.ExecuteGimic_Rat(packet);
        }
        // 2. ¼Ò (Cow)
        else if (msg.StartsWith("[GIMIC_COW]"))
        {
            var packet = JsonUtility.FromJson<Packet_2_Cow>(msg.Substring("[GIMIC_COW]".Length));
            mapManager.ExecuteGimic_Cow(packet);
        }
        // 3. È£¶ûÀÌ (Tiger)
        else if (msg.StartsWith("[GIMIC_TIGER]"))
        {
            var packet = JsonUtility.FromJson<Packet_3_Tiger>(msg.Substring("[GIMIC_TIGER]".Length));
            mapManager.ExecuteGimic_Tiger(packet);
        }
        // 4. Åä³¢ (Rabbit)
        else if (msg.StartsWith("[GIMIC_RABBIT]"))
        {
            var packet = JsonUtility.FromJson<Packet_4_Rabbit>(msg.Substring("[GIMIC_RABBIT]".Length));
            mapManager.ExecuteGimic_Rabbit(packet);
        }
        else if (msg.StartsWith("[GIMIC_DRAGON]"))
        {
            var packet = JsonUtility.FromJson<Packet_5_Dragon>(msg.Substring("[GIMIC_DRAGON]".Length));
            mapManager.ExecuteGimic_Dragon(packet);
        }
        else if (msg.StartsWith("[GIMIC_SNAKE]"))
        {
            var packet = JsonUtility.FromJson<Packet_6_Snake>(msg.Substring("[GIMIC_SNAKE]".Length));
            mapManager.ExecuteGimic_Snake(packet);
        }
        else if (msg.StartsWith("[GIMIC_HORSE]"))
        {
            var packet = JsonUtility.FromJson<Packet_7_Horse>(msg.Substring("[GIMIC_HORSE]".Length));
            mapManager.ExecuteGimic_Horse(packet);
        }
        else if (msg.StartsWith("[GIMIC_SHEEP]"))
        {
            var packet = JsonUtility.FromJson<Packet_8_Sheep>(msg.Substring("[GIMIC_SHEEP]".Length));
            mapManager.ExecuteGimic_Sheep(packet);
        }
        else if (msg.StartsWith("[GIMIC_MONKEY]"))
        {
            var packet = JsonUtility.FromJson<Packet_9_Monkey>(msg.Substring("[GIMIC_MONKEY]".Length));
            mapManager.ExecuteGimic_Monkey(packet);
        }
        else if (msg.StartsWith("[GIMIC_CHICKEN]"))
        {
            var packet = JsonUtility.FromJson<Packet_10_Chicken>(msg.Substring("[GIMIC_CHICKEN]".Length));
            mapManager.ExecuteGimic_Chicken(packet);
        }



        else if (msg.StartsWith("[GIMIC_DOG]"))
        {
            var packet = JsonUtility.FromJson<Packet_11_Dog>(msg.Substring("[GIMIC_DOG]".Length));
            mapManager.ExecuteGimic_Dog(packet);
        }
        else if (msg.StartsWith("[GIMIC_PIG]"))
        {
            var packet = JsonUtility.FromJson<Packet_12_Pig>(msg.Substring("[GIMIC_PIG]".Length));
            mapManager.ExecuteGimic_Pig(packet);
        }
        // È­¸é È¿°ú (Screen)
        else if (msg.StartsWith("[GIMIC_SCREEN]"))
        {
            var packet = JsonUtility.FromJson<Packet_ScreenEffect>(msg.Substring("[GIMIC_SCREEN]".Length));
            mapManager.ExecuteScreenEffect(packet);
        }
    }
}