using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.CompilerServices;
using static UnityEngine.UI.CanvasScaler;

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
        // 8번부터 19번까지의 위치 설정 (고정)
        if (images.Count > 8)
        {
            images[8].GetComponent<RectTransform>().anchoredPosition = new Vector2(580f, -350f);
        }
        if (images.Count > 9)
        {
            images[9].GetComponent<RectTransform>().anchoredPosition = new Vector2(-453f, -440f);
        }
        if (images.Count > 10)
        {
            images[10].GetComponent<RectTransform>().anchoredPosition = new Vector2(-712f, 132f);
        }
        if (images.Count > 11)
        {
            images[11].GetComponent<RectTransform>().anchoredPosition = new Vector2(294f, 354f);
        }
        if (images.Count > 12)
        {
            images[12].GetComponent<RectTransform>().anchoredPosition = new Vector2(-344f, 310f);
        }
        if (images.Count > 13)
        {
            images[13].GetComponent<RectTransform>().anchoredPosition = new Vector2(655f, 39f);
        }

        // 14, 15번 인덱스 0,0 위치로 이동
        if (images.Count > 14)
        {
            images[14].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        if (images.Count > 15)
        {
            images[15].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        // 16번부터 19번까지 위치 설정 (고정)
        if (images.Count > 16)
        {
            images[16].GetComponent<RectTransform>().anchoredPosition = new Vector2(-292f, 368f);
        }
        if (images.Count > 17)
        {
            images[17].GetComponent<RectTransform>().anchoredPosition = new Vector2(585f, -485f);
        }
        if (images.Count > 18)
        {
            images[18].GetComponent<RectTransform>().anchoredPosition = new Vector2(297f, 387f);
        }
        if (images.Count > 19)
        {
            images[19].GetComponent<RectTransform>().anchoredPosition = new Vector2(-715f, -394f);
        }

        // 2번 요소 (index 1) 알파 애니메이션
        if (images.Count > 1)
        {
            StartAlphaAnimation(images[1]);
        }

        // 4, 5, 6, 7번 요소 (index 3, 4, 5, 6) 발톱 애니메이션
        int[] clawIndices = { 6, 5, 4, 3 };
        for (int i = 0; i < clawIndices.Length; i++)
        {
            int clawIndex = clawIndices[i];
            if (clawIndex < images.Count)
            {
                StartClawAnimation(images[clawIndex].GetComponent<RectTransform>(), i);
            }
        }

        // 풀 회전 애니메이션 (8, 9, 10, 11, 12, 13, 16, 17, 18, 19번 인덱스)
        int[] grassIndices = { 8, 9, 10, 11, 12, 13, 16, 17, 18, 19 };
        foreach (int index in grassIndices)
        {
            if (index < images.Count)
            {
                StartGrassRotation(images[index].GetComponent<RectTransform>());
            }
        }
    }

    // 풀처럼 Z축 로테이션으로 흔들리는 애니메이션
    private void StartGrassRotation(RectTransform grassRect)
    {
        // 랜덤한 로테이션 각도 (-4 ~ 4도)
        float randomAngle = Random.Range(-10f, 10f);
        // 랜덤한 회전 속도 (0.5 ~ 1.5초)
        float randomSpeed = Random.Range(0.5f, 1.5f);
        // 랜덤한 시작 지연 시간 (0 ~ 1초)
        float randomDelay = Random.Range(0f, 1f);

        // 시퀀스 생성
        Sequence rotationSequence = DOTween.Sequence();

        // 랜덤한 지연 시간 설정
        rotationSequence.AppendInterval(randomDelay);

        // -2도에서 2도 사이의 랜덤한 각도로 회전
        rotationSequence.Append(grassRect.DOLocalRotate(new Vector3(0, 0, -randomAngle), randomSpeed).SetEase(Ease.InOutSine));
        rotationSequence.Append(grassRect.DOLocalRotate(new Vector3(0, 0, randomAngle), randomSpeed * 2).SetEase(Ease.InOutSine));
        rotationSequence.Append(grassRect.DOLocalRotate(Vector3.zero, randomSpeed).SetEase(Ease.InOutSine));

        // 시퀀스 무한 반복
        rotationSequence.SetLoops(-1);
    }
    

    // 발톱 애니메이션
    private void StartClawAnimation(RectTransform claw, int order)
    {
        Vector2 originalPos = GetOriginalPosition(claw);
        float moveDistance = Random.Range(30f, 50f);
        float animSpeed = 0.4f;
        float delayBetweenClaws = 0.2f;
        float cyclePause = 3f;
        int totalClaws = 4;
        float startDelay = order * delayBetweenClaws + 2;

        Sequence clawSequence = DOTween.Sequence();
        clawSequence.AppendInterval(startDelay);
        clawSequence.Append(claw.DOAnchorPos(originalPos + new Vector2(0, moveDistance), animSpeed).SetEase(Ease.OutSine));
        clawSequence.Append(claw.DOAnchorPos(originalPos, animSpeed).SetEase(Ease.InSine));
        float totalWaitTime = cyclePause + (totalClaws * delayBetweenClaws) - startDelay;
        clawSequence.AppendInterval(totalWaitTime);
        clawSequence.SetLoops(-1);
    }


    // 알파 애니메이션
    private void StartAlphaAnimation(Image element2)
    {
        element2.color = new Color(element2.color.r, element2.color.g, element2.color.b, 0f);
        Sequence alphaSequence = DOTween.Sequence();
        alphaSequence.AppendInterval(1.5f);
        alphaSequence.Append(element2.DOFade(1f, 1f));
        alphaSequence.AppendInterval(1.5f);
        alphaSequence.Append(element2.DOFade(0f, 1f));
        alphaSequence.SetLoops(-1);
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            if (animationImages.Count > 1)
            {
                animationImages[1].DOKill();
            }

            int[] clawIndices = { 3, 4, 5, 6 };
            for (int i = 0; i < clawIndices.Length; i++)
            {
                int clawIndex = clawIndices[i];
                if (clawIndex < animationImages.Count)
                {
                    animationImages[clawIndex].GetComponent<RectTransform>().DOKill();
                }
            }

            // 풀 흔들림 요소 DOTween 정리
            int[] grassIndices = { 8, 9, 10, 11, 12, 13, 16, 17, 18, 19 };
            for (int i = 0; i < grassIndices.Length; i++)
            {
                int grassIndex = grassIndices[i];
                if (grassIndex < animationImages.Count)
                {
                    animationImages[grassIndex].GetComponent<RectTransform>().DOKill();
                }
            }
        }
    }
}



public class CardAnimation_Num_12 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        if (images.Count > 1)
        {
            AnimateElement1(images[1].GetComponent<RectTransform>());
        }
        if (images.Count > 2)
        {
            AnimateElement2(images[2].GetComponent<RectTransform>());
        }
        if (images.Count > 3)
        {
            AnimateElement3(images[3].GetComponent<RectTransform>());
        }

        // 4,5,6,9,10,11,12,17 - 위아래 확 움직임
        int[] verticalElements = { 4, 5, 6, 9, 10, 11, 12, 17 };
        for (int i = 0; i < verticalElements.Length; i++)
        {
            int elementIndex = verticalElements[i];
            if (images.Count > elementIndex)
            {
                AnimateVerticalSnap(images[elementIndex].GetComponent<RectTransform>());
            }
        }

        // 7,8,13,14,15,16 - 좌우 확 움직임
        int[] horizontalElements = { 7, 8, 13, 14, 15, 16 };
        for (int i = 0; i < horizontalElements.Length; i++)
        {
            int elementIndex = horizontalElements[i];
            if (images.Count > elementIndex)
            {
                AnimateHorizontalSnap(images[elementIndex].GetComponent<RectTransform>());
            }
        }
    }

    private void AnimateElement1(RectTransform element1)
    {
        Vector2 originalPos = GetOriginalPosition(element1);
        Vector2 leftDiagonalDown = originalPos + new Vector2(-30f, -30f);
        Vector2 rightDiagonalUp = originalPos + new Vector2(30f, 30f);
        element1.DOAnchorPos(leftDiagonalDown, 3f)
               .SetLoops(-1, LoopType.Yoyo)
               .SetEase(Ease.InOutSine);
    }

    private void AnimateElement2(RectTransform element2)
    {
        Vector2 originalPos = GetOriginalPosition(element2);
        Vector2 leftDiagonalDown = originalPos + new Vector2(-30f, -50f);
        Vector2 rightDiagonalUp = originalPos + new Vector2(30f, 50f);
        element2.DOAnchorPos(leftDiagonalDown, 1.8f)
               .SetLoops(-1, LoopType.Yoyo)
               .SetEase(Ease.InOutSine);
    }

    private void AnimateElement3(RectTransform element3)
    {
        Vector2 originalPos = GetOriginalPosition(element3);
        Vector2 leftDiagonalDown = originalPos + new Vector2(-30f, -50f);
        Vector2 rightDiagonalUp = originalPos + new Vector2(30f, 50f);
        element3.DOAnchorPos(leftDiagonalDown, 1.8f)
               .SetLoops(-1, LoopType.Yoyo)
               .SetEase(Ease.InOutSine);
    }

    private void AnimateVerticalSnap(RectTransform element)
    {
        Vector2 originalPos = GetOriginalPosition(element);
        float moveDistance = Random.Range(30f, 50f);
        Vector2 downPos = originalPos + new Vector2(0f, -moveDistance);

        Sequence elementSequence = DOTween.Sequence();

        // 랜덤 대기 시간 (0.8~2.4초) - 원래보다 0.8배
        float waitTime = Random.Range(0.4f, 1.6f);
        elementSequence.AppendInterval(waitTime);

        // 아래로 확 내려가기 (0.8배 느리게)
        elementSequence.Append(element.DOAnchorPos(downPos, Random.Range(0.16f, 0.32f)).SetEase(Ease.OutQuad));

        // 원래 위치로 확 올라오기 (0.8배 느리게)
        elementSequence.Append(element.DOAnchorPos(originalPos, Random.Range(0.16f, 0.32f)).SetEase(Ease.OutQuad));

        // 다시 랜덤 대기 시간 (1.6~4초) - 원래보다 0.8배
        float pauseTime = Random.Range(0.5f, 1.5f);
        elementSequence.AppendInterval(pauseTime);

        // 무한 반복
        elementSequence.SetLoops(-1);
    }

    private void AnimateHorizontalSnap(RectTransform element)
    {
        Vector2 originalPos = GetOriginalPosition(element);
        float moveDistance = Random.Range(30f, 50f);
        bool moveLeft = Random.Range(0, 2) == 0; // 랜덤하게 좌우 선택
        Vector2 sidePos = originalPos + new Vector2(moveLeft ? -moveDistance : moveDistance, 0f);

        Sequence elementSequence = DOTween.Sequence();

        // 랜덤 대기 시간 (0.8~2.4초) - 원래보다 0.8배
        float waitTime = Random.Range(0.5f, 1.6f);
        elementSequence.AppendInterval(waitTime);

        // 좌우로 확 움직이기 (0.8배 느리게)
        elementSequence.Append(element.DOAnchorPos(sidePos, Random.Range(0.16f, 0.32f)).SetEase(Ease.OutQuad));

        // 원래 위치로 확 돌아오기 (0.8배 느리게)
        elementSequence.Append(element.DOAnchorPos(originalPos, Random.Range(0.16f, 0.32f)).SetEase(Ease.OutQuad));

        // 다시 랜덤 대기 시간 (1.6~4초) - 원래보다 0.8배
        float pauseTime = Random.Range(1.6f, 4f);
        elementSequence.AppendInterval(pauseTime);

        // 무한 반복
        elementSequence.SetLoops(-1);
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            if (animationImages.Count > 1)
            {
                animationImages[1].GetComponent<RectTransform>().DOKill();
            }
            if (animationImages.Count > 2)
            {
                animationImages[2].GetComponent<RectTransform>().DOKill();
            }
            if (animationImages.Count > 3)
            {
                animationImages[3].GetComponent<RectTransform>().DOKill();
            }

            // 4,5,6,9,10,11,12,17
            int[] verticalElements = { 4, 5, 6, 9, 10, 11, 12, 17 };
            for (int i = 0; i < verticalElements.Length; i++)
            {
                int elementIndex = verticalElements[i];
                if (animationImages.Count > elementIndex)
                {
                    animationImages[elementIndex].GetComponent<RectTransform>().DOKill();
                }
            }

            // 7,8,13,14,15,16
            int[] horizontalElements = { 7, 8, 13, 14, 15, 16 };
            for (int i = 0; i < horizontalElements.Length; i++)
            {
                int elementIndex = horizontalElements[i];
                if (animationImages.Count > elementIndex)
                {
                    animationImages[elementIndex].GetComponent<RectTransform>().DOKill();
                }
            }
        }
    }
}



