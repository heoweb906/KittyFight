using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillEffectAnimation : MonoBehaviour
{
    [Header("Target Images")]
    public Image[] targetImages;  // 배열로 변경

    private RectTransform[] targetRectTransforms;
    private Vector3[] originalScales;
    private Vector3[] originalRotations;
    private Sequence[] currentSequences;
    private Sequence[] currentShakeSequences;

    void Start()
    {
        InitializeArrays();
    }

    void InitializeArrays()
    {
        if (targetImages == null || targetImages.Length == 0)
        {
            Debug.LogWarning("Target Images가 할당되지 않았습니다!");
            return;
        }

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

    void Update()
    {
        // 0번 요소: H(Shake), B(Simple)
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayShakeAnimation(0);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlaySimpleAnimation(0);
        }

        // 1번 요소: J(Shake), N(Simple)
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayShakeAnimation(1);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlaySimpleAnimation(1);
        }

        // 2번 요소: K(Shake), M(Simple)
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayShakeAnimation(2);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlaySimpleAnimation(2);
        }
    }

    // 특정 인덱스의 애니메이션 정리
    private void StopCurrentAnimation(int index)
    {
        if (index < 0 || index >= targetRectTransforms.Length) return;

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

        if (targetRectTransforms[index] != null)
        {
            targetRectTransforms[index].localScale = originalScales[index];
            targetRectTransforms[index].localEulerAngles = originalRotations[index];
        }
    }

    // 매개변수로 인덱스를 받는 Shake 애니메이션
    public void PlayShakeAnimation(int index)
    {
        if (index < 0 || index >= targetRectTransforms.Length || targetRectTransforms[index] == null) return;

        StopCurrentAnimation(index);

        // 떨림 애니메이션
        currentShakeSequences[index] = DOTween.Sequence();
        currentShakeSequences[index].Append(targetRectTransforms[index].DOPunchRotation(Vector3.forward * 8f, 0.4f, 20, 0.7f))
                                   .OnComplete(() => currentShakeSequences[index] = null);

        // 스케일 애니메이션
        currentSequences[index] = DOTween.Sequence();
        currentSequences[index].Append(targetRectTransforms[index].DOScale(originalScales[index] * 0.9f, 0.04f))
                              .Append(targetRectTransforms[index].DOScale(originalScales[index] * 1.3f, 0.12f))
                              .Append(targetRectTransforms[index].DOScale(originalScales[index], 0.08f))
                              .OnComplete(() => {
                                  currentSequences[index] = null;
                                  targetRectTransforms[index].localScale = originalScales[index];
                                  targetRectTransforms[index].localEulerAngles = originalRotations[index];
                              });
    }

    // 매개변수로 인덱스를 받는 Simple 애니메이션
    public void PlaySimpleAnimation(int index)
    {
        if (index < 0 || index >= targetRectTransforms.Length || targetRectTransforms[index] == null) return;

        StopCurrentAnimation(index);

        currentSequences[index] = DOTween.Sequence();
        currentSequences[index].Append(targetRectTransforms[index].DOScale(originalScales[index] * 0.9f, 0.04f))
                              .Append(targetRectTransforms[index].DOScale(originalScales[index] * 1.2f, 0.1f))
                              .Append(targetRectTransforms[index].DOScale(originalScales[index], 0.1f))
                              .OnComplete(() => {
                                  currentSequences[index] = null;
                                  targetRectTransforms[index].localScale = originalScales[index];
                              });
    }

    // 매개변수로 Image를 직접 받는 버전 (나중에 사용)
    public void PlayShakeAnimation(Image targetImage)
    {
        int index = System.Array.IndexOf(targetImages, targetImage);
        if (index >= 0)
        {
            PlayShakeAnimation(index);
        }
    }

    public void PlaySimpleAnimation(Image targetImage)
    {
        int index = System.Array.IndexOf(targetImages, targetImage);
        if (index >= 0)
        {
            PlaySimpleAnimation(index);
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < targetRectTransforms.Length; i++)
        {
            StopCurrentAnimation(i);
        }
    }
}