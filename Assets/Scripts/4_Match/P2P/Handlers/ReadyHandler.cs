using UnityEngine;

/// <summary>
/// P2가 씬 준비를 마쳤다는 [READY]{json} 메시지 핸들러
/// </summary>
public class ReadyHandler : IP2PMessageHandler
{
    //private readonly GameStartSync sync;

    public ReadyHandler(GameManager gm)
    {
        // 기존 시그니처 유지, 내부에서 GameStartSync 확보
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
        //    catch { /* 무해: 기본값 사용 */ }
        //}

        //sync.OnReadyMessage(payload);
    }
}