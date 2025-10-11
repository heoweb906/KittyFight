using UnityEngine;

/// <summary>
/// P1이 보낸 "게임 시작" [START]{json} 메시지 핸들러
/// </summary>
public class StartHandler : IP2PMessageHandler
{
    //private readonly GameStartSync sync;

    public StartHandler(GameManager gm)
    {
        // 기존 시그니처 유지, 내부에서 GameStartSync 확보
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
        //    catch { /* 무해: 기본값 사용 */ }
        //}

        //sync.OnStartMessage(payload);
    }
}