public class CardAnimation_Num_15 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        if (images.Count >= 13) // 14번 파츠까지 있는지 확인
        {
            // 1번 인덱스 요소 위치 설정
            if (images.Count > 1)
            {
                RectTransform element1 = images[1].GetComponent<RectTransform>();
                element1.anchoredPosition = new Vector2(-470f, -64f);
            }

            // 2번 인덱스 요소 위치 설정
            if (images.Count > 2)
            {
                RectTransform element2 = images[2].GetComponent<RectTransform>();
                element2.anchoredPosition = new Vector2(-163f, -294f);
            }

            // 3번 인덱스 요소 위치 설정
            if (images.Count > 3)
            {
                RectTransform element3 = images[3].GetComponent<RectTransform>();
                element3.anchoredPosition = new Vector2(243f, 19f);
            }

            // 4번 인덱스 요소 위치 설정
            if (images.Count > 4)
            {
                RectTransform element4 = images[4].GetComponent<RectTransform>();
                element4.anchoredPosition = new Vector2(425f, -17f);
            }

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
                Vector2 element11Target = element11Original + new Vector2(40f, -60);
                element11.DOAnchorPos(element11Target, 2.6f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
            }

            // 12번 요소 (index 11) - 좌측하단으로 이동
            if (images.Count > 11)
            {
                RectTransform element13 = images[11].GetComponent<RectTransform>();
                Vector2 element13Original = GetOriginalPosition(element13);
                Vector2 element13Target = element13Original + new Vector2(-30f, -70f);
                element13.DOAnchorPos(element13Target, 3f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.InOutSine);
            }

            // 13번 요소 (index 12) - 우측하단으로 이동
            if (images.Count > 12)
            {
                RectTransform element12 = images[12].GetComponent<RectTransform>();
                Vector2 element12Original = GetOriginalPosition(element12);
                Vector2 element12Target = element12Original + new Vector2(80f, 0f);
                element12.DOAnchorPos(element12Target, 2.5f)
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



public class CardAnimation_Num_16 : CardAnimationBase
{
    private int currentVisibleIndex = 8; // 8,9,10 중 현재 보이는 인덱스

    protected override void ExecuteAnimation(List<Image> images)
    {
        // 인덱스 1 요소 크기 애니메이션
        if (images.Count > 1)
        {
            RectTransform element1 = images[1].GetComponent<RectTransform>();
            element1.DOScale(1.05f, 2.6f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
        }
        // 인덱스 2 요소 크기 애니메이션
        if (images.Count > 2)
        {
            RectTransform element2 = images[2].GetComponent<RectTransform>();
            element2.DOScale(1.05f, 2.3f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
        }

        // 8,9,10번 요소 초기 설정
        InitializeAlphaElements(images);
        // 알파값 교체 시작
        StartAlphaRotation(images);

        if (images.Count > 11)
        {
            RectTransform element11 = images[11].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(element11);

            float randomYOffset = Random.Range(50f, 75f);
            float randomXOffset = Random.Range(37f, 62f);
            float randomYDuration = Random.Range(2.5f, 4f);
            float randomXDuration = Random.Range(3f, 5.5f);

            element11.DOAnchorPosY(originalPos.y + randomYOffset, randomYDuration)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine);
            element11.DOAnchorPosX(originalPos.x - randomXOffset, randomXDuration)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine);
        }
        // 인덱스 12 요소 두둥실 떠있는 애니메이션

        if (images.Count > 12)
        {
            RectTransform element12 = images[12].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(element12);

            float randomYOffset = Random.Range(50f, 75f);
            float randomXOffset = Random.Range(37f, 62f);
            float randomYDuration = Random.Range(3f, 5f);
            float randomXDuration = Random.Range(2.5f, 4.5f);

            element12.DOAnchorPosY(originalPos.y - randomYOffset, randomYDuration)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine);
            element12.DOAnchorPosX(originalPos.x + randomXOffset, randomXDuration)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine);
        }
        // 인덱스 13 요소 두둥실 떠있는 애니메이션
        if (images.Count > 13)
        {
            RectTransform element13 = images[13].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(element13);

            float randomYOffset = Random.Range(45f, 70f);
            float randomXOffset = Random.Range(30f, 55f);
            float randomYDuration = Random.Range(2.8f, 4.2f);
            float randomXDuration = Random.Range(3.2f, 5f);

            element13.DOAnchorPosY(originalPos.y + randomYOffset, randomYDuration)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine);
            element13.DOAnchorPosX(originalPos.x - randomXOffset, randomXDuration)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine);
        }
    }

    private void InitializeAlphaElements(List<Image> images)
    {
        // 8번만 알파값 1, 나머지는 0
        if (images.Count > 8)
        {
            images[8].color = new Color(images[8].color.r, images[8].color.g, images[8].color.b, 1f);
        }
        if (images.Count > 9)
        {
            images[9].color = new Color(images[9].color.r, images[9].color.g, images[9].color.b, 0f);
        }
        if (images.Count > 10)
        {
            images[10].color = new Color(images[10].color.r, images[10].color.g, images[10].color.b, 0f);
        }
        currentVisibleIndex = 8;
    }

    private void StartAlphaRotation(List<Image> images)
    {
        float randomDelay = Random.Range(0.2f, 1f);
        DOVirtual.DelayedCall(randomDelay, () =>
        {
            // 현재 보이는 요소를 0으로
            if (images.Count > currentVisibleIndex)
            {
                images[currentVisibleIndex].color = new Color(
                    images[currentVisibleIndex].color.r,
                    images[currentVisibleIndex].color.g,
                    images[currentVisibleIndex].color.b,
                    0f
                );
            }

            // 다음 요소로 이동 (8->9->10->8 순환)
            currentVisibleIndex++;
            if (currentVisibleIndex > 10) currentVisibleIndex = 8;

            // 새로운 요소를 1로
            if (images.Count > currentVisibleIndex)
            {
                images[currentVisibleIndex].color = new Color(
                    images[currentVisibleIndex].color.r,
                    images[currentVisibleIndex].color.g,
                    images[currentVisibleIndex].color.b,
                    1f
                );

                // 8,9,10번 요소가 바뀔 때 강하게 떨리기
                RectTransform currentElement = images[currentVisibleIndex].GetComponent<RectTransform>();
                currentElement.DOShakePosition(0.3f, 30f, 50, 90f, false);
            }

            // 4,5,6,7번 요소 Z축 회전 흔들기
            ShakeRotationElements(images);

            // 다시 호출
            StartAlphaRotation(images);
        });
    }

    private void ShakeRotationElements(List<Image> images)
    {
        int[] shakeIndices = { 4, 5, 6, 7 };

        for (int i = 0; i < shakeIndices.Length; i++)
        {
            int index = shakeIndices[i];
            if (images.Count > index)
            {
                RectTransform element = images[index].GetComponent<RectTransform>();
                element.DOShakeRotation(0.2f, new Vector3(0, 0, 4f), 10, 180f, true);
            }
        }

        // 인덱스 3 요소 크기 및 위치 애니메이션
        if (images.Count > 3)
        {
            RectTransform element3 = images[3].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(element3);

            Sequence element3Sequence = DOTween.Sequence();
            element3Sequence.Append(element3.DOScale(1.1f, 0.15f));
            element3Sequence.Join(element3.DOAnchorPosY(originalPos.y + 50f, 0.15f));
            element3Sequence.Append(element3.DOScale(1f, 0.15f));
            element3Sequence.Join(element3.DOAnchorPosY(originalPos.y, 0.15f));
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            if (animationImages.Count > 1)
            {
                animationImages[1].GetComponent<RectTransform>().DOKill();
            }
            if (animationImages.Count > 2)
            {
                animationImages[2].GetComponent<RectTransform>().DOKill();
            }
            // 4,5,6,7번 요소 DOTween 정리
            for (int i = 4; i <= 7; i++)
            {
                if (animationImages.Count > i)
                {
                    animationImages[i].GetComponent<RectTransform>().DOKill();
                }
            }
            if (animationImages.Count > 8)
            {
                animationImages[8].DOKill();
            }
            if (animationImages.Count > 9)
            {
                animationImages[9].DOKill();
            }
            if (animationImages.Count > 10)
            {
                animationImages[10].DOKill();
            }
            if (animationImages.Count > 11)
            {
                animationImages[11].GetComponent<RectTransform>().DOKill();
            }
            if (animationImages.Count > 12)
            {
                animationImages[12].GetComponent<RectTransform>().DOKill();
            }
            if (animationImages.Count > 13)
            {
                animationImages[13].GetComponent<RectTransform>().DOKill();
            }
        }
    }
}



public class CardAnimation_Num_20 : CardAnimationBase
{
    private Vector2[] circlePositions = new Vector2[5]
    {
     new Vector2(-179f, -56f),   // 2번 요소
     new Vector2(-333f, 109f),   // 3번 요소
     new Vector2(-32f, 181f),    // 4번 위치 (circlePositions[2])
     new Vector2(274f, 141f),    // 5번 요소
     new Vector2(129f, -71f)     // 6번 요소
    };

    private Vector2 centerPosition = new Vector2(0f, 0f); // 7번 요소 위치 (중앙)
    private bool isSpinning = false;
    private int targetPositionIndex = 2; // 4번 위치 (circlePositions[2])

    protected override void ExecuteAnimation(List<Image> images)
    {
        if (images.Count > 1)
        {
            AnimateElement1(images[1].GetComponent<RectTransform>());
        }

        if (images.Count > 7)
        {
            AnimateElement7(images[7].GetComponent<RectTransform>(), images[7]);
        }

        // 처음부터 각자 위치에 배치
        SetInitialPositions(images);

        // 1초 후에 룰렛 애니메이션 시작
        DOVirtual.DelayedCall(1f, () =>
        {
            StartRouletteAnimation(images);
        });
    }
    private void SetInitialPositions(List<Image> images)
    {
        int[] elementIndices = { 2, 3, 4, 5, 6 };

        for (int i = 0; i < elementIndices.Length; i++)
        {
            int elementIndex = elementIndices[i];
            if (images.Count > elementIndex)
            {
                images[elementIndex].GetComponent<RectTransform>().anchoredPosition = circlePositions[i];
            }
        }
    }

