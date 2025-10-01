using UnityEngine;

/// <summary>
/// P1�� ���� "���� ����" [START] �޽����� ó���ϴ� �ڵ鷯�Դϴ�.
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