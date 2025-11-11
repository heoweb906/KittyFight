using UnityEngine;
using DG.Tweening;

public class LogoFloting : MonoBehaviour
{
    [Tooltip("위아래로 움직일 거리")]
    public float floatDistance = 0.5f; // (월드/로컬 유닛 기준)

    [Tooltip("한쪽 방향으로 움직이는 데 걸리는 시간")]
    public float duration = 1.5f;

    private float startY;

    void Start()
    {
        // 1. 시작 Y 좌표 저장 (localPosition 기준)
        startY = transform.localPosition.y;

        // 2. 목표 Y 좌표 계산
        float targetY = startY + floatDistance;

        // 3. 트윈 실행: DOLocalMoveY 사용
        transform.DOLocalMoveY(targetY, duration)
            .SetEase(Ease.InOutSine)  // 부드러운 움직임
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        // 4. 오브젝트 파괴 시 트윈 정리
        transform.DOKill();
    }
}