    private void StartRouletteAnimation(List<Image> images)
    {
        if (isSpinning) return;
        isSpinning = true;

        // 랜덤하게 어떤 요소가 4번 위치에 멈출지 결정
        int randomElementToStop = Random.Range(0, 5);
        int totalRotations = CalculateRotationsToTarget(randomElementToStop);

        // 연속적인 회전 애니메이션
        SpinContinuously(images, totalRotations, () =>
        {
            // 회전이 끝나면 4번 위치(제일 위)에 있는 요소에 애니메이션
            AnimateSelectedElement(images, () =>
            {
                // 애니메이션 완료 후 잠시 대기하고 다시 회전
                DOVirtual.DelayedCall(1f, () =>
                {
                    isSpinning = false;
                    StartRouletteAnimation(images);
                });
            });
        });
    }

    private int CalculateRotationsToTarget(int elementIndex)
    {
        // 기본 2-3바퀴 + 해당 요소가 4번 위치에 오도록 추가 이동
        int baseRotations = Random.Range(10, 15); // 2-3바퀴
        int currentPosition = elementIndex;
        int stepsToTarget = (targetPositionIndex - currentPosition + 5) % 5;

        return baseRotations + stepsToTarget;
    }

    private void SpinContinuously(List<Image> images, int totalSteps, TweenCallback onComplete)
    {
        int[] elementIndices = { 2, 3, 4, 5, 6 };

        for (int i = 0; i < elementIndices.Length; i++)
        {
            int elementIndex = elementIndices[i];
            if (images.Count > elementIndex)
            {
                RectTransform elementRect = images[elementIndex].GetComponent<RectTransform>();
                AnimateContinuousRotation(elementRect, i, totalSteps);
            }
        }

        // 물리적 감속 시간 계산 (대략 4-6초)
        float totalDuration = 1f + (totalSteps * 0.15f); // 초기 속도 + 감속 시간
        DOVirtual.DelayedCall(totalDuration, onComplete);
    }

    private void AnimateContinuousRotation(RectTransform element, int startIndex, int totalSteps)
    {
        element.DOKill();

        // DOVirtual.Float으로 연속적인 회전 구현
        float currentStep = 0f;

        DOVirtual.Float(0f, totalSteps, 1f + (totalSteps * 0.15f), (value) =>
        {
            // 현재 진행도에 따라 위치 계산
            int currentPos = Mathf.FloorToInt(value) % 5;
            int nextPos = (currentPos + 1) % 5;
            float lerpProgress = value - Mathf.Floor(value);

            // 현재 위치와 다음 위치 사이를 보간
            int currentIndex = (startIndex + currentPos) % 5;
            int nextIndex = (startIndex + nextPos) % 5;

            Vector2 currentPosition = circlePositions[currentIndex];
            Vector2 nextPosition = circlePositions[nextIndex];
            Vector2 interpolatedPosition = Vector2.Lerp(currentPosition, nextPosition, lerpProgress);

            element.anchoredPosition = interpolatedPosition;
        })
        .SetEase(Ease.OutQuart);
    }

    private void AnimateSelectedElement(List<Image> images, TweenCallback onComplete)
    {
        int[] elementIndices = { 2, 3, 4, 5, 6 };

        // 4번 위치(targetPositionIndex=2)에 있는 요소 찾기
        RectTransform selectedElement = null;
        for (int i = 0; i < elementIndices.Length; i++)
        {
            int elementIndex = elementIndices[i];
            if (images.Count > elementIndex)
            {
                RectTransform elementRect = images[elementIndex].GetComponent<RectTransform>();
                // 4번 위치에 가장 가까운 요소 찾기
                if (Vector2.Distance(elementRect.anchoredPosition, circlePositions[targetPositionIndex]) < 10f)
                {
                    selectedElement = elementRect;
                    break;
                }
            }
        }

        if (selectedElement != null && images.Count > 7)
        {
            Image element7Image = images[7];

            Sequence selectedSequence = DOTween.Sequence();

            // 1. 0.7초에 걸쳐서 1.1배로 커지며 7번 요소 알파값을 1로
            selectedSequence.Append(selectedElement.DOScale(1.2f, 0.7f).SetEase(Ease.OutBack));
            selectedSequence.Join(element7Image.DOFade(1f, 0.7f));

            // 2. 다시 원래 크기로 돌아가며 7번 요소 알파값을 0으로
            selectedSequence.Append(selectedElement.DOScale(1f, 0.5f).SetEase(Ease.InBack));
            selectedSequence.Join(element7Image.DOFade(0f, 0.5f));

            selectedSequence.OnComplete(onComplete);
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private void AnimateElement1(RectTransform element1)
    {
        Vector2 originalPos = GetOriginalPosition(element1);
        element1.DOAnchorPos(originalPos, 0f);

        // 웅웅거리듯이 크기 변화 반복
        element1.DOScale(1.05f, 1.2f)
               .SetEase(Ease.InOutSine)
               .SetLoops(-1, LoopType.Yoyo);
    }

    private void AnimateElement7(RectTransform element7, Image element7Image)
    {
        Vector2 originalPos = GetOriginalPosition(element7);
        Vector2 newPos = new Vector2(originalPos.x - 31f, originalPos.y + 180f);

        element7.DOAnchorPos(newPos, 0f);
        element7.localScale = Vector3.one * 1.2f; // 1.2배로 크기 설정
        element7Image.color = new Color(element7Image.color.r, element7Image.color.g, element7Image.color.b, 0f);

        // 7번 요소를 0번 요소 위, 나머지 요소(2,3,4,5,6번) 아래로 배치
        element7.SetSiblingIndex(1); // 0번 다음, 나머지들보다 앞에

        element7.DORotate(new Vector3(0, 0, -360f), 1.2f, RotateMode.FastBeyond360)
               .SetEase(Ease.Linear)
               .SetLoops(-1, LoopType.Restart);
    }


    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            if (animationImages.Count > 1)
            {
                animationImages[1].GetComponent<RectTransform>().DOKill();
            }

            if (animationImages.Count > 7)
            {
                animationImages[7].GetComponent<RectTransform>().DOKill();
                animationImages[7].DOKill(); // 알파 애니메이션도 정지
            }

            int[] elementIndices = { 2, 3, 4, 5, 6 };
            for (int i = 0; i < elementIndices.Length; i++)
            {
                int elementIndex = elementIndices[i];
                if (animationImages.Count > elementIndex)
                {
                    animationImages[elementIndex].GetComponent<RectTransform>().DOKill();
                }
            }
        }

        isSpinning = false;
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

            RectTransform feather2 = images[2].GetComponent<RectTransform>();
            Vector2 feather20riginal = GetOriginalPosition(feather2);
            Vector2 feather2Target = feather20riginal + new Vector2(-8f, 8);
            feather2.DOAnchorPos(feather2Target, 1.1f) 
                   .SetLoops(-1, LoopType.Yoyo) 
                   .SetEase(Ease.InOutSine); 



            // 3번째 깃털 - 좌우로 크게 움직임 (좌측으로 더 많이)
            RectTransform feather3 = images[3].GetComponent<RectTransform>();
            Vector2 feather3Original = GetOriginalPosition(feather3);
            Vector2 feather3Target = feather3Original + new Vector2(-72f, 0);
            feather3.DOAnchorPos(feather3Target, 1.2f)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine);

            // 4번째 깃털 - 우측상단, 좌측하단 대각선으로 (좌측하단으로 더 많이)
            RectTransform feather4 = images[4].GetComponent<RectTransform>();
            Vector2 feather4Original = GetOriginalPosition(feather4);
            Vector2 feather4Target = feather4Original + new Vector2(-54f, -45f);
            feather4.DOAnchorPos(feather4Target, 1.4f)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine);

            // 5번째 깃털 - 우측상단, 좌측하단 대각선으로 더 크게 (좌측하단으로 더 많이)
            RectTransform feather5 = images[5].GetComponent<RectTransform>();
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



public class CardAnimation_Num_24 : CardAnimationBase
{
    private DG.Tweening.Tween _element1FloatingY;
    private DG.Tweening.Tween _element1FloatingX;

    protected override void ExecuteAnimation(List<Image> images)
    {
        StartAnimationCycle(images);
    }

    private void StartAnimationCycle(List<Image> images)
    {
        // 모든 요소를 초기 상태로 리셋
        ResetAllElements(images);

        // 1번 요소 독립적인 플로팅 효과 시작
        if (images.Count > 1)
        {
            StartElement1FloatingEffect(images[1].GetComponent<RectTransform>());
        }

        // 2,3,4,5,6,7,8 요소의 알파값을 0으로 설정
        SetElementsAlpha(images);
        // 2,3,4 요소 위치 설정
        SetElementsPosition(images);

        // 1초 후 애니메이션 시작
        DOVirtual.DelayedCall(1f, () =>
        {
            StartMovementAnimation(images);
        });
    }

    private void StartElement1FloatingEffect(RectTransform element1)
    {
        Vector2 originalPos = GetOriginalPosition(element1);

        // 기존 플로팅 애니메이션 정리
        if (_element1FloatingY != null) _element1FloatingY.Kill();
        if (_element1FloatingX != null) _element1FloatingX.Kill();

        // 상하 움직임 (20씩)
        _element1FloatingY = element1.DOAnchorPosY(originalPos.y + 20f, 2f)
               .SetEase(Ease.InOutSine)
               .SetLoops(-1, LoopType.Yoyo);

        // 좌우 움직임 (20씩)
        _element1FloatingX = element1.DOAnchorPosX(originalPos.x + 20f, 1.5f)
               .SetEase(Ease.InOutSine)
               .SetLoops(-1, LoopType.Yoyo);
    }

    private void ResetAllElements(List<Image> images)
    {
        // 기존 모든 애니메이션 정리 (1번 요소 플로팅 제외)
        KillAllTweensExceptElement1Floating();

        int[] elementIndices = { 1, 2, 3, 4, 5, 6, 7, 8 };
        for (int i = 0; i < elementIndices.Length; i++)
        {
            int elementIndex = elementIndices[i];
            if (images.Count > elementIndex)
            {
                RectTransform rectTransform = images[elementIndex].GetComponent<RectTransform>();

                // 1번 요소는 위치 초기화하지 않음
                if (elementIndex != 1)
                {
                    // 원래 위치로 복원
                    Vector2 originalPos = GetOriginalPosition(rectTransform);
                    rectTransform.DOAnchorPos(originalPos, 0f);
                }

                // 크기 초기화
                rectTransform.localScale = Vector3.one;

                // 회전 초기화
                rectTransform.rotation = Quaternion.identity;

                // 알파값 초기화
                images[elementIndex].color = new Color(
                    images[elementIndex].color.r,
                    images[elementIndex].color.g,
                    images[elementIndex].color.b,
                    1f
                );

                // 레이어 순서 초기화 (원래 순서대로)
                images[elementIndex].transform.SetSiblingIndex(elementIndex);
            }
        }
    }

    private void KillAllTweensExceptElement1Floating()
    {
        if (animationImages != null)
        {
            int[] elementIndices = { 2, 3, 4, 5, 6, 7, 8 };
            for (int i = 0; i < elementIndices.Length; i++)
            {
                int elementIndex = elementIndices[i];
                if (animationImages.Count > elementIndex)
                {
                    animationImages[elementIndex].GetComponent<RectTransform>().DOKill();
                    animationImages[elementIndex].DOKill(); // 알파 애니메이션도 정지
                }
            }
        }
    }

    private void SetElementsAlpha(List<Image> images)
    {
        int[] elementIndices = { 2, 3, 4, 5, 6, 7, 8 };
        for (int i = 0; i < elementIndices.Length; i++)
        {
            int elementIndex = elementIndices[i];
            if (images.Count > elementIndex)
            {
                images[elementIndex].color = new Color(
                    images[elementIndex].color.r,
                    images[elementIndex].color.g,
                    images[elementIndex].color.b,
                    0f
                );
            }
        }
    }

