using UnityEngine;

/// <summary>
/// ��밡 ���� �÷��� ���¿� ������ �˸��� [PLAYING]{json} ACK �ڵ鷯
/// </summary>
public class PlayingHandler : IP2PMessageHandler
{
    private readonly GameStartSync sync;

    public PlayingHandler(GameManager gm)
    {
        // GameStartSync�� �ڵ����� Ȯ�� (������ ���� ������Ʈ�� ����)
        sync = gm != null
            ? (gm.GetComponent<GameStartSync>() ?? gm.gameObject.AddComponent<GameStartSync>())
            : null;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[PLAYING]");

    public void Handle(string msg)
    {
        if (sync == null) return;

        const int tagLen = 9; // "[PLAYING]"
        var payload = new PlayingPayload { r = -1 };

        if (msg.Length > tagLen)
        {
            try
            {
                var json = msg.Substring(tagLen);
                payload = JsonUtility.FromJson<PlayingPayload>(json);
            }
            catch { /* ����: �⺻�� ��� */ }
        }

        sync.OnPlayingMessage(payload);
    }
}