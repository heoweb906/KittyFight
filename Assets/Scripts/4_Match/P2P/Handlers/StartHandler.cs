using UnityEngine;

/// <summary>
/// P1�� ���� "���� ����" [START]{json} �޽��� �ڵ鷯
/// </summary>
public class StartHandler : IP2PMessageHandler
{
    //private readonly GameStartSync sync;

    public StartHandler(GameManager gm)
    {
        // ���� �ñ״�ó ����, ���ο��� GameStartSync Ȯ��
        //sync = gm != null
        //    ? (gm.GetComponent<GameStartSync>() ?? gm.gameObject.AddComponent<GameStartSync>())
        //    : null;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[START]");

    public void Handle(string msg)
    {
        //if (sync == null) return;

        //const int tagLen = 7; // "[START]"
        //var payload = new StartPayload { r = -1, d = -1 };

        //if (msg.Length > tagLen)
        //{
        //    try
        //    {
        //        var json = msg.Substring(tagLen);
        //        payload = JsonUtility.FromJson<StartPayload>(json);
        //    }
        //    catch { /* ����: �⺻�� ��� */ }
        //}

        //sync.OnStartMessage(payload);
    }
}