    private void SetElementsPosition(List<Image> images)
    {
        if (images.Count > 2)
        {
            Vector2 originalPos2 = GetOriginalPosition(images[2].GetComponent<RectTransform>());
            Vector2 newPos2 = new Vector2(originalPos2.x + 0f, originalPos2.y - 242f);
            images[2].GetComponent<RectTransform>().DOAnchorPos(newPos2, 0f);
            images[2].GetComponent<RectTransform>().localScale = Vector3.one * 0.4f;
        }
        if (images.Count > 3)
        {
            Vector2 originalPos3 = GetOriginalPosition(images[3].GetComponent<RectTransform>());
            Vector2 newPos3 = new Vector2(originalPos3.x + 21f, originalPos3.y - 220f);
            images[3].GetComponent<RectTransform>().DOAnchorPos(newPos3, 0f);
            images[3].GetComponent<RectTransform>().localScale = Vector3.one * 0.4f;
        }
        if (images.Count > 4)
        {
            Vector2 originalPos4 = GetOriginalPosition(images[4].GetComponent<RectTransform>());
            Vector2 newPos4 = new Vector2(originalPos4.x + 21f, originalPos4.y - 220f);
            images[4].GetComponent<RectTransform>().DOAnchorPos(newPos4, 0f);
            images[4].GetComponent<RectTransform>().localScale = Vector3.one * 0.4f;
        }
    }

    private void StartMovementAnimation(List<Image> images)
    {
        // 2번 요소 애니메이션
        if (images.Count > 2)
        {
            AnimateElement2(images[2].GetComponent<RectTransform>(), images[2], images);
        }

        // 3번 요소 애니메이션
        if (images.Count > 3)
        {
            AnimateElement3(images[3].GetComponent<RectTransform>(), images[3], images);
        }

        // 4번 요소 애니메이션
        if (images.Count > 4)
        {
            AnimateElement4(images[4].GetComponent<RectTransform>(), images[4], images);
        }

        if (images.Count > 2)
        {
            images[2].transform.SetSiblingIndex(1); // 2번을 1번 위치로 (1번은 자동으로 2로 밀림)
        }
        if (images.Count > 3)
        {
            images[3].transform.SetSiblingIndex(1); // 3번을 1번 위치로
        }
        if (images.Count > 4)
        {
            images[4].transform.SetSiblingIndex(1); // 4번을 1번 위치로
        }
    }

    private void AnimateElement2(RectTransform element2, Image element2Image, List<Image> images)
    {
        Vector2 originalPos = GetOriginalPosition(element2);
        Vector2 firstTarget = new Vector2(originalPos.x + 0f, originalPos.y + 151f);
        Vector2 secondTarget = new Vector2(originalPos.x + 0f, originalPos.y + 26f);

        Sequence element2Sequence = DOTween.Sequence();

        // 알파값을 1로
        element2Sequence.Join(element2Image.DOFade(1f, 0.3f));

        // 첫 번째 위치까지 1초
        element2Sequence.Append(element2.DOAnchorPos(firstTarget, 0.6f).SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            // 첫 번째 목표 도달 후 레이어 순서 변경
                            if (images.Count > 1)
                            {
                                int element1Index = images[1].transform.GetSiblingIndex();
                                element2.SetSiblingIndex(element1Index + 1);
                            }
                        }));

        // 두 번째 위치까지 천천히 (2초)
        element2Sequence.Append(element2.DOAnchorPos(secondTarget, 1.2f).SetEase(Ease.InOutSine))
                        .OnComplete(() => {
                            // 2번이 최종 위치에 도달했을 때 후속 애니메이션 시작
                            StartPostAnimation(images);

                            // 두둥실 떠있는 효과 시작
                            StartFloatingEffect(element2, secondTarget);
                        });

        // 추가 기능들 (동시 진행)
        // 크기를 0.4에서 1로 서서히 (총 3초)
        element2.DOScale(1f, 1.8f).SetEase(Ease.OutQuad);

        // z축 회전 (속도 1.8초)
        element2.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360)
               .SetEase(Ease.Linear)
               .SetLoops(-1, LoopType.Restart);
    }

    private void AnimateElement3(RectTransform element3, Image element3Image, List<Image> images)
    {
        Vector2 originalPos = GetOriginalPosition(element3);
        Vector2 firstTarget = new Vector2(originalPos.x - 329f, originalPos.y - 291f);
        Vector2 secondTarget = new Vector2(originalPos.x - 238f, originalPos.y - 254f);

        Sequence element3Sequence = DOTween.Sequence();

        // 알파값을 1로
        element3Sequence.Join(element3Image.DOFade(1f, 0.3f));

        // 첫 번째 위치까지 1초
        element3Sequence.Append(element3.DOAnchorPos(firstTarget, 0.6f).SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            // 첫 번째 목표 도달 후 레이어 순서 변경
                            if (images.Count > 1)
                            {
                                int element1Index = images[1].transform.GetSiblingIndex();
                                element3.SetSiblingIndex(element1Index + 2);
                            }
                        }));

        // 두 번째 위치까지 천천히 (2초)
        element3Sequence.Append(element3.DOAnchorPos(secondTarget, 1.2f).SetEase(Ease.InOutSine));

        // 추가 기능들 (동시 진행)
        // 크기를 0.4에서 1로 서서히 (총 3초)
        element3.DOScale(1f, 1.8f).SetEase(Ease.OutQuad);

        // z축 회전 (속도 1.8초)
        element3.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360)
               .SetEase(Ease.Linear)
               .SetLoops(-1, LoopType.Restart);
    }

    private void AnimateElement4(RectTransform element4, Image element4Image, List<Image> images)
    {
        Vector2 originalPos = GetOriginalPosition(element4);
        Vector2 firstTarget = new Vector2(originalPos.x + 351f, originalPos.y - 265f);
        Vector2 secondTarget = new Vector2(originalPos.x + 267f, originalPos.y - 243f);

        Sequence element4Sequence = DOTween.Sequence();

        // 알파값을 1로
        element4Sequence.Join(element4Image.DOFade(1f, 0.3f));

        // 첫 번째 위치까지 1초
        element4Sequence.Append(element4.DOAnchorPos(firstTarget, 0.6f).SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            // 첫 번째 목표 도달 후 레이어 순서 변경
                            if (images.Count > 1)
                            {
                                int element1Index = images[1].transform.GetSiblingIndex();
                                element4.SetSiblingIndex(element1Index + 3);
                            }
                        }));

        // 두 번째 위치까지 천천히 (2초)
        element4Sequence.Append(element4.DOAnchorPos(secondTarget, 1.2f).SetEase(Ease.InOutSine));

        // 추가 기능들 (동시 진행)
        // 크기를 0.4에서 1로 서서히 (총 3초)
        element4.DOScale(1f, 1.8f).SetEase(Ease.OutQuad);

        // z축 회전 (속도 1.8초)
        element4.DORotate(new Vector3(0, 0, 360f),0.5f, RotateMode.FastBeyond360)
               .SetEase(Ease.Linear)
               .SetLoops(-1, LoopType.Restart);
    }

    private void StartFloatingEffect(RectTransform element, Vector2 centerPos)
    {
        // 상하 움직임 (아주 약간씩)
        element.DOAnchorPosY(centerPos.y + 5f, 2f)
               .SetEase(Ease.InOutSine)
               .SetLoops(-1, LoopType.Yoyo);

        // 좌우 움직임 (아주 약간씩)
        element.DOAnchorPosX(centerPos.x + 3f, 1.5f)
               .SetEase(Ease.InOutSine)
               .SetLoops(-1, LoopType.Yoyo);
    }

    private void StartPostAnimation(List<Image> images)
    {
        // 8번 요소의 점멸 효과 (0.1초만에 1로, 0.1초만에 0으로)
        if (images.Count > 8)
        {
            images[8].DOFade(1f, 0f).OnComplete(() => {
                images[8].DOFade(0f, 0.5f);

                // 2,3,4번 요소의 알파값을 0으로
                if (images.Count > 2) images[2].DOFade(0f, 0f);
                if (images.Count > 3) images[3].DOFade(0f, 0f);
                if (images.Count > 4) images[4].DOFade(0f, 0f);

                // 5,6,7을 2,3,4의 위치로 이동시키고 알파값을 1로 설정
                StartReplaceAnimation(images);
            });
        }


    }

    private void StartReplaceAnimation(List<Image> images)
    {
        // 5번을 2번 위치로
        if (images.Count > 5 && images.Count > 2)
        {
            Vector2 pos2 = images[2].GetComponent<RectTransform>().anchoredPosition;
            images[5].GetComponent<RectTransform>().DOAnchorPos(pos2, 0f);
            images[5].DOFade(1f, 0f);

            // 1초 유지 후 2초에 걸쳐 알파값 0
            images[5].DOFade(0f, 2f).SetDelay(1f).OnComplete(() => {
                // 전체 애니메이션 다시 시작 (3초 후)
                DOVirtual.DelayedCall(1.2f, () => {
                    StartAnimationCycle(images);
                });
            });
        }

        // 6번을 3번 위치로
        if (images.Count > 6 && images.Count > 3)
        {
            Vector2 pos3 = images[3].GetComponent<RectTransform>().anchoredPosition;
            images[6].GetComponent<RectTransform>().DOAnchorPos(pos3, 0f);
            images[6].DOFade(1f, 0f);

            // 1초 유지 후 2초에 걸쳐 알파값 0
            images[6].DOFade(0f, 2f).SetDelay(1f);
        }

        // 7번을 4번 위치로
        if (images.Count > 7 && images.Count > 4)
        {
            Vector2 pos4 = images[4].GetComponent<RectTransform>().anchoredPosition;
            images[7].GetComponent<RectTransform>().DOAnchorPos(pos4, 0f);
            images[7].DOFade(1f, 0f);

            // 1초 유지 후 2초에 걸쳐 알파값 0
            images[7].DOFade(0f, 2f).SetDelay(1f);
        }
    }

    protected override void KillAllTweens()
    {
        // 1번 요소 플로팅 애니메이션 정리
        if (_element1FloatingY != null)
        {
            _element1FloatingY.Kill();
            _element1FloatingY = null;
        }
        if (_element1FloatingX != null)
        {
            _element1FloatingX.Kill();
            _element1FloatingX = null;
        }

        if (animationImages != null)
        {
            int[] elementIndices = { 1, 2, 3, 4, 5, 6, 7, 8 };
            for (int i = 0; i < elementIndices.Length; i++)
            {
                int elementIndex = elementIndices[i];
                if (animationImages.Count > elementIndex)
                {
                    animationImages[elementIndex].GetComponent<RectTransform>().DOKill();
                    animationImages[elementIndex].DOKill(); // 알파 애니메이션도 정지
                }
            }
        }
    }
}



public class CardAnimation_Num_103 : CardAnimationBase
{
    private bool isGeneratingClones = false;

