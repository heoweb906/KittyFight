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
        // 1. �÷��̾� ID ����
        myPlayerId = Guid.NewGuid().ToString();
        myNickname = nicknameInput.text;

        // 2. ���� ��Ʈ Ȯ��
        myPort = GetAvailablePort();
        AppendLog("�� ���� Port: " + myPort);

        // 3. STUN ���� ���� ���� IP/��Ʈ Ȯ��
        var udp = new UdpClient(myPort);
        var stunResult = await StunClient.QueryAsync(udp, "stun.l.google.com", 19302);

        if (stunResult?.PublicEndPoint == null)
        {
            AppendLog("STUN ��û ���� - ���� IP/Port ��ȸ ����");
            return;
        }

        myIp = stunResult.PublicEndPoint.Address.ToString();
        myPort = stunResult.PublicEndPoint.Port;

        AppendLog("���� IP: " + myIp);
        AppendLog("���� Port: " + myPort);

        // 4. �� ���� Lambda�� ����
        await LambdaStore.StorePlayerInfo(myPlayerId, myIp, myPort, myNickname);
        AppendLog("�� ���� ���� �Ϸ�");

        // 5. GameLift ��Ī ����
        AppendLog("GameLift ��Ī ����...");
        string ticketId = await GameLiftStartMatch.StartMatchmaking(myPlayerId);

        if (ticketId == null)
        {
            AppendLog("��Ī Ƽ�� ���� ����");
            return;
        }

        // 6. ��Ī �Ϸ���� ���
        AppendLog("��Ī ��� ��...");
        bool matchCompleted = await GameLiftWait.WaitForMatchCompletion(ticketId);

        if (!matchCompleted)
        {
            AppendLog("��Ī ���� �Ǵ� �Ϸ���� ����");
            return;
        }

        AppendLog("��Ī �Ϸ�! ���� ���� ��ȸ ��...");

        // 7. ���� ���� ��ȸ
        var opponent = await LambdaGet.GetOpponentInfo(myPlayerId);
        if (opponent == null)
        {
            AppendLog("��� ���� ��ȸ ����");
            return;
        }

        AppendLog($"��� IP: {opponent.ip}, Port: {opponent.port}, Nickname: {opponent.nickname}");

        // 8. P2P ���� �� ä�� �ʱ�ȭ
        P2PChat.Init(myPort, myNickname, udp, logText);
        await Task.Delay(500);
        P2PChat.Connect(opponent.ip, opponent.port);

        AppendLog("P2P ���� �Ϸ�, ä�� ���� ����");

        // 9. ä�� UI ��ư ����
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