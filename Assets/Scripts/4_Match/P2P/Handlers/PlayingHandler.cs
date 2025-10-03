using UnityEngine;

/// <summary>
/// 상대가 실제 플레이 상태에 들어갔음을 알리는 [PLAYING]{json} ACK 핸들러
/// </summary>
public class PlayingHandler : IP2PMessageHandler
{
    private readonly GameStartSync sync;

    public PlayingHandler(GameManager gm)
    {
        // GameStartSync를 자동으로 확보 (없으면 같은 오브젝트에 부착)
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
            catch { /* 무해: 기본값 사용 */ }
        }

        sync.OnPlayingMessage(payload);
    }
}