    protected override void ExecuteAnimation(List<Image> images)
    {
        // 기존 복사본 생성 중단
        isGeneratingClones = false;

        // 2번 요소 (index 2) 위치 초기화
        if (images.Count > 2)
        {
            RectTransform element2 = images[2].GetComponent<RectTransform>();
            element2.anchoredPosition = new Vector2(-274f, -77f);
        }
        // 3번 요소 (index 3) 위치 초기화
        if (images.Count > 4)
        {
            RectTransform element3 = images[4].GetComponent<RectTransform>();
            element3.anchoredPosition = new Vector2(589f, 0f);
        }
        // 4~8번 요소 (index 4~8) 알파값 0으로 설정
        int[] elementsToHide = { 5, 6, 7, 8, 9 };
        for (int i = 0; i < elementsToHide.Length; i++)
        {
            int elementIndex = elementsToHide[i];
            if (images.Count > elementIndex)
            {
                images[elementIndex].color = new Color(
                    images[elementIndex].color.r,
                    images[elementIndex].color.g,
                    images[elementIndex].color.b,
                    0f
                );
            }
        }
        // 0.7초 딜레이 후 3번 요소 떨림 애니메이션 시작
        DOVirtual.DelayedCall(0.7f, () =>
        {
            StartElement3ShakeAnimation(images);
        });
    }
    private void StartElement3ShakeAnimation(List<Image> images)
    {
        if (images.Count > 4)
        {
            RectTransform element3 = images[4].GetComponent<RectTransform>();
            Vector2 originalPos = element3.anchoredPosition;
            // 2초에 걸쳐서 서서히 강하게 떨리기
            DOVirtual.Float(0f, 1f, 2f, (value) =>
            {
                float intensity = Mathf.Lerp(0f, 35f, value);
                Vector2 randomOffset = new Vector2(
                    Random.Range(-intensity, intensity),
                    Random.Range(-intensity, intensity)
                );
                element3.anchoredPosition = originalPos + randomOffset;
            })
            .OnComplete(() =>
            {
                // 떨림 최대치 도달 시 8번 요소 알파값 1로 만들고 3번 요소 위치 이동
                if (images.Count > 9)
                {
                    images[9].color = new Color(
                        images[9].color.r,
                        images[9].color.g,
                        images[9].color.b,
                        1f
                    );


                    images[9].DOFade(0f, 3f).SetDelay(0.8f).OnComplete(() =>
                    {
                        images[9].DOFade(1f, 0.5f).SetDelay(5f).OnComplete(() =>
                        {
                            // 복사본 생성 중단
                            isGeneratingClones = false;

                            Transform parentTransform = images[5].transform.parent;
                            for (int i = parentTransform.childCount - 1; i >= 0; i--)
                            {
                                Transform child = parentTransform.GetChild(i);
                                if (child.name.Contains("(Clone)")) Destroy(child.gameObject);
                                RectTransform element4 = images[4].GetComponent<RectTransform>();
                                element4.anchoredPosition = new Vector2(589f, 0f);

                                images[3].color = new Color(
                                      images[3].color.r,
                                      images[3].color.g,
                                      images[3].color.b,
                                      1f
                                  );
                            }
                            images[9].DOFade(0f, 0.5f).SetDelay(0.1f).OnComplete(() =>
                            {
                                DOVirtual.DelayedCall(1f, () =>
                                {
                                    ExecuteAnimation(images);
                                });
                            });
                        });
                    });

                    // 복사본 생성 및 애니메이션
                    CreateAndAnimateClones(images);

                    // 2번 요소 계속 z축 회전 시작
                    if (images.Count > 2)
                    {
                        RectTransform element2 = images[2].GetComponent<RectTransform>();
                        // 회전 속도 1/4로 줄임 (8초에 한 바퀴)
                        element2.DORotate(new Vector3(0, 0, -360f), 8f, RotateMode.FastBeyond360)
                               .SetEase(Ease.Linear)
                               .SetLoops(-1, LoopType.Restart);

                        // 크기 1~1.1 사이로 왔다갔다
                        element2.DOScale(1.1f, 1.5f)
                               .SetEase(Ease.InOutSine)
                               .SetLoops(-1, LoopType.Yoyo);
                    }
                }
                // 3번 요소 위치를 0,0으로 이동
                element3.anchoredPosition = new Vector2(0f, 0f);
                images[3].color = new Color(
                      images[3].color.r,
                      images[3].color.g,
                      images[3].color.b,
                      0f
                  );
            });
        }
    }
    private void CreateAndAnimateClones(List<Image> images)
    {
        isGeneratingClones = true;
        StartContinuousCloneGeneration(images);
    }

    private void StartContinuousCloneGeneration(List<Image> images)
    {
        if (!isGeneratingClones) return;

        int[] cloneIndices = { 5, 6, 7, 8 };

        // 랜덤한 딜레이로 계속 생성
        DOVirtual.DelayedCall(Random.Range(0.1f, 0.5f), () =>
        {
            if (!isGeneratingClones) return;

            // 랜덤한 요소 선택
            int randomElementIndex = cloneIndices[Random.Range(0, cloneIndices.Length)];

            // 랜덤한 위치 생성 (범위 내에서)
            float randomX = Random.Range(50f, 360f);
            float randomY;

            // bottom -40~-145 사이는 제외
            if (Random.Range(0f, 1f) > 0.5f)
            {
                // 위쪽 범위: 60 ~ -39
                randomY = Random.Range(-39f, 60f);
            }
            else
            {
                // 아래쪽 범위: -146 ~ -228
                randomY = Random.Range(-228f, -146f);
            }

            Vector2 randomPosition = new Vector2(randomX, randomY);

            if (images.Count > randomElementIndex)
            {
                // 복사본 생성
                GameObject clone = Instantiate(images[randomElementIndex].gameObject, images[randomElementIndex].transform.parent);
                Image cloneImage = clone.GetComponent<Image>();
                RectTransform cloneRect = clone.GetComponent<RectTransform>();

                // 랜덤 위치에 배치
                cloneRect.anchoredPosition = randomPosition;

                // 9번 인덱스 요소보다 아래 레이어로 설정
                if (images.Count > 9)
                {
                    int element9SiblingIndex = images[9].transform.GetSiblingIndex();
                    cloneRect.SetSiblingIndex(element9SiblingIndex - 1);
                }

                // 알파값 1로 설정 (생성 시)
                cloneImage.color = new Color(cloneImage.color.r, cloneImage.color.g, cloneImage.color.b, 1f);

                // 우측으로 400만큼 이동하면서 페이드아웃 시작 (1초 딜레이 후 2초 동안)
                Vector2 targetPos = randomPosition + new Vector2(400f, 0f); // 300f를 400f로 변경

                cloneRect.DOAnchorPos(targetPos, 3f).SetEase(Ease.OutQuad); // 전체 애니메이션 시간을 3초로 조정 (1초 유지 + 2초 이동)
                cloneImage.DOFade(0f, 2f).SetDelay(1f).OnComplete(() => { // 1초 딜레이 후 2초 동안 페이드아웃
                    if (clone != null) Destroy(clone);
                });
            }

            // 다시 호출해서 계속 생성
            StartContinuousCloneGeneration(images);
        });
    }



    protected override void KillAllTweens()
    {
        isGeneratingClones = false;

        if (animationImages != null)
        {
            if (animationImages.Count > 2)
            {
                animationImages[2].GetComponent<RectTransform>().DOKill();
            }
            if (animationImages.Count > 3)
            {
                animationImages[4].GetComponent<RectTransform>().DOKill();
            }
            if (animationImages.Count > 9)
            {
                animationImages[9].DOKill();
            }
            // 4~8번 요소들의 DOTween 정리
            int[] elementIndices = { 4, 5, 6, 7, 8 };
            for (int i = 0; i < elementIndices.Length; i++)
            {
                int elementIndex = elementIndices[i];
                if (animationImages.Count > elementIndex)
                {
                    animationImages[elementIndex].GetComponent<RectTransform>().DOKill();
                    animationImages[elementIndex].DOKill();
                }
            }
        }
    }
}



