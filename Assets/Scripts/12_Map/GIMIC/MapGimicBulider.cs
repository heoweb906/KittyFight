using UnityEngine;

public static class MapGimicBuilder
{
    public static string BuildRat_SyncHP(int gimicId, int targetHP)
    {
        Packet_1_Rat msg = new Packet_1_Rat
        {
            gimicId = gimicId,
            targetHpSync = targetHP
        };
        return "[GIMIC_RAT]" + JsonUtility.ToJson(msg);
    }

    public static string BuildCow(int id)
    {
        Packet_2_Cow msg = new Packet_2_Cow
        {
            gimicId = id
        };
        return "[GIMIC_COW]" + JsonUtility.ToJson(msg);         
    }


}
