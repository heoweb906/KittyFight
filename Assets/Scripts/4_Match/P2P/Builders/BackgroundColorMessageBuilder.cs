using UnityEngine;

public static class BackgroundColorMessageBuilder
{
    public static string Build(int mapIdx, int bgIdx, int gimic = 0)
    {
        var msg = new BackgroundColorMessage
        {
            mapIndex = mapIdx,
            backgroundIndex = bgIdx,
            iMapGimicNum = gimic
        };

        return "[BGCLR]" + JsonUtility.ToJson(msg);
    }
}