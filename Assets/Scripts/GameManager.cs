using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    public Transform spawnPoint1;
    public Transform spawnPoint2;

    public InGameUIController ingameUIController;
    public UpdateManager updateManager;

    public GameTimer gameTimer;

    public PlayerAbility playerAbility_1;
    public PlayerAbility playerAbility_2;
    public SkillWorker skillWorker_1;
    public SkillWorker skillWorker_2;

    private void Start()
    {
        P2PManager.Init(MatchResultStore.myPort, MatchResultStore.udpClient, this);
        P2PManager.ConnectToOpponent(MatchResultStore.opponentIp, MatchResultStore.opponentPort);
    }
    void Update()
    {
        // ��׶��� �����忡�� ���� �÷��׸� ���� �������� ����
        if (P2PManager.IsReadyToStartGame)
        {
            // �� ���� ó���ϵ��� �÷��� ����
            P2PManager.IsReadyToStartGame = false;
            InitializeGame();
        }
    }

    private void InitializeGame()
    {
        //string myNickname = MatchResultStore.myNickname;
        //string opponentNickname = MatchResultStore.opponentNickname;

        Debug.Log("=== MatchResultStore ���� ===");
        Debug.Log($"myPlayerNumber: {MatchResultStore.myPlayerNumber}");
        Debug.Log($"myNickname: {MatchResultStore.myNickname}");
        Debug.Log($"opponentNickname: {MatchResultStore.opponentNickname}");
        Debug.Log($"opponentIp: {MatchResultStore.opponentIp}");
        Debug.Log($"opponentPort: {MatchResultStore.opponentPort}");
        Debug.Log($"myPort: {MatchResultStore.myPort}");
        Debug.Log($"udpClient is null: {MatchResultStore.udpClient == null}");
        Debug.Log("=============================");

        Debug.Log($"player1Prefab null? {player1Prefab == null}");
        Debug.Log($"player2Prefab null? {player2Prefab == null}");
        Debug.Log($"spawnPoint1 null? {spawnPoint1 == null}");
        Debug.Log($"spawnPoint2 null? {spawnPoint2 == null}");

        int myNum = MatchResultStore.myPlayerNumber;
        Debug.Log($"myNum: {myNum}");

        GameObject myPlayerPrefab = (myNum == 1) ? player1Prefab : player2Prefab;
        GameObject opponentPrefab = (myNum == 1) ? player2Prefab : player1Prefab;

        Transform mySpawn = (myNum == 1) ? spawnPoint1 : spawnPoint2;
        Transform opponentSpawn = (myNum == 1) ? spawnPoint2 : spawnPoint1;

        GameObject myPlayer = Instantiate(myPlayerPrefab, mySpawn.position, Quaternion.identity);
        GameObject opponentPlayer = Instantiate(opponentPrefab, opponentSpawn.position, Quaternion.identity);

        // �Է� ���� ����
        myPlayer.GetComponent<PlayerInputRouter>().SetOwnership(true);
        opponentPlayer.GetComponent<PlayerInputRouter>().SetOwnership(false);

        var opponentRb = opponentPlayer.GetComponent<Rigidbody>();
        opponentRb.isKinematic = true;

        myPlayer.GetComponent<PlayerAttack>().myPlayerNumber = myNum;
        myPlayer.GetComponent<PlayerHealth>().playerNumber = myNum;
        opponentPlayer.GetComponent<PlayerHealth>().playerNumber = (myNum == 1) ? 2 : 1;

        PlayerHealth myHealth = myPlayer.GetComponent<PlayerHealth>();
        myHealth.playerNumber = myNum;

        // ���� �ִ� �� HPBar UI�� ã�� ����
        GameObject hpBarUIObj = GameObject.Find("HPBar");
        if (hpBarUIObj != null)
        {
            PlayerHealthUI hpUI = hpBarUIObj.GetComponent<PlayerHealthUI>();
            myHealth.hpUI = hpUI;
        }


        // ���� �÷��̾���� ������ ������
        if (myNum == 1)
        {
            playerAbility_1 = myPlayer.GetComponent<PlayerAbility>();
            skillWorker_1 = myPlayer.GetComponent<SkillWorker>();
            playerAbility_2 = opponentPlayer.GetComponent<PlayerAbility>();
            skillWorker_2 = opponentPlayer.GetComponent<SkillWorker>();
        }
        else
        {
            playerAbility_2 = myPlayer.GetComponent<PlayerAbility>();
            skillWorker_2 = myPlayer.GetComponent<SkillWorker>();
            playerAbility_1 = opponentPlayer.GetComponent<PlayerAbility>();
            skillWorker_1 = opponentPlayer.GetComponent<SkillWorker>();
        }

        opponentPlayer.GetComponent<SkillWorker>().bCantInput = true;




        // ################
        // �ڵ鷯 ���
        // ################

        P2PMessageDispatcher.RegisterHandler(new P2PStateHandler(opponentPlayer, myNum));
        P2PMessageDispatcher.RegisterHandler(new ObjectSpawnHandler(myPlayer, opponentPlayer));
        P2PMessageDispatcher.RegisterHandler(new DamageHandler(opponentPlayer.GetComponent<PlayerHealth>(), myNum));



        P2PMessageDispatcher.RegisterHandler(new P2PSkillSelectHandler(opponentPlayer.GetComponent<SkillWorker>(), ingameUIController.skillCardController, myNum));
        P2PMessageDispatcher.RegisterHandler(new P2PSkillShowHandler(ingameUIController.skillCardController, myNum));

 




        // ä���� ���߿�

        // �ǽð� ������Ʈ
        updateManager.Initialize(myPlayer, opponentPlayer, myNum);
        updateManager.enabled = true;

        if (gameTimer != null)
        {
            gameTimer.StartTimer(90f);
        }
    }



}