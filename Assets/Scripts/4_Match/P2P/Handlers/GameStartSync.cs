using UnityEngine;
using System.Collections;

/// <summary>
/// READY/START/PLAYING을 JSON으로 주고받아
/// 1) 짧은 주기 재전송으로 UDP 유실을 흡수하고
/// 2) 공통 지연(Barrier Start)으로 Instantiate 타이밍 편차를 흡수하는 동기화 모듈.
///
/// 기존 GameManager는 건드리지 않고 같은 GameObject에 부착되어 동작합니다.
/// 핸들러(Ready/Start/Playing)는 GameStartSync를 자동으로 찾아 호출합니다.
/// </summary>
[DisallowMultipleComponent]
public class GameStartSync : MonoBehaviour
{
    [Header("Handshake/Start Settings")]
    [Tooltip("양쪽 공통 시작 지연(초). 이 시간 뒤에 StartGame() 호출")]
    public float startLeadDelay = 0.7f;

    [Tooltip("재전송 간격(초)")]
    public float resendInterval = 0.2f;

    [Tooltip("재전송 총 시간(초)")]
    public float handshakeTimeout = 3f;

    private GameManager gm;

    // 라운드 상태
    private int roundToken = 0;
    private bool gotOpponentPlayingAck = false; // P1: [PLAYING] ACK 수신 여부
    private bool gotStartToken = false;         // P2: [START] 수신 여부
    private bool opponentReady = false;         // P1: 상대 READY 수신 여부

    private Coroutine coReadySpam, coStartSpam, coStartDelay;

    private GameManager.GameState? prevState;

    private void Awake()
    {
        gm = GetComponent<GameManager>();
        prevState = gm != null ? gm.currentState : (GameManager.GameState?)null;
    }

    private void OnDisable() => StopAllHandshakeCoroutines();

    private void Update()
    {
        if (gm == null) return;

        // GameManager의 상태가 Ready로 진입하는 시점을 폴링 감지 (GameManager 코드 무수정)
        if (prevState != gm.currentState)
        {
            var old = prevState;
            prevState = gm.currentState;

            if (gm.currentState == GameManager.GameState.Ready)
                BeginRoundHandshake();
            else if (gm.currentState == GameManager.GameState.Playing)
                StopAllHandshakeCoroutines(); // 플레이 중엔 전송 불필요
        }
    }

    /// <summary>Ready 진입 시 라운드 토큰 증가 및 상태/코루틴 초기화</summary>
    private void BeginRoundHandshake()
    {
        roundToken++;
        gotOpponentPlayingAck = false;
        gotStartToken = false;
        opponentReady = false;

        StopAllHandshakeCoroutines();

        // P2: READY 재전송 시작 (START 수신 시까지)
        if (GetMyNum() == 2)
            coReadySpam = StartCoroutine(SpamReadyUntilStart());
        // P1: READY 수신되면 START 재전송 + 동일 지연 시작 예약
        // (여기서는 기다리기만, 실제 트리거는 OnReadyMessage에서)
    }

    /// <summary>READY 수신 (P1에서만 의미)</summary>
    public void OnReadyMessage(ReadyPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;
        if (GetMyNum() != 1) return;

        int r = payload.r >= 0 ? payload.r : roundToken;
        if (r < roundToken) return;     // 과거 라운드 무시
        if (r > roundToken) roundToken = r;

        opponentReady = true;

        // 동일 지연으로 로컬 시작 예약 (중복 방지)
        if (coStartDelay == null)
            coStartDelay = StartCoroutine(StartAfterDelay(startLeadDelay));

        // START 재전송 시작 (ACK 오면 자동 중단)
        if (coStartSpam == null)
            coStartSpam = StartCoroutine(SpamStartUntilAck());
    }

