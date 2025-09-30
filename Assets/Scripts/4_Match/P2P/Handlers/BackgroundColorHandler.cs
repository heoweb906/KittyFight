using UnityEngine;

public class BackgroundColorHandler : IP2PMessageHandler
{
    private readonly MapManager mapManager;
    private readonly GameManager gameManager;

    public BackgroundColorHandler(GameManager gm)
    {
        gameManager = gm;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[BGCLR]");

    public void Handle(string msg)
    {
        string json = msg.Substring(7);
        var data = JsonUtility.FromJson<BackgroundColorMessage>(json);

        gameManager.OnReceiveSetupMessage(data);
    }
}