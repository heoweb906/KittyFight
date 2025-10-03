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
        prevState = gm != null ? gm.currentState : (GameManager.GameState?)null;
    }

    private void OnDisable() => StopAllHandshakeCoroutines();

    private void Update()
    {
        if (gm == null) return;

        // GameManager�� ���°� Ready�� �����ϴ� ������ ���� ���� (GameManager �ڵ� ������)
        if (prevState != gm.currentState)
        {
            var old = prevState;
            prevState = gm.currentState;

            if (gm.currentState == GameManager.GameState.Ready)
                BeginRoundHandshake();
            else if (gm.currentState == GameManager.GameState.Playing)
                StopAllHandshakeCoroutines(); // �÷��� �߿� ���� ���ʿ�
        }
    }

    /// <summary>Ready ���� �� ���� ��ū ���� �� ����/�ڷ�ƾ �ʱ�ȭ</summary>
    private void BeginRoundHandshake()
    {
        roundToken++;
        gotOpponentPlayingAck = false;
        gotStartToken = false;
        opponentReady = false;

        StopAllHandshakeCoroutines();

        // P2: READY ������ ���� (START ���� �ñ���)
        if (GetMyNum() == 2)
            coReadySpam = StartCoroutine(SpamReadyUntilStart());
        // P1: READY ���ŵǸ� START ������ + ���� ���� ���� ����
        // (���⼭�� ��ٸ��⸸, ���� Ʈ���Ŵ� OnReadyMessage����)
    }

    /// <summary>READY ���� (P1������ �ǹ�)</summary>
    public void OnReadyMessage(ReadyPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;
        if (GetMyNum() != 1) return;

        int r = payload.r >= 0 ? payload.r : roundToken;
        if (r < roundToken) return;     // ���� ���� ����
        if (r > roundToken) roundToken = r;

        opponentReady = true;

        // ���� �������� ���� ���� ���� (�ߺ� ����)
        if (coStartDelay == null)
            coStartDelay = StartCoroutine(StartAfterDelay(startLeadDelay));

        // START ������ ���� (ACK ���� �ڵ� �ߴ�)
        if (coStartSpam == null)
            coStartSpam = StartCoroutine(SpamStartUntilAck());
    }

    /// <summary>START ���� (P2������ �ǹ�)</summary>
    public void OnStartMessage(StartPayload payload)
    {
        if (gm == null || gm.currentState != GameManager.GameState.Ready) return;
        if (GetMyNum() != 2) return;

        int r = payload.r >= 0 ? payload.r : roundToken;

        if (r < roundToken) return;     // ���� ����
        if (r > roundToken) roundToken = r;

        gotStartToken = true;

        // READY ������ ����
        if (coReadySpam != null) { StopCoroutine(coReadySpam); coReadySpam = null; }

        int delayMs = payload.d >= 0 ? payload.d : Mathf.RoundToInt(startLeadDelay * 1000);
        float delay = Mathf.Max(0.05f, delayMs / 1000f);

        // ������ �������� ���� ���� (�ߺ� ����)
        if (coStartDelay == null)
            coStartDelay = StartCoroutine(StartAfterDelay(delay));

        // P1���� "�� �����Ұ�" ACK
        var ack = new PlayingPayload { r = roundToken };
        P2PMessageSender.SendMessage("[PLAYING]" + JsonUtility.ToJson(ack));
    }

    /// <summary>PLAYING(ACK) ���� (P1: START ������ �ߴ�)</summary>
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

    // ����������������������������������������������������������������������������������������������������������������������������������������������������������

    private IEnumerator SpamReadyUntilStart()
    {
        // ������ �ϴ� ������ �� ������ ���
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