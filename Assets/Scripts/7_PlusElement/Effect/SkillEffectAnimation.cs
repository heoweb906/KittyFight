using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillEffectAnimation : MonoBehaviour
{
    [Header("Target Images")]
    public Image[] targetImages;

    private RectTransform[] targetRectTransforms;
    private Vector3[] originalScales;
    private Vector3[] originalRotations;
    private Sequence[] currentSequences;
    private Sequence[] currentShakeSequences;

    void Awake()
    {
        InitializeArrays();
    }

    void InitializeArrays()
    {
        if (targetImages == null || targetImages.Length == 0) return;

        int count = targetImages.Length;
        targetRectTransforms = new RectTransform[count];
        originalScales = new Vector3[count];
        originalRotations = new Vector3[count];
        currentSequences = new Sequence[count];
        currentShakeSequences = new Sequence[count];

        for (int i = 0; i < count; i++)
        {
            if (targetImages[i] != null)
            {
                targetRectTransforms[i] = targetImages[i].GetComponent<RectTransform>();
                originalScales[i] = targetRectTransforms[i].localScale;
                originalRotations[i] = targetRectTransforms[i].localEulerAngles;
            }
        }
    }

    private void StopCurrentAnimation(int index)
    {
        if (targetRectTransforms == null || index < 0 || index >= targetRectTransforms.Length) return;

        // 타겟이 살아있는 경우에만 DOKill 수행
        if (targetRectTransforms[index] != null)
        {
            targetRectTransforms[index].DOKill();
        }

        if (currentSequences[index] != null)
        {
            currentSequences[index].Kill();
            currentSequences[index] = null;
        }

        if (currentShakeSequences[index] != null)
        {
            currentShakeSequences[index].Kill();
            currentShakeSequences[index] = null;
        }

        // 상태 원복 (타겟이 살아있을 때만)
        if (targetRectTransforms[index] != null)
        {
            targetRectTransforms[index].localScale = originalScales[index];
            targetRectTransforms[index].localEulerAngles = originalRotations[index];
        }
    }

    public void PlayShakeAnimation(int index)
    {
        if (targetRectTransforms == null || index < 0 || index >= targetRectTransforms.Length) return;
        RectTransform target = targetRectTransforms[index];
        if (target == null) return;

        StopCurrentAnimation(index);

        // 1. 회전 떨림
        currentShakeSequences[index] = DOTween.Sequence();
        currentShakeSequences[index]
            .Append(target.DOPunchRotation(Vector3.forward * 30f, 0.4f, 30, 0.4f))
            .OnComplete(() =>
            {
                currentShakeSequences[index] = null;
                if (target != null) target.localEulerAngles = originalRotations[index];
            })
            .SetLink(target.gameObject); // [중요] 오브젝트 파괴 시 트윈 자동 삭제

        // 2. 크기 변화
        currentSequences[index] = DOTween.Sequence();
        currentSequences[index]
            .Append(target.DOScale(originalScales[index] * 0.7f, 0.04f))
            .Append(target.DOScale(originalScales[index] * 1.05f, 0.12f))
            .Append(target.DOScale(originalScales[index], 0.08f))
            .OnComplete(() =>
            {
                currentSequences[index] = null;
                if (target != null) target.localScale = originalScales[index];
            })
            .SetLink(target.gameObject); // [중요]
    }

    public void PlaySimpleAnimation(int index)
    {
        if (targetRectTransforms == null || index < 0 || index >= targetRectTransforms.Length) return;
        RectTransform target = targetRectTransforms[index];
        if (target == null) return;

        StopCurrentAnimation(index);

        currentSequences[index] = DOTween.Sequence();
        currentSequences[index]
            .Append(target.DOScale(originalScales[index] * 0.9f, 0.04f))
            .Append(target.DOScale(originalScales[index] * 1.2f, 0.1f))
            .Append(target.DOScale(originalScales[index], 0.1f))
            .OnComplete(() =>
            {
                currentSequences[index] = null;
                if (target != null) target.localScale = originalScales[index];
            })
            .SetLink(target.gameObject); // [중요]
    }

    public void PlayShakeAnimation(Image targetImage)
    {
        if (targetImages == null) return;
        int index = System.Array.IndexOf(targetImages, targetImage);
        if (index >= 0) PlayShakeAnimation(index);
    }

    public void PlaySimpleAnimation(Image targetImage)
    {
        if (targetImages == null) return;
        int index = System.Array.IndexOf(targetImages, targetImage);
        if (index >= 0) PlaySimpleAnimation(index);
    }

    public void PlayDoubleShakeAnimation(int index1, int index2)
    {
        PlaySingleDoubleShake(index1);
        PlaySingleDoubleShake(index2);
    }

    // 중복 코드 방지를 위한 내부 헬퍼 함수
    private void PlaySingleDoubleShake(int index)
    {
        if (targetRectTransforms == null || index < 0 || index >= targetRectTransforms.Length) return;
        RectTransform target = targetRectTransforms[index];
        if (target == null) return;

        StopCurrentAnimation(index);

        // Z축 떨림
        currentShakeSequences[index] = DOTween.Sequence();
        currentShakeSequences[index]
            .Append(target.DOPunchRotation(Vector3.forward * 15f, 0.3f, 30, 0.8f))
            .OnComplete(() =>
            {
                currentShakeSequences[index] = null;
                if (target != null) target.localEulerAngles = originalRotations[index];
            })
            .SetLink(target.gameObject);

        // 크기 변화
        currentSequences[index] = DOTween.Sequence();
        currentSequences[index]
            .Append(target.DOScale(originalScales[index] * 1.1f, 0.15f))
            .Append(target.DOScale(originalScales[index], 0.15f))
            .OnComplete(() =>
            {
                currentSequences[index] = null;
                if (target != null) target.localScale = originalScales[index];
            })
            .SetLink(target.gameObject);
    }

    void OnDestroy()
    {
        if (targetRectTransforms == null) return;

        for (int i = 0; i < targetRectTransforms.Length; i++)
        {
            StopCurrentAnimation(i);
        }
    }
}