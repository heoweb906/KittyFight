using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.CompilerServices;

public interface ICardAnimation
{
    void StartAnimation(List<Image> images);
    void StopAnimation();
}

public abstract class CardAnimationBase : MonoBehaviour, ICardAnimation
{
    protected List<Image> animationImages;
    protected bool isPlaying = false;
    protected Dictionary<RectTransform, Vector2> originalPositions = new Dictionary<RectTransform, Vector2>();

    public void StartAnimation(List<Image> images)
    {
        if (isPlaying) return; // �ߺ� ���� ����

        animationImages = images;
        isPlaying = true;

        // ���� ��ġ�� ����
        SaveOriginalPositions(images);

        // ���� �ִϸ��̼� ���� ����
        ExecuteAnimation(images);
    }

    public void StopAnimation()
    {
        isPlaying = false;

        if (animationImages != null)
        {
            // ��� DOTween ����
            KillAllTweens();

            // ���� ��ġ�� ����
            RestoreOriginalPositions();
        }
    }

    private void SaveOriginalPositions(List<Image> images)
    {
        originalPositions.Clear();
        for (int i = 0; i < images.Count; i++)
        {
            RectTransform rect = images[i].GetComponent<RectTransform>();
            originalPositions[rect] = rect.anchoredPosition;
        }
    }

    private void RestoreOriginalPositions()
    {
        foreach (var pair in originalPositions)
        {
            if (pair.Key != null)
            {
                pair.Key.DOKill();
                pair.Key.anchoredPosition = pair.Value;
            }
        }
    }

    protected Vector2 GetOriginalPosition(RectTransform rect)
    {
        return originalPositions.ContainsKey(rect) ? originalPositions[rect] : rect.anchoredPosition;
    }

    // ���� Ŭ�������� �����ؾ� �� �޼����
    protected abstract void ExecuteAnimation(List<Image> images);
    protected abstract void KillAllTweens();
}

// ============================================================================================
// ============================================================================================
// ============================================================================================




