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

    [SerializeField] private float stallKick = 0.75f;
    private float readySeenAt = -1f;
    private bool handshakeStarted = false;

    [SerializeField] private float stallRestart = 3.0f;  // 3초 동안 진전 없으면 하드 리스타트
    private float lastProgressAt = -1f;

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
        prevState = null;
    }

    private void OnDisable() => StopAllHandshakeCoroutines();
    private void Update()
    {
        if (gm == null) return;

        // 상태 변화 감지
        if (prevState != gm.currentState)
        {
            prevState = gm.currentState;

            if (gm.currentState == GameManager.GameState.Ready)
            {
                readySeenAt = Time.time;
                handshakeStarted = false;
                BeginRoundHandshake(true); // 정상 진입: 토큰 증가
                lastProgressAt = Time.time; // 시작과 동시에 진행 시점 갱신
            }
            else
            {
                StopAllHandshakeCoroutines();
                handshakeStarted = false;
            }
        }
        // Ready 유지 중 watchdog
        else if (gm.currentState == GameManager.GameState.Ready)
        {
            bool loopsOff = (coReadySpam == null && coStartSpam == null && coStartDelay == null);

            // 루프가 아예 꺼졌다면(비정상) 소프트 킥
            if (!handshakeStarted && loopsOff && readySeenAt > 0f && Time.time - readySeenAt > stallKick)
            {
                BeginRoundHandshake(false); // 토큰 증가 없이 재가동
                lastProgressAt = Time.time;
            }

            //  진행이 일정 시간 멈춰 있으면 '하드 리스타트' (토큰 증가+완전 재시작)
            if (lastProgressAt > 0f && Time.time - lastProgressAt > stallRestart)
            {
                // 전부 리셋 후 처음부터 다시
                StopAllHandshakeCoroutines();
                handshakeStarted = false;
                BeginRoundHandshake(true);  // 토큰 bump
                lastProgressAt = Time.time;
            }
        }
    }


    private void BeginRoundHandshake(bool bumpToken)
    {
        if (handshakeStarted) return;
        handshakeStarted = true;

        if (bumpToken) roundToken++;

        gotOpponentPlayingAck = false;
        gotStartToken = false;
        opponentReady = false;

        StopAllHandshakeCoroutines();

        // 역할 구분 없이 양쪽 다 READY 스팸 시작 (대칭화)
        coReadySpam = StartCoroutine(SpamReadyUntilStart());
    }

    // READY 수신: 누구든 받으면 START 스팸/지연 예약
    public void OnReadyMessage(ReadyPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;

        int r = payload.r >= 0 ? payload.r : roundToken;
        if (r < roundToken) return;     // 과거 라운드
        if (r > roundToken) roundToken = r;

        opponentReady = true;
        lastProgressAt = Time.time;     // 진행 갱신

        if (coStartDelay == null)
            coStartDelay = StartCoroutine(StartAfterDelay(startLeadDelay));

        if (coStartSpam == null)
            coStartSpam = StartCoroutine(SpamStartUntilAck());
    }

    // START 수신: 누구든 받으면 READY 스팸 중단 + 동일 지연 예약 + ACK 송신
    public void OnStartMessage(StartPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;

        int r = payload.r >= 0 ? payload.r : roundToken;
        if (r < roundToken) return;
        if (r > roundToken) roundToken = r;

        gotStartToken = true;
        lastProgressAt = Time.time;     // 진행 갱신

        if (coReadySpam != null) { StopCoroutine(coReadySpam); coReadySpam = null; }

        int delayMs = payload.d >= 0 ? payload.d : Mathf.RoundToInt(startLeadDelay * 1000);
        float delay = Mathf.Max(0.05f, delayMs / 1000f);

        if (coStartDelay == null)
            coStartDelay = StartCoroutine(StartAfterDelay(delay));

        var ack = new PlayingPayload { r = roundToken };
        P2PMessageSender.SendMessage("[PLAYING]" + JsonUtility.ToJson(ack));
    }

    // ACK(PLAYING) 수신: 누구든 받으면 START 스팸 중단
    public void OnPlayingMessage(PlayingPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;

        int r = payload.r >= 0 ? payload.r : roundToken;
        if (r < roundToken) return;
        if (r > roundToken) roundToken = r;

        gotOpponentPlayingAck = true;
        lastProgressAt = Time.time;     // 진행 갱신

        if (coStartSpam != null) { StopCoroutine(coStartSpam); coStartSpam = null; }
    }


    // ─────────────────────────────────────────────────────────────────────────────

    private IEnumerator SpamReadyUntilStart()
    {
        var payload = new ReadyPayload { r = roundToken };
        string msg = "[READY]" + JsonUtility.ToJson(payload);

        while (IsStillReady() && !gotStartToken)
        {
            P2PMessageSender.SendMessage(msg);
            lastProgressAt = Time.time;       // 전송 자체도 "진행"으로 간주
            yield return new WaitForSeconds(resendInterval);
        }
        coReadySpam = null;
    }

    private IEnumerator SpamStartUntilAck()
    {
        while (IsStillReady() && !gotOpponentPlayingAck)
        {
            int delayMs = Mathf.RoundToInt(startLeadDelay * 1000);
            var payload = new StartPayload { r = roundToken, d = delayMs };
            P2PMessageSender.SendMessage("[START]" + JsonUtility.ToJson(payload));
            lastProgressAt = Time.time;       // 진행 갱신
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