using UnityEngine;

/// <summary>
/// P2�� �� �غ� ���ƴٴ� [READY]{json} �޽��� �ڵ鷯
/// </summary>
public class ReadyHandler : IP2PMessageHandler
{
    //private readonly GameStartSync sync;

    public ReadyHandler(GameManager gm)
    {
        // ���� �ñ״�ó ����, ���ο��� GameStartSync Ȯ��
        //sync = gm != null
        //    ? (gm.GetComponent<GameStartSync>() ?? gm.gameObject.AddComponent<GameStartSync>())
        //    : null;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[READY]");

    public void Handle(string msg)
    {
        //if (sync == null) return;

        //const int tagLen = 7; // "[READY]"
        //var payload = new ReadyPayload { r = -1 };

        //if (msg.Length > tagLen)
        //{
        //    try
        //    {
        //        var json = msg.Substring(tagLen);
        //        payload = JsonUtility.FromJson<ReadyPayload>(json);
        //    }
        //    catch { /* ����: �⺻�� ��� */ }
        //}

        //sync.OnReadyMessage(payload);
    }
}