public class CardAnimation_Num_5 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        if (images.Count >= 17) // 17�� �������� �ִ��� Ȯ��
        {
            // 2�� ��� (index 1) ���� �ִϸ��̼�
            if (images.Count > 1)
            {
                Image element2 = images[1];
                StartAlphaAnimation(element2);
            }

            // 4,5,6,7�� ��� (index 3,4,5,6) ���� �ִϸ��̼�
            int[] clawIndices = { 6, 5, 4, 3 }; // 7������ �������� (index�� -1)
            for (int i = 0; i < clawIndices.Length; i++)
            {
                int clawIndex = clawIndices[i];
                if (clawIndex < images.Count)
                {
                    RectTransform clawRect = images[clawIndex].GetComponent<RectTransform>();
                    StartClawAnimation(clawRect, i);
                }
            }

            // 10, 11, 14, 15, 16, 17�� ��� �迭
            int[] leafIndices = { 9, 10, 13, 14, 15, 16 }; // �ε����� -1
            for (int i = 0; i < leafIndices.Length; i++)
            {
                int leafIndex = leafIndices[i];
                if (leafIndex < images.Count)
                {
                    RectTransform leafRect = images[leafIndex].GetComponent<RectTransform>();
                    StartLeafRotation(leafRect, i);
                }
            }
        }
    }

    private void StartClawAnimation(RectTransform claw, int order)
    {
        Vector2 originalPos = GetOriginalPosition(claw);
        float moveDistance = Random.Range(30f, 50f); // ���� �ö� �Ÿ�
        float animSpeed = 0.4f; // �ö󰡰� �������� �ӵ�
        float delayBetweenClaws = 0.2f; // ���� �� ������
        float cyclePause = 3f; // �� ����Ŭ �Ϸ� �� ���ð�
        int totalClaws = 4; // 4,5,6,7�� �� 4���� ����

        // ������ ���� ���� ������ (7������ ����)
        float startDelay = order * delayBetweenClaws + 2;

        // ������ ����
        Sequence clawSequence = DOTween.Sequence();

        // �ʱ� ������
        clawSequence.AppendInterval(startDelay);

        // ���� �ö󰡱� (�ε巴��)
        clawSequence.Append(claw.DOAnchorPos(originalPos + new Vector2(0, moveDistance), animSpeed).SetEase(Ease.OutSine));

        // ���� ��ġ�� ���ư��� (�ε巴��)
        clawSequence.Append(claw.DOAnchorPos(originalPos, animSpeed).SetEase(Ease.InSine));

        // ��� ������ �Ϸ�� ������ ��� + �߰� �޽Ľð�
        float totalWaitTime = cyclePause + (totalClaws * delayBetweenClaws) - startDelay;
        clawSequence.AppendInterval(totalWaitTime);

        // ���� �ݺ�
        clawSequence.SetLoops(-1);
    }

    private void StartAlphaAnimation(Image element2)
    {
        // ó�� ������ ���İ� 0���� ����
        element2.color = new Color(element2.color.r, element2.color.g, element2.color.b, 0f);
        // ������ ����
        Sequence alphaSequence = DOTween.Sequence();
        // 1.5�� ���
        alphaSequence.AppendInterval(1.5f);
        // 1�ʿ� ���ļ� ���İ� 1
        alphaSequence.Append(element2.DOFade(1f, 1f));
        // 1.5�� ���
        alphaSequence.AppendInterval(1.5f);
        // 1�ʿ� ���ļ� ���İ� 0
        alphaSequence.Append(element2.DOFade(0f, 1f));
        // ���� �ݺ�
        alphaSequence.SetLoops(-1);
    }

    private void StartLeafRotation(RectTransform leaf, int leafNumber)
    {
        // �� �������� �ٸ� ���� ������ �ڿ�������
        float rotationRange = Random.Range(12f, 22f);    // ȸ�� ���� (8~15��)
        float rotationSpeed = Random.Range(2f, 4f);     // ȸ�� �ӵ� (2~4��)
        float startDelay = Random.Range(0.2f, 1.2f);        // ���� ������
        // ���� ������ �� ȸ�� ����
        DOVirtual.DelayedCall(startDelay, () =>
        {
            // �¿�� ������ ȸ��
            leaf.DORotate(new Vector3(0, 0, rotationRange), rotationSpeed)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        });
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 2�� ��� DOTween ����
            if (animationImages.Count > 1)
            {
                animationImages[1].DOKill();
            }

            // 4,5,6,7�� ��� DOTween ����
            int[] clawIndices = { 3, 4, 5, 6 };
            for (int i = 0; i < clawIndices.Length; i++)
            {
                int clawIndex = clawIndices[i];
                if (clawIndex < animationImages.Count)
                {
                    animationImages[clawIndex].GetComponent<RectTransform>().DOKill();
                }
            }

            // 10, 11, 14, 15, 16, 17�� ��ҵ��� DOTween ����
            int[] leafIndices = { 9, 10, 13, 14, 15, 16 };
            for (int i = 0; i < leafIndices.Length; i++)
            {
                int leafIndex = leafIndices[i];
                if (leafIndex < animationImages.Count)
                {
                    animationImages[leafIndex].GetComponent<RectTransform>().DOKill();
                }
            }
        }
    }
}



