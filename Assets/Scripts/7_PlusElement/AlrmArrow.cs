using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AlrmArrow : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float autoHideDelay;  // 자동 실행 대기 시간 (10초)
    [SerializeField] private float scaleUpMultiplier = 1.3f;
    [SerializeField] private float scaleUpDuration = 0.2f;
    [SerializeField] private float scaleDownDuration = 0.3f;
    [SerializeField] private Ease scaleUpEase = Ease.OutBack; // 이전 대화 반영 (자연스럽게)
    [SerializeField] private Ease scaleDownEase = Ease.InBack; // 이전 대화 반영 (자연스럽게)

    private Vector3 originalScale;
    private Sequence currentSequence;

    private Transform targetTransform;
    private Vector3 offset;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // 1. 크기 초기화
        transform.localScale = originalScale;

        // 2. 10초 뒤에 PlayDisappearAnimation 실행 예약
        // SetLink(gameObject): 이 오브젝트가 10초 전에 꺼지거나 파괴되면 타이머도 자동으로 취소됨
        DOVirtual.DelayedCall(autoHideDelay, PlayDisappearAnimation).SetLink(gameObject);
    }

    private void LateUpdate()
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position + offset;
        }
    }


    public void SetTarget(Transform target, Vector3 offsetVector)
    {
        targetTransform = target;
        offset = offsetVector; // 입력받은 XYZ 오프셋(Z포함)을 그대로 사용
    }

    // Update 함수 제거함 (키 입력 삭제)

    public void PlayDisappearAnimation()
    {
        if (currentSequence != null && currentSequence.IsActive())
        {
            currentSequence.Kill();
        }

        transform.localScale = originalScale;
        currentSequence = DOTween.Sequence();

        // 커지기
        currentSequence.Append(
            transform.DOScale(originalScale * scaleUpMultiplier, scaleUpDuration)
            .SetEase(scaleUpEase)
        );

        // 작아지기
        currentSequence.Append(
            transform.DOScale(Vector3.zero, scaleDownDuration)
            .SetEase(scaleDownEase)
        );

        // 비활성화
        currentSequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        transform.DOKill(); // 트윈 중단
    }
}