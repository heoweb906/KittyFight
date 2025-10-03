using UnityEngine;
using System.Collections;

/// <summary>
/// READY/START/PLAYING�� JSON���� �ְ�޾�
/// 1) ª�� �ֱ� ���������� UDP ������ ����ϰ�
/// 2) ���� ����(Barrier Start)���� Instantiate Ÿ�̹� ������ ����ϴ� ����ȭ ���.
///
/// ���� GameManager�� �ǵ帮�� �ʰ� ���� GameObject�� �����Ǿ� �����մϴ�.
/// �ڵ鷯(Ready/Start/Playing)�� GameStartSync�� �ڵ����� ã�� ȣ���մϴ�.
/// </summary>
[DisallowMultipleComponent]
public class GameStartSync : MonoBehaviour
{
    [Header("Handshake/Start Settings")]
    [Tooltip("���� ���� ���� ����(��). �� �ð� �ڿ� StartGame() ȣ��")]
    public float startLeadDelay = 0.7f;

    [Tooltip("������ ����(��)")]
    public float resendInterval = 0.2f;

    [Tooltip("������ �� �ð�(��)")]
    public float handshakeTimeout = 3f;

    [SerializeField] private float stallKick = 0.75f;
    private float readySeenAt = -1f;
    private bool handshakeStarted = false;

    [SerializeField] private float stallRestart = 3.0f;  // 3�� ���� ���� ������ �ϵ� ����ŸƮ
    private float lastProgressAt = -1f;

    private GameManager gm;

    // ���� ����
    private int roundToken = 0;
    private bool gotOpponentPlayingAck = false; // P1: [PLAYING] ACK ���� ����
    private bool gotStartToken = false;         // P2: [START] ���� ����
    private bool opponentReady = false;         // P1: ��� READY ���� ����

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

        // ���� ��ȭ ����
        if (prevState != gm.currentState)
        {
            prevState = gm.currentState;

            if (gm.currentState == GameManager.GameState.Ready)
            {
                readySeenAt = Time.time;
                handshakeStarted = false;
                BeginRoundHandshake(true); // ���� ����: ��ū ����
                lastProgressAt = Time.time; // ���۰� ���ÿ� ���� ���� ����
            }
            else
            {
                StopAllHandshakeCoroutines();
                handshakeStarted = false;
            }
        }
        // Ready ���� �� watchdog
        else if (gm.currentState == GameManager.GameState.Ready)
        {
            bool loopsOff = (coReadySpam == null && coStartSpam == null && coStartDelay == null);

            // ������ �ƿ� �����ٸ�(������) ����Ʈ ű
            if (!handshakeStarted && loopsOff && readySeenAt > 0f && Time.time - readySeenAt > stallKick)
            {
                BeginRoundHandshake(false); // ��ū ���� ���� �簡��
                lastProgressAt = Time.time;
            }

            //  ������ ���� �ð� ���� ������ '�ϵ� ����ŸƮ' (��ū ����+���� �����)
            if (lastProgressAt > 0f && Time.time - lastProgressAt > stallRestart)
            {
                // ���� ���� �� ó������ �ٽ�
                StopAllHandshakeCoroutines();
                handshakeStarted = false;
                BeginRoundHandshake(true);  // ��ū bump
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

        // ���� ���� ���� ���� �� READY ���� ���� (��Īȭ)
        coReadySpam = StartCoroutine(SpamReadyUntilStart());
    }

    // READY ����: ������ ������ START ����/���� ����
    public void OnReadyMessage(ReadyPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;

        int r = payload.r >= 0 ? payload.r : roundToken;
        if (r < roundToken) return;     // ���� ����
        if (r > roundToken) roundToken = r;

        opponentReady = true;
        lastProgressAt = Time.time;     // ���� ����

        if (coStartDelay == null)
            coStartDelay = StartCoroutine(StartAfterDelay(startLeadDelay));

        if (coStartSpam == null)
            coStartSpam = StartCoroutine(SpamStartUntilAck());
    }

    // START ����: ������ ������ READY ���� �ߴ� + ���� ���� ���� + ACK �۽�
    public void OnStartMessage(StartPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;

        int r = payload.r >= 0 ? payload.r : roundToken;
        if (r < roundToken) return;
        if (r > roundToken) roundToken = r;

        gotStartToken = true;
        lastProgressAt = Time.time;     // ���� ����

        if (coReadySpam != null) { StopCoroutine(coReadySpam); coReadySpam = null; }

        int delayMs = payload.d >= 0 ? payload.d : Mathf.RoundToInt(startLeadDelay * 1000);
        float delay = Mathf.Max(0.05f, delayMs / 1000f);

        if (coStartDelay == null)
            coStartDelay = StartCoroutine(StartAfterDelay(delay));

        var ack = new PlayingPayload { r = roundToken };
        P2PMessageSender.SendMessage("[PLAYING]" + JsonUtility.ToJson(ack));
    }

    // ACK(PLAYING) ����: ������ ������ START ���� �ߴ�
    public void OnPlayingMessage(PlayingPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;

        int r = payload.r >= 0 ? payload.r : roundToken;
        if (r < roundToken) return;
        if (r > roundToken) roundToken = r;

        gotOpponentPlayingAck = true;
        lastProgressAt = Time.time;     // ���� ����

        if (coStartSpam != null) { StopCoroutine(coStartSpam); coStartSpam = null; }
    }


    // ����������������������������������������������������������������������������������������������������������������������������������������������������������

    private IEnumerator SpamReadyUntilStart()
    {
        var payload = new ReadyPayload { r = roundToken };
        string msg = "[READY]" + JsonUtility.ToJson(payload);

        while (IsStillReady() && !gotStartToken)
        {
            P2PMessageSender.SendMessage(msg);
            lastProgressAt = Time.time;       // ���� ��ü�� "����"���� ����
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
            lastProgressAt = Time.time;       // ���� ����
            yield return new WaitForSeconds(resendInterval);
        }
        coStartSpam = null;
    }


    private IEnumerator StartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // StartGame�� Ready ���¿����� ����ǵ��� GameManager�� idempotent�ϰ� ����Ǿ� �־�� ����
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