public class CardAnimation_Num_108 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        if (images.Count >= 7) // 7번 파츠까지 있는지 확인
        {
            SetElementsAlpha(images);

            DOVirtual.DelayedCall(0.6f, () =>
            {
                StartElement1Animation(images);
            });
        }
    }

    private void SetElementsAlpha(List<Image> images)
    {
        int[] elementIndices = { 2, 3, 4, 5, 6, 7 };
        for (int i = 0; i < elementIndices.Length; i++)
        {
            int elementIndex = elementIndices[i];
            if (images.Count > elementIndex)
            {
                images[elementIndex].color = new Color(
                    images[elementIndex].color.r,
                    images[elementIndex].color.g,
                    images[elementIndex].color.b,
                    0f
                );
            }
        }
    }

    private void StartElement1Animation(List<Image> images)
    {
        if (images.Count > 1)
        {
            RectTransform element1 = images[1].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(element1);
            Sequence element1Sequence = DOTween.Sequence();
            // 1초에 걸쳐서 left 40지점까지 이동
            element1Sequence.Append(element1.DOAnchorPos(originalPos + new Vector2(40f, 0f), 0.3f));
            // 0.1초만에 left -40지점까지 이동
            element1Sequence.Append(element1.DOAnchorPos(originalPos + new Vector2(-40f, 0f), 0.02f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    StartElement7AlphaAnimation(images);
                }));
        }
    }

    private void StartElement7AlphaAnimation(List<Image> images)
    {
        if (images.Count > 7)
        {
            Image element7 = images[7];
            // 7번 요소를 0.1초에 걸쳐 알파값 1로
            element7.DOFade(1f, 0.1f).OnComplete(() =>
            {
                // 알파값이 1이 되는 순간 3번 요소 알파값도 1로
                if (images.Count > 2)
                {
                    images[2].color = new Color(
                        images[2].color.r,
                        images[2].color.g,
                        images[2].color.b,
                        1f
                    );
                    images[2].DOFade(0f, 1f).SetDelay(0.4f);

                    // 1.1초 후 두 번째 애니메이션 시작
                    DOVirtual.DelayedCall(0.3f, () =>
                    {
                        StartSecondElement1Animation(images);
                    });
                }
                element7.DOFade(0f, 0.1f);
            });
        }
    }

    private void StartSecondElement1Animation(List<Image> images)
    {
        if (images.Count > 1)
        {
            RectTransform element1 = images[1].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(element1);
            Sequence element1Sequence2 = DOTween.Sequence();
            element1Sequence2.Append(element1.DOAnchorPos(originalPos + new Vector2(40f, 0f), 0.3f));
            element1Sequence2.Append(element1.DOAnchorPos(originalPos + new Vector2(-40f, 0f), 0.03f)
                  .SetEase(Ease.OutQuad)
                  .OnComplete(() =>
                  {
                      StartSecondElement7AlphaAnimation(images);
                  }));
        }
    }

    private void StartSecondElement7AlphaAnimation(List<Image> images)
    {
        if (images.Count > 7)
        {
            Image element7 = images[7];
            // 7번 요소를 0.1초에 걸쳐 알파값 1로
            element7.DOFade(1f, 0.1f).OnComplete(() =>
            {
                // 알파값이 1이 되는 순간 4번 요소 알파값도 1로
                if (images.Count > 3)
                {
                    images[3].color = new Color(
                        images[3].color.r,
                        images[3].color.g,
                        images[3].color.b,
                        1f
                    );
                    images[3].DOFade(0f, 1f).SetDelay(0.4f);

                    // 1.1초 후 세 번째 애니메이션 시작
                    DOVirtual.DelayedCall(0.3f, () =>
                    {
                        StartThirdElement1Animation(images);
                    });
                }
                element7.DOFade(0f, 0.1f);
            });
        }
    }

    private void StartThirdElement1Animation(List<Image> images)
    {
        if (images.Count > 1)
        {
            RectTransform element1 = images[1].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(element1);
            Sequence element1Sequence3 = DOTween.Sequence();
            element1Sequence3.Append(element1.DOAnchorPos(originalPos + new Vector2(40f, 0f), 0.5f));
            element1Sequence3.Append(element1.DOAnchorPos(originalPos + new Vector2(-40f, 0f), 0.03f)
                  .SetEase(Ease.OutQuad)
                  .OnComplete(() =>
                  {
                      StartThirdElement7AlphaAnimation(images);
                  }));
        }
    }

    private void StartThirdElement7AlphaAnimation(List<Image> images)
    {
        if (images.Count > 7)
        {
            Image element7 = images[7];
            // 7번 요소를 0.1초에 걸쳐 알파값 1로
            element7.DOFade(1f, 0.1f).OnComplete(() =>
            {
                // 알파값이 1이 되는 순간 5번 요소 알파값도 1로
                if (images.Count > 4)
                {
                    images[4].color = new Color(
                        images[4].color.r,
                        images[4].color.g,
                        images[4].color.b,
                        1f
                    );

                    // 5번, 6번 요소 복사본들과 함께 동시에 알파값 1로 설정
                    CreateAndAnimateAllElements(images);
                }
                element7.DOFade(0f, 0.1f);
            });
        }
    }

    private void CreateAndAnimateAllElements(List<Image> images)
    {
        // 인덱스 5 요소 복사본들 생성
        List<GameObject> allCopies = new List<GameObject>();

        if (images.Count > 5)
        {
            Vector2[] positions5 = {
                new Vector2(-110f, 198f),
                new Vector2(-523f, -121f),
                new Vector2(-126f, -308f)
            };

            for (int i = 0; i < positions5.Length; i++)
            {
                GameObject copy = GameObject.Instantiate(images[5].gameObject, images[5].transform.parent);
                Image copyImage = copy.GetComponent<Image>();
                RectTransform copyRect = copy.GetComponent<RectTransform>();
                copyRect.anchoredPosition = positions5[i];
                copyImage.color = new Color(copyImage.color.r, copyImage.color.g, copyImage.color.b, 1f);
                allCopies.Add(copy);
            }
        }

        // 인덱스 6 요소 복사본들 생성
        if (images.Count > 6)
        {
            Vector2[] positions6 = {
                new Vector2(-594f, 104f),
                new Vector2(-326f, 106f),
                new Vector2(-478f, -209f)
            };

            for (int i = 0; i < positions6.Length; i++)
            {
                GameObject copy = GameObject.Instantiate(images[6].gameObject, images[6].transform.parent);
                Image copyImage = copy.GetComponent<Image>();
                RectTransform copyRect = copy.GetComponent<RectTransform>();
                copyRect.anchoredPosition = positions6[i];
                copyImage.color = new Color(copyImage.color.r, copyImage.color.g, copyImage.color.b, 1f);
                allCopies.Add(copy);
            }
        }

        // 모든 복사본들 반짝거리는 애니메이션 (각각 다른 타이밍으로)
        for (int i = 0; i < allCopies.Count; i++)
        {
            GameObject copy = allCopies[i];
            RectTransform copyRect = copy.GetComponent<RectTransform>();

            // 각자 다른 시작 딜레이와 애니메이션 속도
            float startDelay = Random.Range(0f, 0.8f);
            float animSpeed = Random.Range(0.15f, 0.35f);

            DOVirtual.DelayedCall(startDelay, () =>
            {
                if (copyRect != null)
                {
                    copyRect.DOScale(0.1f, animSpeed)
                           .SetLoops(-1, LoopType.Yoyo)
                           .SetEase(Ease.InOutSine);
                }
            });
        }

        // 3초 후 모든 요소들 페이드 아웃
        DOVirtual.DelayedCall(3f, () =>
        {
            // 5번 요소(index 4) 페이드 아웃
            if (images.Count > 4)
            {
                images[4].DOFade(0f, 1f);
            }

            // 모든 복사본들 페이드 아웃 후 삭제
            foreach (GameObject copy in allCopies)
            {
                if (copy != null)
                {
                    Image copyImage = copy.GetComponent<Image>();
                    copyImage.DOFade(0f, 1f).OnComplete(() =>
                    {
                        if (copy != null) GameObject.Destroy(copy);
                    });
                }
            }

            // 1초 후 전체 애니메이션 다시 시작
            DOVirtual.DelayedCall(1f, () =>
            {
                StartElement1Animation(images);
            });
        });
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 1번 요소
            if (animationImages.Count > 1)
            {
                animationImages[1].GetComponent<RectTransform>().DOKill();
            }
            // 2번, 3번, 4번, 7번 요소
            if (animationImages.Count > 2)
            {
                animationImages[2].DOKill();
            }
            if (animationImages.Count > 3)
            {
                animationImages[3].DOKill();
            }
            if (animationImages.Count > 4)
            {
                animationImages[4].DOKill();
            }
            if (animationImages.Count > 7)
            {
                animationImages[7].DOKill();
            }
            int[] elementIndices = { 2, 3, 4, 5, 6, 7 };
            for (int i = 0; i < elementIndices.Length; i++)
            {
                int elementIndex = elementIndices[i];
                if (animationImages.Count > elementIndex)
                {
                    animationImages[elementIndex].GetComponent<RectTransform>().DOKill();
                    animationImages[elementIndex].DOKill();
                }
            }
        }
    }
}



public class CardAnimation_Num_109 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        // 2번 요소(인덱스 2) 알파값 0으로 설정
        if (images.Count > 2)
        {
            CanvasGroup element2CanvasGroup = images[2].GetComponent<CanvasGroup>();
            if (element2CanvasGroup == null)
            {
                element2CanvasGroup = images[2].gameObject.AddComponent<CanvasGroup>();
            }
            element2CanvasGroup.alpha = 0f;
        }

        // 딜레이 후 애니메이션 시작
        DOVirtual.DelayedCall(0f, () => {  // 1초 딜레이
            if (images.Count > 1)
            {
                RectTransform element1 = images[1].GetComponent<RectTransform>();
                StartRabbitJumpAnimation(element1, images);
            }
        });
    }
    private void StartRabbitJumpAnimation(RectTransform rabbit, List<Image> images)
    {
        Vector2 startPos = new Vector2(-164f, 30f);
        Vector2 endPos = new Vector2(156f, 30f);
        // 시작 위치로 설정
        rabbit.anchoredPosition = startPos;
        // 점프 애니메이션 시작
        JumpToPosition(rabbit, startPos, endPos, images, true, () => {
            // 첫 번째 점프 완료 후 반대로 점프
            JumpToPosition(rabbit, endPos, startPos, images, false, () => {
                // 무한 반복을 위해 다시 시작
                StartRabbitJumpAnimation(rabbit, images);
            });
        });
    }
    private void JumpToPosition(RectTransform rabbit, Vector2 fromPos, Vector2 toPos, List<Image> images, bool isLeftToRight, TweenCallback onComplete)
    {
        float jumpDuration = 1.2f;
        float jumpHeight = 200f;
        // 중간 지점 계산 (포물선의 정점)
        Vector2 midPos = new Vector2(
            (fromPos.x + toPos.x) / 2f,
            Mathf.Max(fromPos.y, toPos.y) + jumpHeight
        );
        Sequence jumpSequence = DOTween.Sequence();
        // 첫 번째 구간: 시작점에서 정점까지 (위로 뛰기)
        jumpSequence.Append(rabbit.DOAnchorPos(midPos, jumpDuration * 0.4f).SetEase(Ease.OutQuad));
        jumpSequence.Join(rabbit.DOScale(0.8f, jumpDuration * 0.4f).SetEase(Ease.OutQuad));
        // 3,4,5 요소들 스프링 애니메이션 시작 (점프와 동시에)
        StartSpringAnimations(images);
        // 2번 요소 복제본 소환
        SpawnElement2Clone(images, isLeftToRight);
        // 두 번째 구간: 정점에서 도착점까지 (떨어지기)
        jumpSequence.Append(rabbit.DOAnchorPos(toPos, jumpDuration * 0.6f).SetEase(Ease.InQuad));
        jumpSequence.Join(rabbit.DOScale(1f, jumpDuration * 0.6f).SetEase(Ease.InQuad));
        // 착지 후 잠깐 대기
        jumpSequence.AppendInterval(0.3f);
        jumpSequence.OnComplete(onComplete);
    }

    private void SpawnElement2Clone(List<Image> images, bool isLeftToRight)
    {
        if (images.Count <= 2) return;

        // 원본 2번 요소
        Image originalElement2 = images[2];

        // 복제본 생성
        GameObject cloneObj = Instantiate(originalElement2.gameObject, originalElement2.transform.parent);
        Image cloneImage = cloneObj.GetComponent<Image>();
        RectTransform cloneRect = cloneObj.GetComponent<RectTransform>();

        // CanvasGroup 추가 (알파값 조절용)
        CanvasGroup cloneCanvasGroup = cloneObj.GetComponent<CanvasGroup>();
        if (cloneCanvasGroup == null)
        {
            cloneCanvasGroup = cloneObj.AddComponent<CanvasGroup>();
        }

        // 생성 위치 설정
        Vector2 spawnPos = isLeftToRight ? new Vector2(-383f, 0f) : new Vector2(383f, 0f);
        cloneRect.anchoredPosition = spawnPos;

        // 알파값 0에서 시작
        cloneCanvasGroup.alpha = 0f;

        // 목표 위치 (500만큼 위로)
        Vector2 targetPos = new Vector2(spawnPos.x, spawnPos.y + 300f);

        // 알파값 페이드인 (0.2초)
        cloneCanvasGroup.DOFade(1f, 0.2f);

        // 위로 이동하면서 페이드아웃 처리
        float moveDuration = 2f;
        cloneRect.DOAnchorPos(targetPos, moveDuration).SetEase(Ease.OutQuad)
            .OnUpdate(() => {
                // 현재 위치에서 목표 위치까지의 거리 계산
                float remainingDistance = Vector2.Distance(cloneRect.anchoredPosition, targetPos);

                // 남은 거리가 100 이하일 때 페이드아웃 시작
                if (remainingDistance <= 100f && cloneCanvasGroup.alpha > 0f)
                {
                    float fadeOutProgress = 1f - (remainingDistance / 100f);
                    cloneCanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeOutProgress);
                }
            })
            .OnComplete(() => {
                // 애니메이션 완료 후 복제본 삭제
                Destroy(cloneObj);
            });
    }

    private void StartSpringAnimations(List<Image> images)
    {
        // 3번 요소 (index 2)
        if (images.Count > 3)
        {
            RectTransform element3 = images[3].GetComponent<RectTransform>();
            element3.DOPunchScale(Vector3.one * 0.1f, 0.8f, 10, 0.5f);
        }
        // 4번 요소 (index 3) - 0.05초 딜레이
        if (images.Count > 4)
        {
            RectTransform element4 = images[4].GetComponent<RectTransform>();
            DOVirtual.DelayedCall(0.05f, () => {
                element4.DOPunchScale(Vector3.one * 0.1f, 0.8f, 10, 0.5f);
            });
        }
        // 5번 요소 (index 4) - 0.1초 딜레이
        if (images.Count > 5)
        {
            RectTransform element5 = images[5].GetComponent<RectTransform>();
            DOVirtual.DelayedCall(0.1f, () => {
                element5.DOPunchScale(Vector3.one * 0.1f, 0.8f, 10, 0.5f);
            });
        }
    }
    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            if (animationImages.Count > 1)
            {
                animationImages[1].GetComponent<RectTransform>().DOKill();
            }
            if (animationImages.Count > 2)
            {
                animationImages[2].GetComponent<RectTransform>().DOKill();
            }
            if (animationImages.Count > 3)
            {
                animationImages[3].GetComponent<RectTransform>().DOKill();
            }
            if (animationImages.Count > 4)
            {
                animationImages[4].GetComponent<RectTransform>().DOKill();
            }
        }
    }
}



