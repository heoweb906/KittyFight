using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor;
using Unity.VisualScripting;

[System.Serializable]
public class C_MapBoardElement
{
    public GameObject objMine;

    public Image image_board;
    public RectTransform rectTransform_BackGround;
    public TMP_Text text_DescriptonMap;     // BoardLower���� ���� ����

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

    [Header("������ ����� �гε�")]
    public C_MapBoardElement mapBoardElement_Upper;
    public C_MapBoardElement mapBoardElement_Lower;

    [Header("�� ������ mapBoard image")]
    public Sprite[] spirtesUpper;
    public Sprite[] spirtesLower;


    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;

        mapBoardElement_Upper.StoreInitialPosition();
        mapBoardElement_Lower.StoreInitialPosition();

        OpenMapBoardPanelVertical();
    }

    public void OpenMapBoardPanelVertical()
    {
        RectTransform canvasRect = InGameUiController.canvasMain.GetComponent<RectTransform>();

        float canvasHeight = canvasRect.rect.height;
        float imageHeight = mapBoardElement_Upper.rectTransform_BackGround.rect.height;
        float topTargetY = (canvasHeight / 2f) + (imageHeight / 2f);
        float bottomTargetY = -(canvasHeight / 2f) - (imageHeight / 2f);
        mapBoardElement_Upper.rectTransform_BackGround.DOAnchorPosY(topTargetY, 0.3f).SetEase(Ease.InQuint);
        mapBoardElement_Lower.rectTransform_BackGround.DOAnchorPosY(bottomTargetY, 0.3f).SetEase(Ease.InQuint)
            .OnComplete(() => {
                mapBoardElement_Upper.objMine.SetActive(false);
                mapBoardElement_Lower.objMine.SetActive(false);
            });
    }


    // #. Score �г� ���η� �ݱ� (���Ʒ��� �߰����� �̵�)
    public void CloseMapBoardPanelVertical(int iAnimalNum = 0, string sDescription = "NULL")
    {
        iAnimalNum--;

        mapBoardElement_Upper.objMine.SetActive(true);
        mapBoardElement_Lower.objMine.SetActive(true);

        mapBoardElement_Upper.rectTransform_BackGround.DOAnchorPosY(0f, 0.5f).SetEase(Ease.InQuint);
        mapBoardElement_Lower.rectTransform_BackGround.DOAnchorPosY(0f, 0.5f).SetEase(Ease.InQuint);
    }



    public void ChangeImage_MapBoard(int iindex = 0)
    {
        if (mapBoardElement_Upper.image_board != null && spirtesUpper[iindex] != null) 
            mapBoardElement_Upper.image_board.sprite = spirtesUpper[iindex];
        else
            Debug.Log("���� ����ִ� Ȯ���ض�");

        if (mapBoardElement_Lower.image_board != null && spirtesLower[iindex] != null)
            mapBoardElement_Lower.image_board.sprite = spirtesLower[iindex];
        else
            Debug.Log("���� ����ִ� Ȯ���ض�");
    }

    public void ChangeDescriptionTest_MapBoard(string sDescription)
    {
        mapBoardElement_Lower.text_DescriptonMap.text = sDescription;
    }

    


    



}