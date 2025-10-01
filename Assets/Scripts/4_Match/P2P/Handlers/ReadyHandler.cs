using UnityEngine;

/// <summary>
/// P2�� �� �غ� ���ƴٴ� [READY] �޽����� ó���ϴ� �ڵ鷯�Դϴ�.
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