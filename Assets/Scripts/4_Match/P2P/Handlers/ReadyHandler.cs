using UnityEngine;

/// <summary>
/// P2가 씬 준비를 마쳤다는 [READY] 메시지를 처리하는 핸들러입니다.
/// </summary>
public class ReadyHandler : IP2PMessageHandler
{
    private readonly GameManager gameManager;
    public ReadyHandler(GameManager gm) { gameManager = gm; }

    public bool CanHandle(string msg) => msg.StartsWith("[READY]");

    public void Handle(string msg)
    {
        gameManager.OnOpponentReady();
    }
}