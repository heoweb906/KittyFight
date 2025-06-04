using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    public Transform spawnPoint1;
    public Transform spawnPoint2;

    public UpdateManager updateManager;

    //public Button chatSendButton;
    //public TMP_InputField chatInputField;
    //public TMP_Text logText;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        int myNum = MatchResultStore.myPlayerNumber;
        //string myNickname = MatchResultStore.myNickname;
        //string opponentNickname = MatchResultStore.opponentNickname;

        Debug.Log("=== MatchResultStore 정보 ===");
        Debug.Log($"myPlayerNumber: {MatchResultStore.myPlayerNumber}");
        Debug.Log($"myNickname: {MatchResultStore.myNickname}");
        Debug.Log($"opponentNickname: {MatchResultStore.opponentNickname}");
        Debug.Log($"opponentIp: {MatchResultStore.opponentIp}");
        Debug.Log($"opponentPort: {MatchResultStore.opponentPort}");
        Debug.Log($"myPort: {MatchResultStore.myPort}");
        Debug.Log($"udpClient is null: {MatchResultStore.udpClient == null}");
        Debug.Log("=============================");

        GameObject myPlayerPrefab = (myNum == 1) ? player1Prefab : player2Prefab;
        GameObject opponentPrefab = (myNum == 1) ? player2Prefab : player1Prefab;

        Transform mySpawn = (myNum == 1) ? spawnPoint1 : spawnPoint2;
        Transform opponentSpawn = (myNum == 1) ? spawnPoint2 : spawnPoint1;

        GameObject myPlayer = Instantiate(myPlayerPrefab, mySpawn.position, Quaternion.identity);
        GameObject opponentPlayer = Instantiate(opponentPrefab, opponentSpawn.position, Quaternion.identity);

        // 입력 권한 설정
        myPlayer.GetComponent<PlayerInputRouter>().SetOwnership(true);
        opponentPlayer.GetComponent<PlayerInputRouter>().SetOwnership(false);

        myPlayer.GetComponent<PlayerAttack>().myPlayerNumber = myNum;
        myPlayer.GetComponent<PlayerHealth>().playerNumber = myNum;
        opponentPlayer.GetComponent<PlayerHealth>().playerNumber = (myNum == 1) ? 2 : 1;

        // P2P 연결
        P2PManager.Init(MatchResultStore.myPort, MatchResultStore.udpClient);
        P2PManager.ConnectToOpponent(MatchResultStore.opponentIp, MatchResultStore.opponentPort);

        // 핸들러 등록
        //P2PMessageDispatcher.RegisterHandler(new P2PChatHandler(logText, opponentNickname));
        P2PMessageDispatcher.RegisterHandler(new P2PStateHandler(opponentPlayer, myNum));
        P2PMessageDispatcher.RegisterHandler(new ObjectSpawnHandler(myPlayer, opponentPlayer));
        P2PMessageDispatcher.RegisterHandler(new DamageHandler(opponentPlayer.GetComponent<PlayerHealth>(), myNum));
        // 채팅은 나중에

        // 실시간 업데이트
        updateManager.Initialize(myPlayer, opponentPlayer, myNum);
        updateManager.enabled = true;
    }
}