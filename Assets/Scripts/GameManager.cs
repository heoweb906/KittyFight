using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("�÷��̾� ������")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    [Header("���� ����Ʈ")]
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    [Header("UI/������Ʈ")]
    public InGameUIController ingameUIController;
    public UpdateManager updateManager;

    [Header("���")]
    public GameObject backgroundPlane;

    [Header("�⺻ ��ų ������ (Skill ������Ʈ ����)")]
    public GameObject meleeSkillPrefab;
    public GameObject rangedSkillPrefab;

    private GameObject player1;
    private GameObject player2;
    private int myNum;
    private bool gameEnded = false;

    [Header("���� �÷��̾� Ability ����")]
    public PlayerAbility playerAbility_1;
    public PlayerAbility playerAbility_2;

    [Header("���� �÷��̾� ����")]
    public int iPlayerScore_1;
    public int iPlayerScore_2;

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

        // ������/���� ����
        GameObject myPlayerPrefab = (myNum == 1) ? player1Prefab : player2Prefab;
        GameObject opponentPlayerPrefab = (myNum == 1) ? player2Prefab : player1Prefab;

        Transform mySpawn = (myNum == 1) ? spawnPoint1 : spawnPoint2;
        Transform opponentSpawn = (myNum == 1) ? spawnPoint2 : spawnPoint1;

        // Instantiate
        GameObject myPlayer = Instantiate(myPlayerPrefab, mySpawn.position, Quaternion.identity);
        GameObject opponentPlayer = Instantiate(opponentPlayerPrefab, opponentSpawn.position, Quaternion.identity);

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
        var myAbility = myPlayer.GetComponent<PlayerAbility>();
        var oppAbility = opponentPlayer.GetComponent<PlayerAbility>();

        if (myAbility != null) myAbility.playerNumber = myNum;
        if (oppAbility != null) oppAbility.playerNumber = (myNum == 1) ? 2 : 1;

        // �⺻ ��ų ���� (����/���Ÿ�)
        EquipDefaultSkills(myAbility);
        EquipDefaultSkills(oppAbility);

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
        ingameUIController?.StartGameTimer(90f);

        // �ڵ鷯 ���
        P2PMessageDispatcher.RegisterHandler(new P2PStateHandler(opponentPlayer, myNum));
        P2PMessageDispatcher.RegisterHandler(new DamageHandler(opponentPlayer.GetComponent<PlayerHealth>(), myNum));
        P2PMessageDispatcher.RegisterHandler(new BackgroundColorHandler(this));
        P2PMessageDispatcher.RegisterHandler(new SkillExecuteHandler(oppAbility, myNum));

        P2PMessageDispatcher.RegisterHandler(new P2PSkillSelectHandler(oppAbility, ingameUIController.skillCardController, myNum));
        P2PMessageDispatcher.RegisterHandler(new P2PSkillShowHandler(ingameUIController.skillCardController, myNum));



        iPlayerScore_1 = 0;
        iPlayerScore_2 = 0;


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
    }


    public void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log("Game Over");

        player1.GetComponent<PlayerInputRouter>()?.SetOwnership(false);
        player2.GetComponent<PlayerInputRouter>()?.SetOwnership(false);

        int winnerPlayerNum = 0;
        int iWinnerPlayerScore = 0;
        var player1Health = player1.GetComponent<PlayerHealth>();
        var player2Health = player2.GetComponent<PlayerHealth>();

        if (player1Health.CurrentHP <= 0)
        {
            iPlayerScore_2++;
            winnerPlayerNum = 2; // 1���� �׾����Ƿ� 2���� �¸�
            iWinnerPlayerScore = iPlayerScore_2;
        }
        else if (player2Health.CurrentHP <= 0)
        {
            iPlayerScore_1++;
            winnerPlayerNum = 1; // 2���� �׾����Ƿ� 1���� �¸�
            iWinnerPlayerScore = iPlayerScore_1;
        }
           

        ingameUIController.ComeToTheEndGame(winnerPlayerNum, iWinnerPlayerScore);

        Invoke(nameof(ResetGame), 1f);
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