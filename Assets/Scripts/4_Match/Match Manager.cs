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

    public async void OnMatchButtonClicked()
    {
        // 1. �÷��̾� ID ����
        myPlayerId = Guid.NewGuid().ToString();
        myNickname = nicknameInput.text;

        // 2. ���� ��Ʈ Ȯ��
        myPort = GetAvailablePort();
        localPort = myPort;
        localIp = GetLocalIPAddress();

        // 3. STUN ���� ���� ���� IP/��Ʈ Ȯ��
        var udp = new UdpClient(myPort);
        var stunResult = await StunClient.QueryAsync(udp, "stun.l.google.com", 19302);

        if (stunResult?.PublicEndPoint == null)
        {
            //AppendLog("STUN ��û ���� - ���� IP/Port ��ȸ ����");
            return;
        }

        myIp = stunResult.PublicEndPoint.Address.ToString();
        myPort = stunResult.PublicEndPoint.Port;


        // 4. �� ���� Lambda�� ����
        await LambdaStore.StorePlayerInfo(myPlayerId, myIp, myPort, localIp, localPort, myNickname);

        // 5. GameLift ��Ī ����
        AppendLog("Match Search Start!");
        string ticketId = await GameLiftStartMatch.StartMatchmaking(myPlayerId);

        if (ticketId == null)
        {
            //AppendLog("��Ī Ƽ�� ���� ����");
            return;
        }

        // 6. ��Ī �Ϸ���� ���
        AppendLog("Match Searching...");
        bool matchCompleted = await GameLiftWait.WaitForMatchCompletion(ticketId);

        if (!matchCompleted)
        {
            //AppendLog("��Ī ���� �Ǵ� �Ϸ���� ����");
            return;
        }

        AppendLog("Match Completed");

        // 7. ���� ���� ��ȸ
        var opponent = await LambdaGet.GetOpponentInfo(myPlayerId);
        if (opponent == null)
        {
            //AppendLog("��� ���� ��ȸ ����");
            return;
        }

        // 8. P2P ���� �� ä�� �ʱ�ȭ
        string targetIp = (opponent.ip == myIp) ? opponent.localIp : opponent.ip;
        int targetPort = (opponent.ip == myIp) ? opponent.localPort : opponent.port;
        int myPlayerNumber = opponent.myPlayerNumber;

        //GameObject myPlayer = (myPlayerNumber == 1) ? player1Object : player2Object;
        //GameObject opponentPlayer = (myPlayerNumber == 1) ? player2Object : player1Object;

        //P2PManager.Init(myPort, udp);
        //P2PManager.ConnectToOpponent(targetIp, targetPort);

        //P2PMessageDispatcher.RegisterHandler(new P2PChatHandler(logText, opponent.nickname));
        //P2PMessageDispatcher.RegisterHandler(new P2PStateHandler(opponentPlayer, myPlayerNumber));
        //P2PMessageDispatcher.RegisterHandler(new ObjectSpawnHandler(player1Object, player2Object));

        // AppendLog("P2P ���� �Ϸ�, ä�� ���� ����");

        // 9. ä�� UI ��ư ����
        //chatSendButton.onClick.RemoveAllListeners();
        //chatSendButton.onClick.AddListener(() =>
        //{
        //    string msg = chatInputField.text;
        //    if (string.IsNullOrWhiteSpace(msg)) return;

        //    chatInputField.text = "";
        //    logText.text += $"[{myNickname}] {msg}\n";

        //    // ����: Chat �޽��� �� Builder �� Sender
        //    string chatMsg = ChatMessageBuilder.Build(msg);
        //    P2PMessageSender.SendMessage(chatMsg);
        //});

        //// 10. ����ȭ ����

        //// PlayerMover���� ��ȣ �Ҵ�
        //player1Object.GetComponent<PlayerMove>().SetMyPlayerNumber(myPlayerNumber);
        //player2Object.GetComponent<PlayerMove>().SetMyPlayerNumber(myPlayerNumber);

        //// UpdateManager�� ��°�� �ѱ�
        //updateManager.Initialize(myPlayer, opponentPlayer, myPlayerNumber);
        //updateManager.enabled = true;
        //AppendLog($"���� ����ȭ ���� (�� ��ȣ: Player {myPlayerNumber}, UpdateManager Ȱ��ȭ��)");

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
}