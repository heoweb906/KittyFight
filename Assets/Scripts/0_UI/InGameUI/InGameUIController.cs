using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

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
    

    [Header("관리하는 UI 컨트롤러들")]
    public SkillCardController skillCardController;
    public ScoreBoardUIController scoreBoardUIController;

    [Header("연출용")]
    public Image image_FadeOut_White;


    // #. 테스트용 변수들
    public int iPlayer1Score;     // 테스트용 나중에 삭제 요망
    public int iPlayer2Score;     // 테스트용 나중에 삭제 요망

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


    
        iPlayer1Score = 0;
        iPlayer2Score = 0;
    }




    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y) && MatchResultStore.myPlayerNumber == 2)
        {
            skillCardController.ShowSkillCardList(2);
            P2PMessageSender.SendMessage(
                BasicBuilder.Build(MatchResultStore.myPlayerNumber, "[SKILL_SHOW]"));

        }


        if (Input.GetKeyDown(KeyCode.J))
        {
            ComeToTheEndGame(1);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ComeToTheEndGame(2);
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            skillCardController.ShowSkillCardList(2);
        }



    }




    public void StartGameTimer(float duration)
    {
        gameTimer?.SetDuration(duration);
    }

    public void TickGameTimer()
    {
        if (gameTimer != null && gameTimer.Tick(Time.deltaTime))
        {
            GameObject.FindObjectOfType<GameManager>()?.EndGame();
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

    // 게임 시작 시 각 UI 위젯에 abilityRef/slot 할당 (GameManager에서 호출)
    public void WireSkillUIs(PlayerAbility player1Ability, PlayerAbility player2Ability)
    {
        // Player1 (왼쪽)
        hpUI_Player1?.Bind(player1Ability);
        if (skillUI_Player1 != null) { skillUI_Player1.abilityRef = player1Ability; skillUI_Player1.slot = SkillType.Melee; }
        if (skillUI2_Player1 != null) { skillUI2_Player1.abilityRef = player1Ability; skillUI2_Player1.slot = SkillType.Ranged; }
        if (skillUI3_Player1 != null) { skillUI3_Player1.abilityRef = player1Ability; skillUI3_Player1.slot = SkillType.Dash; }
        if (skillUI4_Player1 != null) { skillUI4_Player1.abilityRef = player1Ability; skillUI4_Player1.slot = SkillType.Skill1; }
        if (skillUI5_Player1 != null) { skillUI5_Player1.abilityRef = player1Ability; skillUI5_Player1.slot = SkillType.Skill2; }

        // Player2 (오른쪽)
        hpUI_Player2?.Bind(player2Ability);
        if (skillUI_Player2 != null) { skillUI_Player2.abilityRef = player2Ability; skillUI_Player2.slot = SkillType.Melee; }
        if (skillUI2_Player2 != null) { skillUI2_Player2.abilityRef = player2Ability; skillUI2_Player2.slot = SkillType.Ranged; }
        if (skillUI3_Player2 != null) { skillUI3_Player2.abilityRef = player2Ability; skillUI3_Player2.slot = SkillType.Dash; }
        if (skillUI4_Player2 != null) { skillUI4_Player2.abilityRef = player2Ability; skillUI4_Player2.slot = SkillType.Skill1; }
        if (skillUI5_Player2 != null) { skillUI5_Player2.abilityRef = player2Ability; skillUI5_Player2.slot = SkillType.Skill2; }
    }










    // #. 허재승이 추가한 함수들

    public void ComeToTheEndGame(int winnerPlayerNum)
    {
        scoreBoardUIController.CloseScorePanel(winnerPlayerNum, winnerPlayerNum == 1 ? ++iPlayer1Score : ++iPlayer2Score);
        scoreBoardUIController.OnOffCheering(true);

        int iLosePlayerNum = winnerPlayerNum == 1 ? 2 : 1;

        StartCoroutine(OpenScorePanelAfterDelay(iLosePlayerNum));
    }

    private IEnumerator OpenScorePanelAfterDelay(int iLosePlyerNum)
    {
        yield return new WaitForSeconds(2f);

        int currentScore = iLosePlyerNum == 1 ? iPlayer2Score : iPlayer1Score;

        if (currentScore % 2 == 0)
        {
            MovePlayerImageToCenter(iLosePlyerNum);
            yield return new WaitForSeconds(float.MaxValue);
        }

        scoreBoardUIController.OpenScorePanel();
    }

    // #. 패배한 플레이어를 화면 중앙으로 오도록 배치하는 함수
    private void MovePlayerImageToCenter(int playerNum)
    {
        scoreBoardUIController.OnOffCheering(false);

        C_ScoreImageElement targetPlayer = playerNum == 1 ?
            scoreBoardUIController.scoreImageElement_Player1 :
            scoreBoardUIController.scoreImageElement_Player2;

        Vector2 targetPlayerImagePos = targetPlayer.rectTransform_PlayerImage.anchoredPosition;
        Vector2 targetBackgroundPos = targetPlayer.rectTransform_BackGround.anchoredPosition;

        float playerImageWorldPosX = targetBackgroundPos.x + targetPlayerImagePos.x;
        float offsetX = -playerImageWorldPosX;

        scoreBoardUIController.scoreImageElement_Player1.rectTransform_BackGround.DOAnchorPosX(
     scoreBoardUIController.scoreImageElement_Player1.rectTransform_BackGround.anchoredPosition.x + offsetX, 0.95f)
     .SetEase(Ease.OutQuart);
        scoreBoardUIController.scoreImageElement_Player2.rectTransform_BackGround.DOAnchorPosX(
           scoreBoardUIController.scoreImageElement_Player2.rectTransform_BackGround.anchoredPosition.x + offsetX, 0.95f)
           .SetEase(Ease.OutQuart)
                    .OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(0.8f, () =>
                        {
                            int loserPlayerNum = playerNum == 1 ? 2 : 1;
                            scoreBoardUIController.ActiveFalseBones();

                            // 승자의 점수에 따라 액티브/패시브 결정
                            int winnerScore = playerNum == 1 ? iPlayer1Score : iPlayer2Score;
                            bool bActivePassive = (winnerScore == 2 || winnerScore == 6); // 2,6점: 액티브, 4,8,10점: 패시브

                            skillCardController.ShowSkillCardList(loserPlayerNum, bActivePassive);
                        });
                    });
    }
}