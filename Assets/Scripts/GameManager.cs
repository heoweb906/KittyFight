using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [Header("카메라")]
    public CameraManager cameraManager;


    [Header("배경")]
    public GameObject backgroundPlane;

    
    [Header("기본 스킬 프리팹 (Skill 컴포넌트 포함)")]
    public GameObject meleeSkillPrefab;
    public GameObject rangedSkillPrefab;
    public GameObject dashPrefab;

    private GameObject player1;
    private GameObject player2;
    private int myNum;
    private bool gameEnded = false;
    private PlayerAbility myAbility;

    [Header("양측 플레이어 Ability 참조")]
    public PlayerAbility playerAbility_1;
    public PlayerAbility playerAbility_2;
    // #. 양측 플레이어 점수
    public int IntScorePlayer_1 {  get; set; }
    public int IntScorePlayer_2 {  get; set; }


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
        P2PManager.Init(MatchResultStore.myPort, MatchResultStore.udpClient, this);
        P2PManager.ConnectToOpponent(MatchResultStore.opponentIp, MatchResultStore.opponentPort);

        IntScorePlayer_1 = 0;
        IntScorePlayer_2 = 0;
    }

    private void Update()
    {
        if (P2PManager.IsReadyToStartGame)
        {
            P2PManager.IsReadyToStartGame = false;
            InitializeGame();
        }

        ingameUIController?.TickGameTimer();
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
        GameObject myPlayer = Instantiate(myPlayerPrefab, mySpawn.position, Quaternion.identity);
        GameObject opponentPlayer = Instantiate(opponentPlayerPrefab, opponentSpawn.position, Quaternion.identity);

        // player1 / player2 참조 정리 (화면 좌/우 고정 관점 유지)
        player1 = (myNum == 1) ? myPlayer : opponentPlayer;
        player2 = (myNum == 1) ? opponentPlayer : myPlayer;

        // 입력 권한
        myPlayer.GetComponent<PlayerInputRouter>()?.SetOwnership(true);
        opponentPlayer.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        // 상대 물리 동기화: 원격 대상은 Kinematic
        var oppRb = opponentPlayer.GetComponent<Rigidbody>();
        if (oppRb != null) oppRb.isKinematic = true;

        // Ability / Health 번호 세팅 (권위 일원화)
        myAbility = myPlayer.GetComponent<PlayerAbility>();
        var oppAbility = opponentPlayer.GetComponent<PlayerAbility>();

        if (myAbility != null) myAbility.playerNumber = myNum;
        if (oppAbility != null) oppAbility.playerNumber = (myNum == 1) ? 2 : 1;

        // 기본 스킬 장착 (근접/원거리)
        EquipDefaultSkills(myAbility);
        EquipDefaultSkills(oppAbility);

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
        ingameUIController?.StartGameTimer(90f);

        // 핸들러 등록
        P2PMessageDispatcher.RegisterHandler(new P2PStateHandler(opponentPlayer, myNum));
        P2PMessageDispatcher.RegisterHandler(new DamageHandler(opponentPlayer.GetComponent<PlayerHealth>(), myNum));
        P2PMessageDispatcher.RegisterHandler(new BackgroundColorHandler(this));
        P2PMessageDispatcher.RegisterHandler(new SkillExecuteHandler(oppAbility, myNum));

        P2PMessageDispatcher.RegisterHandler(new P2PSkillSelectHandler(oppAbility, ingameUIController.skillCardController, myNum));
        P2PMessageDispatcher.RegisterHandler(new P2PSkillShowHandler(ingameUIController.skillCardController, myNum));
        

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

    public void EndGame(int iLosePlayerNum)
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log("Game Over");

        player1.GetComponent<PlayerInputRouter>()?.SetOwnership(false);
        player2.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        StartCoroutine(EndGameSequence(iLosePlayerNum));
    }

    private IEnumerator EndGameSequence(int iLosePlayerNum)
    {
        yield return new WaitForSeconds(1f);

        int winnerPlayerNum = iLosePlayerNum == 1 ? 2 : 1;
        ingameUIController?.ComeToTheEndGame(winnerPlayerNum);

        yield return new WaitForSeconds(0.5f);

        ResetGame();
    }


    private void ResetGame()
    {
        Debug.Log("Resetting Game");


        if (MatchResultStore.myPlayerNumber == 1)
        {
            int index = Random.Range(0, backgroundColorCandidates.Length);
            Color selectedColor = backgroundColorCandidates[index];

            P2PMessageSender.SendMessage(
                BackgroundColorMessageBuilder.Build(selectedColor)
            );
            ApplyBackgroundColor(selectedColor);
        }



        if (player1 != null) player1.transform.position = spawnPoint1.position;
        if (player2 != null) player2.transform.position = spawnPoint2.position;
        player1.GetComponent<PlayerHealth>()?.ResetHealth();
        player2.GetComponent<PlayerHealth>()?.ResetHealth();
        player1.GetComponent<PlayerInputRouter>()?.SetOwnership(myNum == 1);
        player2.GetComponent<PlayerInputRouter>()?.SetOwnership(myNum == 2);
        gameEnded = false;
        ingameUIController?.StartGameTimer(90f);

        myAbility.events?.EmitRoundStart(0);
    }


    public void ApplyBackgroundColor(Color color)
    {
        if (backgroundPlane == null) return;

        Renderer rend = backgroundPlane.GetComponent<Renderer>();
        if (rend == null) return;

        if (rend.material.HasProperty("_BaseColor"))
            rend.material.SetColor("_BaseColor", color);
        else if (rend.material.HasProperty("_Color"))
            rend.material.SetColor("_Color", color);
    }
}