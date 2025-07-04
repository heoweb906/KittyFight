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

    public GameObject player1HUD;
    public GameObject player2HUD;

    private GameObject player1;
    private GameObject player2;
    private int myNum;
    
    public GameObject backgroundPlane;

    private bool gameEnded = false;



    [Header("양측 플레이어 정보")]
    public PlayerAbility playerAbility_1;
    public PlayerAbility playerAbility_2;
    public SkillWorker skillWorker_1;
    public SkillWorker skillWorker_2;

    //public Button chatSendButton;
    //public TMP_InputField chatInputField;
    //public TMP_Text logText;

    private void Start()
    {
        P2PManager.Init(MatchResultStore.myPort, MatchResultStore.udpClient, this);
        P2PManager.ConnectToOpponent(MatchResultStore.opponentIp, MatchResultStore.opponentPort);
    }
    void Update()
    {
        // 백그라운드 스레드에서 세운 플래그를 메인 루프에서 감지
        if (P2PManager.IsReadyToStartGame)
        {
            // 한 번만 처리하도록 플래그 리셋
            P2PManager.IsReadyToStartGame = false;
            InitializeGame();
        }
    }

    private void InitializeGame()
    {
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

        Debug.Log($"player1Prefab null? {player1Prefab == null}");
        Debug.Log($"player2Prefab null? {player2Prefab == null}");
        Debug.Log($"spawnPoint1 null? {spawnPoint1 == null}");
        Debug.Log($"spawnPoint2 null? {spawnPoint2 == null}");

        myNum = MatchResultStore.myPlayerNumber;
        Debug.Log($"myNum: {myNum}");

        GameObject myPlayerPrefab = (myNum == 1) ? player1Prefab : player2Prefab;
        GameObject opponentPrefab = (myNum == 1) ? player2Prefab : player1Prefab;

        Transform mySpawn = (myNum == 1) ? spawnPoint1 : spawnPoint2;
        Transform opponentSpawn = (myNum == 1) ? spawnPoint2 : spawnPoint1;

        GameObject myPlayer = Instantiate(myPlayerPrefab, mySpawn.position, Quaternion.identity);
        GameObject opponentPlayer = Instantiate(opponentPrefab, opponentSpawn.position, Quaternion.identity);

        player1 = (myNum == 1) ? myPlayer : opponentPlayer;
        player2 = (myNum == 1) ? opponentPlayer : myPlayer;

        // 입력 권한 설정
        myPlayer.GetComponent<PlayerInputRouter>().SetOwnership(true);
        opponentPlayer.GetComponent<PlayerInputRouter>().SetOwnership(false);

        var opponentRb = opponentPlayer.GetComponent<Rigidbody>();
        opponentRb.isKinematic = true;

        myPlayer.GetComponent<PlayerAttack>().myPlayerNumber = myNum;
        myPlayer.GetComponent<PlayerHealth>().playerNumber = myNum;
        opponentPlayer.GetComponent<PlayerHealth>().playerNumber = (myNum == 1) ? 2 : 1;

        InjectHUDUI(myPlayer, myNum);
        InjectHUDUI(opponentPlayer, (myNum == 1) ? 2 : 1);




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






        // 핸들러 등록
        //P2PMessageDispatcher.RegisterHandler(new P2PChatHandler(logText, opponentNickname));
        P2PMessageDispatcher.RegisterHandler(new P2PStateHandler(opponentPlayer, myNum));
        P2PMessageDispatcher.RegisterHandler(new ObjectSpawnHandler(myPlayer, opponentPlayer));
        P2PMessageDispatcher.RegisterHandler(new DamageHandler(opponentPlayer.GetComponent<PlayerHealth>(), myNum));


        P2PMessageDispatcher.RegisterHandler(new P2PSkillSelectHandler(opponentPlayer.GetComponent<SkillWorker>(), ingameUIController.skillCardController, myNum));
        P2PMessageDispatcher.RegisterHandler(new P2PSkillShowHandler(ingameUIController.skillCardController, myNum));
        // 채팅은 나중에

        // 실시간 업데이트
        updateManager.Initialize(myPlayer, opponentPlayer, myNum);
        updateManager.enabled = true;

        if (gameTimer != null)
        {
            gameTimer.StartTimer(90f);
        }
    }
    private void InjectHUDUI(GameObject player, int playerNum)
    {
        HUDController hud = (playerNum == 1) ? player1HUD.GetComponent<HUDController>() : player2HUD.GetComponent<HUDController>();
        if (hud == null) return;

        var myHealth = player.GetComponent<PlayerHealth>();
        myHealth.playerNumber = playerNum;
        myHealth.InjectUI(hud.GetHealthUI());

        var myAttack = player.GetComponent<PlayerAttack>();
        myAttack.cooldownUI = hud.GetSkillUI();
    }

    public void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("Game Over");

        player1.GetComponent<PlayerInputRouter>().SetOwnership(false);
        player2.GetComponent<PlayerInputRouter>().SetOwnership(false);

        Invoke(nameof(ResetGame), 3f);
    }

    private void ResetGame()
    {
        Debug.Log("Resetting Game");

        RandomizePlaneColor();

        player1.transform.position = spawnPoint1.position;
        player2.transform.position = spawnPoint2.position;

        player1.GetComponent<PlayerHealth>().ResetHealth();
        player2.GetComponent<PlayerHealth>().ResetHealth();

        player1.GetComponent<PlayerInputRouter>().SetOwnership(myNum == 1);
        player2.GetComponent<PlayerInputRouter>().SetOwnership(myNum == 2);

        gameEnded = false;
        gameTimer.StartTimer(90f);
    }

    private void RandomizePlaneColor()
    {
        if (backgroundPlane == null) return;

        Renderer rend = backgroundPlane.GetComponent<Renderer>();
        if (rend == null) return;

        Color randomColor = Random.ColorHSV();

        if (rend.material.HasProperty("_BaseColor"))
            rend.material.SetColor("_BaseColor", randomColor);
        else if (rend.material.HasProperty("_Color"))
            rend.material.SetColor("_Color", randomColor);
    }
}