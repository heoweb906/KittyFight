using UnityEngine;

public static class BackgroundColorMessageBuilder
{
    public static string Build(Color color)
    {
        var msg = new BackgroundColorMessage
        {
            r = color.r,
            g = color.g,
            b = color.b
        };

        return "[BGCLR]" + JsonUtility.ToJson(msg);
    }
}