using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class InGameUIController : MonoBehaviour
{
    public static InGameUIController Instance { get; private set; }

    public Canvas canvasMain;
    public GameManager gameManager;

    [Header("Player1 UI")]
    public PlayerHealthUI hpUI_Player1;
    public SkillCooldownUI skillUI_Player1;     // Player1 - Melee
    public SkillCooldownUI skillUI2_Player1;    // Player1 - Ranged
    public SkillCooldownUI skillUI3_Player1;    // Player1 - Dash
    public SkillCooldownUI skillUI4_Player1;    // Player1 - Skill1
    public SkillCooldownUI skillUI5_Player1;    // Player1 - Skill2

    [Header("Player2 UI")]
    public PlayerHealthUI hpUI_Player2;
    public SkillCooldownUI skillUI_Player2;     // Player2 - Melee
    public SkillCooldownUI skillUI2_Player2;    // Player2 - Ranged
    public SkillCooldownUI skillUI3_Player2;    // Player2 - Dash
    public SkillCooldownUI skillUI4_Player2;    // Player1 - Skill1
    public SkillCooldownUI skillUI5_Player2;    // Player1 - Skill2

    [Header("Game UI etc")]
    public GameTimer gameTimer;
    public GameObject blindOverlay;
    

    [Header("�����ϴ� UI ��Ʈ�ѷ���")]
    public SkillCardController skillCardController;
    public ScoreBoardUIController scoreBoardUIController;
    public MapBoardController mapBoardController;

    [Header("�����")]
    public Image image_FadeOut_White;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        //canvasMain = FindObjectOfType<Canvas>();
        //if (canvasMain == null) return;

        skillCardController.Initialize(this, canvasMain.transform);

        scoreBoardUIController = this.GetComponent<ScoreBoardUIController>();
        scoreBoardUIController.Initialize(this, canvasMain.transform);
        mapBoardController.Initialize(this, canvasMain.transform);
    }

    public void StartGameTimer(float duration)
    {
        gameTimer?.SetDuration(duration);
    }

    public void TickGameTimer()
    {
        if (gameTimer != null && gameTimer.Tick(Time.deltaTime))
        {
            // GameObject.FindObjectOfType<GameManager>()?.EndGame();
            FindObjectOfType<GameManager>()?.EndByTimer();
        }
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            mapBoardController.OpenMapBoardPanelVertical();
        }
    }


    public void ShowBlindOverlay(float duration)
    {
        if (blindOverlay == null) return;

        blindOverlay.SetActive(true);
        StartCoroutine(HideBlindAfterDelay(duration));
    }

    private System.Collections.IEnumerator HideBlindAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        blindOverlay.SetActive(false);
    }

    // ���� ���� �� �� UI ������ abilityRef/slot �Ҵ� (GameManager���� ȣ��)
    public void WireSkillUIs(PlayerAbility player1Ability, PlayerAbility player2Ability)
    {
        // Player1 (����)
        hpUI_Player1?.Bind(player1Ability);
        if (skillUI_Player1 != null) { skillUI_Player1.abilityRef = player1Ability; skillUI_Player1.slot = SkillType.Melee; }
        if (skillUI2_Player1 != null) { skillUI2_Player1.abilityRef = player1Ability; skillUI2_Player1.slot = SkillType.Ranged; }
        if (skillUI3_Player1 != null) { skillUI3_Player1.abilityRef = player1Ability; skillUI3_Player1.slot = SkillType.Dash; }
        if (skillUI4_Player1 != null) { skillUI4_Player1.abilityRef = player1Ability; skillUI4_Player1.slot = SkillType.Skill1; }
        if (skillUI5_Player1 != null) { skillUI5_Player1.abilityRef = player1Ability; skillUI5_Player1.slot = SkillType.Skill2; }

        // Player2 (������)
        hpUI_Player2?.Bind(player2Ability);
        if (skillUI_Player2 != null) { skillUI_Player2.abilityRef = player2Ability; skillUI_Player2.slot = SkillType.Melee; }
        if (skillUI2_Player2 != null) { skillUI2_Player2.abilityRef = player2Ability; skillUI2_Player2.slot = SkillType.Ranged; }
        if (skillUI3_Player2 != null) { skillUI3_Player2.abilityRef = player2Ability; skillUI3_Player2.slot = SkillType.Dash; }
        if (skillUI4_Player2 != null) { skillUI4_Player2.abilityRef = player2Ability; skillUI4_Player2.slot = SkillType.Skill1; }
        if (skillUI5_Player2 != null) { skillUI5_Player2.abilityRef = player2Ability; skillUI5_Player2.slot = SkillType.Skill2; }
    }






    // #. ������� �߰��� �Լ���
    public void ComeToTheEndGame(int winnerPlayerNum)
    {
        scoreBoardUIController.CloseScorePanel(winnerPlayerNum, winnerPlayerNum == 1 ? ++gameManager.IntScorePlayer_1 : ++gameManager.IntScorePlayer_2);
        scoreBoardUIController.OnOffCheering(true);

        StartCoroutine(OpenScorePanelAfterDelay(winnerPlayerNum));
    }

    private IEnumerator OpenScorePanelAfterDelay(int winnerPlayerNum)
    {
        yield return new WaitForSeconds(1.2f);

        gameManager.ResetGame();


        int winPlayerCurrentScore = winnerPlayerNum == 1 ? gameManager.IntScorePlayer_1 : gameManager.IntScorePlayer_2;
        int iLosePlayerNum = winnerPlayerNum == 1 ? 2 : 1;

        if (winPlayerCurrentScore % 2 == 0)
        {
            MovePlayerImageToCenter(iLosePlayerNum);
            yield break; 
        }


        yield return new WaitForSeconds(1f);

        scoreBoardUIController.OpenScorePanel();
    }


    // #. �й��� �÷��̾ ȭ�� �߾����� ������ ��ġ�ϴ� �Լ�
    private void MovePlayerImageToCenter(int iLosePlayerNum)
    {
        scoreBoardUIController.OnOffCheering(false);

        C_ScoreImageElement targetPlayer = iLosePlayerNum == 1 ?
            scoreBoardUIController.scoreImageElement_Player1 :
            scoreBoardUIController.scoreImageElement_Player2;

        Vector2 targetPlayerImagePos = targetPlayer.rectTransform_PlayerImage.anchoredPosition;
        Vector2 targetBackgroundPos = targetPlayer.rectTransform_BackGround.anchoredPosition;

        float playerImageWorldPosX = targetBackgroundPos.x + targetPlayerImagePos.x;
        float offsetX = -playerImageWorldPosX;

        scoreBoardUIController.scoreImageElement_Player1.rectTransform_BackGround.DOAnchorPosX(
     scoreBoardUIController.scoreImageElement_Player1.rectTransform_BackGround.anchoredPosition.x + offsetX, 0.7f)
     .SetEase(Ease.OutQuart);
        scoreBoardUIController.scoreImageElement_Player2.rectTransform_BackGround.DOAnchorPosX(
           scoreBoardUIController.scoreImageElement_Player2.rectTransform_BackGround.anchoredPosition.x + offsetX, 0.7f)
           .SetEase(Ease.OutQuart)
                   .OnComplete(() =>
                   {
                       DOVirtual.DelayedCall(1.5f, () =>
                       {
                          
                           int iWinnerScore = iLosePlayerNum == 1 ? gameManager.IntScorePlayer_2 : gameManager.IntScorePlayer_1;
                           bool bActivePassive = (iWinnerScore == 2 || iWinnerScore == 6);

                           // �й��� �÷��̾ ������ ������ ��ųī�� �����ϵ��� ����
                           if (iLosePlayerNum == MatchResultStore.myPlayerNumber)
                           {
                               skillCardController.ShowSkillCardList(iLosePlayerNum, bActivePassive);
                           }
                           else
                           {
                               // �¸��� �÷��̾�� ��� ���·� ���� (�޽��� ���� �غ�)
                               skillCardController.iAuthorityPlayerNum = iLosePlayerNum;
                           }
                       });
                   });
    }



}