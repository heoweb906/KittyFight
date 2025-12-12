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
    public PlayerHealthHexUI hpUI_Player1;
    public SkillCooldownHexUI skillUI_Player1;     // Player1 - Melee
    public SkillCooldownHexUI skillUI2_Player1;    // Player1 - Ranged
    public SkillCooldownHexUI skillUI3_Player1;    // Player1 - Dash
    public SkillCooldownHexUI skillUI4_Player1;    // Player1 - Skill1
    public SkillCooldownHexUI skillUI5_Player1;    // Player1 - Skill2
    public SkillEffectAnimation effectPlayer1;
    public PassiveSlotUI passiveUI1_Player1; // 슬롯 0
    public PassiveSlotUI passiveUI2_Player1; // 슬롯 1


    public Image image_UpperArea;
    public Image image_LowerArea;
    public Image image_ReadyStart;
    public Sprite[] sprites_ReadyStart;


    [Header("Player2 UI")]
    public PlayerHealthHexUI hpUI_Player2;
    public SkillCooldownHexUI skillUI_Player2;     // Player2 - Melee
    public SkillCooldownHexUI skillUI2_Player2;    // Player2 - Ranged
    public SkillCooldownHexUI skillUI3_Player2;    // Player2 - Dash
    public SkillCooldownHexUI skillUI4_Player2;    // Player2 - Skill1
    public SkillCooldownHexUI skillUI5_Player2;    // Player2 - Skill2
    public SkillEffectAnimation effectPlayer2;
    public PassiveSlotUI passiveUI1_Player2; // 슬롯 0
    public PassiveSlotUI passiveUI2_Player2; // 슬롯 1

    [Header("Game UI etc")]
    public GameTimer gameTimer;
    public GameObject blindOverlay;
    

    [Header("관리하는 UI 컨트롤러들")]
    public SkillCardController skillCardController;
    public ScoreBoardUIController scoreBoardUIController;
    public MapBoardController mapBoardController;
    public FinalEndingController finalEndingController;

    [Header("연출용")]
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
        finalEndingController.Initialize(this, canvasMain.transform);
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
        skillUI_Player1?.Bind(player1Ability, SkillType.Melee);
        skillUI2_Player1?.Bind(player1Ability, SkillType.Ranged);
        skillUI3_Player1?.Bind(player1Ability, SkillType.Dash);
        skillUI4_Player1?.Bind(player1Ability, SkillType.Skill1);
        skillUI5_Player1?.Bind(player1Ability, SkillType.Skill2);
        passiveUI1_Player1?.Bind(player1Ability, 0);
        passiveUI2_Player1?.Bind(player1Ability, 1);

        // Player2 (오른쪽)
        hpUI_Player2?.Bind(player2Ability);
        skillUI_Player2?.Bind(player2Ability, SkillType.Melee);
        skillUI2_Player2?.Bind(player2Ability, SkillType.Ranged);
        skillUI3_Player2?.Bind(player2Ability, SkillType.Dash);
        skillUI4_Player2?.Bind(player2Ability, SkillType.Skill1);
        skillUI5_Player2?.Bind(player2Ability, SkillType.Skill2);
        passiveUI1_Player2?.Bind(player2Ability, 0);
        passiveUI2_Player2?.Bind(player2Ability, 1);


        if (player1Ability != null) player1Ability.effect = effectPlayer1;
        if (player2Ability != null) player2Ability.effect = effectPlayer2;
    }






    // #. 허재승이 추가한 함수들
    public void ComeToTheEndGame(int winnerPlayerNum)
    {
        scoreBoardUIController.CloseScorePanel(winnerPlayerNum, winnerPlayerNum == 1 ? ++gameManager.IntScorePlayer_1 : ++gameManager.IntScorePlayer_2);
        scoreBoardUIController.OnOffCheering(true);

        StartCoroutine(OpenScorePanelAfterDelay(winnerPlayerNum));
    }


    private IEnumerator OpenScorePanelAfterDelay(int winnerPlayerNum)
    {
        yield return new WaitForSeconds(1.2f);

        ChangeReadyStartSprite(0);
        int winPlayerCurrentScore = winnerPlayerNum == 1 ? gameManager.IntScorePlayer_1 : gameManager.IntScorePlayer_2;
        int iLosePlayerNum = winnerPlayerNum == 1 ? 2 : 1;



        if (gameManager.IntScorePlayer_1 >= 11 || gameManager.IntScorePlayer_2 >= 11)
        {
            yield return new WaitForSeconds(1.2f);
            int winnerNum = (gameManager.IntScorePlayer_1 >= 11) ? 1 : 2;
            finalEndingController.ShowFinalEnding(winnerNum);
            yield break;
        }

        // 스킬 획득 부분
        if (winPlayerCurrentScore % 2 == 0)     
        {
            MovePlayerImageToCenter(iLosePlayerNum);
            yield break; 
        }
        
       
        gameManager.ResetGame();
     
        yield return new WaitForSeconds(0.6f); 
 
        scoreBoardUIController.OpenScorePanel(); 
    }


    // #. 패배한 플레이어를 화면 중앙으로 오도록 배치하는 함수
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

                           // 패배한 플레이어만 권한을 가지고 스킬카드 생성하도록 수정
                           if (iLosePlayerNum == MatchResultStore.myPlayerNumber)
                           {
                               skillCardController.ShowSkillCardList(iLosePlayerNum, bActivePassive);
                           }
                           else
                           {
                               skillCardController.iAuthorityPlayerNum = iLosePlayerNum; 
                           }
                       });
                   });
    }



    public void ChangeReadyStartSprite(int iIdx)
    {
        // 0 = Ready;
        // 1 = Start;

        GameObject targetGO = image_ReadyStart.gameObject;
        targetGO.SetActive(true);

        image_ReadyStart.sprite = sprites_ReadyStart[iIdx];
    }



    public void PlayStartPriteAnimation(RectTransform targetRect)
    {
        if (targetRect == null) return;

        // 현재 크기를 저장 (원래 크기 역할)
        Vector3 originalScale = targetRect.localScale;

        // 이전에 실행 중이던 DOTween 애니메이션 중지
        targetRect.DOKill();

        // 트윈 동작을 순서대로 연결하여 애니메이션 흐름을 만듭니다.
        DOTween.Sequence()
            .Append(targetRect.DOScale(originalScale * 0.9f, 0.04f)) // 첫 번째 동작: 축소
            .Append(targetRect.DOScale(originalScale * 1.35f, 0.2f))  // 두 번째 동작: 확대
            .Append(targetRect.DOScale(0f, 0.15f))        // 세 번째 동작: 크기 0으로 사라짐
            .OnComplete(() => {

                targetRect.gameObject.SetActive(false);
                targetRect.localScale = originalScale;
            });
    }


    public void CloseFadePanel_Vertical(RectTransform topImage, RectTransform bottomImage, float fDuration)
    {
        topImage.gameObject.SetActive(true);
        bottomImage.gameObject.SetActive(true);
        float topImageHeight = topImage.rect.height;
        float bottomImageHeight = bottomImage.rect.height;
        float topClosedPosY = (topImageHeight / 2f);
        float bottomClosedPosY = -(bottomImageHeight / 2f);
        topImage.DOAnchorPosY(topClosedPosY, fDuration).SetEase(Ease.OutQuart);
        bottomImage.DOAnchorPosY(bottomClosedPosY, fDuration).SetEase(Ease.OutQuart);
    }

    public void OpenFadePanel_Vertical(RectTransform topImage, RectTransform bottomImage, float fDuration)
    {
        RectTransform canvasRect = canvasMain.GetComponent<RectTransform>();
        float canvasHeight = canvasRect.rect.height;
        float imageHeight = topImage.rect.height;
        float topTargetY = (canvasHeight / 2f) + (imageHeight / 2f);
        float bottomTargetY = -(canvasHeight / 2f) - (imageHeight / 2f);
        topImage.DOAnchorPosY(topTargetY, fDuration).SetEase(Ease.InQuint)
            .OnComplete(() => {
                topImage.gameObject.SetActive(false);
                bottomImage.gameObject.SetActive(false);
            });
        bottomImage.DOAnchorPosY(bottomTargetY, fDuration).SetEase(Ease.InQuint);
    }


}