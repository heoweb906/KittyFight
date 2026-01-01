using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor;

[System.Serializable]
public class C_ScoreImageElement
{
    public GameObject objMine;

    public TMP_Text text_Score;
    public RectTransform rectTransform_BackGround;
    public RectTransform rectTransform_PlayerImage;
    public RectTransform[] rectTransform_Bones;

    public Image imagePlayerCat;
    public Image imageSkillIcon;
    public Sprite[] spritesPlayerCat;
    public RectTransform rectTransfrom_imageShadow;


    private Vector2 initialPlayerImagePosition;

    // #. 플레이어 위치 초기 위치 저장
    public void StoreInitialPosition()
    {
        if (rectTransform_PlayerImage != null)
            initialPlayerImagePosition = rectTransform_PlayerImage.anchoredPosition;
    }


    // #. 위치 초기화
    public void ResetToInitialPosition()
    {
        if (rectTransform_PlayerImage != null)
        {
            rectTransform_PlayerImage.DOKill();
            rectTransform_PlayerImage.anchoredPosition = initialPlayerImagePosition;
        }
    }


    public void ChangePlayerImage(int iActionNum = 0, bool bFlip = false, int iPlayerNum = 1)
    {
        Vector2 shadowPos = Vector2.zero;

        switch (iActionNum)
        {
            case 1:         // IDLE
                imagePlayerCat.sprite = spritesPlayerCat[0];
                shadowPos = new Vector2(600f, -300f);
                break;
            case 2:         // Attack
                imagePlayerCat.sprite = spritesPlayerCat[Random.Range(1, 4)];
                shadowPos = new Vector2(500f, -300f);
                break;
            case 3:         // TakeDamage
                imagePlayerCat.sprite = spritesPlayerCat[Random.Range(4, 7)];
                shadowPos = new Vector2(650f, -300f);
                break;
            case 4:         // Resurrection(부활)
                imagePlayerCat.sprite = spritesPlayerCat[7];
                shadowPos = new Vector2(550f, -300f);

                break;
            default:
                break;
        }

        if (iPlayerNum == 2)
        {
            shadowPos.x *= -1;
        }

        rectTransfrom_imageShadow.anchoredPosition = shadowPos;
        imagePlayerCat.transform.localScale = bFlip ? new Vector3(-0.5f, 0.5f, 0.5f) : new Vector3(0.5f, 0.5f, 0.5f);
    }
}

public class ScoreBoardUIController : MonoBehaviour
{

    public InGameUIController InGameUiController { get; set; }

    public C_ScoreImageElement scoreImageElement_Player1;
    public C_ScoreImageElement scoreImageElement_Player2;

    public float fFlyAwayAimTime;


    [Header("추가 효과들")]
    public C_CheeringAnimal[] cheeringAnimals;
    public TMP_Text text_Timer;

    [Header("점수판 텍스트")]
    public GameObject obj_ScorePlayer1;
    public GameObject obj_ScorePlayer2;


    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;

        scoreImageElement_Player1.StoreInitialPosition();
        scoreImageElement_Player2.StoreInitialPosition();

        scoreImageElement_Player1.imageSkillIcon.gameObject.SetActive(false);
        scoreImageElement_Player2.imageSkillIcon.gameObject.SetActive(false);

