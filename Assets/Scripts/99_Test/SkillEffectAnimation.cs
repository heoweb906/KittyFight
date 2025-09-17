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


        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayShakeAnimation(1);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlaySimpleAnimation(1);
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
        currentShakeSequences[index].Append(targetRectTransforms[index].DOPunchRotation(Vector3.forward * 30f, 0.4f, 30, 0.4f))
                                   .OnComplete(() => currentShakeSequences[index] = null);

        // 스케일 애니메이션
        currentSequences[index] = DOTween.Sequence();
        currentSequences[index].Append(targetRectTransforms[index].DOScale(originalScales[index] * 0.7f, 0.04f))
                              .Append(targetRectTransforms[index].DOScale(originalScales[index] * 1.05f, 0.12f))
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


    // HP바 전용 애니메이션
    public void PlayDoubleShakeAnimation(Image image1, Image image2)
    {
        if (image1 != null)
        {
            int index1 = System.Array.IndexOf(targetImages, image1);
            if (index1 >= 0)
            {
                RectTransform rect1 = targetRectTransforms[index1];
                rect1.DOKill();

                // Z축 떨림
                rect1.DOPunchRotation(Vector3.forward * 15f, 0.3f, 30, 0.8f)
                     .OnComplete(() => rect1.localEulerAngles = originalRotations[index1]);

                // 크기 변화
                Sequence scaleSeq1 = DOTween.Sequence();
                scaleSeq1.Append(rect1.DOScale(originalScales[index1] * 1.1f, 0.15f))
                         .Append(rect1.DOScale(originalScales[index1], 0.15f))
                         .OnComplete(() => rect1.localScale = originalScales[index1]);
            }
        }
        if (image2 != null)
        {
            int index2 = System.Array.IndexOf(targetImages, image2);
            if (index2 >= 0)
            {
                RectTransform rect2 = targetRectTransforms[index2];
                rect2.DOKill();

                // Z축 떨림
                rect2.DOPunchRotation(Vector3.forward * 15f, 0.3f, 30, 0.8f)
                     .OnComplete(() => rect2.localEulerAngles = originalRotations[index2]);

                // 크기 변화
                Sequence scaleSeq2 = DOTween.Sequence();
                scaleSeq2.Append(rect2.DOScale(originalScales[index2] * 1.1f, 0.15f))
                         .Append(rect2.DOScale(originalScales[index2], 0.15f))
                         .OnComplete(() => rect2.localScale = originalScales[index2]);
            }
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