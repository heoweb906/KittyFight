using UnityEngine;

/// <summary>
/// P1이 보낸 "게임 시작" [START] 메시지를 처리하는 핸들러입니다.
/// </summary>
public class StartHandler : IP2PMessageHandler
{
    private readonly GameManager gameManager;
    public StartHandler(GameManager gm) { gameManager = gm; }

    public bool CanHandle(string msg) => msg.StartsWith("[START]");

    public void Handle(string msg)
    {
        gameManager.StartGame();
    }
}