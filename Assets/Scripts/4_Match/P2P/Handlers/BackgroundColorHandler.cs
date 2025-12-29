using UnityEngine;

public class BackgroundColorHandler : IP2PMessageHandler
{
    private readonly MapManager mapManager;
    private readonly GameManager gameManager;

    public BackgroundColorHandler(GameManager gm, MapManager mm)
    {
        gameManager = gm;
        mapManager = mm;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[BGCLR]");

    public void Handle(string msg)
    {
        if (AppLifecycle.IsDisconnecting) return;

        string json = msg.Substring(7);
        var data = JsonUtility.FromJson<BackgroundColorMessage>(json);

        gameManager.ApplyBackground(data.mapIndex, data.backgroundIndex, data.iMapGimicNum);
        mapManager.SetMapGimicIndex(data.iMapGimicNum);
    }
}