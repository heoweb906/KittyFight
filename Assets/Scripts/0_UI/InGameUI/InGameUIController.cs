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

    [Header("Player2 UI")]
    public PlayerHealthUI hpUI_Player2;
    public SkillCooldownUI skillUI_Player2;     // Player2 - Melee
    public SkillCooldownUI skillUI2_Player2;    // Player2 - Ranged
    // HP, DASH, Skill1~2 ���� �� ������ ����
    
    public GameTimer gameTimer;
    public GameObject blindOverlay;
    

    [Header("�����ϴ� UI ��Ʈ�ѷ���")]
    public SkillCardController skillCardController;
    public ScoreBoardUIController scoreBoardUIController;

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

    // ���� ���� �� �� UI ������ abilityRef/slot �Ҵ� (GameManager���� ȣ��)
    public void WireSkillUIs(PlayerAbility player1Ability, PlayerAbility player2Ability)
    {
        // Player1 (����)
        hpUI_Player1?.Bind(player1Ability);
        if (skillUI_Player1 != null) { skillUI_Player1.abilityRef = player1Ability; skillUI_Player1.slot = SkillType.Melee; }
        if (skillUI2_Player1 != null) { skillUI2_Player1.abilityRef = player1Ability; skillUI2_Player1.slot = SkillType.Ranged; }

        // Player2 (������)
        hpUI_Player2?.Bind(player2Ability);
        if (skillUI_Player2 != null) { skillUI_Player2.abilityRef = player2Ability; skillUI_Player2.slot = SkillType.Melee; }
        if (skillUI2_Player2 != null) { skillUI2_Player2.abilityRef = player2Ability; skillUI2_Player2.slot = SkillType.Ranged; }
    }





    // #. ������� �߰��� �Լ���

    public void ComeToTheEndGame(int iWinnerPlayerNum, int iWinnerPlayerScore)
    {
        scoreBoardUIController.CloseScorePanel(iWinnerPlayerNum, iWinnerPlayerScore);
        scoreBoardUIController.OnOffCheering(true);

        int iLosePlayerNum = iWinnerPlayerNum == 1 ? 2 : 1;

        DOVirtual.DelayedCall(2f, () =>
        {
            if (iWinnerPlayerScore % 2 == 0)
            {
                MovePlayerImageToCenter(iLosePlayerNum);
            }
            else
            {
                Debug.Log("3���� �۵��ϰ� ����");
                scoreBoardUIController.OpenScorePanel();
            }
        });
    }

    private void MovePlayerImageToCenter(int iLoseplayerNum)
    {
        scoreBoardUIController.OnOffCheering(false);
        C_ScoreImageElement targetPlayer = iLoseplayerNum == 1 ?
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
                    scoreBoardUIController.ActiveFalseBones();

                    if (iLoseplayerNum == MatchResultStore.myPlayerNumber)
                    {
                        skillCardController.ShowSkillCardList(iLoseplayerNum);
                    }
                });
            });
    }
}



// ���ӿ��� �й��� �÷��̾ �������� ��
// ���ӿ��� �й��� �÷��̾ �������� ����

// �������� ����, ��ų ī�� ����Ʈ�� �����ִ� �������� �ڽ��� �� �������� �ƴ� ��쿡�� ��ų ��ȯ ������ ���� �ȵ��� ó���ؾ� ��
// Ȥ�� ������ �г��� ������ �� �����ϴ� �Լ��� ���� �����