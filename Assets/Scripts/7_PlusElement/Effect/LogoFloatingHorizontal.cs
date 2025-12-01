using UnityEngine;
using DG.Tweening;

public class LogoFloatingHorizontal : MonoBehaviour
{
    [Tooltip("좌우로 움직일 거리")]
    public float floatDistance = 0.5f; // (월드/로컬 유닛 기준)

    [Tooltip("한쪽 방향으로 움직이는 데 걸리는 시간")]
    public float duration = 1.5f;

    private float startX;

    void Start()
    {
        // 1. 시작 X 좌표 저장 (localPosition 기준)
        startX = transform.localPosition.x;

        // 2. 목표 X 좌표 계산
        float targetX = startX + floatDistance;

        // 3. 트윈 실행: DOLocalMoveX 사용
        transform.DOLocalMoveX(targetX, duration)
            .SetEase(Ease.InOutSine)  // 부드러운 움직임
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        // 4. 오브젝트 파괴 시 트윈 정리
        transform.DOKill();
    }
}