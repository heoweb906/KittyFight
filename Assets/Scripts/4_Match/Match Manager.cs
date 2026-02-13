using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using Steamworks;

public class MatchManager : MonoBehaviour
{
    public MainMenuController mainMenuController;

    public TMP_InputField nicknameInput;
    public TMP_Text logText;

    private string myPlayerId;

    // 로컬 바인드 정보(UDP 소켓 바인드용)
    private string myLocalIp;
    private int myLocalPort;

    // 공인 엔드포인트(STUN 결과)
    private string myPublicIp;
    private int myPublicPort;

    private string localIp;
    private int localPort;

    //  Steam + NAT 타입
    private string mySteamId;         // ulong -> string
    private string myNatType;         // "Symmetric" | "NonSymmetric" | "Unknown"
    private bool myRelayMarker;       // Symmetric이면 true

    public string MyNickname { get; set; }

    private string ticketId;
    private bool isMatching = false;    // 매칭 중임....
    private float matchStartTime = 0f;

    public bool BoolMatchSucces { get; set; }    // 매칭 성공

    private const float MATCH_TIMEOUT_SECONDS = 60f;

    private UdpClient matchingUdp;
    private int matchAttemptSeq = 0;
    private bool lastMatchSucceeded = false;

    private void Awake()
    {
        BoolMatchSucces = false;
    }

    private void OnApplicationQuit()
    {
        if (!string.IsNullOrEmpty(ticketId))
        {
            var quitTicket = ticketId;
            CleanupLocalMatchingState();
            _ = GameLiftCancelMatch.CancelMatchmaking(quitTicket);
        }
    }

    private void CleanupLocalMatchingState()
    {
        isMatching = false;
        BoolMatchSucces = false;
        lastMatchSucceeded = false;
        matchStartTime = 0f;

        StopAllCoroutines();

        ticketId = null;

        if (matchingUdp != null)
        {
            try { matchingUdp.Close(); } catch { }
            matchingUdp = null;
        }
    }

