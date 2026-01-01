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
        OpenMapBoardPanelVertical(false);
    }


    // #. 패널 열기 (초기 위치 기준 위/아래로 1000만큼 이동)
    public void OpenMapBoardPanelVertical(bool bSound = true)
    {
        if (mapBoardElement_Upper.rectTransform_BackGround == null || mapBoardElement_Lower.rectTransform_BackGround == null) return;

        if(bSound) InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[1]);

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
    public void CloseMapBoardPanelVertical(int iAnimalNum = 0, bool bSound = true)
    {
        // 1. 이미지 및 텍스트 갱신
        if (iAnimalNum >= 0)
        {
            ChangeImage_MapBoard(iAnimalNum);
        }

        if(bSound) InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[1]);


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
    "Perfect Balance! HP is shared periodically.",  // 0: 체력 공평 분배 
    "Earthquake! Jump or get stunned!",             // 1: 지진 (점프 안 하면 스턴) 
    "Up close and personal! No ranged attacks.",    // 2: 근접 강제 (원거리 불가) 
    "Gravity is fading... Feel the float!",         // 3: 저중력 
    "Dodge the Dragon's Breath from all sides!",    // 4: 용의 불꽃 (사방에서 공격) 
    "Toxic Hazard! Watch where you step.",          // 5: 독 구역 생성 
    "Speed limit broken! Gotta go fast!",           // 6: 이동속도 증가 
    "High Voltage! Avoid the electric traps.",      // 7: 감전 오브젝트 (정전기) 
    "Limit break! Cooldowns reset periodically.",   // 8: 쿨타임 초기화 
    "It's raining Eggs! Cloudy with a chance of protein.", // 9: 달걀 낙하 (영화 패러디) 
    "Berserker Mode! Damage taken increased.",      // 10: 받는 피해 증가 
    "Survival time! Look for healing zones."        // 11: 회복 구역 생성 
};

    public void ChangeDescriptionTest_MapBoard(string sDescription)
    {
        if (mapBoardElement_Lower.text_DescriptionMap != null)
            mapBoardElement_Lower.text_DescriptionMap.text = sDescription;
    }
}