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

    // 외부에서 읽을 수 있도록 public으로 변경하거나 프로퍼티 사용
    public Vector2 InitialPosition { get; private set; }

    public void StoreInitialPosition()
    {
        if (rectTransform_BackGround != null)
            InitialPosition = rectTransform_BackGround.anchoredPosition;
    }

    // 즉시 복구용 (필요시 사용)
    public void ResetToInitialPosition()
    {
        if (rectTransform_BackGround != null)
            rectTransform_BackGround.anchoredPosition = InitialPosition;
    }
}

public class MapBoardController : MonoBehaviour
{
    public InGameUIController InGameUiController { get; set; }

    [Header("실제로 사용할 패널들")]
    public C_MapBoardElement mapBoardElement_Upper;
    public C_MapBoardElement mapBoardElement_Lower;
    public TMP_Text text_MapGimic;

    [Header("각 동물별 mapBoard image")]
    public Sprite[] spritesUpper;
    public Sprite[] spritesLower;

    // 이동 거리 상수
    private const float MOVE_DISTANCE = 1000f;
    private const float TWEEN_DURATION = 0.3f; // 0.15f -> 0.3f (조금 더 부드럽게)

    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;

        // 1. 시작 시 초기 위치 저장 (화면 중앙 등, 현재 배치된 위치)
        mapBoardElement_Upper.StoreInitialPosition();
        mapBoardElement_Lower.StoreInitialPosition();

        text_MapGimic.text = "";

        // 시작하자마자 열어두기 (화면 밖으로 치우기)
        // 만약 처음에 닫혀있어야 한다면 이 줄 주석 처리
        OpenMapBoardPanelVertical();
    }


    // #. 패널 열기 (초기 위치 기준 위/아래로 1000만큼 이동)
    public void OpenMapBoardPanelVertical()
    {
        if (mapBoardElement_Upper.rectTransform_BackGround == null || mapBoardElement_Lower.rectTransform_BackGround == null) return;

        // 1. 상단 패널: 초기 위치 + 1000 (위로)
        mapBoardElement_Upper.rectTransform_BackGround.DOKill();
        mapBoardElement_Upper.rectTransform_BackGround
          .DOAnchorPosY(mapBoardElement_Upper.InitialPosition.y + MOVE_DISTANCE, TWEEN_DURATION)
          .SetEase(Ease.InQuint)
          .SetUpdate(true)
          .SetId("MapBoard");

        // 2. 하단 패널: 초기 위치 - 1000 (아래로)
        mapBoardElement_Lower.rectTransform_BackGround.DOKill();
        mapBoardElement_Lower.rectTransform_BackGround
          .DOAnchorPosY(mapBoardElement_Lower.InitialPosition.y - MOVE_DISTANCE, TWEEN_DURATION)
          .SetEase(Ease.InQuint)
          .SetUpdate(true)
          .SetId("MapBoard")
          .OnComplete(() =>
          {
              // 애니메이션 끝난 후 비활성화 최적화
              mapBoardElement_Upper.objMine.SetActive(false);
              mapBoardElement_Lower.objMine.SetActive(false);
          });
    }

    // #. 패널 닫기 (초기 위치로 복귀)
    public void CloseMapBoardPanelVertical(int iAnimalNum = 0)
    {
        // 1. 이미지 및 텍스트 갱신
        if (iAnimalNum > 0)
        {
            iAnimalNum--; // 1-based index 보정인 경우
            ChangeImage_MapBoard(iAnimalNum);
        }

        // 2. 오브젝트 활성화 (이동 전 켜야 보임)
        mapBoardElement_Upper.objMine.SetActive(true);
        mapBoardElement_Lower.objMine.SetActive(true);

        // 3. 상단 패널: 초기 위치로 이동
        mapBoardElement_Upper.rectTransform_BackGround.DOKill();
        mapBoardElement_Upper.rectTransform_BackGround
          .DOAnchorPosY(mapBoardElement_Upper.InitialPosition.y, TWEEN_DURATION)
          .SetEase(Ease.OutQuart)
          .SetUpdate(true)
          .SetId("MapBoard");

        // 4. 하단 패널: 초기 위치로 이동
        mapBoardElement_Lower.rectTransform_BackGround.DOKill();
        mapBoardElement_Lower.rectTransform_BackGround
          .DOAnchorPosY(mapBoardElement_Lower.InitialPosition.y, TWEEN_DURATION)
          .SetEase(Ease.OutQuart)
          .SetUpdate(true)
          .SetId("MapBoard");
    }

    public void ChangeImage_MapBoard(int iindex = 0)
    {
        // 상단 이미지 교체
        if (spritesUpper != null && iindex >= 0 && iindex < spritesUpper.Length)
        {
            if (mapBoardElement_Upper.image_board != null)
                mapBoardElement_Upper.image_board.sprite = spritesUpper[iindex];
        }

        // 하단 이미지 교체
        if (spritesLower != null && iindex >= 0 && iindex < spritesLower.Length)
        {
            if (mapBoardElement_Lower.image_board != null)
                mapBoardElement_Lower.image_board.sprite = spritesLower[iindex];
        }

        // 텍스트 교체
        if (text_MapGimic != null)
        {
            if (iindex >= 0 && iindex < mapDescriptions.Length)
                text_MapGimic.text = mapDescriptions[iindex];
            else
                text_MapGimic.text = "";
        }
    }

    private readonly string[] mapDescriptions = new string[]
    {
    "All players' HP is halved.",           // 0
        "Beware of the Earthquake!",            // 1
        "Fight fair and square up close!",      // 2
        "Let's jump even higher!",              // 3
        "Beware of the Dragon's Rage!",         // 4
        "Poison is going to hurt... a lot.",    // 5
        "Hurry! Faster! Faster!",               // 6
        "The walls and floors are now fluffy.", // 7
        "Players swap positions!",              // 8
        "Cloudy with a chance of Eggs!",        // 9
        "Stronger! Hit harder!",                // 10
        "Grab your emergency food!"             // 11
      };

    public void ChangeDescriptionTest_MapBoard(string sDescription)
    {
        if (mapBoardElement_Lower.text_DescriptionMap != null)
            mapBoardElement_Lower.text_DescriptionMap.text = sDescription;
    }
}