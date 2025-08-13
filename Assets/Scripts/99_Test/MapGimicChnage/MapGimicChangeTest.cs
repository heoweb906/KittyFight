using UnityEngine;
using DG.Tweening;
using TMPro;

public class MapGimicChangeTest : MonoBehaviour
{
    [Header("Spin Settings")]
    [Tooltip("몇 바퀴 추가 회전할지")]
    public int extraFullRotations = 4;
    [Tooltip("회전 및 감속에 걸리는 시간(초)")]
    public float spinDuration = 2f;
    [Tooltip("자식이 12시(위)에 올 때의 월드 Y축 각도 (보통 90°)")]
    public float topWorldAngle = 90f;

    [Header("UI")]
    [Tooltip("선택된 오브젝트 이름을 표시할 TMP 텍스트")]
    public TMP_Text textMeshPro;

    [Header("Effect Object")]
    [Tooltip("선정 완료 시 활성화할 오브젝트")]
    public GameObject obj;

    private bool isSpinning = false;

    void Update()
    {
         // if (Input.GetKeyDown(KeyCode.Y) && !isSpinning) StartSpin();

    }

    private void StartSpin()
    {
        int count = transform.childCount;
        if (count == 0) return;

        // 1) 스핀 시작 전: 모든 자식 활성화
        for (int i = 0; i < count; i++)
            transform.GetChild(i).gameObject.SetActive(true);

        // 2) 랜덤으로 몇 칸 이동할지 결정 (0 ~ count-1)
        float angleStep = 360f / count;
        int randomSteps = Random.Range(0, count);

        // 3) 목표 각도 계산: 현재 + 전체 회전 + 랜덤 스텝 * 스텝 각도
        float currentZ = transform.eulerAngles.z;
        float targetZ = currentZ
                        + extraFullRotations * 360f
                        + randomSteps * angleStep;

        // 4) DOTween으로 회전 + 감속
        isSpinning = true;
        transform
            .DORotate(new Vector3(0f, 0f, targetZ), spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .OnComplete(OnSpinComplete);
    }

    private void OnSpinComplete()
    {
        isSpinning = false;

        int count = transform.childCount;
        if (count == 0) return;

        // 1) 36° 단위로 스냅: 가장 가까운 단계로 정렬
        float angleStep = 360f / count;
        float snappedZ = Mathf.Round(transform.eulerAngles.z / angleStep) * angleStep;
        transform.rotation = Quaternion.Euler(0f, 0f, snappedZ);

        // 2) 월드 Y좌표가 가장 높은 자식 찾기
        Transform topChild = null;
        float highestY = float.MinValue;
        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            float worldY = child.position.y;
            if (worldY > highestY)
            {
                highestY = worldY;
                topChild = child;
            }
        }

        // 3) 나머지 자식 비활성화, topChild만 남김
        for (int i = 0; i < count; i++)
            transform.GetChild(i).gameObject.SetActive(transform.GetChild(i) == topChild);

        // 4) 이름 표시
        string nameToShow = topChild != null ? topChild.name : "None";
        Debug.Log($"선택된 오브젝트: {nameToShow}");
        if (textMeshPro != null)
            textMeshPro.text = nameToShow;

        // 5) 효과 오브젝트 토글 (1초 유지)
        if (obj != null)
        {
            obj.SetActive(true);
            DOVirtual.DelayedCall(1f, () => obj.SetActive(false));
        }
    }
}