public class CardAnimation_Num_15 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        if (images.Count >= 14) // 14�� �������� �ִ��� Ȯ��
        {
            RectTransform part6 = images[5].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(part6);

            // 8�� ���� (index 7)
            RectTransform element8 = images[7].GetComponent<RectTransform>();
            Vector2 element8Original = GetOriginalPosition(element8);

            // 9�� ���� (index 8)
            RectTransform element9 = images[8].GetComponent<RectTransform>();
            Vector2 element9Original = GetOriginalPosition(element9);

            // 10�� ���� (index 9)
            RectTransform element10 = images[9].GetComponent<RectTransform>();
            Vector2 element10Original = GetOriginalPosition(element10);

            // 6�� ���� ������ (������)
            Sequence part6Sequence = DOTween.Sequence();
            part6Sequence.Append(part6.DOAnchorPos(originalPos + new Vector2(0, -200f), 2f).SetEase(Ease.OutQuad));
            part6Sequence.Append(part6.DOAnchorPos(originalPos, 0.04f).SetEase(Ease.InQuad));
            part6Sequence.Append(part6.DOShakePosition(0.8f, strength: 15f, vibrato: 60, randomness: 90f, fadeOut: false));
            part6Sequence.AppendInterval(2f);
            part6Sequence.SetLoops(-1);


            // 8�� ����
            element8.DOAnchorPos(element8Original + new Vector2(-50f, -50f), 2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);

            // 9�� ����  
            element9.DOAnchorPos(element9Original + new Vector2(50f, 50f), 2.3f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);

            // 10�� ����
            element10.DOAnchorPos(element10Original + new Vector2(50f, 0f), 1.8f)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine);






            // 2~5�� ��� (index 1~4) ���� �����̼�
            for (int i = 1; i <= 4; i++)
            {
                if (i < images.Count)
                {
                    RectTransform partRect = images[i].GetComponent<RectTransform>();
                    StartRandomRotation(partRect, i);
                }
            }

            // 11�� ��� (index 10) - �������� �̵�
            if (images.Count > 10)
            {
                RectTransform element11 = images[10].GetComponent<RectTransform>();
                Vector2 element11Original = GetOriginalPosition(element11);
                Vector2 element11Target = element11Original + new Vector2(72f, 0);
                element11.DOAnchorPos(element11Target, 2.6f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
            }

            // 12�� ��� (index 11) - �����ϴ����� �̵�
            if (images.Count > 11)
            {
                RectTransform element13 = images[11].GetComponent<RectTransform>();
                Vector2 element13Original = GetOriginalPosition(element13);
                Vector2 element13Target = element13Original + new Vector2(70f, -70f);
                element13.DOAnchorPos(element13Target, 3f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
            }

            // 13�� ��� (index 12) - �����ϴ����� �̵�
            if (images.Count > 12)
            {
                RectTransform element12 = images[12].GetComponent<RectTransform>();
                Vector2 element12Original = GetOriginalPosition(element12);
                Vector2 element12Target = element12Original + new Vector2(-80f, -60f);
                element12.DOAnchorPos(element12Target, 4f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
            }

            // 14�� ��� (index 13) - �������� �̵�
            if (images.Count > 13)
            {
                RectTransform element14 = images[13].GetComponent<RectTransform>();
                Vector2 element14Original = GetOriginalPosition(element14);
                Vector2 element14Target = element14Original + new Vector2(72f, 0);
                element14.DOAnchorPos(element14Target, 2.5f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
            }
        }
    }

    private void StartRandomRotation(RectTransform target, int partIndex)
    {
        float randomDelay = Random.Range(2f, 6f);
        DOTween.Sequence()
            .AppendInterval(randomDelay)
            .AppendCallback(() =>
            {
                // ȸ������ ���� (�� �߱�ó�� ���ϰ�)
                target.DOShakeRotation(
                    duration: 0.8f,          // ���� ���ӽð�
                    strength: new Vector3(0, 0, 5f), // Z�� ȸ�� ���� (30��)
                    vibrato: 20,             // ���� Ƚ�� (�������� ������ ����)
                    randomness: 50f,         // ������
                    fadeOut: false           // ���� �������� ����
                )
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    StartRandomRotation(target, partIndex);
                });
            });
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            for (int i = 1; i < animationImages.Count && i <= 5; i++)
            {
                animationImages[i].GetComponent<RectTransform>().DOKill();
            }
        }
    }
}