    /// <summary>START 수신 (P2에서만 의미)</summary>
    public void OnStartMessage(StartPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;
        if (GetMyNum() != 2) return;

        int r = payload.r >= 0 ? payload.r : roundToken;

        if (r < roundToken) return;     // 과거 라운드
        if (r > roundToken) roundToken = r;

        gotStartToken = true;

        // READY 재전송 종료
        if (coReadySpam != null) { StopCoroutine(coReadySpam); coReadySpam = null; }

        int delayMs = payload.d >= 0 ? payload.d : Mathf.RoundToInt(startLeadDelay * 1000);
        float delay = Mathf.Max(0.05f, delayMs / 1000f);

        // 동일한 지연으로 시작 예약 (중복 방지)
        if (coStartDelay == null)
            coStartDelay = StartCoroutine(StartAfterDelay(delay));

        // P1에게 "나 시작할게" ACK
        var ack = new PlayingPayload { r = roundToken };
        P2PMessageSender.SendMessage("[PLAYING]" + JsonUtility.ToJson(ack));
    }

    /// <summary>PLAYING(ACK) 수신 (P1: START 재전송 중단)</summary>
    public void OnPlayingMessage(PlayingPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;
        if (GetMyNum() != 1) return;

        int r = payload.r >= 0 ? payload.r : roundToken;

        if (r < roundToken) return;
        if (r > roundToken) roundToken = r;

        gotOpponentPlayingAck = true;

        if (coStartSpam != null) { StopCoroutine(coStartSpam); coStartSpam = null; }
    }

    // ─────────────────────────────────────────────────────────────────────────────

    private IEnumerator SpamReadyUntilStart()
    {
        // 지금은 일단 무한히 될 때까지 대기
        //float deadline = Time.time + handshakeTimeout;
        var payload = new ReadyPayload { r = roundToken };
        string msg = "[READY]" + JsonUtility.ToJson(payload);
        
        while (IsStillReady() && !gotStartToken)
        {
            P2PMessageSender.SendMessage(msg);
            yield return new WaitForSeconds(resendInterval);
        }
        //while (Time.time < deadline && !gotStartToken && IsStillReady())
        //{
        //    P2PMessageSender.SendMessage(msg);
        //    yield return new WaitForSeconds(resendInterval);
        //}
        coReadySpam = null;
    }

    private IEnumerator SpamStartUntilAck()
    {
        //float deadline = Time.time + handshakeTimeout;

        //while (Time.time < deadline && !gotOpponentPlayingAck && IsStillReady())
        //{
        //    int delayMs = Mathf.RoundToInt(startLeadDelay * 1000);
        //    var payload = new StartPayload { r = roundToken, d = delayMs };
        //    P2PMessageSender.SendMessage("[START]" + JsonUtility.ToJson(payload));
        //    yield return new WaitForSeconds(resendInterval);
        //}
        while (IsStillReady() && !gotOpponentPlayingAck)
        {
            int delayMs = Mathf.RoundToInt(startLeadDelay * 1000);
            var payload = new StartPayload { r = roundToken, d = delayMs };
            P2PMessageSender.SendMessage("[START]" + JsonUtility.ToJson(payload));
            yield return new WaitForSeconds(resendInterval);
        }
        coStartSpam = null;
    }

    private IEnumerator StartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // StartGame은 Ready 상태에서만 진행되도록 GameManager가 idempotent하게 설계되어 있어야 안전
        if (gm != null && gm.currentState == GameManager.GameState.Ready)
            gm.StartGame();

        coStartDelay = null;
    }

    private void StopAllHandshakeCoroutines()
    {
        if (coReadySpam != null) { StopCoroutine(coReadySpam); coReadySpam = null; }
        if (coStartSpam != null) { StopCoroutine(coStartSpam); coStartSpam = null; }
        if (coStartDelay != null) { StopCoroutine(coStartDelay); coStartDelay = null; }
    }

    private bool IsStillReady() => gm != null && gm.currentState == GameManager.GameState.Ready;

    private int GetMyNum()
    {
        return MatchResultStore.myPlayerNumber;
    }
}