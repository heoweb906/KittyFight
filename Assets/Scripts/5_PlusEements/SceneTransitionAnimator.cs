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

        return;

    }
}


public class SceneTransitionAnimator : MonoBehaviour
{
    public Canvas mainCanvas;

    public C_ScoreImageElement scoreImageElement_Player1;
    public C_ScoreImageElement scoreImageElement_Player2;


    public float fFlyAwayAimTime;



    // 테스트용
    private int iPlayer_1_Score = 0;
    private int iPlayer_2_Score = 0;



    private void Start()
    {
        // 각 플레이어의 초기 위치 저장
        scoreImageElement_Player1.StoreInitialPosition();
        scoreImageElement_Player2.StoreInitialPosition();
        OpenScorePanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            CloseScorePanel(1, ++iPlayer_1_Score);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            CloseScorePanel(2, ++iPlayer_2_Score);
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            OpenScorePanel();
        }
    }

    // #. Score 패널 띄우기
    public void CloseScorePanel(int iWinnerPlayerNum, int iWinnerPlayerScore)
    {
        int iLoserPlayerNum = (iWinnerPlayerNum == 1) ? 2 : 1;
        ActiveFalseBones(iLoserPlayerNum);


        RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float halfOverlap = canvasWidth / 4f;

        float leftClosedPosX = -halfOverlap;
        float rightClosedPosX = halfOverlap;

        scoreImageElement_Player1.rectTransform_BackGround.DOAnchorPosX(leftClosedPosX, 0.1f).SetEase(Ease.OutQuart);
        scoreImageElement_Player2.rectTransform_BackGround.DOAnchorPosX(rightClosedPosX, 0.1f).SetEase(Ease.OutQuart);

        if (iWinnerPlayerNum >= 1)
        {
            MovePlayerImage(iWinnerPlayerNum);
            ThrowBone(iWinnerPlayerNum, iWinnerPlayerScore);
        }


    }


    public void OpenScorePanel()
    {
        RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
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

        // 랜덤한 방향 계산
        float throwDistanceX = (iWinnerPlayerNum == 1) ? Random.Range(150f, 300f) : Random.Range(-150f, -300f);
        float throwDistanceY = Random.Range(50f, 200f);

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
        // Player1의 뼈를 Player2 점수만큼 비활성화 (예외 처리)
        int player1DisableCount = iPlayer_2_Score;
        if (iExceptionPlayerNum == 1) player1DisableCount--;

        for (int i = 0; i < player1DisableCount; i++)
        {
            if (i < scoreImageElement_Player1.rectTransform_Bones.Length && scoreImageElement_Player1.rectTransform_Bones[i] != null)
            {
                scoreImageElement_Player1.rectTransform_Bones[i].gameObject.SetActive(false);
            }
        }

        // Player2의 뼈를 Player1 점수만큼 비활성화 (예외 처리)
        int player2DisableCount = iPlayer_1_Score;
        if (iExceptionPlayerNum == 2) player2DisableCount--;

        for (int i = 0; i < player2DisableCount; i++)
        {
            if (i < scoreImageElement_Player2.rectTransform_Bones.Length && scoreImageElement_Player2.rectTransform_Bones[i] != null)
            {
                scoreImageElement_Player2.rectTransform_Bones[i].gameObject.SetActive(false);
            }
        }
    }



}