public class CardAnimation_Num_113 : CardAnimationBase
{
    private DG.Tweening.Tween _shakeTween;
    private DG.Tweening.Tween _repeatTween;

    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images == null || images.Count < 8) return;

        ReorderLayers(images);
        InitializeImageProperties(images);
        StartShakeAnimation(images);
    }

    private void ReorderLayers(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        UnityEngine.UI.Image elementToReorder = images[2];
        elementToReorder.transform.SetAsLastSibling();
    }

    private void InitializeImageProperties(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 모든 DOTween 정리
        for (int i = 1; i < images.Count; i++)
        {
            if (images[i] != null)
            {
                images[i].rectTransform.DOKill();
            }
        }

        // 1번 요소 초기화
        UnityEngine.RectTransform element1 = images[1].GetComponent<UnityEngine.RectTransform>();
        element1.anchoredPosition = new UnityEngine.Vector2(0f, -64f);
        element1.localScale = UnityEngine.Vector3.one * 0.15f;

        // 3-7번 요소들 원래 위치로 초기화
        images[3].rectTransform.anchoredPosition = UnityEngine.Vector2.zero;
        images[4].rectTransform.anchoredPosition = UnityEngine.Vector2.zero;
        images[5].rectTransform.anchoredPosition = UnityEngine.Vector2.zero;
        images[6].rectTransform.anchoredPosition = UnityEngine.Vector2.zero;
        images[7].rectTransform.anchoredPosition = UnityEngine.Vector2.zero;

        int[] elementsToHide = { 1, 3, 4, 5, 6, 7 };
        foreach (int index in elementsToHide)
        {
            if (images.Count > index)
            {
                UnityEngine.CanvasGroup canvasGroup = images[index].GetComponent<UnityEngine.CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = images[index].gameObject.AddComponent<UnityEngine.CanvasGroup>();
                }
                canvasGroup.alpha = 0f;
            }
        }
    }

    private void StartShakeAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        UnityEngine.UI.Image elementToShake = images[2];
        UnityEngine.RectTransform rectTransform = elementToShake.GetComponent<UnityEngine.RectTransform>();
        UnityEngine.Vector2 originalPosition = rectTransform.anchoredPosition;

        float shakeDuration = 2.2f;

        if (_shakeTween != null)
        {
            _shakeTween.Kill(true);
        }

        _shakeTween = DG.Tweening.DOVirtual.Float(0f, 1f, shakeDuration, (value) =>
        {
            float intensity = UnityEngine.Mathf.Lerp(1.5f, 20f, value);
            UnityEngine.Vector2 randomOffset = new UnityEngine.Vector2(
              UnityEngine.Random.Range(-intensity, intensity),
              UnityEngine.Random.Range(-intensity, intensity)
            );
            rectTransform.anchoredPosition = originalPosition + randomOffset;
        })
        .SetAutoKill(false)
        .OnComplete(() =>
        {
            // 떨림 애니메이션 종료 및 위치 복원
            rectTransform.anchoredPosition = originalPosition;

            // 1번 요소 크기 복원 (0.05초) 후 반복 애니메이션 추가
            images[1].rectTransform.DOScale(UnityEngine.Vector3.one, 0.05f).OnComplete(() =>
            {
                images[1].rectTransform.DOShakeScale(1f, 0.2f, 10, 90f, true)
            .SetLoops(-1, DG.Tweening.LoopType.Yoyo);
            });
            // 1번 요소 알파값 즉시 1로 변경
            images[1].GetComponent<UnityEngine.CanvasGroup>().alpha = 1f;

            // 3번부터 7번까지의 요소들 위치 및 알파값 변경
            images[3].rectTransform.DOAnchorPos(new UnityEngine.Vector2(-423, 0), 0.05f).SetEase(DG.Tweening.Ease.OutQuad).OnComplete(() =>
            {
                images[3].rectTransform.DOShakePosition(1f, 5f, 10, 90f, false, true)
            .SetLoops(-1, DG.Tweening.LoopType.Yoyo);
            });
            images[3].GetComponent<UnityEngine.CanvasGroup>().alpha = 1f;

            images[4].rectTransform.DOAnchorPos(new UnityEngine.Vector2(374, 118), 0.05f).SetEase(DG.Tweening.Ease.OutQuad).OnComplete(() =>
            {
                images[4].rectTransform.DOShakePosition(1f, 5f, 10, 90f, false, true)
            .SetLoops(-1, DG.Tweening.LoopType.Yoyo);
            });
            images[4].GetComponent<UnityEngine.CanvasGroup>().alpha = 1f;

            images[5].rectTransform.DOAnchorPos(new UnityEngine.Vector2(-364, -278), 0.05f).SetEase(DG.Tweening.Ease.OutQuad).OnComplete(() =>
            {
                images[5].rectTransform.DOShakePosition(1f, 5f, 10, 90f, false, true)
            .SetLoops(-1, DG.Tweening.LoopType.Yoyo);
            });
            images[5].GetComponent<UnityEngine.CanvasGroup>().alpha = 1f;

            images[6].rectTransform.DOAnchorPos(new UnityEngine.Vector2(393, -302), 0.05f).SetEase(DG.Tweening.Ease.OutQuad).OnComplete(() =>
            {
                images[6].rectTransform.DOShakePosition(1f, 5f, 10, 90f, false, true)
            .SetLoops(-1, DG.Tweening.LoopType.Yoyo);
            });
            images[6].GetComponent<UnityEngine.CanvasGroup>().alpha = 1f;

            images[7].rectTransform.DOAnchorPos(new UnityEngine.Vector2(497, -37), 0.05f).SetEase(DG.Tweening.Ease.OutQuad).OnComplete(() =>
            {
                images[7].rectTransform.DOShakePosition(1f, 5f, 10, 90f, false, true)
            .SetLoops(-1, DG.Tweening.LoopType.Yoyo);
            });
            images[7].GetComponent<UnityEngine.CanvasGroup>().alpha = 1f;



            _shakeTween = DG.Tweening.DOVirtual.Float(0f, 1f, 0.8f, (value) =>
            {
                float intensity = UnityEngine.Mathf.Lerp(15f, 2f, value);
                UnityEngine.Vector2 randomOffset = new UnityEngine.Vector2(
                  UnityEngine.Random.Range(-intensity, intensity),
                  UnityEngine.Random.Range(-intensity, intensity)
                );
                rectTransform.anchoredPosition = originalPosition + randomOffset;
            });


            // 추가 요청 사항: 2초 대기 후 알파값 0으로 만들기
            DG.Tweening.DOVirtual.DelayedCall(2f, () =>
            {
                DG.Tweening.Sequence fadeSequence = DG.Tweening.DOTween.Sequence();

                for (int i = 1; i < images.Count; i++)
                {
                    if (images[i] != null)
                    {
                        UnityEngine.CanvasGroup cg = images[i].GetComponent<UnityEngine.CanvasGroup>();
                        if (cg != null)
                        {
                            fadeSequence.Join(cg.DOFade(0f, 1f));
                        }
                    }
                }

                fadeSequence.OnComplete(() =>
                {
                    _repeatTween = DG.Tweening.DOVirtual.DelayedCall(1f, () =>
                    {
                        ExecuteAnimation(images);
                    });
                });
            });

            _shakeTween = null;
        });
    }

    protected override void KillAllTweens()
    {
        if (_shakeTween != null)
        {
            _shakeTween.Kill(true);
            _shakeTween = null;
        }

        if (_repeatTween != null)
        {
            _repeatTween.Kill(true);
            _repeatTween = null;
        }

        if (animationImages != null && animationImages.Count > 1)
        {
            animationImages[1].rectTransform.DOKill();
            for (int i = 3; i < 8; i++)
            {
                if (animationImages.Count > i)
                {
                    animationImages[i].rectTransform.DOKill();
                }
            }
        }
    }
}



public class CardAnimation_Num_123 : CardAnimationBase
{
    private GameObject centerObject;

    protected override void ExecuteAnimation(List<Image> images)
    {
        if (images.Count >= 10) // 9번 파츠까지 있는지 확인
        {
            // 1. 빈 오브젝트 생성
            CreateCenterObject(images);

            // 2. 빈 오브젝트를 0번 레이어 바로 위로 이동
            MoveToCorrectLayer(images);

            // 3. 1,2,3,4 요소들을 빈 오브젝트 하위로 이동
            MoveToCenterObject(images);

            // 4. 애니메이션 시작
            StartAnimations(images);
        }
    }

    private void CreateCenterObject(List<Image> images)
    {
        if (centerObject != null)
        {
            DestroyImmediate(centerObject);
        }

        centerObject = new GameObject("CenterRotator");
        RectTransform centerRect = centerObject.AddComponent<RectTransform>();

        // 첫 번째 요소의 부모를 기준으로 설정
        centerRect.SetParent(images[1].transform.parent, false);
        centerRect.anchoredPosition = new Vector2(0, -95f);
        centerRect.sizeDelta = Vector2.zero;
    }

    private void MoveToCorrectLayer(List<Image> images)
    {
        // 0번 요소 바로 위로 이동
        int zeroIndex = images[0].transform.GetSiblingIndex();
        centerObject.transform.SetSiblingIndex(zeroIndex + 1);
    }

    private void MoveToCenterObject(List<Image> images)
    {
        for (int i = 1; i <= 4; i++)
        {
            if (i < images.Count)
            {
                RectTransform elementRect = images[i].GetComponent<RectTransform>();

                // 월드 위치 저장
                Vector3 worldPos = elementRect.position;

                // 중심점 하위로 이동
                elementRect.SetParent(centerObject.transform);

                // 월드 위치 복원
                elementRect.position = worldPos;
            }
        }
    }

    private void StartAnimations(List<Image> images)
    {
        // 중심점 회전
        if (centerObject != null)
        {
            RectTransform centerRect = centerObject.GetComponent<RectTransform>();
            StartContinuousRotation(centerRect, 7f);
        }

        // 8번 요소 위치 설정 후 회전
        if (images.Count > 7)
        {
            RectTransform element8 = images[7].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(element8);
            element8.anchoredPosition = new Vector2(originalPos.x, 235f);
            StartContinuousRotation(element8, 7f);
        }

        // 9번 요소 위치 설정 후 회전
        if (images.Count > 8)
        {
            RectTransform element9 = images[8].GetComponent<RectTransform>();
            Vector2 originalPos = GetOriginalPosition(element9);
            element9.anchoredPosition = new Vector2(originalPos.x, 235f);
            StartContinuousRotation(element9, 2f);
        }

        // 10번 요소 크기 애니메이션
        if (images.Count > 9)
        {
            RectTransform element10 = images[9].GetComponent<RectTransform>();
            StartScaleAnimation(element10);
        }
    }

    private void StartContinuousRotation(RectTransform target, float rotationDuration)
    {
        target.DORotate(new Vector3(0, 0, -360f), rotationDuration, RotateMode.FastBeyond360)
              .SetEase(Ease.Linear)
              .SetLoops(-1, LoopType.Restart);
    }

    private void StartScaleAnimation(RectTransform target)
    {
        Vector3 baseScale = target.localScale;
        target.DOScale(baseScale * 0.85f, 1.75f)
              .SetEase(Ease.InOutSine)
              .SetLoops(-1, LoopType.Yoyo);
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 중심점 오브젝트 DOTween 정리
            if (centerObject != null)
            {
                centerObject.GetComponent<RectTransform>().DOKill();
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

        // 중심점 오브젝트 삭제
        if (centerObject != null)
        {
            DestroyImmediate(centerObject);
            centerObject = null;
        }
    }
}




public class CardAnimation_Num_133 : CardAnimationBase
{
    private DG.Tweening.Tween _shakeTween;
    private DG.Tweening.Tween _repeatTween;

    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images == null || images.Count < 8) return;

        ReorderLayers(images);
        InitializeImageProperties(images);
        StartFloatingAnimation(images);

