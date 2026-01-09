using System;
using UnityEngine;

public class BackgroundColorHandler : IP2PMessageHandler
{
    private const string Prefix = "[BGCLR]";

    private readonly MapManager mapManager;
    private readonly GameManager gameManager;

    public BackgroundColorHandler(GameManager gm, MapManager mm)
    {
        gameManager = gm;
        mapManager = mm;
    }

    public bool CanHandle(string msg) => !string.IsNullOrEmpty(msg) && msg.StartsWith(Prefix);

    public void Handle(string msg)
    {
        if (AppLifecycle.IsDisconnecting) return;

        if (string.IsNullOrEmpty(msg)) return;

        if (!msg.StartsWith(Prefix)) return;
        if (msg.Length <= Prefix.Length) return;

        string json = msg.Substring(Prefix.Length);
        if (string.IsNullOrWhiteSpace(json)) return;

        BackgroundColorMessage data;
        try
        {
            data = JsonUtility.FromJson<BackgroundColorMessage>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[P2P][BGCLR] JSON parse failed. msgLen={msg.Length} err={e.Message}");
            return;
        }

        if (gameManager != null)
            gameManager.ApplyBackground(data.mapIndex, data.backgroundIndex, data.iMapGimicNum);
        else
            Debug.LogWarning("[P2P][BGCLR] gameManager is null. Skip ApplyBackground.");

        if (mapManager != null)
            mapManager.SetMapGimicIndex(data.iMapGimicNum);
        else
            Debug.LogWarning("[P2P][BGCLR] mapManager is null. Skip SetMapGimicIndex.");
    }
}