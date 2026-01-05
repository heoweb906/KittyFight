using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using System.Threading.Tasks;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("플레이어 프리팹")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    [Header("스폰 포인트")]
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    [Header("UI/업데이트")]
    public InGameUIController ingameUIController;
    public UpdateManager updateManager;
    public MapManager mapManager;


    [Header("카메라")]
    public CameraManager cameraManager;


    [Header("기본 스킬 프리팹 (Skill 컴포넌트 포함)")]
    public GameObject meleeSkillPrefab;
    public GameObject rangedSkillPrefab;
    public GameObject dashPrefab;

    private GameObject myPlayer;
    private GameObject player1;
    private GameObject player2;
    private int myNum;
    public bool gameEnded = true;
    private PlayerAbility myAbility;

    [Header("양측 플레이어 Ability 참조")]
    public PlayerAbility playerAbility_1;
    public PlayerAbility playerAbility_2;

    private readonly List<GameObject> roundObjects = new List<GameObject>();

    [Header("P2P Disconnect Detect")]
    [SerializeField] private float opponentStateTimeout = 3.0f;
    [SerializeField] private string fallbackSceneName = "MainMenu";
    private float lastOpponentStateReceivedTime = -999f;
    private bool hasReceivedOpponentStateOnce = false;
    private bool returningToMenu = false;

    // #. 양측 플레이어 점수
    public int IntScorePlayer_1 { get; set; }
    public int IntScorePlayer_2 { get; set; }

    // #. 맵 기믹 활용 -> 나중에 분리할 수도 있음
    public int IntMapGimicnumber { get; set; }      // 현재 적용되어 있는 맵 미기 번호
    public bool BoolAcitveMapGimic { get; set; }    // 현재 맵 기믹이 적용되어 있음

    private int currentThemeIndex = -1;


    private readonly Color[] backgroundColorCandidates = new Color[]
    {
        new Color(0.8f, 0.9f, 1f),
        new Color(0.95f, 0.95f, 0.95f),
        new Color(0.9f, 0.8f, 0.8f),
        new Color(0.75f, 1f, 0.75f),
        new Color(1f, 1f, 0.6f),
    };


    private void Start()
    {
        P2PManager.Dispose();
        P2PMessageDispatcher.ClearAllHandlers();

        P2PManager.Init(MatchResultStore.myPort, MatchResultStore.udpClient, this); 
        P2PManager.ConnectToOpponent(MatchResultStore.opponentIp, MatchResultStore.opponentPort); 

        IntScorePlayer_1 = 0; 
        IntScorePlayer_2 = 0; 

        IntMapGimicnumber = 0; 
        BoolAcitveMapGimic = false; 
    }

    public void NotifyOpponentStateReceived()
    {
        hasReceivedOpponentStateOnce = true;
        lastOpponentStateReceivedTime = Time.time;
    }

    private void Update()
    {
        if (P2PManager.IsReadyToStartGame)
        {
            P2PManager.IsReadyToStartGame = false;
            InitializeGame();
        }

        if(!gameEnded) ingameUIController?.TickGameTimer();

        CheckOpponentStateTimeout();
    }


    // #. 상대방과 게임 도중 연결 끊김 감지 
    private void CheckOpponentStateTimeout()
    {
        if (returningToMenu) return;

        if (!hasReceivedOpponentStateOnce) return;

        if (Time.time - lastOpponentStateReceivedTime >= opponentStateTimeout)
        {
            ReturnToTrainingByDisconnect();
        }
    }

    public void ReturnToTrainingByDisconnect()
    {
        // 중복 호출 방지
        if (returningToMenu) return;
        returningToMenu = true;

        Debug.Log("[P2P] Opponent state timeout -> Return to training scene.");

        P2PManager.Dispose();
        P2PMessageDispatcher.ClearAllHandlers();
        MatchResultStore.Reset();

        // 1. UI 패널 닫기 실행
        // (Tip: 0f는 '즉시' 닫힙니다. 부드럽게 닫히길 원하면 0.5f 정도로 변경하세요)
        ingameUIController.CloseFadePanel_Vertical(
            ingameUIController.image_UpperArea.rectTransform,
            ingameUIController.image_LowerArea.rectTransform,
            0.5f // <- 0f 대신 0.5초 정도 애니메이션 시간을 주는 것을 추천합니다.
        );

        // 2. 2초 딜레이 후 씬 로드 (Lambda 식 활용)
        DOVirtual.DelayedCall(2f, () =>
        {
            SceneManager.LoadScene(fallbackSceneName);
        });
    }

    private void InitializeGame()
    {
        myNum = MatchResultStore.myPlayerNumber;

        // 프리팹/스폰 선택
        GameObject myPlayerPrefab = (myNum == 1) ? player1Prefab : player2Prefab;
        GameObject opponentPlayerPrefab = (myNum == 1) ? player2Prefab : player1Prefab;

        Transform mySpawn = (myNum == 1) ? spawnPoint1 : spawnPoint2;
        Transform opponentSpawn = (myNum == 1) ? spawnPoint2 : spawnPoint1;

        // Instantiate
        myPlayer = Instantiate(myPlayerPrefab, mySpawn.position, mySpawn.rotation);
        GameObject opponentPlayer = Instantiate(opponentPlayerPrefab, opponentSpawn.position, opponentSpawn.rotation);

        // player1 / player2 참조 정리 (화면 좌/우 고정 관점 유지)
        player1 = (myNum == 1) ? myPlayer : opponentPlayer;
        player2 = (myNum == 1) ? opponentPlayer : myPlayer;

        // 입력 권한
        myPlayer.GetComponent<PlayerInputRouter>()?.SetOwnership(false); 
        opponentPlayer.GetComponent<PlayerInputRouter>()?.SetOwnership(false); 

        // 상대 물리 동기화: 원격 대상은 Kinematic
        var myRb = myPlayer.GetComponent<Rigidbody>();
        var oppRb = opponentPlayer.GetComponent<Rigidbody>();
        if (myRb != null) myRb.isKinematic = true;
        if (oppRb != null) oppRb.isKinematic = true;

        // Ability / Health 번호 세팅 (권위 일원화)
        myAbility = myPlayer.GetComponent<PlayerAbility>();
        var oppAbility = opponentPlayer.GetComponent<PlayerAbility>();

        if (myAbility != null) myAbility.playerNumber = myNum;
        if (oppAbility != null) oppAbility.playerNumber = (myNum == 1) ? 2 : 1;

        // 공개 참조 채워두기
        if (myNum == 1)
        {
            playerAbility_1 = myAbility;
            playerAbility_2 = oppAbility;
        }
        else
        {
            playerAbility_2 = myAbility;
            playerAbility_1 = oppAbility;
        }

        // UI 와이어링 (쿨다운은 Ability pull 기반)
        ingameUIController?.WireSkillUIs(playerAbility_1, playerAbility_2);
        ingameUIController?.StartGameTimer(60f);

        // 기본 스킬 장착 (근접/원거리)
        EquipDefaultSkills(myAbility);
        EquipDefaultSkills(oppAbility);

        // 핸들러 등록
        P2PMessageDispatcher.RegisterHandler(new P2PStateHandler(opponentPlayer, myNum, this));
        P2PMessageDispatcher.RegisterHandler(new DamageHandler(opponentPlayer.GetComponent<PlayerHealth>(), myPlayer.GetComponent<PlayerHealth>(), myNum));
        P2PMessageDispatcher.RegisterHandler(new BackgroundColorHandler(this, mapManager)); 
        P2PMessageDispatcher.RegisterHandler(new SkillExecuteHandler(oppAbility, myNum));
        P2PMessageDispatcher.RegisterHandler(new PassiveProcHandler(oppAbility, myNum));


        P2PMessageDispatcher.RegisterHandler(new P2PSkillSelectHandler(oppAbility, ingameUIController.skillCardController, myNum));
        P2PMessageDispatcher.RegisterHandler(new P2PSkillShowHandler(ingameUIController.skillCardController, myNum));

        P2PMessageDispatcher.RegisterHandler(new MapGimicHandler(this, mapManager, myNum));


        var myNicknameText = myPlayer ? myPlayer.GetComponentInChildren<TextMeshProUGUI>() : null;
        var oppNicknameText = opponentPlayer ? opponentPlayer.GetComponentInChildren<TextMeshProUGUI>() : null;
        if (myNicknameText) myNicknameText.text = MatchResultStore.myNickname;
        if (oppNicknameText) oppNicknameText.text = MatchResultStore.opponentNickname;


        ingameUIController.CloseFadePanel_Vertical(ingameUIController.image_UpperArea.rectTransform, ingameUIController.image_LowerArea.rectTransform, 0f);
        ingameUIController.ChangeReadyStartSprite(0); 

        StartCoroutine(DelayedInitialize()); 

        // 상태 동기화 시작
        updateManager?.Initialize(myPlayer, opponentPlayer, myNum);
        if (updateManager != null) updateManager.enabled = true;
    }

    private void EquipDefaultSkills(PlayerAbility ability)
    {
        if (ability == null) return;

        // 근접
        if (meleeSkillPrefab != null)
        {
            GameObject meleeObj = Instantiate(meleeSkillPrefab, ability.transform);
            Skill melee = meleeObj.GetComponent<Skill>();
            if (melee != null)
            {
                melee.Bind(ability);
                ability.SetSkill(SkillType.Melee, melee);
            }
        }

        // 원거리
        if (rangedSkillPrefab != null)
        {
            GameObject rangedObj = Instantiate(rangedSkillPrefab, ability.transform);
            Skill ranged = rangedObj.GetComponent<Skill>();
            if (ranged != null)
            {
                ranged.Bind(ability);
                ability.SetSkill(SkillType.Ranged, ranged);
            }
        }

        // 대쉬
        if (dashPrefab != null)
        {
            GameObject dashObj = Instantiate(dashPrefab, ability.transform);
            Skill dash = dashObj.GetComponent<Skill>();
            if (dash != null)
            {
                dash.Bind(ability);
                ability.SetSkill(SkillType.Dash, dash);
            }
        }
    }

    public void EndGame(int iLosePlayerNum)
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log("Game Over");

        player1.GetComponent<PlayerInputRouter>()?.SetOwnership(false);
        player2.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        mapManager.StopCurrentGimmick();

        StartCoroutine(EndGameSequence(iLosePlayerNum));
    }


    private IEnumerator EndGameSequence(int iLosePlayerNum)
    {
        yield return new WaitForSeconds(1f);

        int winnerPlayerNum = iLosePlayerNum == 1 ? 2 : 1;
        ingameUIController?.ComeToTheEndGame(winnerPlayerNum);

        var mv1 = player1.GetComponent<PlayerMovement>();
        if (mv1 != null)
        {
            mv1.LockFacing(new Vector3(1f, 0f, 0f), 0.1f);
        }

        var mv2 = player2.GetComponent<PlayerMovement>();
        if (mv2 != null)
        {
            mv2.LockFacing(new Vector3(-1f, 0f, 0f), 0.1f);
        }

        //yield return new WaitForSeconds(0.5f);
        //ResetGame();
    }

    private IEnumerator DelayedInitialize()
    {
        yield return new WaitForSeconds(1.5f);
        ResetGame();
    }


    public void EndByTimer()
    {
        if (gameEnded) return;

        int winner = GetWinnerByHP();

        int loser = (winner == 1) ? 2 : 1;
        EndGame(loser);
    }

    private int GetWinnerByHP()
    {
        int hp1 = GetHP(player1);
        int hp2 = GetHP(player2);

        if (hp1 == hp2)
        {
            return (hp1 % 2 == 1) ? 1 : 2;
        }
        return (hp1 > hp2) ? 1 : 2;
    }

    private int GetHP(GameObject go)
    {
        if (go == null) return 0;
        var ph = go.GetComponent<PlayerHealth>();
        if (ph == null) return 0;

        return ph.CurrentHP;
    }


    public void ResetGame()
    {
        Debug.Log("Resetting Game");

        ClearRoundObjects();

        player1?.GetComponent<PlayerMovement>()?.ForceDetachFromPlatform();
        player2?.GetComponent<PlayerMovement>()?.ForceDetachFromPlatform();
        player1?.GetComponentInChildren<WallCheck>()?.ForceClearContacts();
        player2?.GetComponentInChildren<WallCheck>()?.ForceClearContacts();

        // --- [공통 로직] BGM 테마 결정 (플레이어 1, 2 모두 수행) ---
        int totalScore = IntScorePlayer_1 + IntScorePlayer_2;
        int newThemeIndex = 0;

        if (totalScore < 5) newThemeIndex = 0;
        else if (totalScore < 14) newThemeIndex = 1;
        else newThemeIndex = 2;

        if (newThemeIndex != currentThemeIndex)
        {
            currentThemeIndex = newThemeIndex;
            if (ingameUIController.bgmClips.Length > newThemeIndex)
            {
                ingameUIController.PlayBGM(ingameUIController.bgmClips[newThemeIndex]);
            }
        }

        // --- [호스트 로직] 맵/기믹 결정 및 패킷 전송 ---
        if (MatchResultStore.myPlayerNumber == 1)
        {
            int mapIdx = 0;
            int bgIdx = 0;

            // 테마별 맵/배경 랜덤 선택
            if (currentThemeIndex == 0)
            {
                mapIdx = Random.Range(0, 8);
                bgIdx = Random.Range(0, 4);
            }
            else if (currentThemeIndex == 1)
            {
                mapIdx = Random.Range(8, 16);
                bgIdx = Random.Range(4, 8);
            }
            else
            {
                mapIdx = Random.Range(16, 24);
                bgIdx = Random.Range(8, 10);
            }

            // 기믹 교체 로직 (5의 배수마다)
            if (totalScore >= 5)
            {
                if (totalScore % 5 == 0)
                {
                    IntMapGimicnumber = Random.Range(1, 13); // 현재 드래곤 기믹 고정
                }
                mapManager.SetMapGimicIndex(IntMapGimicnumber);
                BoolAcitveMapGimic = true;
            }
            else
            {
                BoolAcitveMapGimic = false;
                IntMapGimicnumber = 0;
            }

            int gimicToSend = BoolAcitveMapGimic ? IntMapGimicnumber : 0;

            // 상대방에게 맵 정보 전송
            P2PMessageSender.SendMessage(
                BackgroundColorMessageBuilder.Build(mapIdx, bgIdx, gimicToSend)
            );

            // 자신의 화면 적용
            ApplyBackground(mapIdx, bgIdx, gimicToSend);
        }
    }


    public void ApplyBackground(int mapIndex, int backgroundIndex, int iMapGimicNum)
    {
        StartCoroutine(ApplyBackground_(mapIndex, backgroundIndex, iMapGimicNum));
    }

    private void ResetPlayerAnimation(GameObject player)
    {
        if (!player) return;

        var anim = player.GetComponentInChildren<Animator>();
        if (anim == null) return;

        anim.SetBool("isGround", true);
        anim.SetBool("isRun", false);
        anim.SetBool("isHanging", false);
        anim.SetBool("isDash", false);
        anim.SetBool("isDamage", false);
        anim.SetBool("isAttack", false);
        anim.SetBool("isShock", false);
    }

    public IEnumerator ApplyBackground_(int mapIndex, int backgroundIndex, int iMapGimicNum)
    {
        // 최초 실행 시 페이드 인
        if (IntScorePlayer_1 == 0 && IntScorePlayer_2 == 0)
        {
            ingameUIController.OpenFadePanel_Vertical(ingameUIController.image_UpperArea.rectTransform, ingameUIController.image_LowerArea.rectTransform, 0.5f);
        }

        // 맵 및 배경 변경 (전달받은 인덱스 기반)
        mapManager.ChangeMap(mapIndex);
        mapManager.ChangeBackground(backgroundIndex);
        mapManager.SetMapGimicIndex(iMapGimicNum); // 기믹 인덱스 동기화

        // 플레이어 스폰 위치 및 체력 초기화
        var sp1 = mapManager.GetSpawnPoint(1);
        var sp2 = mapManager.GetSpawnPoint(2);

        if (player1 && sp1) player1.transform.position = sp1.position;
        if (player2 && sp2) player2.transform.position = sp2.position;

        player1.GetComponent<PlayerHealth>()?.ResetHealth();
        player2.GetComponent<PlayerHealth>()?.ResetHealth();

        ResetPlayerAnimation(player1);
        ResetPlayerAnimation(player2);

        playerAbility_1.ResetAllCooldowns();
        playerAbility_2.ResetAllCooldowns();

        ingameUIController?.StartGameTimer(61f);

        yield return new WaitForSeconds(0.6f);
        ingameUIController.scoreBoardUIController.OpenScorePanel();

        int totalScore = IntScorePlayer_1 + IntScorePlayer_2;

        // 5의 배수 점수일 때만 대기 시간 연장
        if (totalScore % 5 == 0 && totalScore > 0) yield return new WaitForSeconds(3f);
        else yield return new WaitForSeconds(1f);

        // 준비/시작 연출 및 제어권 할당
        ingameUIController.ChangeReadyStartSprite(1);
        ingameUIController.PlaySFX(ingameUIController.sfxClips_InGameSystem[2]);
        ingameUIController.PlayStartPriteAnimation(ingameUIController.image_ReadyStart.rectTransform);

        player1.GetComponent<PlayerInputRouter>()?.SetOwnership(myNum == 1);
        player2.GetComponent<PlayerInputRouter>()?.SetOwnership(myNum == 2);

        gameEnded = false;

        // 초기 바라보는 방향 설정
        player1.GetComponent<PlayerMovement>()?.LockFacing(new Vector3(1f, 0f, 0f), 0.1f);
        player2.GetComponent<PlayerMovement>()?.LockFacing(new Vector3(-1f, 0f, 0f), 0.1f);

        if (myPlayer.GetComponent<Rigidbody>() != null)
            myPlayer.GetComponent<Rigidbody>().isKinematic = false;

        myAbility.events?.EmitRoundStart(0);
        mapManager.StartCurrentGimmick(); // 동기화된 기믹 시작
    }


    public void RegisterRoundObject(GameObject go)
    {
        if (go && !roundObjects.Contains(go))
            roundObjects.Add(go);
    }

    public void ClearRoundObjects()
    {
        for (int i = 0; i < roundObjects.Count; i++)
        {
            if (roundObjects[i])
                Destroy(roundObjects[i]);
        }
        roundObjects.Clear();
    }


    public bool GetGameEnded()
    {
        return gameEnded;
    }
}