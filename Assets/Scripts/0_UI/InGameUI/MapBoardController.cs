using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class C_MapBoardElement
{
    public GameObject objMine;
    public Image image_board;
    public RectTransform rectTransform_BackGround;
    public TMP_Text text_DescriptionMap;

    private Vector2 initialPosition;

    public void StoreInitialPosition()
    {
        if (rectTransform_BackGround != null)
            initialPosition = rectTransform_BackGround.anchoredPosition;
    }

    public void ResetToInitialPosition()
    {
        if (rectTransform_BackGround != null)
            rectTransform_BackGround.anchoredPosition = initialPosition;
    }
}

public class MapBoardController : MonoBehaviour
{
    public InGameUIController InGameUiController { get; set; }

    [Header("실제로 사용할 패널들")]
    public C_MapBoardElement mapBoardElement_Upper;
    public C_MapBoardElement mapBoardElement_Lower;

    [Header("각 동물별 mapBoard image")]
    public Sprite[] spritesUpper;
    public Sprite[] spritesLower;

    // 고정 이동 거리
    private const float UPPER_TARGET_Y = -361f;
    private const float LOWER_TARGET_Y = 360f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            int randomNum = Random.Range(1, 13);
            CloseMapBoardPanelVertical(randomNum);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            OpenMapBoardPanelVertical();
        }
    }

    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;

        mapBoardElement_Upper.StoreInitialPosition();
        mapBoardElement_Lower.StoreInitialPosition();

        OpenMapBoardPanelVertical();
    }

    // #. 패널 열기 (화면 밖으로 밀어냄)
    public void OpenMapBoardPanelVertical()
    {
        if (InGameUiController == null || InGameUiController.canvasMain == null) return;

        RectTransform canvasRect = InGameUiController.canvasMain.GetComponent<RectTransform>();
        float canvasHeight = canvasRect.rect.height;

        float topTargetY = canvasHeight;
        float bottomTargetY = -canvasHeight;

        mapBoardElement_Upper.rectTransform_BackGround
            .DOAnchorPosY(topTargetY, 0.3f)
            .SetEase(Ease.InQuint);

        mapBoardElement_Lower.rectTransform_BackGround
            .DOAnchorPosY(bottomTargetY, 0.3f)
            .SetEase(Ease.InQuint)
            .OnComplete(() =>
            {
                mapBoardElement_Upper.objMine.SetActive(false);
                mapBoardElement_Lower.objMine.SetActive(false);
            });
    }

    // #. 패널 닫기 (고정된 위치로 등장)
    public void CloseMapBoardPanelVertical(int iAnimalNum = 0, string sDescription = "NULL")
    {
        // 이미지 및 텍스트 설정
        if (iAnimalNum > 0)
        {
            iAnimalNum--;
            ChangeImage_MapBoard(iAnimalNum);
        }

        if (sDescription != "NULL")
        {
            ChangeDescriptionTest_MapBoard(sDescription);
        }

        // 오브젝트 활성화
        mapBoardElement_Upper.objMine.SetActive(true);
        mapBoardElement_Lower.objMine.SetActive(true);

        // 고정된 목표 위치 이동
        mapBoardElement_Upper.rectTransform_BackGround
            .DOAnchorPosY(UPPER_TARGET_Y, 0.5f)
            .SetEase(Ease.InQuint);

        mapBoardElement_Lower.rectTransform_BackGround
            .DOAnchorPosY(LOWER_TARGET_Y, 0.5f)
            .SetEase(Ease.InQuint);
    }

    public void ChangeImage_MapBoard(int iindex = 0)
    {
        if (spritesUpper != null && iindex >= 0 && iindex < spritesUpper.Length)
        {
            if (mapBoardElement_Upper.image_board != null)
                mapBoardElement_Upper.image_board.sprite = spritesUpper[iindex];
        }

        if (spritesLower != null && iindex >= 0 && iindex < spritesLower.Length)
        {
            if (mapBoardElement_Lower.image_board != null)
                mapBoardElement_Lower.image_board.sprite = spritesLower[iindex];
        }
    }

    public void ChangeDescriptionTest_MapBoard(string sDescription)
    {
        if (mapBoardElement_Lower.text_DescriptionMap != null)
            mapBoardElement_Lower.text_DescriptionMap.text = sDescription;
    }
}
