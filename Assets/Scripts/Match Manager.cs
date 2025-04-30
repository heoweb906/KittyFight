using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    public InputField nicknameInput;
    public Text logText;

    public InputField chatInputField;
    public Button chatSendButton;

    public string myPlayerId;
    public string opponentId;

    private string myIp;
    private int myPort;
    private string myNickname;

    public async void OnMatchButtonClicked()
    {
        // 1. 플레이어 ID 생성
        myPlayerId = Guid.NewGuid().ToString();
        myNickname = nicknameInput.text;

        // 2. 로컬 포트 확보
        myPort = GetAvailablePort();
        AppendLog("내 로컬 Port: " + myPort);

        // 3. STUN 서버 통해 공인 IP/포트 확인
        var udp = new UdpClient(myPort);
        var stunResult = await StunClient.QueryAsync(udp, "stun.l.google.com", 19302);

        if (stunResult?.PublicEndPoint == null)
        {
            AppendLog("STUN 요청 실패 - 공인 IP/Port 조회 실패");
            return;
        }

        myIp = stunResult.PublicEndPoint.Address.ToString();
        myPort = stunResult.PublicEndPoint.Port;

        AppendLog("공인 IP: " + myIp);
        AppendLog("공인 Port: " + myPort);

        // 4. 내 정보 Lambda에 저장
        await LambdaStore.StorePlayerInfo(myPlayerId, myIp, myPort, myNickname);
        AppendLog("내 정보 저장 완료");

        // 5. GameLift 매칭 시작
        AppendLog("GameLift 매칭 시작...");
        string ticketId = await GameLiftStartMatch.StartMatchmaking(myPlayerId);

        if (ticketId == null)
        {
            AppendLog("매칭 티켓 생성 실패");
            return;
        }

        // 6. 매칭 완료까지 대기
        AppendLog("매칭 대기 중...");
        bool matchCompleted = await GameLiftWait.WaitForMatchCompletion(ticketId);

        if (!matchCompleted)
        {
            AppendLog("매칭 실패 또는 완료되지 않음");
            return;
        }

        AppendLog("매칭 완료! 상대방 정보 조회 중...");

        // 7. 상대방 정보 조회
        var opponent = await LambdaGet.GetOpponentInfo(myPlayerId);
        if (opponent == null)
        {
            AppendLog("상대 정보 조회 실패");
            return;
        }

        AppendLog($"상대 IP: {opponent.ip}, Port: {opponent.port}, Nickname: {opponent.nickname}");

        // 8. P2P 연결 및 채팅 초기화
        P2PChat.Init(myPort, myNickname, udp, logText);
        await Task.Delay(500);
        P2PChat.Connect(opponent.ip, opponent.port);

        AppendLog("P2P 연결 완료, 채팅 시작 가능");

        // 9. 채팅 UI 버튼 연결
        chatSendButton.onClick.RemoveAllListeners();
        chatSendButton.onClick.AddListener(() =>
        {
            string msg = chatInputField.text;
            if (string.IsNullOrWhiteSpace(msg)) return;

            chatInputField.text = "";
            P2PChat.SendChat(msg);
            logText.text += $"[{myNickname}] {msg}\n";
        });
    }

    private int GetAvailablePort()
    {
        System.Net.Sockets.TcpListener listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        int port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private void AppendLog(string msg)
    {
        Debug.Log(msg);
        if (logText != null)
            logText.text += msg + "\n";
    }
}