    public async void OnMatchButtonClicked()
    {
        if (isMatching) return;

        // 이전 상태가 남아있을 가능성까지 고려해서 시작 시 정리
        CleanupLocalMatchingState();
        MatchResultStore.Reset();

        // 이번 시도 토큰 발급
        matchAttemptSeq++;
        int myAttempt = matchAttemptSeq;

        isMatching = true;
        lastMatchSucceeded = false;

        try
        {
            // 1. 플레이어 ID 생성
            AppendLog("Generating ID...");
            myPlayerId = Guid.NewGuid().ToString();

            // 2. 로컬 포트 확보
            AppendLog("Acquiring local port...");
            myLocalPort = GetAvailablePort();
            myLocalIp = GetLocalIPAddress();

            localPort = myLocalPort;
            localIp = myLocalIp;

            // 2-1) SteamID 확보
            if (!SteamManager.Initialized)
            {
                AppendLog("SteamManager not initialized.");
                return;
            }
            mySteamId = SteamUser.GetSteamID().m_SteamID.ToString();

            // 3. STUN 서버 통해 공인 IP/포트 확인
            AppendLog("Checking IP...");
            matchingUdp = new UdpClient(myLocalPort);

            var stun1 = await StunClient.QueryAsync(matchingUdp, "stun.l.google.com", 19302);

            // await 이후: 이미 취소/타임아웃/다른 시도 시작됐으면 즉시 중단
            if (!isMatching || myAttempt != matchAttemptSeq) return;

            if (stun1?.PublicEndPoint == null)
            {
                AppendLog("STUN #1 failed.");
                return;
            }

            var ep1 = stun1.PublicEndPoint;
            myPublicIp = ep1.Address.ToString();
            myPublicPort = ep1.Port;

            AppendLog($"PublicEP1 = {myPublicIp}:{myPublicPort}");

            AppendLog("Checking IP (STUN #2)...");
            var stun2 = await StunClient.QueryAsync(matchingUdp, "stun1.l.google.com", 19302);

            if (!isMatching || myAttempt != matchAttemptSeq) return;

            bool natUnknown = (stun2?.PublicEndPoint == null);
            var ep2 = stun2?.PublicEndPoint;

            if (!natUnknown)
                AppendLog($"PublicEP2 = {ep2.Address}:{ep2.Port}");

            bool symmetricSuspect =
                !natUnknown &&
                (!ep1.Address.Equals(ep2.Address) || ep1.Port != ep2.Port);

            myNatType = natUnknown ? "Unknown" : (symmetricSuspect ? "Symmetric" : "NonSymmetric");
            myRelayMarker = (myNatType == "Symmetric");

            AppendLog($"NAT = {myNatType}");

            // 4. 내 정보 Lambda에 저장
            AppendLog("Saving my Info...");
            await LambdaStore.StorePlayerInfo(
                myPlayerId,
                myPublicIp,
                myPublicPort,
                localIp,
                localPort,
                MyNickname,
                mySteamId,
                myNatType,
                myRelayMarker
            );
            if (!isMatching || myAttempt != matchAttemptSeq) return;

            // 5. GameLift 매칭 시작
            ticketId = await GameLiftStartMatch.StartMatchmaking(myPlayerId);

            if (!isMatching || myAttempt != matchAttemptSeq) return;

            if (string.IsNullOrEmpty(ticketId))
            {
                AppendLog("Failed to start matchmaking.");
                return;
            }

            // 6. 매칭 완료까지 대기
            matchStartTime = Time.time;
            AppendLog("Match Searching...");
            StartCoroutine(UpdateMatchElapsedTime());

            bool matchCompleted = await GameLiftWait.WaitForMatchCompletion(ticketId);

            if (!isMatching || myAttempt != matchAttemptSeq) return;

            if (!matchCompleted)
            {
                AppendLog("Match wait failed or canceled.");
                return;
            }

            AppendLog("Match Completed");
            mainMenuController.PlaySFX(mainMenuController.sfxClips[4]);
            mainMenuController.PlayBGM(null);

            // 성공 플래그 설정 (finally에서 정리 방지)
            BoolMatchSucces = true;

            await Task.Delay(1000);
            if (!isMatching || myAttempt != matchAttemptSeq) return;

            mainMenuController.MatchSuccessfEffect();

            // 7. 상대방 정보 조회
            var opponent = await LambdaGet.GetOpponentInfo(myPlayerId);
            while ((opponent == null || string.IsNullOrEmpty(opponent.ip)) && isMatching && myAttempt == matchAttemptSeq)
            {
                await Task.Delay(500);
                if (!isMatching || myAttempt != matchAttemptSeq) return;
                opponent = await LambdaGet.GetOpponentInfo(myPlayerId);
            }

            if (!isMatching || myAttempt != matchAttemptSeq) return;

            // 8. P2P 연결 및 초기화
            string targetIp = (opponent.ip == myPublicIp) ? opponent.localIp : opponent.ip;
            int targetPort = (opponent.ip == myPublicIp) ? opponent.localPort : opponent.port;
            int myPlayerNumber = opponent.myPlayerNumber;

            MatchResultStore.myPlayerNumber = myPlayerNumber;
            MatchResultStore.myNickname = MyNickname;
            MatchResultStore.opponentNickname = opponent.nickname;
            MatchResultStore.opponentIp = targetIp;
            MatchResultStore.opponentPort = targetPort;
            MatchResultStore.myPort = myLocalPort;
            MatchResultStore.udpClient = matchingUdp;

            MatchResultStore.mySteamId = mySteamId;
            MatchResultStore.myNatType = myNatType;

            MatchResultStore.opponentSteamId = opponent.opponentSteamId;
            MatchResultStore.opponentNatType = opponent.opponentNatType;
            MatchResultStore.opponentRelayMarker = opponent.opponentRelayMarker;

            MatchResultStore.useSteam = true;
            //MatchResultStore.useSteam =
            //    myNatType == "Symmetric" ||
            //    opponent.opponentNatType == "Symmetric" ||
            //    opponent.opponentRelayMarker;

            lastMatchSucceeded = true;

            // 여기서 isMatching을 false로 내리고 씬 로드
            isMatching = false;
            SceneManager.LoadScene("example");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            AppendLog("Matching failed due to an error.");
        }
        finally
        {
            if (myAttempt == matchAttemptSeq && !lastMatchSucceeded)
            {
                CleanupLocalMatchingState();
            }
        }
    }

    private int GetAvailablePort()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    private void AppendLog(string msg)
    {
        Debug.Log(msg);
        logText.text = msg;
    }

    private IEnumerator UpdateMatchElapsedTime()
    {
        while (isMatching)
        {
            if (BoolMatchSucces) yield break;

            float elapsed = Time.time - matchStartTime;
            AppendLog($"Match Searching \n{Mathf.FloorToInt(elapsed)} seconds");

            if (elapsed >= MATCH_TIMEOUT_SECONDS)
            {
                AppendLog("Matching Timed Out.");

                var timeoutTicket = ticketId;
                CleanupLocalMatchingState();

                _ = GameLiftCancelMatch.CancelMatchmaking(timeoutTicket);

                mainMenuController.OnClickStopMatchingButton();
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public async void OnCancelMatchButtonClicked()
    {
        if (string.IsNullOrEmpty(ticketId))
            return;

        var cancelTicket = ticketId;

        CleanupLocalMatchingState();
        AppendLog("Match Canceled");

        await GameLiftCancelMatch.CancelMatchmaking(cancelTicket);
    }
}