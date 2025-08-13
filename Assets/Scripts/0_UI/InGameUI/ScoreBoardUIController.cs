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
    public Sprite[] spritesPlayerCat;

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
            rectTransform_PlayerImage.anchoredPosition = initialPlayerImagePosition;
    }


    public void ChangePlayerImage(int iActionNum = 0)
    {
        switch (iActionNum)
        {
            case 1:
                imagePlayerCat.sprite = spritesPlayerCat[0];
                break;
            case 2:
                imagePlayerCat.sprite = spritesPlayerCat[1];
                break;
            case 3:
                imagePlayerCat.sprite = spritesPlayerCat[2];
                break;
            default:
                break;
        }
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


    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;

        scoreImageElement_Player1.StoreInitialPosition();
        scoreImageElement_Player2.StoreInitialPosition();

        OpenScorePanel();

        // OnOffCheering(false);
    }


    // #. Score 패널 닫기
    public void CloseScorePanel(int iWinnerPlayerNum, int iWinnerPlayerScore)
    {
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
    }


    // #. Score 패널 열기
    public void OpenScorePanel()
    {
        // InGameUIController의 canvasMain 사용
        RectTransform canvasRect = InGameUiController.canvasMain.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float imageWidth = scoreImageElement_Player1.rectTransform_BackGround.rect.width;
        float leftTargetX = -(canvasWidth / 2f) - (imageWidth / 2f);
        float rightTargetX = (canvasWidth / 2f) + (imageWidth / 2f);

        scoreImageElement_Player1.rectTransform_BackGround.DOAnchorPosX(leftTargetX, 0.2f).SetEase(Ease.InQuint);
        scoreImageElement_Player2.rectTransform_BackGround.DOAnchorPosX(rightTargetX, 0.2f).SetEase(Ease.InQuint)
            .OnComplete(() => {
                scoreImageElement_Player1.ResetToInitialPosition();
                scoreImageElement_Player2.ResetToInitialPosition();

                scoreImageElement_Player1.ChangePlayerImage(1);
                scoreImageElement_Player2.ChangePlayerImage(1);

                scoreImageElement_Player1.objMine.SetActive(false);
                scoreImageElement_Player2.objMine.SetActive(false);

                OnOffCheering(false);
            });
    }


    // #. 플레이어 이미지 2개 동시에 움직이게 하는 함수
    public void MovePlayerImage(int iWinnerPlayerNum = 0)
    {
        if (iWinnerPlayerNum == 1)
        {
            scoreImageElement_Player1.ChangePlayerImage(2);
            scoreImageElement_Player2.ChangePlayerImage(3);

            float newPosX1 = scoreImageElement_Player1.rectTransform_PlayerImage.anchoredPosition.x + 80f;
            float newPosX2 = scoreImageElement_Player2.rectTransform_PlayerImage.anchoredPosition.x + 80f;

            scoreImageElement_Player1.rectTransform_PlayerImage.DOAnchorPosX(newPosX1, fFlyAwayAimTime).SetEase(Ease.OutQuad);
            scoreImageElement_Player2.rectTransform_PlayerImage.DOAnchorPosX(newPosX2, fFlyAwayAimTime).SetEase(Ease.OutQuad);
        }
        else if (iWinnerPlayerNum == 2)
        {
            scoreImageElement_Player1.ChangePlayerImage(3);
            scoreImageElement_Player2.ChangePlayerImage(2);

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
        float throwDistanceX = (iWinnerPlayerNum == 1) ? Random.Range(-800f, -600f) : Random.Range(-800f, -600f);
        float throwDistanceY = Random.Range(-100f, 200f);

        float newPosX = boneToThrow.anchoredPosition.x + throwDistanceX;
        float newPosY = boneToThrow.anchoredPosition.y + throwDistanceY;

        // 랜덤 회전 (360도 * 1~3바퀴)
        float randomRotation = Random.Range(200f, 400f);
        if (Random.Range(0, 2) == 0) randomRotation = -randomRotation; // 50% 확률로 반대 방향

        boneToThrow.DOAnchorPos(new Vector2(newPosX, newPosY), fFlyAwayAimTime + 1f).SetEase(Ease.OutQuad);
        boneToThrow.DORotate(new Vector3(0, 0, randomRotation), fFlyAwayAimTime + 1f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
    }

    public void ActiveFalseBones(int iExceptionPlayerNum = 0)
    {
        int player1DisableCount = InGameUiController.gameManager.iPlayerScore_2;

        if (iExceptionPlayerNum == 1) player1DisableCount--;

        for (int i = 0; i < player1DisableCount; i++)
        {
            if (i < scoreImageElement_Player1.rectTransform_Bones.Length && scoreImageElement_Player1.rectTransform_Bones[i] != null)
            {
                scoreImageElement_Player1.rectTransform_Bones[i].gameObject.SetActive(false);
            }
        }

        int player2DisableCount = InGameUiController.gameManager.iPlayerScore_1;

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


}