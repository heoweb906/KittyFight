using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// iSiliceCnt 값에 따라 이미지를 원형으로 조각내어 배치하는 클래스입니다.
/// 이미지 프리팹은 Image.Type.Filled, FillMethod.Radial360 (Origin: Top)을 기준으로 합니다.
/// </summary>
public class CircleSlicer : MonoBehaviour
{
    [Header("UI References")]
    public Canvas canvas; // UI 캔버스 (참조용)
    public GameObject objParent; // 생성된 조각 이미지들이 위치할 부모 오브젝트 (RectTransform)

    [Header("Prefab")]
    [Tooltip("조각으로 사용할 이미지 프리팹 (Image 컴포넌트 포함)")]
    public GameObject imagePrefab;

    [Header("Slice Settings")]
    [Tooltip("나눌 조각의 개수")]
    public int iSiliceCnt = 1;

    // 생성된 이미지 조각들을 관리하기 위한 리스트
    private List<Image> createdSlices = new List<Image>();

    /// <summary>
    /// 테스트를 위해 시작 시 iSiliceCnt 값으로 조각을 생성합니다.
    /// </summary>
    void Start()
    {
        GenerateSlices();
    }

    /// <summary>
    /// 현재 iSiliceCnt 값을 기준으로 원형 조각들을 생성하고 배치합니다.
    /// </summary>
    [ContextMenu("Generate Slices Now")] // 인스펙터에서 우클릭으로 테스트 실행 가능
    public void GenerateSlices()
    {
        // 1. 유효성 검사
        if (!ValidateInputs())
        {
            return;
        }

        // 2. 기존에 생성된 조각들 삭제
        ClearSlices();

        // 3. iSiliceCnt 값 유효성 확인
        if (iSiliceCnt <= 0)
        {
            Debug.LogWarning("iSiliceCnt (조각 개수)는 1 이상이어야 합니다.", this);
            return;
        }

        // 4. 조각별 계산
        float fillAmountPerSlice = 1.0f / iSiliceCnt; // 각 조각의 fillAmount (예: 3개면 0.333...)
        float angleStep = 360.0f / iSiliceCnt;      // 각 조각이 차지하는 각도 (예: 3개면 120도)

        // 5. 조각 개수(iSiliceCnt)만큼 이미지 생성 및 설정
        for (int i = 0; i < iSiliceCnt; i++)
        {
            // 프리팹으로부터 이미지 오브젝트 생성
            // 부모를 objParent.transform으로 지정하여 캔버스 UI 계층 구조에 맞게 생성
            GameObject newSliceObj = Instantiate(imagePrefab, objParent.transform);
            newSliceObj.name = $"Slice_{i + 1}";

            Image newSliceImage = newSliceObj.GetComponent<Image>();

            if (newSliceImage == null)
            {
                Debug.LogError($"'{imagePrefab.name}' 프리팹에 Image 컴포넌트가 없습니다. 조각 생성을 중단합니다.", imagePrefab);
                Destroy(newSliceObj); // 잘못 생성된 오브젝트 삭제
                ClearSlices(); // 이미 생성된 다른 조각들도 모두 정리
                return;
            }

            // 6. Image 컴포넌트 설정 (Filled, Radial360)
            newSliceImage.type = Image.Type.Filled;
            newSliceImage.fillMethod = Image.FillMethod.Radial360;

            // 12시 방향(Top)에서 시계방향으로 채우기 시작 (기본값)
            newSliceImage.fillOrigin = (int)Image.Origin360.Top;

            // 계산된 fillAmount 설정
            newSliceImage.fillAmount = fillAmountPerSlice;

            // 7. RectTransform 회전 설정
            RectTransform rectTransform = newSliceObj.GetComponent<RectTransform>();

            // 각 조각의 시작점이 올바른 위치에 오도록 (시계 방향으로) 회전시킴
            // 0번째 조각: 0도
            // 1번째 조각: -120도 (120도 시계방향 회전)
            // 2번째 조각: -240도 (240도 시계방향 회전)
            float zRotation = -i * angleStep;
            rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);

            // 8. (권장) 프리팹이 아닌 경우, 부모에 꽉 차도록 앵커 및 위치 초기화
            // 프리팹에 이미 설정되어 있다면 이 부분은 생략 가능합니다.
            rectTransform.localScale = Vector3.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;

            // 9. (추가된 기능) 랜덤 색상 설정
            // 알파(투명도)는 1 (불투명)으로 고정합니다.
            newSliceImage.color = new Color(
                Random.Range(0f, 1f), // R
                Random.Range(0f, 1f), // G
                Random.Range(0f, 1f), // B
                1f                    // A
            );

            // 관리 리스트에 추가
            createdSlices.Add(newSliceImage);
        }
    }

    /// <summary>
    /// 기존에 생성된 모든 조각들을 파괴하고 리스트를 비웁니다.
    /// </summary>
    public void ClearSlices()
    {
        if (createdSlices.Count > 0)
        {
            foreach (Image slice in createdSlices)
            {
                if (slice != null)
                {
                    // 에디터 모드와 런타임 모드 모두에서 안전하게 파괴
                    if (Application.isPlaying)
                    {
                        Destroy(slice.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(slice.gameObject);
                    }
                }
            }
            createdSlices.Clear();
        }
        else
        {
            // 리스트가 비어있더라도, 만약의 경우를 대비해 objParent의 자식들을 모두 정리 (안전 장치)
            foreach (Transform child in objParent.transform)
            {
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 필수 변수들이 할당되었는지 확인합니다.
    /// </summary>
    private bool ValidateInputs()
    {
        if (imagePrefab == null)
        {
            Debug.LogError("Image Prefab이 할당되지 않았습니다. 인스펙터에서 설정해주세요.", this);
            return false;
        }
        if (objParent == null)
        {
            Debug.LogError("Obj Parent가 할당되지 않았습니다. 인스펙터에서 설정해주세요.", this);
            return false;
        }
        if (canvas == null)
        {
            // 캔버스는 Instantiate 시점에 부모(objParent)를 따라가므로 치명적 오류는 아님
            Debug.LogWarning("Canvas 참조가 없습니다. (UI 스케일링에 영향을 줄 수 있음)", this);
        }
        return true;
    }
}