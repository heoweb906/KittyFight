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

    // === 프리팹 (호환성 유지를 위해 남겨둠: 현재 초기화 흐름에선 사용하지 않음)
    [Header("플레이어 프리팹 (현재 흐름에선 미사용, 호환용)")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    // === 씬 배치 플레이어 참조 (이번 리팩터 핵심)
    [Header("씬에 미리 배치된 플레이어")]
    public GameObject player1; // 왼쪽(플레이어 1)
    public GameObject player2; // 오른쪽(플레이어 2)

    // === 시스템/매니저
    [Header("UI/업데이트")]
    public MapManager mapManager;
    public InGameUIController ingameUIController;
    public UpdateManager updateManager;

    [Header("카메라")]
    public CameraManager cameraManager;

    [Header("배경")]
    public GameObject backgroundPlane;

    [Header("기본 스킬 프리팹 (Skill 컴포넌트 포함)")]
    public GameObject meleeSkillPrefab;
    public GameObject rangedSkillPrefab;
    public GameObject dashPrefab;

    // 양측 Ability 공개 참조(좌=1, 우=2)
    [Header("양측 플레이어 Ability 참조")]
    public PlayerAbility playerAbility_1;
    public PlayerAbility playerAbility_2;

    // 점수/맵 기믹
    public int IntScorePlayer_1 { get; set; }
    public int IntScorePlayer_2 { get; set; }
    public int IntMapGimicnumber { get; set; }   // 현재 적용 중인 맵 기믹 번호
    public bool BoolAcitveMapGimic { get; set; } // 기믹 활성 여부

    // 내부 상태
    int myNum;                   // 1 or 2
    bool isOpponentReady = false;
    bool isBootstrapped = false;
    bool gameEnded = false;
    PlayerAbility myAbility;

    // 배경 후보(원 코드 유지)
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
        // P2P 연결/핸들러 등록 (원 코드 유지)
        P2PManager.Init(MatchResultStore.myPort, MatchResultStore.udpClient, this);
        P2PManager.ConnectToOpponent(MatchResultStore.opponentIp, MatchResultStore.opponentPort);

        P2PMessageDispatcher.RegisterHandler(new PlayingHandler(this));
        P2PMessageDispatcher.RegisterHandler(new BackgroundColorHandler(this));
        P2PMessageDispatcher.RegisterHandler(new ReadyHandler(this));
        P2PMessageDispatcher.RegisterHandler(new StartHandler(this));

        IntScorePlayer_1 = 0;
        IntScorePlayer_2 = 0;

        // 기존: InitializeGame(0,0) 호출 → (Instantiate로 인한 충돌 루트)
        // 리팩터: 초기엔 아무것도 생성/시작하지 않고, P1 세팅 수신 후 구축.
        IntMapGimicnumber = 0;
        BoolAcitveMapGimic = false;

        if (updateManager != null) updateManager.enabled = false; // 시작 전 위치 동기화 OFF
    }

    void Update()
    {
        if (P2PManager.IsReadyToStartGame && currentState == GameState.Connecting)
        {
            myNum = MatchResultStore.myPlayerNumber;
            BootstrapSceneIfNeeded(); // 씬 배치 객체 1회 주입

            if (myNum == 1)
            {
                currentState = GameState.SceneSetup;
                int mapIdx = Random.Range(0, mapManager.mapObjects.Length);
                int bgIdx = Random.Range(0, mapManager.backgroundSprites.Length);
                P2PMessageSender.SendMessage(BackgroundColorMessageBuilder.Build(mapIdx, bgIdx, 0));
                ApplyResetData(mapIdx, bgIdx, 0);
            }
        }

        // 실제 타이머 카운트는 StartGame에서 시작하지만, UI Tick은 유지
        ingameUIController?.TickGameTimer();
    }

    // P1이 보낸 세팅 수신(P2 최초/라운드 리셋 모두 공용)
    public void OnReceiveSetupMessage(BackgroundColorMessage data)
    {
        myNum = MatchResultStore.myPlayerNumber;
        BootstrapSceneIfNeeded();

        currentState = GameState.SceneSetup;
        ApplyResetData(data.mapIndex, data.backgroundIndex, data.iMapGimicNum);
    }

    // P2가 준비됐다고 알림 수신(P1에서만 의미)
    public void OnOpponentReady()
    {
        if (myNum != 1) return;
        isOpponentReady = true;
        CheckIfBothPlayersAreReady();
    }

    // START 수신(양쪽 공통)
    public void OnReceiveStart()
    {
        StartGame();
    }

    // ==== 내부: 씬 배치 객체 주입(1회) ====
    void BootstrapSceneIfNeeded()
    {
        if (isBootstrapped) return;
        isBootstrapped = true;

        // 좌우 고정 기준(플레이어1=왼쪽, 플레이어2=오른쪽)
        var myPlayer = (myNum == 1) ? player1 : player2;
        var oppPlayer = (myNum == 1) ? player2 : player1;

        // 닉네임 표시(있을 때만)
        var myNicknameText = myPlayer ? myPlayer.GetComponentInChildren<TextMeshProUGUI>() : null;
        var oppNicknameText = oppPlayer ? oppPlayer.GetComponentInChildren<TextMeshProUGUI>() : null;
        if (myNicknameText) myNicknameText.text = MatchResultStore.myNickname;
        if (oppNicknameText) oppNicknameText.text = MatchResultStore.opponentNickname;

        // Ability playerNumber 설정
        var myAb = myPlayer ? myPlayer.GetComponent<PlayerAbility>() : null;
        var oppAb = oppPlayer ? oppPlayer.GetComponent<PlayerAbility>() : null;
        if (myAb) myAb.playerNumber = myNum;
        if (oppAb) oppAb.playerNumber = (myNum == 1) ? 2 : 1;


        // 공개 참조(좌=1, 우=2)
        playerAbility_1 = player1 ? player1.GetComponent<PlayerAbility>() : null;
        playerAbility_2 = player2 ? player2.GetComponent<PlayerAbility>() : null;

        // 입력 권한: 내 것만 허용(시작 전에도 내 입력은 받을 수 있게 유지)
        myPlayer?.GetComponent<PlayerInputRouter>()?.SetOwnership(true);
        oppPlayer?.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        // 원격 대상은 Kinematic (물리 충돌 최소화)
        var myRb = myPlayer ? myPlayer.GetComponent<Rigidbody>() : null;
        var oppRb = oppPlayer ? oppPlayer.GetComponent<Rigidbody>() : null;
        if (myRb) myRb.isKinematic = true;
        if (oppRb) oppRb.isKinematic = true;

        // 기본 스킬(중복 방지)
        EquipDefaultSkills(myAb);
        EquipDefaultSkills(oppAb);

        // UI 와이어링
        ingameUIController?.WireSkillUIs(playerAbility_1, playerAbility_2);

        // P2P 핸들러 등록(원격 참조 필요)
        P2PMessageDispatcher.RegisterHandler(new P2PStateHandler(oppPlayer, myNum));
        P2PMessageDispatcher.RegisterHandler(new DamageHandler(oppPlayer ? oppPlayer.GetComponent<PlayerHealth>() : null, myNum));
        P2PMessageDispatcher.RegisterHandler(new SkillExecuteHandler(oppAb, myNum));
        P2PMessageDispatcher.RegisterHandler(new P2PSkillSelectHandler(oppAb, ingameUIController ? ingameUIController.skillCardController : null, myNum));
        P2PMessageDispatcher.RegisterHandler(new P2PSkillShowHandler(ingameUIController ? ingameUIController.skillCardController : null, myNum));

        // 위치 동기화 매니저 초기화(시작 전 OFF)
        if (updateManager != null)
        {
            updateManager.enabled = false;
            updateManager.Initialize(myPlayer, oppPlayer, myNum);
        }

        myAbility = myAb;
    }

    // ==== 내부: 라운드 리셋/최초 세팅 ====
    void ApplyResetData(int mapIdx, int bgIdx, int gimic)
    {
        // 맵/배경
        mapManager.ChangeMap(mapIdx);
        mapManager.ChangeBackground(bgIdx);

        // 기믹 상태 저장(이번 단계에선 동작 제어 X, 값만 유지)
        IntMapGimicnumber = gimic;
        BoolAcitveMapGimic = gimic > 0;

        // 스폰 배치 + HP 리셋 (씬 배치 플레이어 사용)
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

        if (updateManager != null) updateManager.enabled = false;  // 대기 중 송신 금지
        player1?.GetComponent<PlayerInputRouter>()?.SetOwnership(false);
        player2?.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        // P2는 READY 알림
        if (myNum == 2)
        {
            P2PMessageSender.SendMessage("[READY]");
        }

        // P1은 자동 체크
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

        // 위치 동기화 시작
        if (updateManager != null) updateManager.enabled = true;

        // 라운드 타이머 시작(여기서만)
        ingameUIController?.StartGameTimer(60f);

        // 패시브 이벤트(있으면)
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

        // 입력 차단(양쪽)
        player1?.GetComponent<PlayerInputRouter>()?.SetOwnership(false);
        player2?.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        StartCoroutine(EndGameSequence(iLosePlayerNum));
    }

    IEnumerator EndGameSequence(int iLosePlayerNum)
    {
        yield return new WaitForSeconds(1f);

        int winnerPlayerNum = iLosePlayerNum == 1 ? 2 : 1;
        ingameUIController?.ComeToTheEndGame(winnerPlayerNum);

        gameEnded = false; // 다음 라운드 대비
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
            // 동점 임시 처리(원 코드 로직 유지)
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

        // 근접
        if (meleeSkillPrefab != null)
        {
            GameObject meleeObj = Instantiate(meleeSkillPrefab, ability.transform);
            Skill melee = meleeObj.GetComponent<Skill>();
            if (melee != null) ability.SetSkill(SkillType.Melee, melee);
        }

        // 원거리
        if (rangedSkillPrefab != null)
        {
            GameObject rangedObj = Instantiate(rangedSkillPrefab, ability.transform);
            Skill ranged = rangedObj.GetComponent<Skill>();
            if (ranged != null) ability.SetSkill(SkillType.Ranged, ranged);
        }

        // 대쉬
        if (dashPrefab != null)
        {
            GameObject dashObj = Instantiate(dashPrefab, ability.transform);
            Skill dash = dashObj.GetComponent<Skill>();
            if (dash != null) ability.SetSkill(SkillType.Dash, dash);
        }
    }
}