using UnityEngine;

public static class BackgroundColorMessageBuilder
{
    public static string Build(Color color, int imapgimicnum = 0)
    {
        var msg = new BackgroundColorMessage
        {
            r = color.r,
            g = color.g,
            b = color.b,
            iMapGimicNum = imapgimicnum
        };

        return "[BGCLR]" + JsonUtility.ToJson(msg);
    }
}