public class CardAnimation_Num_22 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        if (images.Count >= 5)
        {
            // �� ĳ���� (2��° ���) - ���� ��ġ���� ���� �ణ�� ���� ������
            RectTransform chickenRect = images[1].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(chickenRect);

            float radius = 6f;
            float duration = 3f;

            chickenRect.DOMove(originalPos + new Vector2(radius, 0), duration)
                      .SetLoops(-1, LoopType.Restart)
                      .SetEase(Ease.InOutSine)
                      .OnUpdate(() =>
                      {
                          float time = (Time.time / duration) * 2 * Mathf.PI;
                          Vector2 circlePos = new Vector2(
                              originalPos.x + Mathf.Cos(time) * radius,
                              originalPos.y + Mathf.Sin(time) * radius
                          );
                          chickenRect.anchoredPosition = circlePos;
                      });

            // 3��° ���� - �¿�� ũ�� ������ (�������� �� ����)
            RectTransform feather3 = images[2].GetComponent<RectTransform>();
            Vector2 feather3Original = GetOriginalPosition(feather3);
            Vector2 feather3Target = feather3Original + new Vector2(-72f, 0);
            feather3.DOAnchorPos(feather3Target, 1.2f)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine);

            // 4��° ���� - �������, �����ϴ� �밢������ (�����ϴ����� �� ����)
            RectTransform feather4 = images[3].GetComponent<RectTransform>();
            Vector2 feather4Original = GetOriginalPosition(feather4);
            Vector2 feather4Target = feather4Original + new Vector2(-54f, -45f);
            feather4.DOAnchorPos(feather4Target, 1.4f)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine);

            // 5��° ���� - �������, �����ϴ� �밢������ �� ũ�� (�����ϴ����� �� ����)
            RectTransform feather5 = images[4].GetComponent<RectTransform>();
            Vector2 feather5Original = GetOriginalPosition(feather5);
            Vector2 feather5Target = feather5Original + new Vector2(-72f, -63f);
            feather5.DOAnchorPos(feather5Target, 1.3f)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine);
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            for (int i = 1; i < animationImages.Count && i <= 4; i++)
            {
                animationImages[i].GetComponent<RectTransform>().DOKill();
            }
        }
    }
}



public class CardAnimation_Num_123 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        if (images.Count >= 10) // 9�� �������� �ִ��� Ȯ��
        {
            // 9�� ��� (index 8) - 4�� ������ ȸ��
            if (images.Count > 8)
            {
                RectTransform element2 = images[1].GetComponent<RectTransform>();
                StartContinuousRotation(element2, 7f);
                RectTransform element3 = images[2].GetComponent<RectTransform>();
                StartContinuousRotation(element3, 7f);
                RectTransform element4 = images[3].GetComponent<RectTransform>();
                StartContinuousRotation(element4, 7f);
                RectTransform element5 = images[4].GetComponent<RectTransform>();
                StartContinuousRotation(element5, 7f);
                RectTransform element8 = images[7].GetComponent<RectTransform>();
                StartContinuousRotation(element8, 1.5f); // 1�ʿ� 360��
                RectTransform element9 = images[8].GetComponent<RectTransform>();
                StartContinuousRotation(element9, 5f); // 4�ʿ� 360��
            }

            // 10�� ��� (index 9) - ũ�� �ִϸ��̼�
            if (images.Count > 9)
            {
                RectTransform element10 = images[9].GetComponent<RectTransform>();
                StartScaleAnimation(element10);
            }
        }
    }

    private void StartContinuousRotation(RectTransform target, float rotationDuration)
    {
        // ��� �������� ȸ�� (360����)
        target.DORotate(new Vector3(0, 0, -360f), rotationDuration, RotateMode.FastBeyond360)
              .SetEase(Ease.Linear)
              .SetLoops(-1, LoopType.Restart);
    }

    private void StartScaleAnimation(RectTransform target)
    {
        Vector3 baseScale = target.localScale;

        // ���� ũ�� �� 0.85��� ����ϰ� Yoyo�� �ݺ�
        target.DOScale(baseScale * 0.85f, 1.75f)
              .SetEase(Ease.InOutSine)
              .SetLoops(-1, LoopType.Yoyo);
    }
    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 2~5�� ��� DOTween ����
            for (int i = 1; i <= 4; i++)
            {
                if (i < animationImages.Count)
                {
                    animationImages[i].GetComponent<RectTransform>().DOKill();
                }
            }

            // 8�� ��� DOTween ����
            if (animationImages.Count > 7)
            {
                animationImages[7].GetComponent<RectTransform>().DOKill();
            }

            // 9�� ��� DOTween ����
            if (animationImages.Count > 8)
            {
                animationImages[8].GetComponent<RectTransform>().DOKill();
            }

            // 10�� ��� DOTween ����
            if (animationImages.Count > 9)
            {
                animationImages[9].GetComponent<RectTransform>().DOKill();
            }
        }
    }
}