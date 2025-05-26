using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MatchManager : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    public TMP_Text logText;

    private string myPlayerId;
    private string myIp;
    private int myPort;
    private string myNickname;

    private string localIp;
    private int localPort;

    private string ticketId;
    private bool isMatching = false;

    public async void OnMatchButtonClicked()
    {
        // 1. 플레이어 ID 생성
        myPlayerId = Guid.NewGuid().ToString();
        myNickname = nicknameInput.text;

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
            return;
        }

        myIp = stunResult.PublicEndPoint.Address.ToString();
        myPort = stunResult.PublicEndPoint.Port;

        // 4. 내 정보 Lambda에 저장
        await LambdaStore.StorePlayerInfo(myPlayerId, myIp, myPort, localIp, localPort, myNickname);

        // 5. GameLift 매칭 시작
        AppendLog("Match Search Start!");
        ticketId = await GameLiftStartMatch.StartMatchmaking(myPlayerId);

        if (ticketId == null)
        {
            //AppendLog("매칭 티켓 생성 실패");
            return;
        }

        // 6. 매칭 완료까지 대기
        isMatching = true;
        AppendLog("Match Searching...");
        bool matchCompleted = await GameLiftWait.WaitForMatchCompletion(ticketId);

        if (!matchCompleted)
        {
            //AppendLog("매칭 실패 또는 완료되지 않음");
            return;
        }

        AppendLog("Match Completed");

        // 7. 상대방 정보 조회
        var opponent = await LambdaGet.GetOpponentInfo(myPlayerId);
        if (opponent == null)
        {
            //AppendLog("상대 정보 조회 실패");
            return;
        }

        // 8. P2P 연결 및 채팅 초기화
        string targetIp = (opponent.ip == myIp) ? opponent.localIp : opponent.ip;
        int targetPort = (opponent.ip == myIp) ? opponent.localPort : opponent.port;
        int myPlayerNumber = opponent.myPlayerNumber;

        MatchResultStore.myPlayerNumber = myPlayerNumber;
        MatchResultStore.myNickname = myNickname;
        MatchResultStore.opponentNickname = opponent.nickname;
        MatchResultStore.opponentIp = targetIp;
        MatchResultStore.opponentPort = targetPort;
        MatchResultStore.myPort = myPort;
        MatchResultStore.udpClient = udp;

        AppendLog("Game Start!!");
        // SceneManager.LoadScene("GameScene");
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
        if (logText != null)
            logText.text += msg + "\n";
    }

    public async void OnCancelMatchButtonClicked()
    {
        if (!isMatching || string.IsNullOrEmpty(ticketId))
        {
            AppendLog("현재 매칭 중이 아닙니다.");
            return;
        }

        AppendLog("매칭 취소 시도 중...");
        bool success = await GameLiftCancelMatch.CancelMatchmaking(ticketId);

        if (success)
        {
            AppendLog("매칭이 취소되었습니다.");
            isMatching = false;
        }
        else
        {
            AppendLog("매칭 취소 실패");
        }
    }
}