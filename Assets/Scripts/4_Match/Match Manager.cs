using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class MatchManager : MonoBehaviour
{
    public MainMenuController mainMenuController;

    public TMP_InputField nicknameInput;
    public TMP_Text logText;

    private string myPlayerId;
    private string myIp;
    private int myPort;
    public string MyNickname { get; set; }

    private string localIp;
    private int localPort;

    private string ticketId;
    private bool isMatching = false;    // 매칭 중임....
    private float matchStartTime = 0f;

    public bool BoolMatchSucces { get; set; }    // 매칭 성공


    private void Awake()
    {
        BoolMatchSucces = false;
    }


    public async void OnMatchButtonClicked()
    {
        if (isMatching) return;
        MatchResultStore.Reset();

        matchStartTime = 0;
        isMatching = true;

        // 1. 플레이어 ID 생성
        myPlayerId = Guid.NewGuid().ToString();

        // 2. 로컬 포트 확보
        myPort = GetAvailablePort();
        localPort = myPort;
        localIp = GetLocalIPAddress();

        // 3. STUN 서버 통해 공인 IP/포트 확인
        var udp = new UdpClient(myPort);
        var stunResult = await StunClient.QueryAsync(udp, "stun.l.google.com", 19302);

        if (stunResult?.PublicEndPoint == null)
        {
            //AppendLog("STUN 요청 실패 - 공인 IP/Port 조회 실패");
            isMatching = false; // 실패 시 플래그 정리
            return;
        }

        myIp = stunResult.PublicEndPoint.Address.ToString();
        myPort = stunResult.PublicEndPoint.Port;

        // 4. 내 정보 Lambda에 저장
        await LambdaStore.StorePlayerInfo(myPlayerId, myIp, myPort, localIp, localPort, MyNickname);

        // 5. GameLift 매칭 시작
        // AppendLog("Match Search Start!");
        ticketId = await GameLiftStartMatch.StartMatchmaking(myPlayerId);

        if (ticketId == null)
        {
            //AppendLog("매칭 티켓 생성 실패");
            isMatching = false; // 실패 시 플래그 정리
            return;
        }

        // 6. 매칭 완료까지 대기
        matchStartTime = Time.time;
        AppendLog("Match Searching...");
        StartCoroutine(UpdateMatchElapsedTime());

        bool matchCompleted = await GameLiftWait.WaitForMatchCompletion(ticketId);

        if (!matchCompleted)
        {
            //AppendLog("매칭 실패 또는 완료되지 않음");
            isMatching = false; // 실패 시 플래그 정리
            return;
        }

        AppendLog("Match Completed");

        // 매칭 성공 응답 후, 취소 요청이 있었는지 확인 (경쟁 조건 방지)
        if (!isMatching)
        {
            // 취소 버튼을 눌러 isMatching이 false가 되었다면 여기서 종료
            return;
        }

        // BoolMatchSucces 플래그를 여기서 설정하여 코루틴을 즉시 종료시킵니다.
        BoolMatchSucces = true;

        // 1초 지연 (UX 개선) 및 UI 효과 실행
        await Task.Delay(1000);

        // BoolMatchSucces = true; // 제거됨 (바로 위에서 설정)
        mainMenuController.MatchSuccessfEffect();


        // 7. 상대방 정보 조회
        var opponent = await LambdaGet.GetOpponentInfo(myPlayerId);
        while (opponent == null || string.IsNullOrEmpty(opponent.ip))
        {
            //AppendLog("상대 정보 조회 실패");
            // 취소 요청이 있었는지 재확인
            if (!isMatching) return;

            opponent = await LambdaGet.GetOpponentInfo(myPlayerId);
            await Task.Delay(500);
        }

        // 8. P2P 연결 및 채팅 초기화
        string targetIp = (opponent.ip == myIp) ? opponent.localIp : opponent.ip;
        int targetPort = (opponent.ip == myIp) ? opponent.localPort : opponent.port;
        int myPlayerNumber = opponent.myPlayerNumber;

        MatchResultStore.myPlayerNumber = myPlayerNumber;
        MatchResultStore.myNickname = MyNickname;
        MatchResultStore.opponentNickname = opponent.nickname;
        MatchResultStore.opponentIp = targetIp;
        MatchResultStore.opponentPort = targetPort;
        MatchResultStore.myPort = myPort;
        MatchResultStore.udpClient = udp;

        // AppendLog("Game Start!!"); // (선택 사항: 디버그 콘솔에만 기록됨)

        isMatching = false;
        SceneManager.LoadScene("example");
    }


    private int GetAvailablePort()
    {
        System.Net.Sockets.TcpListener listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        int port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
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
        //if (logText != null)
        //    logText.text += msg + "\n";
        logText.text = msg;
    }


    private IEnumerator UpdateMatchElapsedTime()
    {
        while (isMatching)
        {
            // BoolMatchSucces를 먼저 확인하여 로그 출력을 방지합니다.
            if (BoolMatchSucces) yield break;

            float elapsed = Time.time - matchStartTime;
            AppendLog($"Match Searching \n{Mathf.FloorToInt(elapsed)} seconds");

            yield return new WaitForSeconds(1f);
        }
    }




    public async void OnCancelMatchButtonClicked()
    {
        if (!isMatching || string.IsNullOrEmpty(ticketId))
        {
            // AppendLog("Currently not matching");
            return;
        }

        // AppendLog("try to cancel matching...");
        bool success = await GameLiftCancelMatch.CancelMatchmaking(ticketId);

        if (success)
        {
            AppendLog("Match Canceled");
            isMatching = false;
        }
        else
        {
            // AppendLog("Matching cancellation failed");
        }
    }
}