        OpenScorePanel(false);
    }




    // #. Score 패널 닫기
    public void CloseScorePanel(int iWinnerPlayerNum, int iWinnerPlayerScore, bool bSoundOn = true)
    {
        UpdateScoreText();
        SkillIconImageOnOff(false);

        scoreImageElement_Player1.objMine.SetActive(true);
        scoreImageElement_Player2.objMine.SetActive(true);

        int iLoserPlayerNum = (iWinnerPlayerNum == 1) ? 2 : 1;
        ActiveFalseBones(iLoserPlayerNum);

        RectTransform canvasRect = InGameUiController.canvasMain.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;

        float player1ImageWidth = scoreImageElement_Player1.rectTransform_BackGround.rect.width;
        float player2ImageWidth = scoreImageElement_Player2.rectTransform_BackGround.rect.width;
        float leftClosedPosX = -(player1ImageWidth / 2f); 
        float rightClosedPosX = (player2ImageWidth / 2f);

        scoreImageElement_Player1.rectTransform_BackGround.DOAnchorPosX(leftClosedPosX, 0.15f).SetEase(Ease.OutQuart);
        scoreImageElement_Player2.rectTransform_BackGround.DOAnchorPosX(rightClosedPosX, 0.15f).SetEase(Ease.OutQuart);

        if (iWinnerPlayerNum >= 1)
        {
            MovePlayerImage(iWinnerPlayerNum);
            ThrowBone(iWinnerPlayerNum, iWinnerPlayerScore);

            OnOffCheering(true);
        }


        int randomIndex = Random.Range(11, 13);

        if (InGameUiController != null && bSoundOn)
        {
            InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[randomIndex]);
        }

    }


    // #. Score 패널 열기
    public void OpenScorePanel(bool bSound = true)
    {
        text_Timer.text = "60";

        int iSumPlayerScore = InGameUiController.gameManager.IntScorePlayer_1 + InGameUiController.gameManager.IntScorePlayer_2;

        if ((iSumPlayerScore % 5 == 0 && (iSumPlayerScore) > 0))
        {
            InGameUiController.mapBoardController.CloseMapBoardPanelVertical(InGameUiController.gameManager.mapManager.GetMapGimicIndex(), false);
            
            DOVirtual.DelayedCall(1f, () => {
                StartScoreBoardAnimation(iSumPlayerScore);
            });
        }
        else
        {
            StartScoreBoardAnimation(iSumPlayerScore);
        }

        if(bSound) InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[1]);


    }

    private void StartScoreBoardAnimation(int iSumPlayerScore)
    {
        RectTransform canvasRect = InGameUiController.canvasMain.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float imageWidth = scoreImageElement_Player1.rectTransform_BackGround.rect.width;
        float leftTargetX = -(canvasWidth / 2f) - (imageWidth / 2f);
        float rightTargetX = (canvasWidth / 2f) + (imageWidth / 2f);

        scoreImageElement_Player1.rectTransform_BackGround.DOKill();
        scoreImageElement_Player2.rectTransform_BackGround.DOKill();

        scoreImageElement_Player1.rectTransform_BackGround.DOAnchorPosX(leftTargetX, 0.2f).SetEase(Ease.InQuint);
        scoreImageElement_Player2.rectTransform_BackGround.DOAnchorPosX(rightTargetX, 0.2f).SetEase(Ease.InQuint)
            .OnComplete(() => {
                scoreImageElement_Player1.ResetToInitialPosition();
                scoreImageElement_Player2.ResetToInitialPosition();
                scoreImageElement_Player1.ChangePlayerImage(1, false, 1);
                scoreImageElement_Player2.ChangePlayerImage(1, false, 1);
                scoreImageElement_Player1.objMine.SetActive(false);
                scoreImageElement_Player2.objMine.SetActive(false);
                OnOffCheering(false);

                OnOffScoreTextObj(true);

                if ((iSumPlayerScore % 5 == 0 && (iSumPlayerScore) > 0))
                {
                    DOVirtual.DelayedCall(1f, () => {
                        InGameUiController.mapBoardController.OpenMapBoardPanelVertical();
                    });
                }
            });
    }


    // #. 플레이어 이미지 2개 동시에 움직이게 하는 함수
    public void MovePlayerImage(int iWinnerPlayerNum = 0)
    {
        if (iWinnerPlayerNum == 1)
        {
            scoreImageElement_Player1.ChangePlayerImage(2, false, 1);
            scoreImageElement_Player2.ChangePlayerImage(3, false, 2);

            float newPosX1 = scoreImageElement_Player1.rectTransform_PlayerImage.anchoredPosition.x + 80f;
            float newPosX2 = scoreImageElement_Player2.rectTransform_PlayerImage.anchoredPosition.x + 80f;

            scoreImageElement_Player1.rectTransform_PlayerImage.DOAnchorPosX(newPosX1, fFlyAwayAimTime).SetEase(Ease.OutQuad);
            scoreImageElement_Player2.rectTransform_PlayerImage.DOAnchorPosX(newPosX2, fFlyAwayAimTime).SetEase(Ease.OutQuad);
        }
        else if (iWinnerPlayerNum == 2)
        {
            scoreImageElement_Player1.ChangePlayerImage(3, true, 1);
            scoreImageElement_Player2.ChangePlayerImage(2, true, 2);

            float newPosX1 = scoreImageElement_Player1.rectTransform_PlayerImage.anchoredPosition.x - 80f;
            float newPosX2 = scoreImageElement_Player2.rectTransform_PlayerImage.anchoredPosition.x - 80f;

            scoreImageElement_Player1.rectTransform_PlayerImage.DOAnchorPosX(newPosX1, fFlyAwayAimTime).SetEase(Ease.OutQuad);
            scoreImageElement_Player2.rectTransform_PlayerImage.DOAnchorPosX(newPosX2, fFlyAwayAimTime).SetEase(Ease.OutQuad);
        }
    }


    // #. 이미지 조작 관련 함수
    public void ThrowBone(int iWinnerPlayerNum, int iBoneIndex)
    {
        if (iBoneIndex < 1) return;
        C_ScoreImageElement targetPlayer = null;
        if (iWinnerPlayerNum == 1) targetPlayer = scoreImageElement_Player2;
        else if (iWinnerPlayerNum == 2) targetPlayer = scoreImageElement_Player1;
        else return;
        int arrayIndex = iBoneIndex - 1;
        if (arrayIndex >= targetPlayer.rectTransform_Bones.Length) return;
        RectTransform boneToThrow = targetPlayer.rectTransform_Bones[arrayIndex];
        if (boneToThrow == null) return;
        // 방향별로 다른 거리 설정
        float throwDistanceX = (iWinnerPlayerNum == 1) ? Random.Range(-800f, -600f) : Random.Range(-800f, 800f);
        float throwDistanceY = Random.Range(-200f, 200f);
        float newPosX = boneToThrow.anchoredPosition.x + throwDistanceX;
        float newPosY = boneToThrow.anchoredPosition.y + throwDistanceY;
        // 약한 회전 (이동 방향에 따라)
        float randomRotation = Random.Range(40f, 80f);
        if (iWinnerPlayerNum == 1) randomRotation = -randomRotation;
        boneToThrow.DOAnchorPos(new Vector2(newPosX, newPosY), fFlyAwayAimTime + 1f).SetEase(Ease.OutQuad);
        boneToThrow.DORotate(new Vector3(0, 0, randomRotation), fFlyAwayAimTime + 1f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
    }

    public void ActiveFalseBones(int iExceptionPlayerNum = 0)
    {

        int player1DisableCount = InGameUiController.gameManager.IntScorePlayer_2;

       

        if (iExceptionPlayerNum == 1) player1DisableCount--;

        for (int i = 0; i < player1DisableCount; i++)
        {
            if (i < scoreImageElement_Player1.rectTransform_Bones.Length && scoreImageElement_Player1.rectTransform_Bones[i] != null)
            {
                scoreImageElement_Player1.rectTransform_Bones[i].gameObject.SetActive(false);
            }
        }


        int player2DisableCount = InGameUiController.gameManager.IntScorePlayer_1;


        if (iExceptionPlayerNum == 2) player2DisableCount--;

        for (int i = 0; i < player2DisableCount; i++)
        {
            if (i < scoreImageElement_Player2.rectTransform_Bones.Length && scoreImageElement_Player2.rectTransform_Bones[i] != null)
            {
                scoreImageElement_Player2.rectTransform_Bones[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnOffCheering(bool bOnOff = false)
    {
        if (cheeringAnimals == null) return;
        for (int i = 0; i < cheeringAnimals.Length; i++)
        {
            if (cheeringAnimals[i] == null) continue;
            if (bOnOff)
            {
                cheeringAnimals[i].On();
            }
            else
            {
                cheeringAnimals[i].Off();
            }
        }
    }

    public void OnOffScoreTextObj(bool bActive)
    {
        obj_ScorePlayer1.SetActive(bActive);
        obj_ScorePlayer2.SetActive(bActive);
    }


    public void UpdateScoreText()
    {
        scoreImageElement_Player1.text_Score.text = InGameUiController.gameManager.IntScorePlayer_1.ToString();
        scoreImageElement_Player2.text_Score.text = InGameUiController.gameManager.IntScorePlayer_2.ToString();
    }


    public void SkillIconImageOnOff(bool bActive)
    {

        scoreImageElement_Player1.imageSkillIcon.gameObject.SetActive(bActive);
        scoreImageElement_Player2.imageSkillIcon.gameObject.SetActive(bActive);


    }

}