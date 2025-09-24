using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Threading.Tasks;

public class MatchManager : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    public TMP_Text logText;

    private string myPlayerId;
    private string myIp;
    private int myPort;
    public string MyNickname { get; set; }

    private string localIp;
    private int localPort;

    private string ticketId;
    private bool isMatching = false;
    private float matchStartTime = 0f;

    public async void OnMatchButtonClicked()
    {
        if (isMatching) return;
        isMatching = true;
        // 1. �÷��̾� ID ����
        myPlayerId = Guid.NewGuid().ToString();
        MyNickname = "Kitty";

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
        await LambdaStore.StorePlayerInfo(myPlayerId, myIp, myPort, localIp, localPort, MyNickname);

        // 5. GameLift ��Ī ����
        AppendLog("Match Search Start!");
        ticketId = await GameLiftStartMatch.StartMatchmaking(myPlayerId);

        if (ticketId == null)
        {
            //AppendLog("��Ī Ƽ�� ���� ����");
            return;
        }

        // 6. ��Ī �Ϸ���� ���
        matchStartTime = Time.time;
        AppendLog("Match Searching...");
        StartCoroutine(UpdateMatchElapsedTime());

        bool matchCompleted = await GameLiftWait.WaitForMatchCompletion(ticketId);

        if (!matchCompleted)
        {
            //AppendLog("��Ī ���� �Ǵ� �Ϸ���� ����");
            return;
        }

        AppendLog("Match Completed");

        // 7. ���� ���� ��ȸ
        var opponent = await LambdaGet.GetOpponentInfo(myPlayerId);
        while (opponent == null || string.IsNullOrEmpty(opponent.ip))
        {
            //AppendLog("��� ���� ��ȸ ����");
            opponent = await LambdaGet.GetOpponentInfo(myPlayerId);
            await Task.Delay(500);
            //return;
        }

        // 8. P2P ���� �� ä�� �ʱ�ȭ
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

        AppendLog("Game Start!!");
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
            float elapsed = Time.time - matchStartTime;
            AppendLog($"Match Searching... {Mathf.FloorToInt(elapsed)}s...");
            yield return new WaitForSeconds(1f);
        }
    }

    public async void OnCancelMatchButtonClicked()
    {
        if (!isMatching || string.IsNullOrEmpty(ticketId))
        {
            AppendLog("Currently not matching");
            return;
        }

        AppendLog("try to cancel matching...");
        bool success = await GameLiftCancelMatch.CancelMatchmaking(ticketId);

        if (success)
        {
            AppendLog("Match Canceled.");
            isMatching = false;
        }
        else
        {
            AppendLog("Matching cancellation failed");
        }
    }
}