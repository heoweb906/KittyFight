using UnityEngine;

public class ProjectileStretcher : MonoBehaviour
{
    [Header("연결 정보")]
    public Transform visualModel;
    public Transform colliderObject;

    [Header("모양 설정")]
    [Tooltip("전체 길이 배율 (1이면 거리만큼, 2면 거리의 2배)")]
    public float lengthMultiplier = 4f;

    [Tooltip("늘어나는 기준점 (0.5: 양쪽, 1.0: 뒤로만 늘어남, 0.0: 앞으로만 늘어남)")]
    [Range(0f, 1f)]
    public float anchorRatio = 1.0f; // [핵심] 기본값을 1.0으로 설정하여 뒤로만 늘어나게 함

    private Transform startTarget;
    private Vector3 fixedStartPos;
    private bool isLaunched = false;

    public void OnLaunch(Transform ownerTransform, Vector3 origin)
    {
        startTarget = ownerTransform;
        fixedStartPos = origin;
        isLaunched = true;

        if (visualModel != null)
        {
            // 초기화
            visualModel.localScale = new Vector3(0f, visualModel.localScale.y, visualModel.localScale.z);
            visualModel.localPosition = Vector3.zero;
        }

        if (colliderObject != null)
        {
            colliderObject.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        if (!isLaunched || visualModel == null) return;

        // 1. 거리 및 방향 계산
        Vector3 startPos = (startTarget != null) ? startTarget.position : fixedStartPos;
        Vector3 currentPos = transform.position;
        Vector3 direction = currentPos - startPos;
        float actualDistance = direction.magnitude;

        // 2. 회전 (X축이 항상 시작점을 향하도록)
        if (actualDistance > 0.001f)
        {
            transform.right = direction.normalized;
        }

        // 3. 스케일 적용 (배율 포함)
        float finalLength = actualDistance * lengthMultiplier;

        Vector3 newScale = visualModel.localScale;
        newScale.x = finalLength;
        visualModel.localScale = newScale;

        // 4. [핵심 수정] 위치 보정 (앵커 비율 적용)
        // anchorRatio가 1.0이면: -finalLength / 2 * (1 + (1-1)) ??? -> 복잡한 식 대신 직관적으로:

        // 정육면체 Mesh의 피벗이 정중앙(Center)에 있다고 가정할 때:
        // 중앙에서 길이의 절반(finalLength * 0.5)만큼 뒤로 밀면(-X) 앞면이 0에 오게 됩니다.
        // anchorRatio를 곱해서 미세 조정이 가능하게 합니다.

        // anchorRatio = 0.5 (중앙 고정) -> 오프셋 0
        // anchorRatio = 1.0 (머리 고정, 뒤로 늘어남) -> 오프셋 -Length/2

        // 공식: (0.5 - anchorRatio) * finalLength
        // 예) Ratio 1.0 대입 -> (0.5 - 1.0) * L = -0.5L (뒤로 절반만큼 이동 -> 앞면이 0에 딱 맞음)

        float xOffset = (0.5f - anchorRatio) * finalLength;
        visualModel.localPosition = new Vector3(xOffset, 0f, 0f);

        // 5. 콜라이더 유지
        if (colliderObject != null)
        {
            colliderObject.localPosition = Vector3.zero;
        }
    }
}