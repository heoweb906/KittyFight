using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Connecting,
        SceneSetup,
        Ready,
        Playing
    }
    public GameState currentState = GameState.Connecting;

    // === ������ (ȣȯ�� ������ ���� ���ܵ�: ���� �ʱ�ȭ �帧���� ������� ����)
    [Header("�÷��̾� ������ (���� �帧���� �̻��, ȣȯ��)")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    // === �� ��ġ �÷��̾� ���� (�̹� ������ �ٽ�)
    [Header("���� �̸� ��ġ�� �÷��̾�")]
    public GameObject player1; // ����(�÷��̾� 1)
    public GameObject player2; // ������(�÷��̾� 2)

    // === �ý���/�Ŵ���
    [Header("UI/������Ʈ")]
    public MapManager mapManager;
    public InGameUIController ingameUIController;
    public UpdateManager updateManager;

    [Header("ī�޶�")]
    public CameraManager cameraManager;

    [Header("���")]
    public GameObject backgroundPlane;

    [Header("�⺻ ��ų ������ (Skill ������Ʈ ����)")]
    public GameObject meleeSkillPrefab;
    public GameObject rangedSkillPrefab;
    public GameObject dashPrefab;

    // ���� Ability ���� ����(��=1, ��=2)
    [Header("���� �÷��̾� Ability ����")]
    public PlayerAbility playerAbility_1;
    public PlayerAbility playerAbility_2;

    // ����/�� ���
    public int IntScorePlayer_1 { get; set; }
    public int IntScorePlayer_2 { get; set; }
    public int IntMapGimicnumber { get; set; }   // ���� ���� ���� �� ��� ��ȣ
    public bool BoolAcitveMapGimic { get; set; } // ��� Ȱ�� ����

    // ���� ����
    int myNum;                   // 1 or 2
    bool isOpponentReady = false;
    bool isBootstrapped = false;
    bool gameEnded = false;
    PlayerAbility myAbility;

    // ��� �ĺ�(�� �ڵ� ����)
    readonly Color[] backgroundColorCandidates = new Color[]
    {
        new Color(0.8f, 0.9f, 1f),
        new Color(0.95f, 0.95f, 0.95f),
        new Color(0.9f, 0.8f, 0.8f),
        new Color(0.75f, 1f, 0.75f),
        new Color(1f, 1f, 0.6f),
    };

    void Start()
    {
        // P2P ����/�ڵ鷯 ��� (�� �ڵ� ����)
        P2PManager.Init(MatchResultStore.myPort, MatchResultStore.udpClient, this);
        P2PManager.ConnectToOpponent(MatchResultStore.opponentIp, MatchResultStore.opponentPort);

        P2PMessageDispatcher.RegisterHandler(new PlayingHandler(this));
        P2PMessageDispatcher.RegisterHandler(new BackgroundColorHandler(this));
        P2PMessageDispatcher.RegisterHandler(new ReadyHandler(this));
        P2PMessageDispatcher.RegisterHandler(new StartHandler(this));

        IntScorePlayer_1 = 0;
        IntScorePlayer_2 = 0;

        // ����: InitializeGame(0,0) ȣ�� �� (Instantiate�� ���� �浹 ��Ʈ)
        // ������: �ʱ⿣ �ƹ��͵� ����/�������� �ʰ�, P1 ���� ���� �� ����.
        IntMapGimicnumber = 0;
        BoolAcitveMapGimic = false;

        if (updateManager != null) updateManager.enabled = false; // ���� �� ��ġ ����ȭ OFF
    }

    void Update()
    {
        if (P2PManager.IsReadyToStartGame && currentState == GameState.Connecting)
        {
            myNum = MatchResultStore.myPlayerNumber;
            BootstrapSceneIfNeeded(); // �� ��ġ ��ü 1ȸ ����

            if (myNum == 1)
            {
                currentState = GameState.SceneSetup;
                int mapIdx = Random.Range(0, mapManager.mapObjects.Length);
                int bgIdx = Random.Range(0, mapManager.backgroundSprites.Length);
                P2PMessageSender.SendMessage(BackgroundColorMessageBuilder.Build(mapIdx, bgIdx, 0));
                ApplyResetData(mapIdx, bgIdx, 0);
            }
        }

        // ���� Ÿ�̸� ī��Ʈ�� StartGame���� ����������, UI Tick�� ����
        ingameUIController?.TickGameTimer();
    }

    // P1�� ���� ���� ����(P2 ����/���� ���� ��� ����)
    public void OnReceiveSetupMessage(BackgroundColorMessage data)
    {
        myNum = MatchResultStore.myPlayerNumber;
        BootstrapSceneIfNeeded();

        currentState = GameState.SceneSetup;
        ApplyResetData(data.mapIndex, data.backgroundIndex, data.iMapGimicNum);
    }

    // P2�� �غ�ƴٰ� �˸� ����(P1������ �ǹ�)
    public void OnOpponentReady()
    {
        if (myNum != 1) return;
        isOpponentReady = true;
        CheckIfBothPlayersAreReady();
    }

    // START ����(���� ����)
    public void OnReceiveStart()
    {
        StartGame();
    }

    // ==== ����: �� ��ġ ��ü ����(1ȸ) ====
    void BootstrapSceneIfNeeded()
    {
        if (isBootstrapped) return;
        isBootstrapped = true;

        // �¿� ���� ����(�÷��̾�1=����, �÷��̾�2=������)
        var myPlayer = (myNum == 1) ? player1 : player2;
        var oppPlayer = (myNum == 1) ? player2 : player1;

        // �г��� ǥ��(���� ����)
        var myNicknameText = myPlayer ? myPlayer.GetComponentInChildren<TextMeshProUGUI>() : null;
        var oppNicknameText = oppPlayer ? oppPlayer.GetComponentInChildren<TextMeshProUGUI>() : null;
        if (myNicknameText) myNicknameText.text = MatchResultStore.myNickname;
        if (oppNicknameText) oppNicknameText.text = MatchResultStore.opponentNickname;

        // Ability playerNumber ����
        var myAb = myPlayer ? myPlayer.GetComponent<PlayerAbility>() : null;
        var oppAb = oppPlayer ? oppPlayer.GetComponent<PlayerAbility>() : null;
        if (myAb) myAb.playerNumber = myNum;
        if (oppAb) oppAb.playerNumber = (myNum == 1) ? 2 : 1;


        // ���� ����(��=1, ��=2)
        playerAbility_1 = player1 ? player1.GetComponent<PlayerAbility>() : null;
        playerAbility_2 = player2 ? player2.GetComponent<PlayerAbility>() : null;

        // �Է� ����: �� �͸� ���(���� ������ �� �Է��� ���� �� �ְ� ����)
        myPlayer?.GetComponent<PlayerInputRouter>()?.SetOwnership(true);
        oppPlayer?.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        // ���� ����� Kinematic (���� �浹 �ּ�ȭ)
        var myRb = myPlayer ? myPlayer.GetComponent<Rigidbody>() : null;
        var oppRb = oppPlayer ? oppPlayer.GetComponent<Rigidbody>() : null;
        if (myRb) myRb.isKinematic = true;
        if (oppRb) oppRb.isKinematic = true;

        // �⺻ ��ų(�ߺ� ����)
        EquipDefaultSkills(myAb);
        EquipDefaultSkills(oppAb);

        // UI ���̾
        ingameUIController?.WireSkillUIs(playerAbility_1, playerAbility_2);

        // P2P �ڵ鷯 ���(���� ���� �ʿ�)
        P2PMessageDispatcher.RegisterHandler(new P2PStateHandler(oppPlayer, myNum));
        P2PMessageDispatcher.RegisterHandler(new DamageHandler(oppPlayer ? oppPlayer.GetComponent<PlayerHealth>() : null, myNum));
        P2PMessageDispatcher.RegisterHandler(new SkillExecuteHandler(oppAb, myNum));
        P2PMessageDispatcher.RegisterHandler(new P2PSkillSelectHandler(oppAb, ingameUIController ? ingameUIController.skillCardController : null, myNum));
        P2PMessageDispatcher.RegisterHandler(new P2PSkillShowHandler(ingameUIController ? ingameUIController.skillCardController : null, myNum));

        // ��ġ ����ȭ �Ŵ��� �ʱ�ȭ(���� �� OFF)
        if (updateManager != null)
        {
            updateManager.enabled = false;
            updateManager.Initialize(myPlayer, oppPlayer, myNum);
        }

        myAbility = myAb;
    }

    // ==== ����: ���� ����/���� ���� ====
    void ApplyResetData(int mapIdx, int bgIdx, int gimic)
    {
        // ��/���
        mapManager.ChangeMap(mapIdx);
        mapManager.ChangeBackground(bgIdx);

        // ��� ���� ����(�̹� �ܰ迡�� ���� ���� X, ���� ����)
        IntMapGimicnumber = gimic;
        BoolAcitveMapGimic = gimic > 0;

        // ���� ��ġ + HP ���� (�� ��ġ �÷��̾� ���)
        var sp1 = mapManager.GetSpawnPoint(1);
        var sp2 = mapManager.GetSpawnPoint(2);
        if (player1 && sp1) player1.transform.position = sp1.position;
        if (player2 && sp2) player2.transform.position = sp2.position;

        player1?.GetComponent<PlayerHealth>()?.ResetHealth();
        player2?.GetComponent<PlayerHealth>()?.ResetHealth();

        var rb1 = player1 ? player1.GetComponent<Rigidbody>() : null;
        var rb2 = player2 ? player2.GetComponent<Rigidbody>() : null;
        if (rb1) { rb1.isKinematic = true; rb1.velocity = Vector3.zero; }
        if (rb2) { rb2.isKinematic = true; rb2.velocity = Vector3.zero; }

        currentState = GameState.Ready;
        isOpponentReady = false;

        if (updateManager != null) updateManager.enabled = false;  // ��� �� �۽� ����
        player1?.GetComponent<PlayerInputRouter>()?.SetOwnership(false);
        player2?.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        // P2�� READY �˸�
        if (myNum == 2)
        {
            P2PMessageSender.SendMessage("[READY]");
        }

        // P1�� �ڵ� üũ
        if (myNum == 1) CheckIfBothPlayersAreReady();
    }

    void CheckIfBothPlayersAreReady()
    {
        if (myNum == 1 && currentState == GameState.Ready && isOpponentReady)
        {
            P2PMessageSender.SendMessage("[START]");
            StartGame();
        }
    }

    public void StartGame()
    {
        if (currentState != GameState.Ready) return;
        currentState = GameState.Playing;

        var myPlayer = (myNum == 1) ? player1 : player2;
        var oppPlayer = (myNum == 1) ? player2 : player1;
        var myRb = myPlayer ? myPlayer.GetComponent<Rigidbody>() : null;
        var oppRb = oppPlayer ? oppPlayer.GetComponent<Rigidbody>() : null;
        if (myRb) { myRb.isKinematic = false; myRb.useGravity = true; myRb.velocity = Vector3.zero; }
        if (oppRb) { oppRb.isKinematic = true; oppRb.velocity = Vector3.zero; }

        player1?.GetComponent<PlayerInputRouter>()?.SetOwnership(myNum == 1);
        player2?.GetComponent<PlayerInputRouter>()?.SetOwnership(myNum == 2);

        // ��ġ ����ȭ ����
        if (updateManager != null) updateManager.enabled = true;

        // ���� Ÿ�̸� ����(���⼭��)
        ingameUIController?.StartGameTimer(60f);

        // �нú� �̺�Ʈ(������)
        myAbility?.events?.EmitRoundStart(0);
    }

    public void ResetGame()
    {
        currentState = GameState.SceneSetup;
        isOpponentReady = false;

        if (myNum == 1)
        {
            int mapIdx = Random.Range(0, mapManager.mapObjects.Length);
            int bgIdx = Random.Range(0, mapManager.backgroundSprites.Length);

            int randomValue = 0;
            if ((IntScorePlayer_1 + IntScorePlayer_2) > 0)
            {
                randomValue = Random.Range(1, 13);
                BoolAcitveMapGimic = true;
            }

            P2PMessageSender.SendMessage(BackgroundColorMessageBuilder.Build(mapIdx, bgIdx, randomValue));
            ApplyResetData(mapIdx, bgIdx, randomValue);
        }
    }

    public void EndGame(int iLosePlayerNum)
    {
        if (gameEnded) return;
        gameEnded = true;
        currentState = GameState.SceneSetup;

        var rb1 = player1 ? player1.GetComponent<Rigidbody>() : null;
        var rb2 = player2 ? player2.GetComponent<Rigidbody>() : null;
        if (rb1) { rb1.isKinematic = true; rb1.velocity = Vector3.zero; }
        if (rb2) { rb2.isKinematic = true; rb2.velocity = Vector3.zero; }

        // �Է� ����(����)
        player1?.GetComponent<PlayerInputRouter>()?.SetOwnership(false);
        player2?.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        StartCoroutine(EndGameSequence(iLosePlayerNum));
    }

    IEnumerator EndGameSequence(int iLosePlayerNum)
    {
        yield return new WaitForSeconds(1f);

        int winnerPlayerNum = iLosePlayerNum == 1 ? 2 : 1;
        ingameUIController?.ComeToTheEndGame(winnerPlayerNum);

        gameEnded = false; // ���� ���� ���
    }

    public void EndByTimer()
    {
        if (gameEnded) return;

        int winner = GetWinnerByHP();
        int loser = (winner == 1) ? 2 : 1;
        EndGame(loser);
    }

    int GetWinnerByHP()
    {
        int hp1 = GetHP(player1);
        int hp2 = GetHP(player2);

        if (hp1 == hp2)
        {
            // ���� �ӽ� ó��(�� �ڵ� ���� ����)
            return (hp1 % 2 == 1) ? 1 : 2;
        }
        return (hp1 > hp2) ? 1 : 2;
    }

    int GetHP(GameObject go)
    {
        if (!go) return 0;
        var ph = go.GetComponent<PlayerHealth>();
        return ph ? ph.CurrentHP : 0;
    }

    private void EquipDefaultSkills(PlayerAbility ability)
    {
        if (ability == null) return;

        // ����
        if (meleeSkillPrefab != null)
        {
            GameObject meleeObj = Instantiate(meleeSkillPrefab, ability.transform);
            Skill melee = meleeObj.GetComponent<Skill>();
            if (melee != null) ability.SetSkill(SkillType.Melee, melee);
        }

        // ���Ÿ�
        if (rangedSkillPrefab != null)
        {
            GameObject rangedObj = Instantiate(rangedSkillPrefab, ability.transform);
            Skill ranged = rangedObj.GetComponent<Skill>();
            if (ranged != null) ability.SetSkill(SkillType.Ranged, ranged);
        }

        // �뽬
        if (dashPrefab != null)
        {
            GameObject dashObj = Instantiate(dashPrefab, ability.transform);
            Skill dash = dashObj.GetComponent<Skill>();
            if (dash != null) ability.SetSkill(SkillType.Dash, dash);
        }
    }
}