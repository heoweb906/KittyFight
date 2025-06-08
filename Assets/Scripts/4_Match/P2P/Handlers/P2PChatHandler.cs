using UnityEngine.UI;
using UnityEngine;

public class P2PChatHandler : IP2PMessageHandler
{
    private Text logTextUI;
    private string opponentNickname;

    public P2PChatHandler(Text logText, string opponentNick)
    {
        logTextUI = logText;
        opponentNickname = opponentNick;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[CHAT]");

    public void Handle(string msg)
    {
        string chatMsg = msg.Substring(6);
        string logMsg = $"[{opponentNickname}] {chatMsg}";
        Debug.Log(logMsg);
        if (logTextUI != null)
            logTextUI.text += logMsg + "\n";
    }
}