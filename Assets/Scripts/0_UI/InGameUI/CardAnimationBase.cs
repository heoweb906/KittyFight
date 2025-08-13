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
        if (isPlaying) return; // 중복 실행 방지

        animationImages = images;
        isPlaying = true;

        // 원래 위치들 저장
        SaveOriginalPositions(images);

        // 실제 애니메이션 로직 실행
        ExecuteAnimation(images);
    }

    public void StopAnimation()
    {
        isPlaying = false;

        if (animationImages != null)
        {
            // 모든 DOTween 정지
            KillAllTweens();

            // 원래 위치로 복원
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

    // 하위 클래스에서 구현해야 할 메서드들
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
        if (images.Count >= 17) // 17번 파츠까지 있는지 확인
        {
            // 2번 요소 (index 1) 알파 애니메이션
            if (images.Count > 1)
            {
                Image element2 = images[1];
                StartAlphaAnimation(element2);
            }

            // 4,5,6,7번 요소 (index 3,4,5,6) 발톱 애니메이션
            int[] clawIndices = { 6, 5, 4, 3 }; // 7번부터 역순으로 (index는 -1)
            for (int i = 0; i < clawIndices.Length; i++)
            {
                int clawIndex = clawIndices[i];
                if (clawIndex < images.Count)
                {
                    RectTransform clawRect = images[clawIndex].GetComponent<RectTransform>();
                    StartClawAnimation(clawRect, i);
                }
            }

            // 10, 11, 14, 15, 16, 17번 요소 배열
            int[] leafIndices = { 9, 10, 13, 14, 15, 16 }; // 인덱스는 -1
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
        float moveDistance = Random.Range(30f, 50f); // 위로 올라갈 거리
        float animSpeed = 0.4f; // 올라가고 내려오는 속도
        float delayBetweenClaws = 0.2f; // 발톱 간 딜레이
        float cyclePause = 3f; // 한 사이클 완료 후 대기시간
        int totalClaws = 4; // 4,5,6,7번 총 4개의 발톱

        // 순서에 따른 시작 딜레이 (7번부터 시작)
        float startDelay = order * delayBetweenClaws + 2;

        // 시퀀스 생성
        Sequence clawSequence = DOTween.Sequence();

        // 초기 딜레이
        clawSequence.AppendInterval(startDelay);

        // 위로 올라가기 (부드럽게)
        clawSequence.Append(claw.DOAnchorPos(originalPos + new Vector2(0, moveDistance), animSpeed).SetEase(Ease.OutSine));

        // 원래 위치로 돌아가기 (부드럽게)
        clawSequence.Append(claw.DOAnchorPos(originalPos, animSpeed).SetEase(Ease.InSine));

        // 모든 발톱이 완료될 때까지 대기 + 추가 휴식시간
        float totalWaitTime = cyclePause + (totalClaws * delayBetweenClaws) - startDelay;
        clawSequence.AppendInterval(totalWaitTime);

        // 무한 반복
        clawSequence.SetLoops(-1);
    }

    private void StartAlphaAnimation(Image element2)
    {
        // 처음 시작은 알파값 0으로 설정
        element2.color = new Color(element2.color.r, element2.color.g, element2.color.b, 0f);
        // 시퀀스 생성
        Sequence alphaSequence = DOTween.Sequence();
        // 1.5초 대기
        alphaSequence.AppendInterval(1.5f);
        // 1초에 거쳐서 알파값 1
        alphaSequence.Append(element2.DOFade(1f, 1f));
        // 1.5초 대기
        alphaSequence.AppendInterval(1.5f);
        // 1초에 거쳐서 알파값 0
        alphaSequence.Append(element2.DOFade(0f, 1f));
        // 무한 반복
        alphaSequence.SetLoops(-1);
    }

    private void StartLeafRotation(RectTransform leaf, int leafNumber)
    {
        // 각 낙엽마다 다른 랜덤 값으로 자연스럽게
        float rotationRange = Random.Range(12f, 22f);    // 회전 각도 (8~15도)
        float rotationSpeed = Random.Range(2f, 4f);     // 회전 속도 (2~4초)
        float startDelay = Random.Range(0.2f, 1.2f);        // 시작 딜레이
        // 시작 딜레이 후 회전 시작
        DOVirtual.DelayedCall(startDelay, () =>
        {
            // 좌우로 살랑살랑 회전
            leaf.DORotate(new Vector3(0, 0, rotationRange), rotationSpeed)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        });
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 2번 요소 DOTween 정리
            if (animationImages.Count > 1)
            {
                animationImages[1].DOKill();
            }

            // 4,5,6,7번 요소 DOTween 정리
            int[] clawIndices = { 3, 4, 5, 6 };
            for (int i = 0; i < clawIndices.Length; i++)
            {
                int clawIndex = clawIndices[i];
                if (clawIndex < animationImages.Count)
                {
                    animationImages[clawIndex].GetComponent<RectTransform>().DOKill();
                }
            }

            // 10, 11, 14, 15, 16, 17번 요소들의 DOTween 정리
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
        if (images.Count >= 14) // 14번 파츠까지 있는지 확인
        {
            RectTransform part6 = images[5].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(part6);

            // 8번 파츠 (index 7)
            RectTransform element8 = images[7].GetComponent<RectTransform>();
            Vector2 element8Original = GetOriginalPosition(element8);

            // 9번 파츠 (index 8)
            RectTransform element9 = images[8].GetComponent<RectTransform>();
            Vector2 element9Original = GetOriginalPosition(element9);

            // 10번 파츠 (index 9)
            RectTransform element10 = images[9].GetComponent<RectTransform>();
            Vector2 element10Original = GetOriginalPosition(element10);

            // 6번 파츠 시퀀스 (독립적)
            Sequence part6Sequence = DOTween.Sequence();
            part6Sequence.Append(part6.DOAnchorPos(originalPos + new Vector2(0, -200f), 2f).SetEase(Ease.OutQuad));
            part6Sequence.Append(part6.DOAnchorPos(originalPos, 0.04f).SetEase(Ease.InQuad));
            part6Sequence.Append(part6.DOShakePosition(0.8f, strength: 15f, vibrato: 60, randomness: 90f, fadeOut: false));
            part6Sequence.AppendInterval(2f);
            part6Sequence.SetLoops(-1);


            // 8번 파츠
            element8.DOAnchorPos(element8Original + new Vector2(-50f, -50f), 2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);

            // 9번 파츠  
            element9.DOAnchorPos(element9Original + new Vector2(50f, 50f), 2.3f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);

            // 10번 파츠
            element10.DOAnchorPos(element10Original + new Vector2(50f, 0f), 1.8f)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine);






            // 2~5번 요소 (index 1~4) 랜덤 로테이션
            for (int i = 1; i <= 4; i++)
            {
                if (i < images.Count)
                {
                    RectTransform partRect = images[i].GetComponent<RectTransform>();
                    StartRandomRotation(partRect, i);
                }
            }

            // 11번 요소 (index 10) - 좌측으로 이동
            if (images.Count > 10)
            {
                RectTransform element11 = images[10].GetComponent<RectTransform>();
                Vector2 element11Original = GetOriginalPosition(element11);
                Vector2 element11Target = element11Original + new Vector2(72f, 0);
                element11.DOAnchorPos(element11Target, 2.6f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
            }

            // 12번 요소 (index 11) - 좌측하단으로 이동
            if (images.Count > 11)
            {
                RectTransform element13 = images[11].GetComponent<RectTransform>();
                Vector2 element13Original = GetOriginalPosition(element13);
                Vector2 element13Target = element13Original + new Vector2(70f, -70f);
                element13.DOAnchorPos(element13Target, 3f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
            }

            // 13번 요소 (index 12) - 우측하단으로 이동
            if (images.Count > 12)
            {
                RectTransform element12 = images[12].GetComponent<RectTransform>();
                Vector2 element12Original = GetOriginalPosition(element12);
                Vector2 element12Target = element12Original + new Vector2(-80f, -60f);
                element12.DOAnchorPos(element12Target, 4f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
            }

            // 14번 요소 (index 13) - 우측으로 이동
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
                // 회전으로 떨기 (말 발굽처럼 강하게)
                target.DOShakeRotation(
                    duration: 0.8f,          // 떨림 지속시간
                    strength: new Vector3(0, 0, 5f), // Z축 회전 강도 (30도)
                    vibrato: 20,             // 떨림 횟수 (높을수록 빠르게 떨림)
                    randomness: 50f,         // 랜덤성
                    fadeOut: false           // 점점 약해지지 않음
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
            // 닭 캐릭터 (2번째 요소) - 시작 위치에서 아주 약간씩 원형 움직임
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

            // 3번째 깃털 - 좌우로 크게 움직임 (좌측으로 더 많이)
            RectTransform feather3 = images[2].GetComponent<RectTransform>();
            Vector2 feather3Original = GetOriginalPosition(feather3);
            Vector2 feather3Target = feather3Original + new Vector2(-72f, 0);
            feather3.DOAnchorPos(feather3Target, 1.2f)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine);

            // 4번째 깃털 - 우측상단, 좌측하단 대각선으로 (좌측하단으로 더 많이)
            RectTransform feather4 = images[3].GetComponent<RectTransform>();
            Vector2 feather4Original = GetOriginalPosition(feather4);
            Vector2 feather4Target = feather4Original + new Vector2(-54f, -45f);
            feather4.DOAnchorPos(feather4Target, 1.4f)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine);

            // 5번째 깃털 - 우측상단, 좌측하단 대각선으로 더 크게 (좌측하단으로 더 많이)
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
        if (images.Count >= 10) // 9번 파츠까지 있는지 확인
        {
            // 9번 요소 (index 8) - 4배 느리게 회전
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
                StartContinuousRotation(element8, 1.5f); // 1초에 360도
                RectTransform element9 = images[8].GetComponent<RectTransform>();
                StartContinuousRotation(element9, 5f); // 4초에 360도
            }

            // 10번 요소 (index 9) - 크기 애니메이션
            if (images.Count > 9)
            {
                RectTransform element10 = images[9].GetComponent<RectTransform>();
                StartScaleAnimation(element10);
            }
        }
    }

    private void StartContinuousRotation(RectTransform target, float rotationDuration)
    {
        // 계속 한쪽으로 회전 (360도씩)
        target.DORotate(new Vector3(0, 0, -360f), rotationDuration, RotateMode.FastBeyond360)
              .SetEase(Ease.Linear)
              .SetLoops(-1, LoopType.Restart);
    }

    private void StartScaleAnimation(RectTransform target)
    {
        Vector3 baseScale = target.localScale;

        // 원래 크기 → 0.85배로 축소하고 Yoyo로 반복
        target.DOScale(baseScale * 0.85f, 1.75f)
              .SetEase(Ease.InOutSine)
              .SetLoops(-1, LoopType.Yoyo);
    }
    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 2~5번 요소 DOTween 정리
            for (int i = 1; i <= 4; i++)
            {
                if (i < animationImages.Count)
                {
                    animationImages[i].GetComponent<RectTransform>().DOKill();
                }
            }

            // 8번 요소 DOTween 정리
            if (animationImages.Count > 7)
            {
                animationImages[7].GetComponent<RectTransform>().DOKill();
            }

            // 9번 요소 DOTween 정리
            if (animationImages.Count > 8)
            {
                animationImages[8].GetComponent<RectTransform>().DOKill();
            }

            // 10번 요소 DOTween 정리
            if (animationImages.Count > 9)
            {
                animationImages[9].GetComponent<RectTransform>().DOKill();
            }
        }
    }
}