        // 2초 딜레이 후 시작
        DG.Tweening.DOVirtual.DelayedCall(2f, () =>
        {
            StartSequentialAnimations(images);
        });
    }

    private void ReorderLayers(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        images[2].transform.SetSiblingIndex(images[1].transform.GetSiblingIndex());
        images[3].transform.SetSiblingIndex(images[2].transform.GetSiblingIndex());
        images[4].transform.SetSiblingIndex(images[3].transform.GetSiblingIndex());
        images[1].transform.SetAsLastSibling();
    }

    private void InitializeImageProperties(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 모든 DOTween 정리
        for (int i = 0; i < images.Count; i++)
        {
            if (images[i] != null)
            {
                images[i].rectTransform.DOKill();

                // CanvasGroup DOTween도 정리
                UnityEngine.CanvasGroup existingCG = images[i].GetComponent<UnityEngine.CanvasGroup>();
                if (existingCG != null)
                {
                    existingCG.DOKill();
                }
            }
        }

        // 2,3,4번 요소 알파값 강제로 0으로 설정
        int[] elementsToHide = { 2, 3, 4 };
        foreach (int index in elementsToHide)
        {
            if (images.Count > index)
            {
                UnityEngine.CanvasGroup canvasGroup = images[index].GetComponent<UnityEngine.CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = images[index].gameObject.AddComponent<UnityEngine.CanvasGroup>();
                }
                canvasGroup.alpha = 0f;
            }
        }
    }

    private void StartFloatingAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 5번 요소 위아래 움직임
        images[5].rectTransform.DOAnchorPosY(images[5].rectTransform.anchoredPosition.y + 10f, 1f)
            .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
            .SetEase(DG.Tweening.Ease.InOutSine);

        // 6번 요소 위아래 움직임 (약간 다른 타이밍)
        images[6].rectTransform.DOAnchorPosY(images[6].rectTransform.anchoredPosition.y + 15f, 1.2f)
            .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
            .SetEase(DG.Tweening.Ease.InOutSine);

        // 7번 요소 위아래 움직임 (또 다른 타이밍)
        images[7].rectTransform.DOAnchorPosY(images[7].rectTransform.anchoredPosition.y + 8f, 0.8f)
            .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
            .SetEase(DG.Tweening.Ease.InOutSine);
    }


    private void StartSequentialAnimations(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 2,3,4번 요소 알파값을 0.3초에 걸쳐서 1로 만들기
        DG.Tweening.Sequence fadeInSequence = DG.Tweening.DOTween.Sequence();
        int[] elementsToShow = { 2, 3, 4 };
        foreach (int index in elementsToShow)
        {
            if (images.Count > index)
            {
                UnityEngine.CanvasGroup canvasGroup = images[index].GetComponent<UnityEngine.CanvasGroup>();
                if (canvasGroup != null)
                {
                    fadeInSequence.Join(canvasGroup.DOFade(1f, 0.3f));
                }
            }
        }
        // 페이드인 완료 후 0.5초 딜레이 후에 1번 요소 떨기 시작
        fadeInSequence.OnComplete(() =>
        {
            DG.Tweening.DOVirtual.DelayedCall(0.1f, () =>
            {
                StartShakeAnimation(images);
            });
        });
    }

    private void StartShakeAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        UnityEngine.RectTransform rectTransform = images[1].GetComponent<UnityEngine.RectTransform>();
        UnityEngine.Vector2 originalPosition = rectTransform.anchoredPosition;

        float shakeDuration = 2f;

        if (_shakeTween != null)
        {
            _shakeTween.Kill(true);
        }

        _shakeTween = DG.Tweening.DOVirtual.Float(0f, 0.7f, shakeDuration, (value) =>
        {
            float intensity = UnityEngine.Mathf.Lerp(5f, 10f, value);
            UnityEngine.Vector2 randomOffset = new UnityEngine.Vector2(
              UnityEngine.Random.Range(-intensity, intensity),
              UnityEngine.Random.Range(-intensity, intensity)
            );
            rectTransform.anchoredPosition = originalPosition + randomOffset;
        })
        .SetAutoKill(false)
        .OnComplete(() =>
        {
            // 떨림 끝나고 부드럽게 원래 위치로 복원
            rectTransform.DOAnchorPos(originalPosition, 0.3f).SetEase(DG.Tweening.Ease.OutQuad).OnComplete(() =>
            {
                DG.Tweening.DOVirtual.DelayedCall(1f, () =>
                {
                    // 떨림이 끝나면 2,3,4번 요소 알파값을 1초에 걸쳐 0으로 만들기
                    DG.Tweening.Sequence fadeOutSequence = DG.Tweening.DOTween.Sequence();

                    int[] elementsToHide = { 2, 3, 4 };
                    foreach (int index in elementsToHide)
                    {
                        if (images.Count > index)
                        {
                            UnityEngine.CanvasGroup canvasGroup = images[index].GetComponent<UnityEngine.CanvasGroup>();
                            if (canvasGroup != null)
                            {
                                fadeOutSequence.Join(canvasGroup.DOFade(0f, 1f));
                            }
                        }
                    }

                    fadeOutSequence.OnComplete(() =>
                    {
                        _repeatTween = DG.Tweening.DOVirtual.DelayedCall(1.5f, () =>
                        {
                            StartSequentialAnimations(images);
                        });
                    });
                });
            });

            _shakeTween = null;
        });
    }

    protected override void KillAllTweens()
    {
        if (_shakeTween != null)
        {
            _shakeTween.Kill(true);
            _shakeTween = null;
        }

        if (_repeatTween != null)
        {
            _repeatTween.Kill(true);
            _repeatTween = null;
        }

        if (animationImages != null)
        {
            for (int i = 1; i < animationImages.Count; i++)
            {
                if (animationImages[i] != null)
                {
                    animationImages[i].rectTransform.DOKill();

                    // CanvasGroup의 DOTween도 정리
                    UnityEngine.CanvasGroup cg = animationImages[i].GetComponent<UnityEngine.CanvasGroup>();
                    if (cg != null)
                    {
                        cg.DOKill();
                    }
                }
            }
        }
    }
}



public class CardAnimation_Num_137 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        // 1번 요소를 가장 위쪽 레이어로 이동
        if (images.Count > 1)
        {
            images[1].transform.SetAsLastSibling();
        }

        // 10,11,12,13번 요소들 투명하게 설정
        SetElementsAlpha(images);

        // 2,3,4,5,6,7,8,9번 요소들 개별적으로 위아래 부드러운 움직임
        int[] floatingElements = { 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < floatingElements.Length; i++)
        {
            int elementIndex = floatingElements[i];
            if (images.Count > elementIndex)
            {
                RectTransform elementRect = images[elementIndex].GetComponent<RectTransform>();
                StartFloatingAnimation(elementRect, i);
            }
        }

        // 1초마다 복사본 생성 시작
        StartSpawningCopies(images);
    }

    private void SetElementsAlpha(List<Image> images)
    {
        int[] elementsToHide = { 10, 11, 12, 13 };
        for (int i = 0; i < elementsToHide.Length; i++)
        {
            int elementIndex = elementsToHide[i];
            if (images.Count > elementIndex)
            {
                images[elementIndex].color = new Color(
                    images[elementIndex].color.r,
                    images[elementIndex].color.g,
                    images[elementIndex].color.b,
                    0f
                );
            }
        }
    }

    private void StartFloatingAnimation(RectTransform element, int index)
    {
        Vector2 originalPos = GetOriginalPosition(element);

        // 각 요소마다 다른 값들
        float moveDistance = Random.Range(15f, 40f);
        float animationDuration = Random.Range(1.5f, 3.5f);
        float startDelay = Random.Range(0f, 2f);

        DOVirtual.DelayedCall(startDelay, () =>
        {
            if (element != null)
            {
                element.DOAnchorPosY(originalPos.y + moveDistance, animationDuration)
                       .SetEase(Ease.InOutSine)
                       .SetLoops(-1, LoopType.Yoyo);
            }
        });
    }

    private void StartSpawningCopies(List<Image> images)
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            SpawnRandomCopy(images);
        });
    }

    private void SpawnRandomCopy(List<Image> images)
    {
        // 10,11,12번 요소 중 랜덤 선택
        int[] sourceElements = { 10, 11, 12 };
        int randomIndex = Random.Range(0, sourceElements.Length);
        int selectedElement = sourceElements[randomIndex];

        if (images.Count > selectedElement)
        {
            // 복사본 생성
            GameObject copy = GameObject.Instantiate(images[selectedElement].gameObject, images[selectedElement].transform.parent);
            Image copyImage = copy.GetComponent<Image>();
            RectTransform copyRect = copy.GetComponent<RectTransform>();

            // 복사본을 1번 요소보다 뒤로 배치 (1번이 맨 위에 있으므로 그 바로 앞으로)
            if (images.Count > 1)
            {
                int element1Index = images[1].transform.GetSiblingIndex();
                copy.transform.SetSiblingIndex(element1Index - 1);
            }

            // 시작 위치 설정 (400, 43)
            Vector2 spawnPos = new Vector2(400f, 43f);
            copyRect.anchoredPosition = spawnPos;

            // 목표 위치 (-356, -263)
            Vector2 targetPos = new Vector2(-356f, -263f);

            // 알파값 0에서 시작
            copyImage.color = new Color(copyImage.color.r, copyImage.color.g, copyImage.color.b, 0f);

            // 이동과 페이드인 동시 진행
            copyRect.DOAnchorPos(targetPos, 1f).SetEase(Ease.InOutSine);
            copyImage.DOFade(1f, 0.5f);

            // 이동 완료 후 1번 요소 스프링 애니메이션과 복사본 삭제
            DOVirtual.DelayedCall(1f, () =>
            {
                // 1번 요소 스프링 애니메이션
                if (images.Count > 1)
                {
                    RectTransform element1 = images[1].GetComponent<RectTransform>();
                    element1.DOPunchScale(Vector3.one * 0.03f, 0.5f, 8, 0.8f);
                }

                // 13번 요소 생성 및 애니메이션
                if (images.Count > 13)
                {
                    GameObject element13Copy = GameObject.Instantiate(images[13].gameObject, images[13].transform.parent);
                    Image element13Image = element13Copy.GetComponent<Image>();
                    RectTransform element13Rect = element13Copy.GetComponent<RectTransform>();

                    // 시작 위치 (-300, 80)
                    element13Rect.anchoredPosition = new Vector2(-300f, 50f);

                    // 알파값 즉시 1로
                    element13Image.color = new Color(element13Image.color.r, element13Image.color.g, element13Image.color.b, 1f);

                    // 270까지 올라가면서 서서히 알파값 0
                    element13Rect.DOAnchorPosY(270f, 1f).SetEase(Ease.OutQuad);
                    element13Image.DOFade(0f, 1f).OnComplete(() =>
                    {
                        if (element13Copy != null)
                        {
                            GameObject.Destroy(element13Copy);
                        }
                    });
                }

                // 복사본 삭제
                if (copy != null)
                {
                    GameObject.Destroy(copy);
                }
            });

            // 다음 복사본 스폰 예약
            DOVirtual.DelayedCall(1f, () =>
            {
                SpawnRandomCopy(images);
            });
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 1번 요소 DOTween 정리
            if (animationImages.Count > 1)
            {
                animationImages[1].GetComponent<RectTransform>().DOKill();
            }

            int[] floatingElements = { 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = 0; i < floatingElements.Length; i++)
            {
                int elementIndex = floatingElements[i];
                if (animationImages.Count > elementIndex)
                {
                    animationImages[elementIndex].GetComponent<RectTransform>().DOKill();
                }
            }
        }
    }
}