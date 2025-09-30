using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening.Core.Easing;

public class GameManager : MonoBehaviour
{
    [Header("�÷��̾� ������")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;

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

    private GameObject player1;
    private GameObject player2;
    private int myNum;
    private bool gameEnded = false;
    private PlayerAbility myAbility;

    [Header("���� �÷��̾� Ability ����")]
    public PlayerAbility playerAbility_1;
    public PlayerAbility playerAbility_2;

    // #. ���� �÷��̾� ����
    public int IntScorePlayer_1 {  get; set; }
    public int IntScorePlayer_2 {  get; set; }

    // #. �� ��� Ȱ�� -> ���߿� �и��� ���� ����
    public int IntMapGimicnumber { get; set; }      // ���� ����Ǿ� �ִ� �� �̱� ��ȣ
    public bool BoolAcitveMapGimic { get; set; }    // ���� �� ����� ����Ǿ� ����
    private bool isInitialSetupDone = false;        // ���� ���� �Ϸ� ���� �÷���


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

        P2PMessageDispatcher.RegisterHandler(new BackgroundColorHandler(this));

        IntScorePlayer_1 = 0;
        IntScorePlayer_2 = 0;

        IntMapGimicnumber = 0;
        BoolAcitveMapGimic = false;
    }

    private void Update()
    {
        if (P2PManager.IsReadyToStartGame && !isInitialSetupDone)
        {
            myNum = MatchResultStore.myPlayerNumber;

            if (myNum == 1)
            {
                int initialMapIndex = Random.Range(0, mapManager.mapObjects.Length);
                int initialBgIndex = Random.Range(0, mapManager.backgroundSprites.Length);

                string message = BackgroundColorMessageBuilder.Build(initialMapIndex, initialBgIndex);
                P2PMessageSender.SendMessage(message);

                InitializeGame(initialMapIndex, initialBgIndex);
            }
        }

        ingameUIController?.TickGameTimer();
    }
    public void OnReceiveSetupMessage(BackgroundColorMessage data)
    {
        if (!isInitialSetupDone) // ���� �ʱ�ȭ ���̶��, '���� ����'���� ����
        {
            myNum = MatchResultStore.myPlayerNumber;
            InitializeGame(data.mapIndex, data.backgroundIndex);
        }
        else // �̹� �ʱ�ȭ�� �����ٸ�, '���� ����'���� ����
        {
            ApplyResetData(data.mapIndex, data.backgroundIndex, data.iMapGimicNum);
        }
    }

    private void InitializeGame(int mapIndex, int bgIndex)
    {
        if (isInitialSetupDone) return;
        isInitialSetupDone = true;

        mapManager.ChangeMap(mapIndex);
        mapManager.ChangeBackground(bgIndex);

        myNum = MatchResultStore.myPlayerNumber;

        // ������/���� ����
        GameObject myPlayerPrefab = (myNum == 1) ? player1Prefab : player2Prefab;
        GameObject opponentPlayerPrefab = (myNum == 1) ? player2Prefab : player1Prefab;

        Transform mySpawn = mapManager.GetSpawnPoint(myNum);
        Transform opponentSpawn = mapManager.GetSpawnPoint(myNum == 1 ? 2 : 1);

        // Instantiate
        GameObject myPlayer = Instantiate(myPlayerPrefab, mySpawn.position, mySpawn.rotation);
        GameObject opponentPlayer = Instantiate(opponentPlayerPrefab, opponentSpawn.position, opponentSpawn.rotation);

        // player1 / player2 ���� ���� (ȭ�� ��/�� ���� ���� ����)
        player1 = (myNum == 1) ? myPlayer : opponentPlayer;
        player2 = (myNum == 1) ? opponentPlayer : myPlayer;

        // �Է� ����
        myPlayer.GetComponent<PlayerInputRouter>()?.SetOwnership(true);
        opponentPlayer.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        // ��� ���� ����ȭ: ���� ����� Kinematic
        var oppRb = opponentPlayer.GetComponent<Rigidbody>();
        if (oppRb != null) oppRb.isKinematic = true;

        // Ability / Health ��ȣ ���� (���� �Ͽ�ȭ)
        myAbility = myPlayer.GetComponent<PlayerAbility>();
        var oppAbility = opponentPlayer.GetComponent<PlayerAbility>();

        if (myAbility != null) myAbility.playerNumber = myNum;
        if (oppAbility != null) oppAbility.playerNumber = (myNum == 1) ? 2 : 1;

        // ���� ���� ä���α�
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

        // UI ���̾ (��ٿ��� Ability pull ���)
        ingameUIController?.WireSkillUIs(playerAbility_1, playerAbility_2);
        ingameUIController?.StartGameTimer(60f);

        // �⺻ ��ų ���� (����/���Ÿ�)
        EquipDefaultSkills(myAbility);
        EquipDefaultSkills(oppAbility);

        // �ڵ鷯 ���
        P2PMessageDispatcher.RegisterHandler(new P2PStateHandler(opponentPlayer, myNum));
        P2PMessageDispatcher.RegisterHandler(new DamageHandler(opponentPlayer.GetComponent<PlayerHealth>(), myNum));
        P2PMessageDispatcher.RegisterHandler(new SkillExecuteHandler(oppAbility, myNum));

        P2PMessageDispatcher.RegisterHandler(new P2PSkillSelectHandler(oppAbility, ingameUIController.skillCardController, myNum));
        P2PMessageDispatcher.RegisterHandler(new P2PSkillShowHandler(ingameUIController.skillCardController, myNum));
        

        // ���� ����ȭ ����
        updateManager?.Initialize(myPlayer, opponentPlayer, myNum);
        if (updateManager != null) updateManager.enabled = true;
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

        //yield return new WaitForSeconds(0.5f);
        //ResetGame();
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

        if (myNum == 1)
        {
            int nextMapIndex = Random.Range(0, mapManager.mapObjects.Length);
            int nextBgIndex = Random.Range(0, mapManager.backgroundSprites.Length);

            // �� ��� ó��
            int randomValue = 0;
            if ((IntScorePlayer_1 + IntScorePlayer_2) % 1 == 0 && (IntScorePlayer_1 + IntScorePlayer_2) > 0)
            {
                randomValue = Random.Range(1, 13);
                BoolAcitveMapGimic = true;
            }

            P2PMessageSender.SendMessage( 
                BackgroundColorMessageBuilder.Build(nextMapIndex, nextBgIndex, randomValue) 
            );

            ApplyResetData(nextMapIndex, nextBgIndex, randomValue);
        }

        myAbility.events?.EmitRoundStart(0);
    }

    private void ApplyResetData(int mapIdx, int bgIdx, int gimic)
    {
        mapManager.ChangeMap(mapIdx);
        mapManager.ChangeBackground(bgIdx);
        IntMapGimicnumber = gimic;
        BoolAcitveMapGimic = gimic > 0;

        if (player1 != null) player1.transform.position = mapManager.GetSpawnPoint(1).position;
        if (player2 != null) player2.transform.position = mapManager.GetSpawnPoint(2).position;

        player1.GetComponent<PlayerHealth>()?.ResetHealth();
        player2.GetComponent<PlayerHealth>()?.ResetHealth();
        player1.GetComponent<PlayerInputRouter>().SetOwnership(myNum == 1);
        player2.GetComponent<PlayerInputRouter>().SetOwnership(myNum == 2);
        gameEnded = false;
        ingameUIController?.StartGameTimer(60f);
        myAbility.events?.EmitRoundStart(0);
    }
}