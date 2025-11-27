using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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


public class CardAnimation_Num_1 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {

    }
    protected override void KillAllTweens()
    {
    }
}



public class CardAnimation_Num_3 : CardAnimationBase
{
    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 킬 트윈 및 초기화는 다음 애니메이션 요청 시 활성화됩니다.
        KillAllTweens();
        InitElements(images);

        // 여기에 메인 애니메이션 로직을 추가해주십시오.
    }

    private void InitElements(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 0. 전체 요소 초기화
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.rotation = UnityEngine.Quaternion.identity;
            images[i].rectTransform.localScale = UnityEngine.Vector3.one;
            SetAlpha(images[i], 1f);
        }

        // --- 기존 요청된 개별 위치 및 스케일 세팅 (1번 ~ 4번) ---

        // 1번 요소 (Index 1): (-322, 193, 0.3배)
        if (images.Count > 1) SetRect(images[1], -322f, 193f, 0.3f);

        // 2번 요소 (Index 2): (-271, 4, 0.3배)
        if (images.Count > 2) SetRect(images[2], -271f, 4f, 0.3f);

        // 3번 요소 (Index 3): (233, 292, 0.3배)
        if (images.Count > 3) SetRect(images[3], 233f, 292f, 0.3f);

        // 4번 요소 (Index 4): (374, 158, 0.3배)
        if (images.Count > 4) SetRect(images[4], 374f, 158f, 0.3f);

        // --- ⭐ 신규 요청된 개별 위치 및 스케일 세팅 (5번 ~ 6번) ⭐ ---

        // 5번 요소 (Index 5): (-300, 36, 1.7배)
        if (images.Count > 5) SetRect(images[5], -300f, 36f, 1.7f);

        // 6번 요소 (Index 6): (370, 135, 1.7배)
        if (images.Count > 6) SetRect(images[6], 370f, 135f, 1.7f);
    }

    // ===================================
    // 🛠️ 필수 헬퍼 함수
    // ===================================

    private void SetRect(UnityEngine.UI.Image img, float x, float y, float scale)
    {
        UnityEngine.RectTransform rt = img.rectTransform;
        rt.anchoredPosition = new UnityEngine.Vector2(x, y);
        rt.localScale = UnityEngine.Vector3.one * scale;
    }

    private void SetAlpha(UnityEngine.UI.Image img, float alpha)
    {
        img.color = new UnityEngine.Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    protected override void KillAllTweens()
    {
        // 여기에 모든 DOTween 트윈을 정리하는 로직을 추가해주십시오.
    }
}




public class CardAnimation_Num_4 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {

    }
    protected override void KillAllTweens()
    {
    }
}




public class CardAnimation_Num_5 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        // ====================================================================
        // 1. 초기 위치 및 스케일 설정 (8번부터 15번까지)
        // 기존 8번부터 19번까지의 설정은 삭제하고 아래 값으로 대체
        // ====================================================================

        // 8번: -325 470 / 0.6배
        if (images.Count > 8)
        {
            RectTransform rt = images[8].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-325f, 470f);
            rt.localScale = Vector3.one * 0.6f;
        }
        // 9번: -822 173 / 0.478배
        if (images.Count > 9)
        {
            RectTransform rt = images[9].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-822f, 173f);
            rt.localScale = Vector3.one * 0.478f;
        }
        // 10번: -716 -408 / 0.6배
        if (images.Count > 10)
        {
            RectTransform rt = images[10].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-716f, -408f);
            rt.localScale = Vector3.one * 0.6f;
        }
        // 11번: -391 -539 / 0.6배
        if (images.Count > 11)
        {
            RectTransform rt = images[11].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-391f, -539f);
            rt.localScale = Vector3.one * 0.6f;
        }
        // 12번: 566 -551 / 0.6배
        if (images.Count > 12)
        {
            RectTransform rt = images[12].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(566f, -551f);
            rt.localScale = Vector3.one * 0.6f;
        }
        // 13번: 791 -274 / 0.6배
        if (images.Count > 13)
        {
            RectTransform rt = images[13].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(791f, -274f);
            rt.localScale = Vector3.one * 0.6f;
        }
        // 14번: 787 180 / 0.6배
        if (images.Count > 14)
        {
            RectTransform rt = images[14].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(787f, 180f);
            rt.localScale = Vector3.one * 0.6f;
        }
        // 15번: 401 478 / 0.6배
        if (images.Count > 15)
        {
            RectTransform rt = images[15].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(401f, 478f);
            rt.localScale = Vector3.one * 0.6f;
        }

        // 16번 이후의 설정은 삭제됨

        // ====================================================================
        // 2. 애니메이션 시작 (0~8번 요소 애니메이션 유지)
        // ====================================================================

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

        // 풀 회전 애니메이션 (8번 인덱스만 유지, 9번 이후 요소는 삭제됨)
        int[] grassIndices = { 8, 9, 10,11,12,13,14,15 };
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
        float randomSpeed = Random.Range(1f, 2.3f);
        // 랜덤한 시작 지연 시간 (0 ~ 1초)
        float randomDelay = Random.Range(0f, 1f);

        // 시퀀스 생성
        Sequence rotationSequence = DOTween.Sequence();

        // 랜덤한 지연 시간 설정
        rotationSequence.AppendInterval(randomDelay);
        rotationSequence.Append(grassRect.DOLocalRotate(new Vector3(0, 0, -randomAngle), randomSpeed).SetEase(Ease.InOutSine));
        rotationSequence.Append(grassRect.DOLocalRotate(new Vector3(0, 0, randomAngle), randomSpeed * 2).SetEase(Ease.InOutSine));
        rotationSequence.Append(grassRect.DOLocalRotate(Vector3.zero, randomSpeed).SetEase(Ease.InOutSine));

        // 시퀀스 무한 반복
        rotationSequence.SetLoops(-1);
    }


    // 발톱 애니메이션
    private void StartClawAnimation(RectTransform claw, int order)
    {
        Vector2 originalPos = GetOriginalPosition(claw); // CardAnimationBase에 정의되어 있다고 가정
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
            // 2번 요소 (index 1) 트윈 Kill
            if (animationImages.Count > 1)
            {
                animationImages[1].DOKill();
            }

            // 발톱 요소 (index 3, 4, 5, 6) 트윈 Kill
            int[] clawIndices = { 3, 4, 5, 6 };
            for (int i = 0; i < clawIndices.Length; i++)
            {
                int clawIndex = clawIndices[i];
                if (clawIndex < animationImages.Count)
                {
                    animationImages[clawIndex].GetComponent<RectTransform>().DOKill();
                }
            }

            // 풀 흔들림 요소 (index 8) 트윈 Kill
            int[] grassIndices = { 8 };
            for (int i = 0; i < grassIndices.Length; i++)
            {
                int grassIndex = grassIndices[i];
                if (grassIndex < animationImages.Count)
                {
                    animationImages[grassIndex].GetComponent<RectTransform>().DOKill();
                }
            }
            // 9번 이후 요소 정리 로직은 모두 삭제되었습니다.
        }
    }
}



public class CardAnimation_Num_6 : CardAnimationBase
{
    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // ====================================================================
        // 초기 설정 (이전 단계에서 설정된 값)
        // ====================================================================
        SetPosAndScale(images, 2, 60f, -61f, 0.815f, 0.85f);
        SetPosAndScale(images, 3, 233f, -52f, 0.79f, 0.65f);
        SetPosAndScale(images, 4, 284f, -46f, 0.434f, 0.434f);
        SetPosAndScale(images, 5, 319f, -93f, 0.47f, 0.47f);
        SetPosAndScale(images, 6, 146f, 30f, 1.188f, 1.188f);

        for (int i = 7; i <= 10; i++)
        {
            SetPosAndScale(images, i, 20f, -6f, 1.0f, 1.0f);
        }

        SetPosAndScale(images, 11, -303f, 559f, 0.6f, 0.6f);
        SetPosAndScale(images, 12, -961f, 167f, 0.6f, 0.6f);
        SetPosAndScale(images, 13, -770f, -465f, 0.6f, 0.6f);
        SetPosAndScale(images, 14, -480f, -564f, 0.6f, 0.6f);
        SetPosAndScale(images, 15, 615f, -600f, 0.6f, 0.6f);
        SetPosAndScale(images, 16, 842f, -277f, 0.6f, 0.6f);
        SetPosAndScale(images, 17, 824f, 203f, 0.6f, 0.6f);
        SetPosAndScale(images, 18, 427f, 545f, 0.6f, 0.6f);

        // ====================================================================
        // 1. 맥동(Pulsing) 애니메이션 (1번, 2번, 3번, 4번 요소)
        // ====================================================================
        for (int i = 1; i <= 4; i++)
        {
            if (i < images.Count)
            {
                StartScalePulse(images[i].GetComponent<UnityEngine.RectTransform>(), 1.1f, 3f);
            }
        }

        // ====================================================================
        // 2. 고정 비율 이동 애니메이션 (7번, 8번, 9번, 10번 요소)
        // ====================================================================
        StartCoordinatedSwarm(images);

        // ====================================================================
        // 3. Z축 회전 애니메이션 (11번부터 18번 요소)
        // ====================================================================
        for (int i = 11; i <= 18; i++)
        {
            if (i < images.Count)
            {
                StartGentleRotation(images[i].GetComponent<UnityEngine.RectTransform>(), 5f, 1.5f);
            }
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 1~4번 (Scale), 7~10번 (Position), 11~18번 (Rotation) 트윈 모두 정리
            for (int i = 1; i <= 18; i++)
            {
                if (i < animationImages.Count)
                {
                    animationImages[i].GetComponent<UnityEngine.RectTransform>().DOKill(true);
                }
            }
        }
    }

    // ====================================================================
    // A. 개별 애니메이션 함수
    // ====================================================================

    // 1. 맥동 애니메이션
    private void StartScalePulse(UnityEngine.RectTransform rect, float maxScale, float duration)
    {
        // 참고: 초기 설정 스케일 값을 기준으로 10% 증감합니다.
        float initialScale = rect.localScale.x;
        float targetMax = initialScale * maxScale;
        float targetMin = initialScale * 0.95f;

        // 먼저 최대치로 갔다가 최소치로 돌아오는 Yoyo 루프 설정
        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence().SetTarget(rect);

        seq.Append(rect.DOScale(targetMax, duration).SetEase(DG.Tweening.Ease.InOutSine));
        seq.Append(rect.DOScale(targetMin, duration).SetEase(DG.Tweening.Ease.InOutSine));

        seq.SetLoops(-1, DG.Tweening.LoopType.Yoyo);
    }

    // 2. 고정 비율 이동 애니메이션 (7, 8, 9, 10)
    private void StartCoordinatedSwarm(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 7번은 기준점이므로 애니메이션 없음
        if (images.Count <= 7) return;

        // 7번 요소의 초기 위치를 기준점으로 사용
        UnityEngine.Vector2 anchorPos = images[7].GetComponent<UnityEngine.RectTransform>().anchoredPosition;
        float baseDistance = 60f; // 8번 요소의 기본 이동 거리

        // 고정 거리 비율: 8번(1.0) < 9번(1.5) < 10번(2.0)
        float[] ratios = { 0.0f, 0.4f, 0.8f, 1.2f };

        for (int i = 8; i <= 10; i++)
        {
            if (i >= images.Count) break;

            UnityEngine.RectTransform rt = images[i].GetComponent<UnityEngine.RectTransform>();
            float ratio = ratios[i - 7];

            // 좌상단 이동 벡터 (거리 * 비율)
            UnityEngine.Vector2 moveVector = new UnityEngine.Vector2(-1f, 1f) * (baseDistance * ratio);
            UnityEngine.Vector2 startPos = rt.anchoredPosition;

            DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence().SetTarget(rt);

            // 1. 좌상단으로 이동 (2.0s)
            seq.Append(rt.DOAnchorPos(startPos + moveVector, 2.5f).SetEase(DG.Tweening.Ease.InOutSine));

            // 2. 원위치로 복귀 (2.0s)
            seq.Append(rt.DOAnchorPos(startPos, 2.5f).SetEase(DG.Tweening.Ease.InOutSine));

            seq.SetLoops(-1, DG.Tweening.LoopType.Yoyo);
        }
    }

    // 3. Z축 회전 애니메이션
    private void StartGentleRotation(UnityEngine.RectTransform rect, float angle, float duration)
    {
        // -angle <-> +angle을 오가며 부드럽게 회전
        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence().SetTarget(rect);

        // 1. -angle 회전
        seq.Append(rect.DOLocalRotate(new UnityEngine.Vector3(0, 0, -angle), duration).SetEase(DG.Tweening.Ease.InOutSine));
        // 2. +angle 회전
        seq.Append(rect.DOLocalRotate(new UnityEngine.Vector3(0, 0, angle), duration * 2).SetEase(DG.Tweening.Ease.InOutSine));
        // 3. 0도로 복귀
        seq.Append(rect.DOLocalRotate(UnityEngine.Vector3.zero, duration).SetEase(DG.Tweening.Ease.InOutSine));

        seq.SetLoops(-1, DG.Tweening.LoopType.Restart);
    }

    // ========================================================================
    // B. 보조 함수 (FQN 사용)
    // ========================================================================

    private void SetPosAndScale(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float x, float y, float scaleX, float scaleY)
    {
        if (index < imgs.Count)
        {
            UnityEngine.RectTransform rt = imgs[index].GetComponent<UnityEngine.RectTransform>();
            if (rt == null) return;

            rt.anchoredPosition = new UnityEngine.Vector2(x, y);
            rt.localScale = new UnityEngine.Vector3(scaleX, scaleY, 1f);
        }
    }
}


public class CardAnimation_Num_7 : CardAnimationBase
{
    private const float InitialY_2 = -140f;

    protected override void ExecuteAnimation(List<Image> images)
    {
        // ====================================================================
        // 1. 초기 설정 (크기, 위치, 알파)
        // ====================================================================

        // 1번 요소 (미지정: 기본값 유지)
        if (images.Count > 1)
        {
            images[1].rectTransform.localScale = Vector3.one * 1f;
            SetPosition(images, 1, 0f, 0f);
            SetAlphaZero(images, 1); // 1번 요소 알파 0 설정
        }

        // 2번 요소: 0, -128 / 0.25배
        if (images.Count > 2)
        {
            images[2].rectTransform.localScale = Vector3.one * 0.25f;
            SetPosition(images, 2, 0f, InitialY_2);
        }

        // 3번 요소부터 10번 요소까지 초기 설정은 유지됩니다. (생략) 
        if (images.Count > 3) SetPosition(images, 3, 0f, -550f); images[3].rectTransform.localScale = Vector3.one * 0.54f;
        if (images.Count > 4) SetPosition(images, 4, -237f, -376f); images[4].rectTransform.localScale = Vector3.one * 0.285f;
        if (images.Count > 5) SetPosition(images, 5, -186f, -326f); images[5].rectTransform.localScale = Vector3.one * 0.32f;
        if (images.Count > 6) SetPosition(images, 6, 168f, -356f); images[6].rectTransform.localScale = Vector3.one * 0.28f;
        if (images.Count > 7) SetPosition(images, 7, 605f, 316f); images[7].rectTransform.localScale = Vector3.one * 0.4f;
        if (images.Count > 8) SetPosition(images, 8, -396f, 203f); images[8].rectTransform.localScale = Vector3.one * 0.23f;
        if (images.Count > 9) SetPosition(images, 9, 739f, -243f); images[9].rectTransform.localScale = Vector3.one * 0.4f;
        if (images.Count > 10) SetPosition(images, 10, -764f, -232f); images[10].rectTransform.localScale = Vector3.one * 0.42f;

        // 11번 요소 초기 설정
        if (images.Count > 11)
        {
            images[11].rectTransform.localScale = Vector3.one * 1f;
            SetPosition(images, 11, 0f, 0f);
            SetAlphaZero(images, 11);
        }

        // 레이어 순서 조정은 이전과 동일하게 유지됩니다. (생략)
        if (images.Count > 10)
        {
            int baseIndex = images[0].rectTransform.GetSiblingIndex();
            int currentSiblingIndex = baseIndex + 1;

            images[1].rectTransform.SetSiblingIndex(currentSiblingIndex++); images[4].rectTransform.SetSiblingIndex(currentSiblingIndex++);
            images[5].rectTransform.SetSiblingIndex(currentSiblingIndex++); images[6].rectTransform.SetSiblingIndex(currentSiblingIndex++);
            images[3].rectTransform.SetSiblingIndex(currentSiblingIndex++); images[2].rectTransform.SetSiblingIndex(currentSiblingIndex++);
            images[7].rectTransform.SetSiblingIndex(currentSiblingIndex++); images[8].rectTransform.SetSiblingIndex(currentSiblingIndex++);
            images[9].rectTransform.SetSiblingIndex(currentSiblingIndex++); images[10].rectTransform.SetSiblingIndex(currentSiblingIndex++);
            if (images.Count > 11) images[11].rectTransform.SetAsLastSibling();
        }

        // ====================================================================
        // 3. 반복 애니메이션 (7, 8, 9, 10) - 유지
        // ====================================================================

        const float floatDistance = 15f;
        const float floatDuration = 4.0f;
        const float rotateDuration7_8 = 10f;
        const float fullRotation = 360f;
        const float loopRotateDuration9_10 = 1.5f;
        const float loopRotateAngle = 5f;

        // 7, 8, 9, 10번 요소의 연속 애니메이션 로직은 유지됩니다. (생략)
        if (images.Count > 7) { RectTransform rt7 = images[7].rectTransform; Vector2 initialPos7 = rt7.anchoredPosition; rt7.DOAnchorPos(initialPos7 + new Vector2(floatDistance, floatDistance), floatDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetTarget(rt7); rt7.DOLocalRotate(new Vector3(0, 0, fullRotation), rotateDuration7_8, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).SetTarget(rt7); }
        if (images.Count > 8) { RectTransform rt8 = images[8].rectTransform; Vector2 initialPos8 = rt8.anchoredPosition; rt8.DOAnchorPos(initialPos8 + new Vector2(-floatDistance * 0.7f, floatDistance * 0.7f), floatDuration * 1.2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetTarget(rt8); rt8.DOLocalRotate(new Vector3(0, 0, -fullRotation), rotateDuration7_8 * 1.5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).SetTarget(rt8); }
        if (images.Count > 9) { images[9].rectTransform.DOLocalRotate(new Vector3(0, 0, loopRotateAngle), loopRotateDuration9_10).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetTarget(images[9].rectTransform); }
        if (images.Count > 10) { images[10].rectTransform.DOLocalRotate(new Vector3(0, 0, -loopRotateAngle * 1.2f), loopRotateDuration9_10 * 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetTarget(images[10].rectTransform); }


        // ====================================================================
        // 4. 메인 무한 루프 애니메이션 (2, 11, 1)
        // ====================================================================

        if (images.Count > 11)
        {
            const float TremorTotalDuration = 1.5f;
            const float FlashDuration = 0.1f;
            const float WaitDuration = 0.2f;
            const float Fade11OutDuration = 0.8f;
            const float ReturnDuration = 1.0f;
            const float MoveY_2 = 300f;
            const float TremorAngle = 5f;

            Sequence mySequence = DOTween.Sequence()
                .SetLoops(-1, LoopType.Restart)
                .SetTarget(this);

            // ----------------------------------------------------------------------
            // P1. 2번 요소 떨림 가속 (총 1.5s)
            // ----------------------------------------------------------------------

            Sequence tremorSequence = DOTween.Sequence();
            tremorSequence.SetTarget(images[2].rectTransform);

            // 1. 느린 떨림 (0.5s)
            tremorSequence.Append(images[2].rectTransform.DOLocalRotate(new Vector3(0, 0, TremorAngle * 1.5f), 0.25f).SetEase(Ease.OutSine));
            tremorSequence.Append(images[2].rectTransform.DOLocalRotate(new Vector3(0, 0, -TremorAngle * 1.5f), 0.25f).SetEase(Ease.OutSine));

            // 2. 중간 떨림 (0.5s)
            tremorSequence.Append(images[2].rectTransform.DOLocalRotate(new Vector3(0, 0, TremorAngle * 3f), 0.2f).SetEase(Ease.OutSine));
            tremorSequence.Append(images[2].rectTransform.DOLocalRotate(new Vector3(0, 0, -TremorAngle * 3f), 0.3f).SetEase(Ease.OutSine));

            // 3. 최대치 떨림 직전 (0.5s)
            tremorSequence.Append(images[2].rectTransform.DOLocalRotate(new Vector3(0, 0, TremorAngle * 7f), 0.25f).SetEase(Ease.OutQuad));
            tremorSequence.Append(images[2].rectTransform.DOLocalRotate(new Vector3(0, 0, -TremorAngle * 7f), 0.25f).SetEase(Ease.OutQuad));

            mySequence.Append(tremorSequence);

            // ----------------------------------------------------------------------
            // P2. 떨림 종료 시점 (Flash & Move/Alpha 통합)
            // ----------------------------------------------------------------------

            // 1. 떨림이 끝난 직후, 2번 요소의 회전을 0초 트윈으로 즉시 복구합니다.
            mySequence.Append(images[2].rectTransform.DOLocalRotate(Vector3.zero, 0f));

            // 2. Flash (0.1s) 시작: 11번 알파 1
            mySequence.Append(GetCanvasGroup(images[11]).DOFade(1f, FlashDuration));

            // 3. Join: 2번 요소 이동 (+300Y) 및 1번 등장 (Alpha 1) - 0.1s 동안 동시 처리
            mySequence.Join(images[2].rectTransform.DOAnchorPosY(InitialY_2 + MoveY_2, FlashDuration));
            mySequence.Join(GetCanvasGroup(images[1]).DOFade(1f, FlashDuration));

            // ----------------------------------------------------------------------
            // P3 & P4. Fade Out 11 & Return
            // ----------------------------------------------------------------------

            // 11번 요소의 알파값 0.2초 뒤에 0.8초에 걸쳐서 0으로 돌아감
            mySequence.AppendInterval(WaitDuration);
            mySequence.Append(GetCanvasGroup(images[11]).DOFade(0f, Fade11OutDuration));

            // 이후 2번 요소 1초에 걸쳐 제자리 복귀 + 1번 요소 1초에 걸쳐 알파 0
            mySequence.Append(images[2].rectTransform.DOAnchorPosY(InitialY_2, ReturnDuration));
            mySequence.Join(GetCanvasGroup(images[1]).DOFade(0f, ReturnDuration));

            // 다음 루프를 위해 최종 회전 복구는 이미 0으로 설정되어 있습니다.
        }
    }

    private void SetPosition(List<Image> images, int index, float x, float y)
    {
        if (index < images.Count)
        {
            images[index].rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    // CanvasGroup을 가져오거나 없으면 추가하여 반환
    private CanvasGroup GetCanvasGroup(Image image)
    {
        CanvasGroup cg = image.GetComponent<CanvasGroup>();
        if (cg == null) cg = image.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    private void SetAlphaZero(List<Image> images, int index)
    {
        if (index < images.Count)
        {
            GetCanvasGroup(images[index]).alpha = 0f;
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null) img.rectTransform.DOKill();
            }
        }
        DOTween.Kill(this);
    }
}
public class CardAnimation_Num_8 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        // ====================================================================
        // 1. 초기 설정 (위치 + scale)
        // ====================================================================
        if (images.Count > 1) { images[1].rectTransform.localScale = Vector3.one * 1f; SetPos(images, 1, 82f, -122f); }
        if (images.Count > 2) { images[2].rectTransform.localScale = Vector3.one * 1f; SetPos(images, 2, 82f, -122f); }
        if (images.Count > 3) { images[3].rectTransform.localScale = Vector3.one * 1f; SetPos(images, 3, 82f, -122f); }
        if (images.Count > 4) { images[4].rectTransform.localScale = Vector3.one * 1f; SetPos(images, 4, 82f, -122f); }
        if (images.Count > 5) { images[5].rectTransform.localScale = Vector3.one * 0.45f; SetPos(images, 5, 68f, 23f); }
        if (images.Count > 6) { images[6].rectTransform.localScale = Vector3.one * 0.3f; SetPos(images, 6, 518f, 215f); }
        if (images.Count > 7) { images[7].rectTransform.localScale = Vector3.one * 0.2f; SetPos(images, 7, -367f, 264f); }
        if (images.Count > 8) { images[8].rectTransform.localScale = Vector3.one * 0.3f; SetPos(images, 8, -406f, -338f); }

        // ====================================================================
        // 2. 메인 반복 루프 (5번 요소 점프 → 착지 → 착지 직후 지진)
        // ====================================================================
        if (images.Count > 5)
        {
            const float Delay = 1f;
            const float JumpDur = 0.4f;
            const float HoldDur = 0.3f;
            const float SlamDur = 0.1f;

            Vector2 startPos = images[5].rectTransform.anchoredPosition;
            Vector2 jumpPos = new Vector2(-71f, 283f);

            int[] shakeTargets = { 1, 2, 3, 4, 6, 7, 8 };

            Sequence seq = DOTween.Sequence()
                .SetLoops(-1, LoopType.Restart)
                .SetTarget(this);

            // 대기
            seq.AppendInterval(Delay);

            // 점프
            seq.Append(
                images[5].rectTransform.DOAnchorPos(jumpPos, JumpDur)
                    .SetEase(Ease.OutSine)
            );

            // 최고점 유지
            seq.AppendInterval(HoldDur);

            // Slam(착지)
            seq.Append(
                images[5].rectTransform.DOAnchorPos(startPos, SlamDur)
                    .SetEase(Ease.InQuad)
            );

            // 착지 애니메이션이 **완전히 끝난 순간** 지진 실행
            seq.AppendCallback(() =>
            {
                foreach (int i in shakeTargets)
                {
                    if (i >= images.Count) continue;

                    RectTransform rt = images[i].rectTransform;

                    // 시퀀스를 생성하되 일단 Pause 상태로 생성
                    Sequence s = CreateEarthquakeShake(rt).Pause();

                    // 즉시 재생
                    s.Play();
                }
            });
        }
    }

    // ========================================================================
    // 지진 패턴 (각도 너무 크지 않게 부드럽고 빠른 좌우 떨림)
    // ========================================================================
    Sequence CreateEarthquakeShake(RectTransform rt)
    {
        float step = 0.05f;   // 빠르고 짧은 시간
        float a1 = 8f;
        float a2 = 6f;
        float a3 = 4f;
        float a4 = 2.5f;

        Sequence s = DOTween.Sequence();

        s.Append(rt.DOLocalRotate(new Vector3(0, 0, -a1), step).SetEase(Ease.Linear));
        s.Append(rt.DOLocalRotate(new Vector3(0, 0, a1), step).SetEase(Ease.Linear));

        s.Append(rt.DOLocalRotate(new Vector3(0, 0, -a2), step).SetEase(Ease.Linear));
        s.Append(rt.DOLocalRotate(new Vector3(0, 0, a2), step).SetEase(Ease.Linear));

        s.Append(rt.DOLocalRotate(new Vector3(0, 0, -a3), step).SetEase(Ease.Linear));
        s.Append(rt.DOLocalRotate(new Vector3(0, 0, a3), step).SetEase(Ease.Linear));

        s.Append(rt.DOLocalRotate(new Vector3(0, 0, -a4), step).SetEase(Ease.Linear));
        s.Append(rt.DOLocalRotate(new Vector3(0, 0, a4), step).SetEase(Ease.Linear));

        // 잔진동 후 원위치
        s.Append(rt.DOLocalRotate(Vector3.zero, 0.02f));

        return s;
    }

    // ========================================================================
    // 보조 함수들
    // ========================================================================
    void SetPos(List<Image> imgs, int index, float x, float y)
    {
        if (index < imgs.Count)
            imgs[index].rectTransform.anchoredPosition = new Vector2(x, y);
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img == null) continue;
                img.rectTransform.DOKill();
                CanvasGroup cg = img.GetComponent<CanvasGroup>();
                if (cg != null) cg.DOKill();
            }
        }
        DOTween.Kill(this);
    }
}
public class CardAnimation_Num_9 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        // ====================================================================
        // 1. 초기 설정 (위치 + scale)
        // ====================================================================

        // 1번 요소: -162, 300 / 0.54배 (초기 스케일 저장)
        const float Scale1 = 0.54f;
        if (images.Count > 1) { SetScale(images, 1, Scale1); SetPos(images, 1, -162f, 300f); }

        // 2번 요소: 237, -304 / 0.46배 (초기 스케일 저장)
        const float Scale2 = 0.46f;
        if (images.Count > 2) { SetScale(images, 2, Scale2); SetPos(images, 2, 237f, -304f); }

        // 3번 요소: -200, 116 / 0.54배
        if (images.Count > 3) { SetScale(images, 3, 0.54f); SetPos(images, 3, -200f, 116f); }
        // 4번 요소: -71, 6 / 0.21배
        if (images.Count > 4) { SetScale(images, 4, 0.21f); SetPos(images, 4, -71f, 6f); }
        // 5번 요소: -200, 116 / 0.54배
        if (images.Count > 5) { SetScale(images, 5, 0.54f); SetPos(images, 5, -200f, 116f); }
        // 6번 요소: 254, -225 / 0.46배
        if (images.Count > 6) { SetScale(images, 6, 0.46f); SetPos(images, 6, 254f, -225f); }
        // 7번 요소: 80, -332 / 0.21배
        if (images.Count > 7) { SetScale(images, 7, 0.21f); SetPos(images, 7, 80f, -332f); }
        // 8번 요소: 46, -329 / 0.21배
        if (images.Count > 8) { SetScale(images, 8, 0.21f); SetPos(images, 8, 46f, -329f); }
        // 9번 요소: 280, -233 / 0.46배
        if (images.Count > 9) { SetScale(images, 9, 0.46f); SetPos(images, 9, 280f, -220f); }

        // 10번 요소: -560, 110 / 0.18배 (초기 위치 저장)
        Vector2 initialPos10 = Vector2.zero;
        if (images.Count > 10)
        {
            SetScale(images, 10, 0.18f);
            SetPos(images, 10, -900f, 110f);
            initialPos10 = images[10].rectTransform.anchoredPosition;
        }

        // ====================================================================
        // 2. 무한 맥동 애니메이션 (1, 2번 요소 - 메인 루프와 별개)
        // ====================================================================

        // 1번 요소 (Scale1 * 1.1f까지 커졌다가 Scale1 * 0.9f까지 작아지는 반복)
        if (images.Count > 1)
        {
            images[1].rectTransform.DOScale(Scale1 * 1.1f, 1.4f) // 1초 동안 10% 커짐
                .SetLoops(-1, LoopType.Yoyo)                     // Yoyo 타입으로 무한 반복 (커짐->작아짐->커짐)
                .SetEase(Ease.InOutSine)                         // 부드러운 가속/감속
                .SetTarget(images[1].rectTransform);             // 트윈 대상 지정
        }

        // 2번 요소 (Scale2 * 1.1f까지 커졌다가 Scale2 * 0.9f까지 작아지는 반복)
        if (images.Count > 2)
        {
            images[2].rectTransform.DOScale(Scale2 * 1.3f, 2.1f) // 1초 동안 10% 커짐
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                // 딜레이를 주어 1번 요소와 비대칭적인 박자감 부여 (선택 사항)
                // .SetDelay(0.5f) 
                .SetTarget(images[2].rectTransform);
        }

        // ====================================================================
        // 3. 메인 반복 루프 (10번 요소 점프 이동)
        // ====================================================================

        if (images.Count > 10)
        {
            // 참고: images[10]의 초기 설정 위치 (-560, 110)를 가져와 첫 번째 사이클에 사용합니다.
            Vector2 initialPosForFirstLoop = images[10].rectTransform.anchoredPosition;

            const float InitialScale10 = 0.18f;
            const float JumpHeight = 150f;
            const float JumpDuration = 0.5f;
            const int JumpCount = 1;

            // 초기 점프 목표 위치 (Y는 초기 설정값 110을 사용)
            Vector2 targetPos1 = new Vector2(-400f, initialPosForFirstLoop.y); // X = -400f
            Vector2 targetPos2 = new Vector2(-200f, initialPosForFirstLoop.y); // X = -200f

            // 새로 등장할 위치 (X: 250, Y: -220)
            Vector2 appearPos = new Vector2(250f, -220f);

            // 추가 점프 목표 위치 (Y는 appearPos의 Y값 -220을 유지)
            Vector2 jumpTarget3 = new Vector2(520f, appearPos.y); // X = 520f
            Vector2 jumpTarget4 = new Vector2(920f, appearPos.y); // X = 920f 

            // 최종 복귀 위치 (새로운 시작 위치가 됨)
            Vector2 finalReturnPos = new Vector2(-800f, 110f); // X = -800f, Y = 110f

            Sequence seq = DOTween.Sequence()
                .SetLoops(-1, LoopType.Restart)
                .SetTarget(this);

            // 1. (X: -560) -> (X: -400)으로 점프 이동
            seq.Append(
                images[10].rectTransform.DOJumpAnchorPos(targetPos1, JumpHeight, JumpCount, JumpDuration)
                    .SetEase(Ease.Linear)
            );
            seq.AppendInterval(0.2f);

            // 2. (X: -400) -> (X: -200)으로 점프 이동 (도착)
            seq.Append(
                images[10].rectTransform.DOJumpAnchorPos(targetPos2, JumpHeight, JumpCount, JumpDuration)
                    .SetEase(Ease.Linear)
            );

            // 3. 잠시 대기
            seq.AppendInterval(0.2f);

            // 4. 크기가 살짝 커짐 (1.2배)
            seq.Append(
                images[10].rectTransform.DOScale(InitialScale10 * 1.2f, 0.15f)
                    .SetEase(Ease.OutQuad)
            );

            // 5. 사이즈를 0까지 줄임 (축소/페이드 아웃)
            seq.Append(
                images[10].rectTransform.DOScale(0f, 0.2f)
                    .SetEase(Ease.InQuad)
            );

            // 6. (크기 0인 상태에서) 새 위치로 즉시 이동
            seq.AppendCallback(() =>
            {
                SetPos(images, 10, appearPos.x, appearPos.y);
            });

            // 7. 뛰용 하면서 다시 등장
            seq.Append(
                images[10].rectTransform.DOScale(InitialScale10 * 1.3f, 0.2f)
                    .SetEase(Ease.OutQuad)
            );
            seq.Append(
                images[10].rectTransform.DOScale(InitialScale10, 0.1f)
                    .SetEase(Ease.InQuad)
            );

            // 8. 잠시 대기 (새 위치에서 잠시 머뭄)
            seq.AppendInterval(0.5f);

            // 9. (X: 250) -> (X: 520)으로 점프 이동 
            seq.Append(
                images[10].rectTransform.DOJumpAnchorPos(jumpTarget3, JumpHeight, JumpCount, JumpDuration)
                    .SetEase(Ease.Linear)
            );

            // 10. 잠시 대기
            seq.AppendInterval(0.2f);

            // 11. (X: 520) -> (X: 920)으로 점프 이동
            seq.Append(
                images[10].rectTransform.DOJumpAnchorPos(jumpTarget4, JumpHeight, JumpCount, JumpDuration)
                    .SetEase(Ease.Linear)
            );



            // 14. 다음 반복까지 대기 (시퀀스 종료 시 다음 루프는 -800, 110에서 시작합니다.)
            seq.AppendInterval(1.0f);
        }
    }

    // ====================================================================
    // 보조 함수들
    // ====================================================================

    void SetPos(List<Image> imgs, int index, float x, float y)
    {
        if (index < imgs.Count)
            imgs[index].rectTransform.anchoredPosition = new Vector2(x, y);
    }

    void SetScale(List<Image> imgs, int index, float scale)
    {
        if (index < imgs.Count)
            imgs[index].rectTransform.localScale = Vector3.one * scale;
    }

    protected override void KillAllTweens()
    {
        // 1번과 2번 요소의 무한 반복 트윈도 반드시 Kill합니다.
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img == null) continue;
                // rectTransform에 걸린 트윈을 Kill (Scale, Jump, Sequence 모두 포함)
                img.rectTransform.DOKill();
                CanvasGroup cg = img.GetComponent<CanvasGroup>();
                if (cg != null) cg.DOKill();
            }
        }
        // 이 스크립트(this)에 걸린 메인 Sequence도 Kill합니다.
        DOTween.Kill(this);
    }
}


public class CardAnimation_Num_10 : CardAnimationBase
{
    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        KillAllTweens();
        InitElements(images);

        // 2. 메인 애니메이션 시퀀스 실행
        StartMainSequence(images);
    }

    private void InitElements(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 0. 전체 요소 초기화
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.rotation = UnityEngine.Quaternion.identity;
            images[i].rectTransform.localScale = UnityEngine.Vector3.one;
            SetAlpha(images[i], 1f);
        }

        // --- 요청하신 개별 알파값 세팅 ---

        // ⭐ 1번 요소 (Index 1): 알파 0으로 설정
        if (images.Count > 1)
        {
            SetAlpha(images[1], 0f);
        }

        // ⭐ 2번 요소 (Index 2): 알파 0으로 설정
        if (images.Count > 2)
        {
            SetAlpha(images[2], 0f);
        }

        // 3번 요소 (Index 3)는 기본 설정(Alpha 1)을 유지합니다.
    }

    // ===================================
    // ✨ 메인 시퀀스 함수: 1번과 2번 요소의 교차 페이드 반복 (수정됨)
    // ===================================

    private void StartMainSequence(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count <= 2) return; // 최소 2번 요소까지 있어야 함

        UnityEngine.UI.Image imgA = images[1]; // ⭐ 1번 요소로 변경
        UnityEngine.UI.Image imgB = images[2]; // ⭐ 2번 요소로 변경

        // 1번 요소와 2번 요소의 교차 페이드 루프 시작
        StartCrossFadeBlink(imgA, imgB);
    }

    // ===================================
    // ⚙️ 헬퍼 함수: 교차 페이드 반복 로직
    // ===================================
    // (로직 내부의 A/B 변수 역할은 그대로 유지하며, StartMainSequence에서 참조만 변경됨)

    private void StartCrossFadeBlink(UnityEngine.UI.Image imgA, UnityEngine.UI.Image imgB)
    {
        // 1 사이클 (A 등장 -> 유지 -> 소멸 -> B 등장 -> 유지 -> 소멸)을 위한 메인 시퀀스
        DG.Tweening.Sequence mainSeq = DG.Tweening.DOTween.Sequence()
            .SetLoops(-1, DG.Tweening.LoopType.Restart); // 무한 반복

        // --- A (1번 요소) 페이드 ---

        // 1. A 등장 (0.5초)
        mainSeq.Append(imgA.DOFade(1f, 0.5f).SetEase(DG.Tweening.Ease.Linear));

        // 2. A 유지 (2.5초)
        mainSeq.AppendInterval(2.5f);

        // 3. A 소멸 (1.5초)
        mainSeq.Append(imgA.DOFade(0f, 1.5f).SetEase(DG.Tweening.Ease.Linear));

        // 4. B 등장과 A 소멸이 겹치도록 Insert (A 소멸 시작 시점인 3.0초에 B 등장 시작)
        // A 소멸 시작 시점 = 0.5초 + 2.5초 = 3.0초
        mainSeq.Insert(3.0f, imgB.DOFade(1f, 0.5f).SetEase(DG.Tweening.Ease.Linear));


        // --- B (2번 요소) 페이드 ---

        // B 등장 후 2.5초 유지 (A 소멸이 끝나는 시점 4.5초에서 2.5초 유지)
        // B 유지 시작 시점 = A 소멸 종료 시점 (4.5초)
        mainSeq.AppendInterval(2.5f);

        // B 소멸 (1.5초)
        mainSeq.Append(imgB.DOFade(0f, 1.5f).SetEase(DG.Tweening.Ease.Linear));

        // 5. 1초 대기 후 루프 반복
        mainSeq.AppendInterval(1.0f);
    }


    // ===================================
    // 🛠️ 필수 헬퍼 함수
    // ===================================

    private void SetRect(UnityEngine.UI.Image img, float x, float y, float scale)
    {
        UnityEngine.RectTransform rt = img.rectTransform;
        rt.anchoredPosition = new UnityEngine.Vector2(x, y);
        rt.localScale = UnityEngine.Vector3.one * scale;
    }

    private void SetAlpha(UnityEngine.UI.Image img, float alpha)
    {
        img.color = new UnityEngine.Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    protected override void KillAllTweens()
    {
        // 1번, 2번 요소의 모든 트윈 (Sequence) 정리
        if (animationImages != null)
        {
            for (int i = 1; i <= 2; i++) // ⭐ 1번, 2번 요소만 정리하도록 수정
            {
                if (animationImages.Count > i && animationImages[i] != null)
                {
                    DG.Tweening.DOTween.Kill(animationImages[i]);
                }
            }
        }
    }
}


public class CardAnimation_Num_11 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        // 모든 트윈을 정리하고 애니메이션 시작 (루프 시 매번 호출됨)
        KillAllTweens();
        InitElements(images);

        // 메인 애니메이션 실행
        StartMainSequence(images);
    }

    private void InitElements(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 0. 전체 요소 초기화 (위치, 회전, 스케일)
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.localRotation = UnityEngine.Quaternion.identity;
            images[i].rectTransform.localScale = UnityEngine.Vector3.one;
            SetAlpha(images[i], 1f);
        }

        // --- 요청하신 개별 위치, 크기 및 알파값 세팅 ---

        // 1번 요소 (Index 1): (-900, -496, 0.3배)
        if (images.Count > 1) SetRect(images[1], -900f, -496f, 0.3f);

        // 2번 요소 (Index 2): (-303, 374, 0.52배, 알파 0, Z축 -228)
        if (images.Count > 2)
        {
            SetRect(images[2], -303f, 374f, 0.52f);
            images[2].rectTransform.localRotation = UnityEngine.Quaternion.Euler(0f, 0f, -228f);
            SetAlpha(images[2], 0f);
            images[2].rectTransform.localScale = UnityEngine.Vector3.zero;
        }

        // 3번 요소 (Index 3): (-510, 182, 0.3배, 알파 0)
        if (images.Count > 3) { SetRect(images[3], -510f, 182f, 0.3f); SetAlpha(images[3], 0f); }

        // 4번 요소 (Index 4): (120, 192, 0.3배, 알파 0)
        if (images.Count > 4) { SetRect(images[4], 120f, 192f, 0.3f); SetAlpha(images[4], 0f); }

        // 5번 요소 (Index 5): (37, 87, 0.3배, 알파 0)
        if (images.Count > 5) { SetRect(images[5], 37f, 87f, 0.3f); SetAlpha(images[5], 0f); }

        // 6번 요소 (Index 6): (483, -413, 0.46배, 알파 0, Z축 159)
        if (images.Count > 6)
        {
            SetRect(images[6], 483f, -413f, 0.46f);
            images[6].rectTransform.localRotation = UnityEngine.Quaternion.Euler(0f, 0f, 159f);
            SetAlpha(images[6], 0f);
            images[6].rectTransform.localScale = UnityEngine.Vector3.zero;
        }

        // 7~9번 요소 (알파 0)
        if (images.Count > 7) { SetRect(images[7], 90f, -289f, 0.3f); SetAlpha(images[7], 0f); }
        if (images.Count > 8) { SetRect(images[8], 585f, -217f, 0.3f); SetAlpha(images[8], 0f); }
        if (images.Count > 9) { SetRect(images[9], 521f, -142f, 0.3f); SetAlpha(images[9], 0f); }

        // 10~11번 요소 (알파 1 유지)
        if (images.Count > 10) SetRect(images[10], 0f, 205f, 1.51f);
        if (images.Count > 11) SetRect(images[11], 0f, -282f, 1.65f);
    }

    // ===================================
    // ✨ 메인 시퀀스 함수: 1번 요소 릴레이 이동 및 팝인 이벤트
    // ===================================

    private void StartMainSequence(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count <= 9) return;

        UnityEngine.RectTransform rt1 = images[1].rectTransform;

        UnityEngine.Vector2 posA = new UnityEngine.Vector2(-303f, 374f);
        UnityEngine.Vector2 posB = new UnityEngine.Vector2(483f, -413f);
        UnityEngine.Vector2 posC = new UnityEngine.Vector2(994f, 422f);

        float fastMoveDuration = 0.45f;
        float popinDuration = 0.1f;
        float targetScale2 = 0.52f;
        float targetScale6 = 0.46f;
        float overshootRatio = 1.2f;

        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence();

        // 1. 처음 1.5초 대기
        seq.AppendInterval(1.5f);

        // --- 1차 이동 단계 (Move A) ---
        // 2. A 지점까지 이동 (0.45s) - Linear 적용 (빠르고 강한 이동)
        seq.Append(rt1.DOAnchorPos(posA, fastMoveDuration).SetEase(DG.Tweening.Ease.Linear));
        float timeA = seq.Duration();

        // 2-1. A 지점 도달 시 팝인 이벤트 + Z축 회전 (-54)
        if (images.Count > 5)
        {
            // 1번 요소 Z축 회전 (-54)
            seq.Insert(timeA, rt1.DORotate(new UnityEngine.Vector3(0, 0, -54f), 0.001f));

            // 2,3,4,5 알파값 1로 0.1초만에 만듬
            seq.Insert(timeA, images[2].DOFade(1f, popinDuration));
            seq.Insert(timeA, images[3].DOFade(1f, popinDuration));
            seq.Insert(timeA, images[4].DOFade(1f, popinDuration));
            seq.Insert(timeA, images[5].DOFade(1f, popinDuration));

            // 2번 요소 팝인 애니메이션
            if (images.Count > 2)
            {
                DG.Tweening.Sequence popin2 = CreatePopinTween(images[2].rectTransform, targetScale2, overshootRatio, popinDuration);
                seq.Insert(timeA, popin2);
            }
        }

        // --- 2차 이동 단계 (Move B) ---
        // 3. B 지점까지 이동 (0.45s)
        seq.Append(rt1.DOAnchorPos(posB, fastMoveDuration).SetEase(DG.Tweening.Ease.Linear));
        float timeB = seq.Duration();

        // 3-1. B 지점 도달 시 팝인 이벤트 + Z축 회전 (13)
        if (images.Count > 9)
        {
            // 1번 요소 Z축 회전 (13)
            seq.Insert(timeB, rt1.DORotate(new UnityEngine.Vector3(0, 0, 13f), 0.001f));

            // 6,7,8,9 알파값 1로 0.1초만에 만듬
            seq.Insert(timeB, images[6].DOFade(1f, popinDuration));
            seq.Insert(timeB, images[7].DOFade(1f, popinDuration));
            seq.Insert(timeB, images[8].DOFade(1f, popinDuration));
            seq.Insert(timeB, images[9].DOFade(1f, popinDuration));

            // 6번 요소 팝인 애니메이션
            if (images.Count > 6)
            {
                DG.Tweening.Sequence popin6 = CreatePopinTween(images[6].rectTransform, targetScale6, overshootRatio, popinDuration);
                seq.Insert(timeB, popin6);
            }
        }

        // --- 최종 이동 단계 (Move C) ---
        // 4. C 지점까지 최종 이동 (0.45s)
        seq.Append(rt1.DOAnchorPos(posC, fastMoveDuration).SetEase(DG.Tweening.Ease.Linear));

        // ⭐ 애니메이션 완료 시 루프 시퀀스 호출
        seq.OnComplete(() => StartLoopSequence(images));
    }

    // ===================================
    // ⚙️ 헬퍼 함수: 루프 시퀀스 (2초 유지 후 즉시 초기화)
    // ===================================
    private void StartLoopSequence(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count <= 9) return;

        UnityEngine.RectTransform rt1 = images[1].rectTransform;
        DG.Tweening.Sequence loopSeq = DG.Tweening.DOTween.Sequence();

        UnityEngine.Vector2 initialPos1 = new UnityEngine.Vector2(-900f, -496f);

        float instantDuration = 0f;
        float fadeDuration = 0.5f; // 서서히 사라지는 시간 설정

        // 1. 애니메이션 완료 상태를 2.0초간 유지
        loopSeq.AppendInterval(2.0f);

        // 2. Teleport (1번) 및 Fade (2-9번) 동시 시작

        // 2a. 1번 요소: 위치 및 회전 즉시 복귀 (순간이동)
        loopSeq.Append(rt1.DOAnchorPos(initialPos1, instantDuration));
        loopSeq.Join(rt1.DORotate(UnityEngine.Vector3.zero, instantDuration));

        // 2b. 2-9번 요소: Gradual Fade Out & Instant Scale/Rot Reset (수정된 로직)
        for (int i = 2; i <= 9; i++)
        {
            if (images.Count > i)
            {
                UnityEngine.UI.Image img = images[i];

                // 1. Gradual Fade Out 트윈 생성
                DG.Tweening.Tween fadeTween = img.DOFade(0f, fadeDuration);

                // ⭐ 2. Fade Tween 완료 시점에 Scale/Rot 리셋 로직을 연결 ⭐
                fadeTween.OnComplete(() => {

                    // Rotation Reset (Instant, 알파 0 상태에서 실행)
                    img.rectTransform.DORotate(UnityEngine.Vector3.zero, instantDuration);

                    // Scale Reset (Instant, 알파 0 상태에서 실행)
                    if (i == 2 || i == 6)
                    {
                        img.rectTransform.DOScale(UnityEngine.Vector3.zero, instantDuration);
                    }
                });

                // 3. Fade Tween을 Loop Sequence에 Join
                loopSeq.Join(fadeTween);
            }
        }

        // 3. Fade Out 완료를 기다림 (0.5s)
        loopSeq.AppendInterval(fadeDuration);

        // 4. 애니메이션 재시작
        loopSeq.OnComplete(() =>
        {
            ExecuteAnimation(images);
        });
    }
    // ===================================
    // ⚙️ 헬퍼 함수: 팝인 스케일 시퀀스 생성
    // ===================================

    private DG.Tweening.Sequence CreatePopinTween(UnityEngine.RectTransform rt, float targetScale, float overshootRatio, float duration)
    {
        DG.Tweening.Sequence popinSeq = DG.Tweening.DOTween.Sequence();
        float overshootScale = targetScale * overshootRatio;
        float halfDuration = duration / 2f;

        // 1. Overshoot: 0 -> 1.2 * Target Scale (0.05s)
        popinSeq.Append(rt.DOScale(overshootScale, halfDuration).SetEase(DG.Tweening.Ease.OutQuad));

        // 2. Return: 1.2 * Target Scale -> Target Scale (0.05s)
        popinSeq.Append(rt.DOScale(targetScale, halfDuration).SetEase(DG.Tweening.Ease.InQuad));

        return popinSeq;
    }


    // ===================================
    // 🛠️ 필수 헬퍼 함수
    // ===================================

    private void SetRect(UnityEngine.UI.Image img, float x, float y, float scale)
    {
        UnityEngine.RectTransform rt = img.rectTransform;
        rt.anchoredPosition = new UnityEngine.Vector2(x, y);
        rt.localScale = UnityEngine.Vector3.one * scale;
    }

    private void SetAlpha(UnityEngine.UI.Image img, float alpha)
    {
        img.color = new UnityEngine.Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    protected override void KillAllTweens()
    {
        // 1번부터 11번 요소의 트윈 정리
        if (animationImages != null)
        {
            for (int i = 1; i < 12; i++)
            {
                if (animationImages.Count > i && animationImages[i] != null)
                {
                    DG.Tweening.DOTween.Kill(animationImages[i]);
                    DG.Tweening.DOTween.Kill(animationImages[i].rectTransform);
                }
            }
            // 전체 시퀀스 및 루프 시퀀스 정리
            DG.Tweening.DOTween.Kill(this);
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

public class CardAnimation_Num_14 : CardAnimationBase
{
    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        KillAllTweens();

        // --------------------------------------------------------
        // [초기 세팅]
        // --------------------------------------------------------

        // 1번 요소: -20, -20
        if (images.Count > 1)
        {
            images[1].rectTransform.anchoredPosition = new UnityEngine.Vector2(-20f, -20f);
            images[1].color = new UnityEngine.Color(images[1].color.r, images[1].color.g, images[1].color.b, 1f);
        }

        // 2번 요소: -5, 28
        if (images.Count > 2)
        {
            images[2].rectTransform.anchoredPosition = new UnityEngine.Vector2(-5f, 28f);
            images[2].color = new UnityEngine.Color(images[2].color.r, images[2].color.g, images[2].color.b, 1f);
        }

        // 3번 요소: 5, 20
        if (images.Count > 3)
        {
            images[3].rectTransform.anchoredPosition = new UnityEngine.Vector2(5f, 20f);
        }

        // 4번 요소: -300, -93, 0.5배
        if (images.Count > 4)
        {
            images[4].rectTransform.anchoredPosition = new UnityEngine.Vector2(-300f, -93f);
            images[4].rectTransform.localScale = UnityEngine.Vector3.one * 0.5f;
            images[4].color = new UnityEngine.Color(images[4].color.r, images[4].color.g, images[4].color.b, 1f);
        }

        // 5번 요소: 384, -93, 0.5배
        if (images.Count > 5)
        {
            images[5].rectTransform.anchoredPosition = new UnityEngine.Vector2(384f, -93f);
            images[5].rectTransform.localScale = UnityEngine.Vector3.one * 0.5f;
        }

        // 6번 요소: 0, 0, 1배
        if (images.Count > 6)
        {
            images[6].rectTransform.anchoredPosition = new UnityEngine.Vector2(0f, 0f);
            images[6].rectTransform.localScale = UnityEngine.Vector3.one * 1f;
        }

        // 8번 요소(Index 7): 비활성화
        if (images.Count > 7)
        {
            images[7].color = new UnityEngine.Color(images[7].color.r, images[7].color.g, images[7].color.b, 0f);
        }

        // --------------------------------------------------------
        // [메인 시퀀스 실행]
        // --------------------------------------------------------
        StartMainSequence(images);
    }

    private void StartMainSequence(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence();

        // 1. 초반 대기 딜레이 1.5초
        seq.SetDelay(1.5f);

        // 2. 4번 요소 알파값 0으로 (0.5초)
        if (images.Count > 4)
        {
            seq.Append(images[4].DOFade(0f, 0.5f));
        }

        // 3. 0.3초 대기
        seq.AppendInterval(0.3f);

        // 4. 5, 6번 요소 이동 및 1, 2번 사라짐 (0.6초)
        if (images.Count > 5)
        {
            float targetX5 = images[5].rectTransform.anchoredPosition.x - 663f;
            seq.Append(images[5].rectTransform.DOAnchorPosX(targetX5, 0.6f).SetEase(DG.Tweening.Ease.OutQuad));
        }

        if (images.Count > 6)
        {
            float targetX6 = images[6].rectTransform.anchoredPosition.x - 600f;
            seq.Join(images[6].rectTransform.DOAnchorPosX(targetX6, 0.6f).SetEase(DG.Tweening.Ease.OutQuad));

            if (images.Count > 1) seq.Join(images[1].DOFade(0f, 0.5f));
            if (images.Count > 2) seq.Join(images[2].DOFade(0f, 0.5f));
        }

        // 0.3초 대기
        seq.AppendInterval(0.3f);

        // 5. 4번 요소 위치 이동, 반전 및 재등장
        if (images.Count > 4)
        {
            seq.AppendCallback(() =>
            {
                UnityEngine.RectTransform rt4 = images[4].rectTransform;
                rt4.anchoredPosition = new UnityEngine.Vector2(384f, -95f);

                // 좌우 반전 (현재 X 스케일의 부호를 반대로)
                UnityEngine.Vector3 currentScale = rt4.localScale;
                rt4.localScale = new UnityEngine.Vector3(-UnityEngine.Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
            });

            seq.Append(images[4].DOFade(1f, 0.3f));
        }

        // 6. 4번 등장 0.1초 뒤 5번 요소 파르르 떨림
        if (images.Count > 5)
        {
            seq.AppendInterval(0.1f);
            seq.Append(images[5].rectTransform.DOShakeRotation(0.3f, strength: new UnityEngine.Vector3(0, 0, 20f), vibrato: 120, randomness: 90f));
        }

        // --- [마무리 반복 로직] ---

        // 7. 애니메이션 끝난 후 1.5초 대기
        seq.AppendInterval(1.5f);

        // 8. 8번 요소(Index 7) 등장 (1초)
        if (images.Count > 7)
        {
            seq.Append(images[7].DOFade(1f, 1.0f));
        }

        // 9. 등장 직후 나머지 요소 초기화 트리거
        seq.AppendCallback(() => {
            ResetOtherElements(images);
        });

        // 10. 8번 요소(Index 7) 다시 사라짐 (1초)
        if (images.Count > 7)
        {
            seq.Append(images[7].DOFade(0f, 1.0f));
        }

        // 11. 루프 재시작
        seq.AppendInterval(1f);
        seq.OnComplete(() => StartMainSequence(images));
    }

    private void ResetOtherElements(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 1번
        if (images.Count > 1)
        {
            images[1].rectTransform.anchoredPosition = new UnityEngine.Vector2(-20f, -20f);
            images[1].color = new UnityEngine.Color(images[1].color.r, images[1].color.g, images[1].color.b, 1f);
        }
        // 2번
        if (images.Count > 2)
        {
            images[2].rectTransform.anchoredPosition = new UnityEngine.Vector2(-5f, 28f);
            images[2].color = new UnityEngine.Color(images[2].color.r, images[2].color.g, images[2].color.b, 1f);
        }
        // 3번
        if (images.Count > 3)
        {
            images[3].rectTransform.anchoredPosition = new UnityEngine.Vector2(5f, 20f);
        }
        // 4번 (스케일 양수 복구)
        if (images.Count > 4)
        {
            images[4].rectTransform.anchoredPosition = new UnityEngine.Vector2(-300f, -93f);
            UnityEngine.Vector3 s = images[4].rectTransform.localScale;
            images[4].rectTransform.localScale = new UnityEngine.Vector3(UnityEngine.Mathf.Abs(s.x), s.y, s.z);
            images[4].color = new UnityEngine.Color(images[4].color.r, images[4].color.g, images[4].color.b, 1f);
        }
        // 5번
        if (images.Count > 5)
        {
            images[5].rectTransform.anchoredPosition = new UnityEngine.Vector2(384f, -93f);
        }
        // 6번
        if (images.Count > 6)
        {
            images[6].rectTransform.anchoredPosition = new UnityEngine.Vector2(0f, 0f);
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    img.DOKill();
                    img.GetComponent<UnityEngine.RectTransform>().DOKill();
                }
            }
        }
    }
}

public class CardAnimation_Num_15 : CardAnimationBase
{
    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // --------------------------------------------------------
        // [초기 세팅] (인덱스 = 요소 번호)
        // --------------------------------------------------------

        // 1번 요소 (Index 1): -433, 75, 0.3배
        if (images.Count > 1)
        {
            UnityEngine.RectTransform element1 = images[1].GetComponent<UnityEngine.RectTransform>();
            element1.anchoredPosition = new UnityEngine.Vector2(-433f, 75f);
            element1.localScale = UnityEngine.Vector3.one * 0.3f;
        }

        // 2번 요소 (Index 2): -217, -196, 0.3배
        if (images.Count > 2)
        {
            UnityEngine.RectTransform element2 = images[2].GetComponent<UnityEngine.RectTransform>();
            element2.anchoredPosition = new UnityEngine.Vector2(-217f, -196f);
            element2.localScale = UnityEngine.Vector3.one * 0.3f;
        }

        // 3번 요소 (Index 3): 207, 158, 0.3배
        if (images.Count > 3)
        {
            UnityEngine.RectTransform element3 = images[3].GetComponent<UnityEngine.RectTransform>();
            element3.anchoredPosition = new UnityEngine.Vector2(207f, 158f);
            element3.localScale = UnityEngine.Vector3.one * 0.3f;
        }

        // 4번 요소 (Index 4): 373, 106, 0.3배
        if (images.Count > 4)
        {
            UnityEngine.RectTransform element4 = images[4].GetComponent<UnityEngine.RectTransform>();
            element4.anchoredPosition = new UnityEngine.Vector2(373f, 106f);
            element4.localScale = UnityEngine.Vector3.one * 0.3f;
        }

        // 6번 요소 (Index 6): 비활성화 (Alpha 0)
        if (images.Count > 6)
        {
            images[6].color = new UnityEngine.Color(images[6].color.r, images[6].color.g, images[6].color.b, 0f);
        }

        // 10번 요소 (Index 10): 289, -10, 0.4배
        if (images.Count > 10)
        {
            UnityEngine.RectTransform element10 = images[10].GetComponent<UnityEngine.RectTransform>();
            element10.anchoredPosition = new UnityEngine.Vector2(289f, -10f);
            element10.localScale = UnityEngine.Vector3.one * 0.4f;
        }

        // 11번 요소 (Index 11): -297, -60, 0.4배
        if (images.Count > 11)
        {
            UnityEngine.RectTransform element11 = images[11].GetComponent<UnityEngine.RectTransform>();
            element11.anchoredPosition = new UnityEngine.Vector2(-297f, -60f);
            element11.localScale = UnityEngine.Vector3.one * 0.4f;
        }

        // 12번 요소 (Index 12): 290, 285, 0.4배
        if (images.Count > 12)
        {
            UnityEngine.RectTransform element12 = images[12].GetComponent<UnityEngine.RectTransform>();
            element12.anchoredPosition = new UnityEngine.Vector2(290f, 285f);
            element12.localScale = UnityEngine.Vector3.one * 0.4f;
        }


        // --------------------------------------------------------
        // [애니메이션 로직]
        // --------------------------------------------------------

        // 6번 파츠 (index 6) - 비활성화 상태지만 움직임은 유지 (필요 시)
        // 기존 코드에서 6번 파츠(Index 5) 로직이 있었으나, 
        // 요청하신 대로 6번 요소(Index 6)가 비활성화 대상이므로 해당 인덱스에 로직 적용 여부를 확인해야 합니다.
        // 여기서는 기존 로직의 '6번 파츠'가 Index 5였던 것을 감안하여, 
        // 말씀하신 '6번 요소(Index 6)'와 별개로 기존 5번 인덱스 애니메이션은 유지합니다.

        if (images.Count > 5)
        {
            UnityEngine.RectTransform part6 = images[5].GetComponent<UnityEngine.RectTransform>();
            UnityEngine.Vector2 originalPos = GetOriginalPosition(part6);

            DG.Tweening.Sequence part6Sequence = DG.Tweening.DOTween.Sequence();
            part6Sequence.Append(part6.DOAnchorPos(originalPos + new UnityEngine.Vector2(0, -200f), 2f).SetEase(DG.Tweening.Ease.OutQuad));
            part6Sequence.Append(part6.DOAnchorPos(originalPos, 0.04f).SetEase(DG.Tweening.Ease.InQuad));
            part6Sequence.Append(part6.DOShakePosition(0.8f, strength: 15f, vibrato: 60, randomness: 90f, fadeOut: false));
            part6Sequence.AppendInterval(2f);
            part6Sequence.SetLoops(-1);
        }

        // 8번 파츠 (index 7)
        if (images.Count > 7)
        {
            UnityEngine.RectTransform element8 = images[7].GetComponent<UnityEngine.RectTransform>();
            UnityEngine.Vector2 element8Original = GetOriginalPosition(element8);
            element8.DOAnchorPos(element8Original + new UnityEngine.Vector2(-50f, -50f), 2f)
                    .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                    .SetEase(DG.Tweening.Ease.InOutSine);
        }

        // 9번 파츠 (index 8)
        if (images.Count > 8)
        {
            UnityEngine.RectTransform element9 = images[8].GetComponent<UnityEngine.RectTransform>();
            UnityEngine.Vector2 element9Original = GetOriginalPosition(element9);
            element9.DOAnchorPos(element9Original + new UnityEngine.Vector2(50f, 50f), 2.3f)
                    .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                    .SetEase(DG.Tweening.Ease.InOutSine);
        }

        // 10번 파츠 (index 9)
        if (images.Count > 9)
        {
            UnityEngine.RectTransform element10 = images[9].GetComponent<UnityEngine.RectTransform>();
            UnityEngine.Vector2 element10Original = GetOriginalPosition(element10);
            element10.DOAnchorPos(element10Original + new UnityEngine.Vector2(50f, 0f), 1.8f)
                     .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                     .SetEase(DG.Tweening.Ease.InOutSine);
        }

        // 1~4번 요소 랜덤 로테이션 (index 1~4)
        for (int i = 1; i <= 4; i++)
        {
            if (i < images.Count)
            {
                UnityEngine.RectTransform partRect = images[i].GetComponent<UnityEngine.RectTransform>();
                StartRandomRotation(partRect, i);
            }
        }

        // [주석 해제 및 인덱스 적용] 10번 요소 (index 10) - 좌측으로 이동
        if (images.Count > 10)
        {
            UnityEngine.RectTransform element11 = images[10].GetComponent<UnityEngine.RectTransform>();
            // 초기 세팅 위치를 기준으로 움직임
            UnityEngine.Vector2 element11Original = element11.anchoredPosition;
            UnityEngine.Vector2 element11Target = element11Original + new UnityEngine.Vector2(40f, -60);
            element11.DOAnchorPos(element11Target, 2.6f)
                     .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                     .SetEase(DG.Tweening.Ease.InOutSine);
        }

        // [주석 해제 및 인덱스 적용] 11번 요소 (index 11) - 좌측하단으로 이동
        if (images.Count > 11)
        {
            UnityEngine.RectTransform element12 = images[11].GetComponent<UnityEngine.RectTransform>();
            UnityEngine.Vector2 element12Original = element12.anchoredPosition;
            UnityEngine.Vector2 element12Target = element12Original + new UnityEngine.Vector2(-30f, -70f);
            element12.DOAnchorPos(element12Target, 3f)
                     .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                     .SetEase(DG.Tweening.Ease.InOutSine);
        }

        // [주석 해제 및 인덱스 적용] 12번 요소 (index 12) - 우측하단으로 이동
        if (images.Count > 12)
        {
            UnityEngine.RectTransform element13 = images[12].GetComponent<UnityEngine.RectTransform>();
            UnityEngine.Vector2 element13Original = element13.anchoredPosition;
            UnityEngine.Vector2 element13Target = element13Original + new UnityEngine.Vector2(80f, 0f);
            element13.DOAnchorPos(element13Target, 2.5f)
                     .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                     .SetEase(DG.Tweening.Ease.InOutSine);
        }

        // 13번 요소 (index 13) - 우측으로 이동 (기존 14번 로직)
        if (images.Count > 13)
        {
            UnityEngine.RectTransform element14 = images[13].GetComponent<UnityEngine.RectTransform>();
            UnityEngine.Vector2 element14Original = GetOriginalPosition(element14);
            UnityEngine.Vector2 element14Target = element14Original + new UnityEngine.Vector2(72f, 0);
            element14.DOAnchorPos(element14Target, 2.5f)
                     .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                     .SetEase(DG.Tweening.Ease.InOutSine);
        }
    }

    private void StartRandomRotation(UnityEngine.RectTransform target, int partIndex)
    {
        float randomDelay = UnityEngine.Random.Range(2f, 6f);
        DG.Tweening.DOTween.Sequence()
            .AppendInterval(randomDelay)
            .AppendCallback(() =>
            {
                target.DOShakeRotation(
                    duration: 0.8f,
                    strength: new UnityEngine.Vector3(0, 0, 5f),
                    vibrato: 20,
                    randomness: 50f,
                    fadeOut: false
                )
                .SetEase(DG.Tweening.Ease.InOutQuad)
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
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    img.DOKill();
                    img.GetComponent<UnityEngine.RectTransform>().DOKill();
                }
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

public class CardAnimation_Num_23 : CardAnimationBase
{
    // 입력된 좌표 데이터
    private readonly Vector2[] _targetPositions = new Vector2[]
    {
        new Vector2(0, -87),       // 1번
        new Vector2(1157, 487),    // 2번
        new Vector2(868, 600),     // 3번
        new Vector2(-539, 631),    // 4번
        new Vector2(-1156, 587),   // 5번
        new Vector2(-1018, 376),   // 6번
        new Vector2(-1072, 121),   // 7번
        new Vector2(-1049, -488),  // 8번
        new Vector2(-769, -596),   // 9번
        new Vector2(686, -703),    // 10번
        new Vector2(1117, -581),   // 11번
        new Vector2(195, 136),     // 12번 (이펙트용 원본)
        // 13~20번 (폭죽 파티클)
        new Vector2(4, -54),       // 13번
        new Vector2(9, -50),       // 14번
        new Vector2(-8, -42),      // 15번
        new Vector2(30, 31),       // 16번
        new Vector2(4, 16),        // 17번
        new Vector2(-30, -18),     // 18번
        new Vector2(-21, -17),     // 19번
        new Vector2(2, -71)        // 20번
    };

    protected override void ExecuteAnimation(List<Image> images)
    {
        KillAllTweens();

        // 1. 기본 배치
        for (int i = 1; i <= 20; i++)
        {
            if (images.Count <= i) break;

            RectTransform rt = images[i].GetComponent<RectTransform>();
            rt.anchoredPosition = _targetPositions[i - 1];

            // 크기 설정
            if (i == 1) rt.localScale = Vector3.one * 0.275f;
            else if (i == 12) rt.localScale = Vector3.one * 0.3f;
            else rt.localScale = Vector3.one;

            rt.rotation = Quaternion.identity;

            // [수정됨] 12번(Index 11)과 13~20번은 처음에 안 보이게 설정
            if (i == 12 || (i >= 13 && i <= 20))
            {
                images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f);
            }
            else
            {
                images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 1f);
            }
        }

        // 2. 1번 점프 & 이펙트 시작
        if (images.Count > 1)
        {
            StartJumpAndBurstAnimation(images[1].GetComponent<RectTransform>(), images);
        }

        // 3. 나머지 플로팅
        StartRadialFloating(images);
    }

    private void StartJumpAndBurstAnimation(RectTransform target, List<Image> images)
    {
        Vector2 leftPos = new Vector2(-282, -720);
        Vector2 rightPos = new Vector2(282, -720);
        Vector2 jumpPeak = new Vector2(-13, -77);

        target.anchoredPosition = leftPos;

        // (1) 무한 회전 (1.5초)
        target.DORotate(new Vector3(0, 0, -360f), 1.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        // (2) 점프 시퀀스
        Sequence jumpSeq = DOTween.Sequence();

        // 왼쪽 -> 점프 -> 이펙트(폭죽+생성) -> 착지
        jumpSeq.Append(target.DOAnchorPos(jumpPeak, 0.7f).SetEase(Ease.OutQuad));
        jumpSeq.AppendCallback(() => PlayPeakEffects(images, jumpPeak)); // ★ 이펙트 통합 함수 호출
        jumpSeq.Append(target.DOAnchorPos(rightPos, 0.7f).SetEase(Ease.InQuad));
        jumpSeq.AppendInterval(0.5f);

        // 오른쪽 -> 점프 -> 이펙트(폭죽+생성) -> 착지
        jumpSeq.Append(target.DOAnchorPos(jumpPeak, 0.7f).SetEase(Ease.OutQuad));
        jumpSeq.AppendCallback(() => PlayPeakEffects(images, jumpPeak)); // ★ 이펙트 통합 함수 호출
        jumpSeq.Append(target.DOAnchorPos(leftPos, 0.7f).SetEase(Ease.InQuad));
        jumpSeq.AppendInterval(0.5f);

        jumpSeq.SetLoops(-1);
    }

    // 피크 지점에서 발생하는 모든 이펙트 관리 (13~20번 폭죽 + 12번 생성)
    private void PlayPeakEffects(List<Image> images, Vector2 centerPos)
    {
        // 1. 기존 13~20번 폭죽 효과 실행
        PlayBurstParticles(images, centerPos);

        // 2. [신규] 12번 요소 복사 생성 효과 실행
        if (images.Count > 11)
        {
            SpawnElement12Copies(images[12]);
        }
    }

    // [신규] 12번 요소 3개 생성 및 소멸 로직
    private void SpawnElement12Copies(Image originalElement)
    {
        for (int k = 0; k < 3; k++)
        {
            // 원본 복제 (Instantiate)
            Image clone = Object.Instantiate(originalElement, originalElement.transform.parent);
            RectTransform cloneRt = clone.GetComponent<RectTransform>();

            // 위치 랜덤 설정 (-211~211, -300~113)
            float randX = Random.Range(-270f, 270f);
            float randY = Random.Range(-300f, 113f);
            cloneRt.anchoredPosition = new Vector2(randX, randY);

            // 초기 상태: 크기 유지, 알파 0
            cloneRt.localScale = Vector3.one * 0.3f; // 12번의 원래 크기
            cloneRt.rotation = Quaternion.identity;
            clone.color = new Color(clone.color.r, clone.color.g, clone.color.b, 0f);

            // 애니메이션 시퀀스
            Sequence cloneSeq = DOTween.Sequence();

            // (1) 순식간에 나타남 (0.1초)
            cloneSeq.Append(clone.DOFade(1f, 0.1f));

            // (2) 잠깐 보였다가 사라짐 (0.4초 동안 FadeOut)
            cloneSeq.Append(clone.DOFade(0f, 0.4f));

            // (3) 애니메이션 끝나면 오브젝트 삭제 (Destroy)
            cloneSeq.OnComplete(() =>
            {
                Object.Destroy(clone.gameObject);
            });
        }
    }

    // 기존 13~20번 폭죽 로직 (함수명 변경됨: PlayBurstEffect -> PlayBurstParticles)
    private void PlayBurstParticles(List<Image> images, Vector2 centerPos)
    {
        float burstDistance = 450f;

        for (int i = 13; i <= 20; i++)
        {
            if (images.Count <= i) break;

            Image particleImg = images[i];
            RectTransform particleRt = particleImg.GetComponent<RectTransform>();

            particleRt.anchoredPosition = centerPos;
            Color c = particleImg.color;
            particleImg.color = new Color(c.r, c.g, c.b, 0f);

            Vector2 direction = Vector2.zero;
            switch (i)
            {
                case 13: direction = Vector2.right; break;
                case 14: direction = new Vector2(1f, 1f).normalized; break;
                case 15: direction = new Vector2(-0.5f, 1f).normalized; break;
                case 16: direction = new Vector2(-1f, 0.6f).normalized; break;
                case 17: direction = new Vector2(-1f, -0.3f).normalized; break;
                case 18: direction = new Vector2(-0.4f, -1f).normalized; break;
                case 19: direction = new Vector2(1f, -1f).normalized; break;
                case 20: direction = Vector2.down; break;
            }

            Vector2 burstTarget = centerPos + (direction * burstDistance);

            Sequence burstSeq = DOTween.Sequence();
            burstSeq.Append(particleImg.DOFade(1f, 0.1f));
            burstSeq.Join(particleRt.DOAnchorPos(burstTarget, 0.5f).SetEase(Ease.OutCubic));
            burstSeq.Join(particleImg.DOFade(0f, 0.3f).SetDelay(0.2f));
        }
    }

    private void StartRadialFloating(List<Image> images)
    {
        for (int i = 2; i <= 11; i++)
        {
            if (images.Count <= i) break;

            RectTransform rt = images[i].GetComponent<RectTransform>();
            Vector2 originalPos = rt.anchoredPosition;
            Vector2 direction = originalPos.normalized;

            float moveDistance = Random.Range(40f, 70f);
            float duration = Random.Range(1.4f, 2.2f);
            Vector2 targetPos = originalPos + (direction * moveDistance);

            rt.DOAnchorPos(targetPos, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(Random.Range(0f, 1f));
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    img.DOKill();
                    img.GetComponent<RectTransform>().DOKill();
                }
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
        ResetAllElements(images);

        if (images.Count > 1)
        {
            StartElement1FloatingEffect(images[1].GetComponent<RectTransform>());
        }

        SetElementsAlpha(images);
        SetElementsPosition(images); // 여기서 2,3,4번 크기를 0.2로 다시 잡습니다.

        DOVirtual.DelayedCall(1f, () =>
        {
            StartMovementAnimation(images);
        });
    }

    private void StartElement1FloatingEffect(RectTransform element1)
    {
        Vector2 originalPos = GetOriginalPosition(element1);

        if (_element1FloatingY != null) _element1FloatingY.Kill();
        if (_element1FloatingX != null) _element1FloatingX.Kill();

        _element1FloatingY = element1.DOAnchorPosY(originalPos.y + 20f, 2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

        _element1FloatingX = element1.DOAnchorPosX(originalPos.x + 20f, 1.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
    }

    private void ResetAllElements(List<Image> images)
    {
        KillAllTweensExceptElement1Floating();

        int[] elementIndices = { 1, 2, 3, 4, 5, 6, 7, 8 };
        for (int i = 0; i < elementIndices.Length; i++)
        {
            int elementIndex = elementIndices[i];
            if (images.Count > elementIndex)
            {
                RectTransform rectTransform = images[elementIndex].GetComponent<RectTransform>();

                if (elementIndex != 1)
                {
                    Vector2 originalPos = GetOriginalPosition(rectTransform);
                    rectTransform.DOAnchorPos(originalPos, 0f);
                }

                // [수정 포인트 1] 요청하신 분기 로직 적용
                if (elementIndex >= 2 && elementIndex <= 4)
                {
                    // 2, 3, 4번은 0.2배
                    rectTransform.localScale = Vector3.one * 0.2f;
                }
                else if (elementIndex >= 5 && elementIndex <= 7)
                {
                    // 5, 6, 7번은 0.5배
                    rectTransform.localScale = Vector3.one * 0.5f;
                }
                else
                {
                    // 1, 8번은 1배
                    rectTransform.localScale = Vector3.one;
                }

                rectTransform.rotation = Quaternion.identity;

                images[elementIndex].color = new Color(
                    images[elementIndex].color.r,
                    images[elementIndex].color.g,
                    images[elementIndex].color.b,
                    1f
                );

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
                    animationImages[elementIndex].DOKill();
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
        // [수정 포인트 2] 여기서 0.3f를 0.2f로 변경해야 Reset 설정과 충돌하지 않음
        if (images.Count > 2)
        {
            Vector2 originalPos2 = GetOriginalPosition(images[2].GetComponent<RectTransform>());
            Vector2 newPos2 = new Vector2(originalPos2.x + 0f, originalPos2.y - 242f);
            images[2].GetComponent<RectTransform>().DOAnchorPos(newPos2, 0f);
            images[2].GetComponent<RectTransform>().localScale = Vector3.one * 0.2f; // 0.2로 수정
        }
        if (images.Count > 3)
        {
            Vector2 originalPos3 = GetOriginalPosition(images[3].GetComponent<RectTransform>());
            Vector2 newPos3 = new Vector2(originalPos3.x + 21f, originalPos3.y - 220f);
            images[3].GetComponent<RectTransform>().DOAnchorPos(newPos3, 0f);
            images[3].GetComponent<RectTransform>().localScale = Vector3.one * 0.2f; // 0.2로 수정
        }
        if (images.Count > 4)
        {
            Vector2 originalPos4 = GetOriginalPosition(images[4].GetComponent<RectTransform>());
            Vector2 newPos4 = new Vector2(originalPos4.x + 21f, originalPos4.y - 220f);
            images[4].GetComponent<RectTransform>().DOAnchorPos(newPos4, 0f);
            images[4].GetComponent<RectTransform>().localScale = Vector3.one * 0.2f; // 0.2로 수정
        }
    }

    private void StartMovementAnimation(List<Image> images)
    {
        if (images.Count > 2) AnimateElement2(images[2].GetComponent<RectTransform>(), images[2], images);
        if (images.Count > 3) AnimateElement3(images[3].GetComponent<RectTransform>(), images[3], images);
        if (images.Count > 4) AnimateElement4(images[4].GetComponent<RectTransform>(), images[4], images);

        if (images.Count > 2) images[2].transform.SetSiblingIndex(1);
        if (images.Count > 3) images[3].transform.SetSiblingIndex(1);
        if (images.Count > 4) images[4].transform.SetSiblingIndex(1);
    }

    private void AnimateElement2(RectTransform element2, Image element2Image, List<Image> images)
    {
        Vector2 originalPos = GetOriginalPosition(element2);
        Vector2 firstTarget = new Vector2(originalPos.x + 0f, originalPos.y + 151f);
        Vector2 secondTarget = new Vector2(originalPos.x + 0f, originalPos.y + 26f);

        Sequence element2Sequence = DOTween.Sequence();
        element2Sequence.Join(element2Image.DOFade(1f, 0.3f));
        element2Sequence.Append(element2.DOAnchorPos(firstTarget, 0.6f).SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            if (images.Count > 1)
                            {
                                int element1Index = images[1].transform.GetSiblingIndex();
                                element2.SetSiblingIndex(element1Index + 1);
                            }
                        }));
        element2Sequence.Append(element2.DOAnchorPos(secondTarget, 1.2f).SetEase(Ease.InOutSine))
                        .OnComplete(() => {
                            StartPostAnimation(images);
                            StartFloatingEffect(element2, secondTarget);
                        });

        // 크기 변경 코드 없음 (0.2 유지)

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
        element3Sequence.Join(element3Image.DOFade(1f, 0.3f));
        element3Sequence.Append(element3.DOAnchorPos(firstTarget, 0.6f).SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            if (images.Count > 1)
                            {
                                int element1Index = images[1].transform.GetSiblingIndex();
                                element3.SetSiblingIndex(element1Index + 2);
                            }
                        }));
        element3Sequence.Append(element3.DOAnchorPos(secondTarget, 1.2f).SetEase(Ease.InOutSine));

        // 크기 변경 코드 없음 (0.2 유지)

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
        element4Sequence.Join(element4Image.DOFade(1f, 0.3f));
        element4Sequence.Append(element4.DOAnchorPos(firstTarget, 0.6f).SetEase(Ease.OutQuad)
                        .OnComplete(() => {
                            if (images.Count > 1)
                            {
                                int element1Index = images[1].transform.GetSiblingIndex();
                                element4.SetSiblingIndex(element1Index + 3);
                            }
                        }));
        element4Sequence.Append(element4.DOAnchorPos(secondTarget, 1.2f).SetEase(Ease.InOutSine));

        // 크기 변경 코드 없음 (0.2 유지)

        element4.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
    }

    private void StartFloatingEffect(RectTransform element, Vector2 centerPos)
    {
        element.DOAnchorPosY(centerPos.y + 5f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        element.DOAnchorPosX(centerPos.x + 3f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    private void StartPostAnimation(List<Image> images)
    {
        if (images.Count > 8)
        {
            images[8].DOFade(1f, 0f).OnComplete(() => {
                images[8].DOFade(0f, 0.5f);
                if (images.Count > 2) images[2].DOFade(0f, 0f);
                if (images.Count > 3) images[3].DOFade(0f, 0f);
                if (images.Count > 4) images[4].DOFade(0f, 0f);
                StartReplaceAnimation(images);
            });
        }
    }

    private void StartReplaceAnimation(List<Image> images)
    {
        // 5번(0.5배)이 2번 위치로 이동
        if (images.Count > 5 && images.Count > 2)
        {
            Vector2 pos2 = images[2].GetComponent<RectTransform>().anchoredPosition;
            images[5].GetComponent<RectTransform>().DOAnchorPos(pos2, 0f);
            images[5].DOFade(1f, 0f);
            images[5].DOFade(0f, 2f).SetDelay(1f).OnComplete(() => {
                DOVirtual.DelayedCall(1.2f, () => {
                    StartAnimationCycle(images);
                });
            });
        }

        // 6번(0.5배)이 3번 위치로 이동
        if (images.Count > 6 && images.Count > 3)
        {
            Vector2 pos3 = images[3].GetComponent<RectTransform>().anchoredPosition;
            images[6].GetComponent<RectTransform>().DOAnchorPos(pos3, 0f);
            images[6].DOFade(1f, 0f);
            images[6].DOFade(0f, 2f).SetDelay(1f);
        }

        // 7번(0.5배)이 4번 위치로 이동
        if (images.Count > 7 && images.Count > 4)
        {
            Vector2 pos4 = images[4].GetComponent<RectTransform>().anchoredPosition;
            images[7].GetComponent<RectTransform>().DOAnchorPos(pos4, 0f);
            images[7].DOFade(1f, 0f);
            images[7].DOFade(0f, 2f).SetDelay(1f);
        }
    }

    protected override void KillAllTweens()
    {
        if (_element1FloatingY != null) { _element1FloatingY.Kill(); _element1FloatingY = null; }
        if (_element1FloatingX != null) { _element1FloatingX.Kill(); _element1FloatingX = null; }

        if (animationImages != null)
        {
            int[] elementIndices = { 1, 2, 3, 4, 5, 6, 7, 8 };
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



public class CardAnimation_Num_25 : CardAnimationBase
{
    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // ====================================================================
        // 초기 설정 (수정된 값 반영)
        // ====================================================================
        SetPosScaleRotate(images, 1, 845f, -11f, 0.55f, 0f);
        SetPosScaleRotate(images, 2, -174f, -150f, 0.44f, 0f);
        SetPosScaleRotate(images, 3, 321f, -120f, 0.42f, 0f);
        SetAlpha(images, 3, 0f);
        SetPosScaleRotate(images, 4, 265f, 70f, 0.35f, 0f);
        SetAlpha(images, 4, 0f);

        // ====================================================================
        // 애니메이션 시작 (각각 독립적인 루프)
        // ====================================================================

        if (images.Count > 1) StartElement1SlideLoop(images[1].GetComponent<UnityEngine.RectTransform>());
        if (images.Count > 2) StartElement2VerticalPulse(images[2].GetComponent<UnityEngine.RectTransform>());
        // 3번 요소가 인덱스 3, 4번 요소가 인덱스 4이므로 images.Count > 4가 맞습니다.
        if (images.Count > 4) StartAlternatingFadeLoop(images[3], images[4]);
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 1번, 2번 요소 트윈 정리 (회전 및 이동)
            if (animationImages.Count > 1) animationImages[1].GetComponent<UnityEngine.RectTransform>().DOKill(true);
            if (animationImages.Count > 2) animationImages[2].GetComponent<UnityEngine.RectTransform>().DOKill(true);

            // 3번, 4번 요소 트윈 정리 (알파)
            if (animationImages.Count > 3) animationImages[3].DOKill(true);
            if (animationImages.Count > 4) animationImages[4].DOKill(true);
        }
    }

    // ========================================================================
    // 1. 1번 요소 슬라이드 루프 (속도 2배 증가: 3.0s -> 1.5s)
    // ========================================================================

    private void StartElement1SlideLoop(UnityEngine.RectTransform rect)
    {
        UnityEngine.Vector2 originalPos = rect.anchoredPosition;
        UnityEngine.Vector2 targetPos = new UnityEngine.Vector2(-1000f, originalPos.y);

        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence().SetTarget(rect);

        // 이동 시간 1.5초
        seq.Append(rect.DOAnchorPos(targetPos, 0.75f).SetEase(DG.Tweening.Ease.Linear));

        // 1.7초 대기 (유지)
        seq.AppendInterval(1.2f);

        // 루프 시 초기 위치로 즉시 복귀
        seq.OnStepComplete(() =>
        {
            rect.anchoredPosition = originalPos;
        });

        seq.SetLoops(-1, DG.Tweening.LoopType.Restart);
    }

    // ========================================================================
    // 2. 2번 요소 수직 펄스 루프 (LoopType.Yoyo로 변경, 끊김 없는 연속 루프)
    // ========================================================================

    private void StartElement2VerticalPulse(UnityEngine.RectTransform rect)
    {
        UnityEngine.Vector2 originalPos = rect.anchoredPosition;
        float moveDistance = 25f; // 위아래 총 50px 이동 (중심에서 25씩)
        float duration = 0.3f;   // 편도 이동 시간 (속도 2배 증가 반영)

        // 🌟 단일 Tween에 LoopType.Yoyo를 적용하여 끊김 없는 왕복 운동 구현 🌟
        rect.DOAnchorPosY(originalPos.y + moveDistance, duration) // 원위치에서 25만큼 위로
            .SetEase(DG.Tweening.Ease.InOutSine)
            .SetLoops(-1, DG.Tweening.LoopType.Yoyo) // 위로 갔다가 자동으로 다시 내려오고 반복
            .SetTarget(rect); // 트윈 정리용 타겟 설정
    }

    // ========================================================================
    // 3. 3번, 4번 요소 교차 페이드 루프 (마스터 시퀀스로 안정화)
    // ========================================================================

    private void StartAlternatingFadeLoop(UnityEngine.UI.Image element3, UnityEngine.UI.Image element4)
    {
        // 3번 요소의 페이드 인/아웃 시퀀스 생성
        DG.Tweening.Sequence seq3 = CreateFadeSequence(element3);

        // 4번 요소의 페이드 인/아웃 시퀀스 생성
        DG.Tweening.Sequence seq4 = CreateFadeSequence(element4);

        // 마스터 시퀀스를 만들어 두 시퀀스를 순차적으로 실행하고 무한 반복합니다.
        DG.Tweening.Sequence masterSeq = DG.Tweening.DOTween.Sequence().SetTarget(element3.transform.parent);

        // 3번 요소의 시퀀스 추가
        masterSeq.Append(seq3);

        // 3번 완료 후 4번 요소의 시퀀스 추가
        masterSeq.Append(seq4);

        // 마스터 시퀀스를 무한 반복
        masterSeq.SetLoops(-1, DG.Tweening.LoopType.Restart);
        masterSeq.Play();
    }

    // 3/4번 요소의 공통 페이드 시퀀스 생성
    private DG.Tweening.Sequence CreateFadeSequence(UnityEngine.UI.Image element)
    {
        DG.Tweening.Sequence fadeSeq = DG.Tweening.DOTween.Sequence().SetTarget(element);

        // 1. Fade In (0.1s)
        fadeSeq.Append(element.DOFade(1f, 0.1f).SetEase(DG.Tweening.Ease.Linear));

        // 2. Hold (0.2s)
        fadeSeq.AppendInterval(0.1f);

        // 3. Fade Out (0.3s)
        fadeSeq.Append(element.DOFade(0f, 0.1f).SetEase(DG.Tweening.Ease.Linear));

        return fadeSeq;
    }


    // ========================================================================
    // 보조 함수 (FQN 사용)
    // ========================================================================

    private void SetPosScaleRotate(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float x, float y, float scale, float rotZ)
    {
        if (index < imgs.Count)
        {
            UnityEngine.RectTransform rt = imgs[index].GetComponent<UnityEngine.RectTransform>();
            if (rt == null) return;

            rt.anchoredPosition = new UnityEngine.Vector2(x, y);
            rt.localScale = UnityEngine.Vector3.one * scale;
            rt.localRotation = UnityEngine.Quaternion.Euler(0f, 0f, rotZ);
        }
    }

    private void SetAlpha(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float alpha)
    {
        if (index < imgs.Count)
        {
            UnityEngine.Color c = imgs[index].color;
            imgs[index].color = new UnityEngine.Color(c.r, c.g, c.b, alpha);
        }
    }
}

public class CardAnimation_Num_26 : CardAnimationBase
{
    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // ====================================================================
        // 초기 설정
        // ====================================================================
        SetPosScaleRotate(images, 1, 0f, -110f, 0.2f, 0f);
        SetPosScaleRotate(images, 2, 0f, 107f, 0.16f, 0f);
        SetAlpha(images, 2, 0f); // 2번 요소 알파 0 초기화
        SetPosScaleRotate(images, 3, -524f, -428f, 0.6f, 0f);
        SetAlpha(images, 3, 0f);
        SetPosScaleRotate(images, 4, 524f, -428f, 0.6f, 0f);
        SetAlpha(images, 4, 0f);
        SetPosScaleRotate(images, 5, -506f, 301f, 0.6f, 0f);
        SetAlpha(images, 5, 0f);
        SetPosScaleRotate(images, 6, 506f, 301f, 0.6f, 0f);
        SetAlpha(images, 6, 0f);

        // ====================================================================
        // 애니메이션 시작
        // ====================================================================
        if (images.Count > 6)
        {
            StartAnimationLoop(images);
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (animationImages.Count > i)
                {
                    UnityEngine.RectTransform rt = animationImages[i].GetComponent<UnityEngine.RectTransform>();
                    if (rt != null) rt.DOKill(true);
                    animationImages[i].DOKill(true);
                }
            }
        }
        if (this != null) DG.Tweening.DOTween.Kill(this);
    }

    // ========================================================================
    // 메인 애니메이션 루프
    // ========================================================================

    private void StartAnimationLoop(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        UnityEngine.RectTransform rt1 = images[1].GetComponent<UnityEngine.RectTransform>();
        UnityEngine.UI.Image img2 = images[2];
        UnityEngine.UI.Image img3 = images[3];
        UnityEngine.UI.Image img4 = images[4];
        UnityEngine.UI.Image img5 = images[5];
        UnityEngine.UI.Image img6 = images[6];

        DG.Tweening.Sequence masterSeq = DG.Tweening.DOTween.Sequence().SetTarget(this);

        // --- 1. 첫 딜레이 1초 ---
        masterSeq.AppendInterval(1.0f);

        // --- 2. Phase 1: 3,4,5,6 요소 1초에 걸쳐 알파값 1 (동시) ---
        DG.Tweening.Tween fadeInTween = DG.Tweening.DOTween.Sequence()
            .Append(img3.DOFade(1f, 1.0f).SetEase(DG.Tweening.Ease.Linear).SetTarget(img3))
            .Join(img4.DOFade(1f, 1.0f).SetEase(DG.Tweening.Ease.Linear).SetTarget(img4))
            .Join(img5.DOFade(1f, 1.0f).SetEase(DG.Tweening.Ease.Linear).SetTarget(img5))
            .Join(img6.DOFade(1f, 1.0f).SetEase(DG.Tweening.Ease.Linear).SetTarget(img6));

        masterSeq.Append(fadeInTween);

        // --- 3. Phase 2: 떨림 시작 및 2번 요소 등장 시퀀스 (0.6초) ---
        DG.Tweening.Sequence phase2Seq = DG.Tweening.DOTween.Sequence().SetTarget(rt1.gameObject);

        // 1번 요소 떨림 시작
        phase2Seq.AppendCallback(() => StartElement1Tremble(rt1));

        // 2번 요소 페이드 인 (0.3초)을 Phase 2 시퀀스에 병합
        phase2Seq.Join(img2.DOFade(1f, 0.3f).SetEase(DG.Tweening.Ease.Linear).SetTarget(img2));

        // Phase 2 시퀀스의 지속시간을 확보
        phase2Seq.AppendInterval(0.3f);

        masterSeq.Append(phase2Seq);

        // 🌟 4. 떨림 중지 (2.0초 일찍 멈춤 - AppendInterval(2.0f) 삭제) 🌟
        masterSeq.AppendCallback(() =>
        {
            rt1.DOKill(true); // 1번 요소의 떨림 트윈 종료
            rt1.localRotation = UnityEngine.Quaternion.identity;
        });

        // 🌟 5. 중지 후 2.0초 대기 🌟
        masterSeq.AppendInterval(2.0f);

        // --- 6. Phase 4: 부드러운 리셋 (0.3초 페이드 아웃) ---

        // 2, 3, 4, 5, 6번 요소 알파 0으로 0.3초에 걸쳐 페이드 아웃
        DG.Tweening.Tween fadeOutTween = DG.Tweening.DOTween.Sequence()
            .Append(img2.DOFade(0f, 0.3f).SetEase(DG.Tweening.Ease.Linear).SetTarget(img2))
            .Join(img3.DOFade(0f, 0.3f).SetEase(DG.Tweening.Ease.Linear).SetTarget(img3))
            .Join(img4.DOFade(0f, 0.3f).SetEase(DG.Tweening.Ease.Linear).SetTarget(img4))
            .Join(img5.DOFade(0f, 0.3f).SetEase(DG.Tweening.Ease.Linear).SetTarget(img5))
            .Join(img6.DOFade(0f, 0.3f).SetEase(DG.Tweening.Ease.Linear).SetTarget(img6));

        masterSeq.Append(fadeOutTween);

        // --- 7. Phase 5: 반복 대기 딜레이 2초 ---
        masterSeq.AppendInterval(2.0f);

        // --- 8. 무한 반복 ---
        masterSeq.SetLoops(-1, DG.Tweening.LoopType.Restart);
    }

    // 1번 요소 떨림 효과 구현 (무한 루프)
    private void StartElement1Tremble(UnityEngine.RectTransform rect)
    {
        float trembleAngle = 5f; 
        float duration = 0.02f;

        DG.Tweening.Sequence trembleSeq = DG.Tweening.DOTween.Sequence().SetTarget(rect);

        trembleSeq.Append(rect.DOLocalRotate(new UnityEngine.Vector3(0f, 0f, trembleAngle), duration).SetEase(DG.Tweening.Ease.Linear));

        trembleSeq.Append(rect.DOLocalRotate(new UnityEngine.Vector3(0f, 0f, -trembleAngle), duration).SetEase(DG.Tweening.Ease.Linear));

        trembleSeq.SetLoops(-1, DG.Tweening.LoopType.Restart);
    }

    // ========================================================================
    // 보조 함수 (FQN 사용)
    // ========================================================================

    private void SetPosScaleRotate(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float x, float y, float scale, float rotZ)
    {
        if (index < imgs.Count)
        {
            UnityEngine.RectTransform rt = imgs[index].GetComponent<UnityEngine.RectTransform>();
            if (rt == null) return;

            rt.anchoredPosition = new UnityEngine.Vector2(x, y);
            rt.localScale = UnityEngine.Vector3.one * scale;
            rt.localRotation = UnityEngine.Quaternion.Euler(0f, 0f, rotZ);
        }
    }

    private void SetAlpha(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float alpha)
    {
        if (index < imgs.Count)
        {
            UnityEngine.Color c = imgs[index].color;
            imgs[index].color = new UnityEngine.Color(c.r, c.g, c.b, alpha);
        }
    }
}


public class CardAnimation_Num_101 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {

    }
    protected override void KillAllTweens()
    {
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
            element2.anchoredPosition = new Vector2(-220f, -77f);
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

public class CardAnimation_Num_104 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {

    }
    protected override void KillAllTweens()
    {
    }
}




public class CardAnimation_Num_105 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {

    }
    protected override void KillAllTweens()
    {
    }
}



public class CardAnimation_Num_106 : CardAnimationBase
{
    // 1번 요소 초기 Z 로테이션 값 (22도)
    private const float InitialRotZ1 = 22f;
    // 2번 요소 초기 Z 로테이션 값 (6.5度)
    private const float InitialRotZ2 = 6.5f;

    // 5번 요소의 인덱스 (복제 대상)
    private const int ELEMENT_5_INDEX = 5;

    // 동적으로 생성된 요소들을 관리하기 위한 리스트
    private System.Collections.Generic.List<UnityEngine.GameObject> _activeCopies = new System.Collections.Generic.List<UnityEngine.GameObject>();

    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // ====================================================================
        // 초기 설정
        // ====================================================================
        SetPosScaleRotate(images, 1, -862f, -292f, 0.8f, InitialRotZ1);
        SetPosScaleRotate(images, 2, 824f, -188f, 0.65f, InitialRotZ2);

        SetPosScaleRotate(images, 3, -104f, 5f, 0.7f, 0f);
        SetAlpha(images, 3, 0f);
        SetPosScaleRotate(images, 4, 365f, -20f, 0.28f, 0f);
        SetAlpha(images, 4, 0f);
        SetPosScaleRotate(images, ELEMENT_5_INDEX, 46f, 61f, 0.3f, 0f);
        SetAlpha(images, ELEMENT_5_INDEX, 0f); // 5번 요소도 페이드 인을 위해 초기 알파 0

        // ====================================================================
        // 애니메이션 시작
        // ====================================================================

        // 1번 요소 애니메이션 (3번 요소 페이드 & 5번 요소 3개 생성과 동기화)
        if (images.Count > 1)
        {
            StartRockingRotation(
                images[1].GetComponent<UnityEngine.RectTransform>(),
                InitialRotZ1,
                15f,
                0.6f,
                0.1f,
                0.2f,
                () => // 🌟 복귀 시 콜백 체인 🌟
                {
                    HandleElement3Fade(images);
                    CreateAndFadeElement5(images, -168f, 42f, 219f, -295f, 3);
                }
            );
        }

        // 2번 요소 애니메이션 (4번 요소 페이드 & 5번 요소 2개 생성과 동기화)
        if (images.Count > 2)
        {
            StartRockingRotation(
                images[2].GetComponent<UnityEngine.RectTransform>(),
                InitialRotZ2,
                -15f,
                0.8f,
                0.15f,
                0.3f,
                () => // 🌟 복귀 시 콜백 체인 🌟
                {
                    HandleElement4Fade(images);
                    CreateAndFadeElement5(images, 278f, 442f, 116f, -228f, 2);
                }
            );
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 1, 2번 (회전) 및 3, 4번 (페이드) 트윈 정리
            if (animationImages.Count > 1) animationImages[1].GetComponent<UnityEngine.RectTransform>().DOKill(true);
            if (animationImages.Count > 2) animationImages[2].GetComponent<UnityEngine.RectTransform>().DOKill(true);
            if (animationImages.Count > 3) animationImages[3].DOKill(true);
            if (animationImages.Count > 4) animationImages[4].DOKill(true);
            if (animationImages.Count > ELEMENT_5_INDEX) animationImages[ELEMENT_5_INDEX].DOKill(true); // 원본 5번 요소 트윈 정리

            // 🌟 동적으로 생성된 5번 요소 복사본들 정리 및 파괴 🌟
            foreach (UnityEngine.GameObject copy in _activeCopies)
            {
                if (copy != null)
                {
                    copy.GetComponent<UnityEngine.UI.Image>().DOKill(true);
                    UnityEngine.GameObject.Destroy(copy);
                }
            }
            _activeCopies.Clear();
        }
    }

    // ====================================================================
    // 1. Z축 반복 회전 로직 (콜백 추가)
    // ====================================================================

    private void StartRockingRotation(
        UnityEngine.RectTransform rect,
        float initialAngle,
        float angleIncrement,
        float durationToTarget,
        float durationToInitial,
        float delay,
        System.Action onReturnCallback // 추가된 콜백
    )
    {
        float targetAngle = initialAngle + angleIncrement;

        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence().SetTarget(rect);

        // 1. 목표 각도로 기울임
        seq.Append(
            rect.DOLocalRotate(new UnityEngine.Vector3(0f, 0f, targetAngle), durationToTarget)
                .SetEase(DG.Tweening.Ease.OutSine)
        );

        // 2. 원래 각도로 빠르게 복귀
        seq.Append(
            rect.DOLocalRotate(new UnityEngine.Vector3(0f, 0f, initialAngle), durationToInitial)
                .SetEase(DG.Tweening.Ease.InSine)
        );

        // 🌟 콜백 실행: 복귀 트윈 완료 직후
        if (onReturnCallback != null)
        {
            seq.AppendCallback(() => onReturnCallback());
        }

        // 3. 대기 후 루프
        seq.AppendInterval(delay);

        seq.SetLoops(-1, DG.Tweening.LoopType.Restart);
    }

    // ====================================================================
    // 2. 3번 요소 페이드 애니메이션 콜백
    // ====================================================================

    private void HandleElement3Fade(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count <= 3) return;
        UnityEngine.UI.Image element3 = images[3];

        element3.DOKill(true); // 기존 트윈 정리 (안전을 위해) 

        DG.Tweening.Sequence fadeSeq = DG.Tweening.DOTween.Sequence().SetTarget(element3);

        fadeSeq.Append(element3.DOFade(1f, 0.1f));      // Fade In (0.1s)
        fadeSeq.AppendInterval(0.2f);                   // Hold (0.2s)
        fadeSeq.Append(element3.DOFade(0f, 0.2f));      // Fade Out (0.2s)
    }

    // ====================================================================
    // 3. 4번 요소 페이드 애니메이션 콜백
    // ====================================================================

    private void HandleElement4Fade(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count <= 4) return;
        UnityEngine.UI.Image element4 = images[4];

        element4.DOKill(true); // 기존 트윈 정리 (안전을 위해) 

        DG.Tweening.Sequence fadeSeq = DG.Tweening.DOTween.Sequence().SetTarget(element4);

        fadeSeq.Append(element4.DOFade(1f, 0.1f));      // Fade In (0.1s)
        fadeSeq.AppendInterval(0.2f);                   // Hold (0.2s)
        fadeSeq.Append(element4.DOFade(0f, 0.2f));      // Fade Out (0.2s)
    }

    // ====================================================================
    // 4. 5번 요소 복제 및 페이드 애니메이션
    // ====================================================================

    private void CreateAndFadeElement5(
        System.Collections.Generic.List<UnityEngine.UI.Image> images,
        float minX, float maxX, float minY, float maxY,
        int count
    )
    {
        if (images.Count <= ELEMENT_5_INDEX) return;

        UnityEngine.GameObject originalElement5 = images[ELEMENT_5_INDEX].gameObject;
        UnityEngine.Transform parentTransform = originalElement5.transform.parent;

        for (int i = 0; i < count; i++)
        {
            // 1. 5번 요소 복제 및 리스트에 추가
            UnityEngine.GameObject copy = UnityEngine.GameObject.Instantiate(originalElement5, parentTransform);
            UnityEngine.UI.Image copyImage = copy.GetComponent<UnityEngine.UI.Image>();
            copyImage.color = new UnityEngine.Color(copyImage.color.r, copyImage.color.g, copyImage.color.b, 0f); // 초기 알파 0
            _activeCopies.Add(copy);

            // 2. 무작위 위치 설정
            float randomX = UnityEngine.Random.Range(minX, maxX);
            float randomY = UnityEngine.Random.Range(minY, maxY);
            copy.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(randomX, randomY);

            // 3. 페이드 애니메이션 시퀀스
            DG.Tweening.Sequence fadeSeq = DG.Tweening.DOTween.Sequence().SetTarget(copyImage);

            fadeSeq.Append(copyImage.DOFade(1f, 0.1f));          // Fade In (0.1s)
            fadeSeq.AppendInterval(0.1f);                       // Hold (0.1s)
            fadeSeq.Append(copyImage.DOFade(0f, 0.3f));          // Fade Out (0.3s)

            // 4. 애니메이션 완료 후 GameObject 파괴
            fadeSeq.OnComplete(() =>
            {
                if (copy != null)
                {
                    _activeCopies.Remove(copy); // 리스트에서 제거
                    UnityEngine.GameObject.Destroy(copy);
                }
            });
        }
    }

    // ====================================================================
    // 보조 함수 (FQN 사용)
    // ====================================================================

    private void SetPosScaleRotate(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float x, float y, float scale, float rotZ)
    {
        if (index < imgs.Count)
        {
            UnityEngine.RectTransform rt = imgs[index].GetComponent<UnityEngine.RectTransform>();
            if (rt == null) return;

            rt.anchoredPosition = new UnityEngine.Vector2(x, y);
            rt.localScale = UnityEngine.Vector3.one * scale;
            rt.localRotation = UnityEngine.Quaternion.Euler(0f, 0f, rotZ);
        }
    }

    private void SetAlpha(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float alpha)
    {
        if (index < imgs.Count)
        {
            UnityEngine.Color c = imgs[index].color;
            imgs[index].color = new UnityEngine.Color(c.r, c.g, c.b, alpha);
        }
    }
}




public class CardAnimation_Num_107 : CardAnimationBase
{
    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        KillAllTweens();
        InitElements(images);

        StartMainSequence(images);
    }

    private void InitElements(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 0. 전체 요소 초기화 (위치, 회전, 스케일)
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.rotation = UnityEngine.Quaternion.identity;
            images[i].rectTransform.localScale = UnityEngine.Vector3.one;
            SetAlpha(images[i], 1f);
        }

        // 2번, 3번, 4번 요소 초기 위치 및 스케일 설정
        if (images.Count > 2) SetRect(images[2], 0f, -448f, 1f);
        if (images.Count > 3) SetRect(images[3], 0f, -61f, 0.45f);
        if (images.Count > 4)
        {
            SetRect(images[4], 0f, 0f, 0.3f);
            SetAlpha(images[4], 0f);
        }
    }

    private void StartMainSequence(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 1번 요소 웅웅거림
        if (images.Count > 1)
        {
            ApplyPulsingScale(images[1].rectTransform, 1.1f, 0.9f, 2.0f, 0f);
        }

        // 3번 요소 덜덜 떨림 (4번 요소 스폰 동기화)
        if (images.Count > 3)
        {
            StartDogShakeEffectRecursive(images[3].rectTransform, images);
        }
    }

    private void StartDogShakeEffectRecursive(UnityEngine.RectTransform target, System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count > 4)
        {
            StartSpawnEffect(images[4]);
        }

        DG.Tweening.Sequence shakeBurst = DG.Tweening.DOTween.Sequence();

        for (int i = 0; i < 5; i++)
        {
            shakeBurst.Append(target.DORotate(new UnityEngine.Vector3(0, 0, 10f), 0.05f).SetEase(DG.Tweening.Ease.Linear));
            shakeBurst.Append(target.DORotate(new UnityEngine.Vector3(0, 0, -10f), 0.05f).SetEase(DG.Tweening.Ease.Linear));
        }

        DG.Tweening.Sequence controlSequence = DG.Tweening.DOTween.Sequence();
        controlSequence.Append(shakeBurst);
        controlSequence.Append(target.DORotate(UnityEngine.Vector3.zero, 0.1f));

        float restDuration = UnityEngine.Random.Range(0.2f, 1.0f);

        controlSequence.AppendInterval(restDuration)
            .OnComplete(() => {
                if (target != null && target.gameObject.activeInHierarchy)
                {
                    StartDogShakeEffectRecursive(target, images);
                }
            });
    }

    private void StartSpawnEffect(UnityEngine.UI.Image originalImage)
    {
        // 생성 범위 정의
        UnityEngine.Vector2 range1X = new UnityEngine.Vector2(-528f, -355f);
        UnityEngine.Vector2 rangeY = new UnityEngine.Vector2(0f, 100f);
        UnityEngine.Vector2 range2X = new UnityEngine.Vector2(355f, 528f);

        // 1. Clone 1: Left Range
        float x1 = UnityEngine.Random.Range(range1X.x, range1X.y);
        float y1 = UnityEngine.Random.Range(rangeY.x, rangeY.y);
        // ⭐ 수정된 CreateAndSetupElement 호출
        UnityEngine.UI.Image clone1 = CreateAndSetupElement(originalImage, "SpawnL", x1, y1, originalImage.rectTransform.localScale.x, 0f);
        if (clone1 != null) AnimateClone(clone1);

        // 2. Clone 2: Right Range
        float x2 = UnityEngine.Random.Range(range2X.x, range2X.y);
        float y2 = UnityEngine.Random.Range(rangeY.x, rangeY.y);
        // ⭐ 수정된 CreateAndSetupElement 호출
        UnityEngine.UI.Image clone2 = CreateAndSetupElement(originalImage, "SpawnR", x2, y2, originalImage.rectTransform.localScale.x, 0f);
        if (clone2 != null) AnimateClone(clone2);
    }

    private void AnimateClone(UnityEngine.UI.Image clone)
    {
        UnityEngine.RectTransform rt = clone.rectTransform;
        UnityEngine.Vector2 targetPos = rt.anchoredPosition + new UnityEngine.Vector2(0f, 300f);

        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence();
        float moveDuration = 0.5f;
        float fadeOutDuration = 0.3f;

        seq.Append(clone.DOFade(1f, 0.2f).SetEase(DG.Tweening.Ease.Linear));
        seq.Join(rt.DOAnchorPos(targetPos, moveDuration).SetEase(DG.Tweening.Ease.OutQuad));
        seq.Insert(seq.Duration() - fadeOutDuration, clone.DOFade(0f, fadeOutDuration).SetEase(DG.Tweening.Ease.InSine));

        seq.OnComplete(() => {
            if (clone != null && clone.gameObject != null)
            {
                UnityEngine.Object.Destroy(clone.gameObject);
            }
        });

        seq.Play();
    }

    private void ApplyPulsingScale(UnityEngine.RectTransform target, float maxRatio, float minRatio, float duration, float delay)
    {
        UnityEngine.Vector3 baseScale = target.localScale;
        UnityEngine.Vector3 targetScaleMin = baseScale * minRatio;

        target.DOScale(targetScaleMin, duration * 0.5f)
              .SetEase(DG.Tweening.Ease.InOutSine)
              .SetDelay(delay)
              .SetLoops(-1, DG.Tweening.LoopType.Yoyo);
    }

    // ===================================
    // 🛠️ 필수 헬퍼 함수 (부모 설정 로직 수정됨)
    // ===================================

    private void SetRect(UnityEngine.UI.Image img, float x, float y, float scale)
    {
        UnityEngine.RectTransform rt = img.rectTransform;
        rt.anchoredPosition = new UnityEngine.Vector2(x, y);
        rt.localScale = UnityEngine.Vector3.one * scale;
    }

    private void SetAlpha(UnityEngine.UI.Image img, float alpha)
    {
        img.color = new UnityEngine.Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    /// <summary>
    /// 원본 Image를 복제하고 위치, 스케일, 알파값을 설정합니다. (Instantiate 오버로드 수정)
    /// </summary>
    private UnityEngine.UI.Image CreateAndSetupElement(UnityEngine.UI.Image original, string name, float x, float y, float scale, float alpha)
    {
        // ⭐ 수정: 복제와 동시에 부모를 지정하여 RectTransform 데이터 손상을 방지합니다 (119번 방식).
        UnityEngine.GameObject newObj = UnityEngine.Object.Instantiate(original.gameObject, original.transform.parent);
        newObj.name = original.name + "_" + name;
        UnityEngine.UI.Image newImage = newObj.GetComponent<UnityEngine.UI.Image>();

        if (newImage == null) return null;

        // 활성화 및 Sibling Index 설정
        newObj.SetActive(true);
        newImage.transform.SetAsLastSibling();

        SetRect(newImage, x, y, scale);
        SetAlpha(newImage, alpha);

        return newImage;
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    DG.Tweening.DOTween.Kill(img);
                    DG.Tweening.DOTween.Kill(img.GetComponent<UnityEngine.RectTransform>());
                }
            }
        }
    }
}


public class CardAnimation_Num_108 : CardAnimationBase
{
    // ====================================================================
    // 1. 멤버 변수 및 상수
    // ====================================================================

    private const int FLASH_ELEMENT_INDEX = 6;
    private const int SPARKLE_ELEMENT_INDEX = 5;

    private readonly int[] ACTIVATION_TARGETS = { 2, 3, 4 };

    // 동적으로 생성된 요소들을 관리하기 위한 리스트
    private System.Collections.Generic.List<UnityEngine.GameObject> _activeCopies = new System.Collections.Generic.List<UnityEngine.GameObject>();

    private int _moveCount = 0; // 구조 유지를 위해 포함

    // ====================================================================
    // 2. ExecuteAnimation (초기 설정 및 애니메이션 시작)
    // ====================================================================

    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        _moveCount = 0;

        // 초기 값 설정 (사용자 지정)
        SetPosAndScale(images, 1, 114f, -63f, 1.2f);
        SetPosAndScale(images, 2, -130f, -24f, 0.33f);
        SetAlpha(images, 2, 0f);
        SetPosAndScale(images, 3, -200f, -40f, 0.8f);
        SetAlpha(images, 3, 0f);
        SetPosAndScale(images, 4, -290f, -10f, 0.8f);
        SetAlpha(images, 4, 0f);
        SetPosAndScale(images, 5, 0f, 0f, 0.57f);
        SetAlpha(images, 5, 0f);
        SetAlpha(images, 6, 0f);

        if (images.Count > 1)
        {
            DG.Tweening.DOVirtual.DelayedCall(1.0f, () =>
            {
                StartMovementSequence(images);
            }).SetTarget(this);
        }
    }

    // ====================================================================
    // 3. KillAllTweens (트윈 및 동적 객체 정리)
    // ====================================================================

    protected override void KillAllTweens()
    {
        if (this != null) DG.Tweening.DOTween.Kill(this);

        if (animationImages != null)
        {
            int[] elementsToKill = { 1, 2, 3, 4, 6 };
            foreach (int index in elementsToKill)
            {
                if (animationImages.Count > index)
                {
                    animationImages[index].DOKill(true);
                    if (index != 1) animationImages[index].GetComponent<UnityEngine.RectTransform>().DOKill(true);
                }
            }

            foreach (UnityEngine.GameObject copy in _activeCopies)
            {
                if (copy != null)
                {
                    copy.GetComponent<UnityEngine.UI.Image>().DOKill(true);
                    copy.GetComponent<UnityEngine.RectTransform>().DOKill(true);
                    UnityEngine.GameObject.Destroy(copy);
                }
            }
            _activeCopies.Clear();
        }
    }

    // ====================================================================
    // 4. 메인 시퀀스 로직
    // ====================================================================

    private void StartMovementSequence(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count <= 1) return;

        UnityEngine.RectTransform element1 = images[1].GetComponent<UnityEngine.RectTransform>();
        float initialY = element1.anchoredPosition.y;

        UnityEngine.Vector2 targetPos1 = new UnityEngine.Vector2(225f, initialY);
        UnityEngine.Vector2 targetPos2 = new UnityEngine.Vector2(72f, initialY);

        DG.Tweening.Sequence moveSequence = DG.Tweening.DOTween.Sequence().SetTarget(element1);

        // 3회 이동 시퀀스를 명시적으로 Append
        for (int i = 0; i < 3; i++)
        {
            moveSequence.Append(CreateSingleMoveCycle(element1, targetPos1, targetPos2, images, i));
        }

        // 시퀀스 끝에 리셋 및 재시작 콜백 추가 (부드러운 정리 포함)
        moveSequence.AppendCallback(() => ResetAndLoop(images));
    }

    private DG.Tweening.Sequence CreateSingleMoveCycle(UnityEngine.RectTransform element1, UnityEngine.Vector2 targetPos1, UnityEngine.Vector2 targetPos2, System.Collections.Generic.List<UnityEngine.UI.Image> images, int cycleIndex)
    {
        DG.Tweening.Sequence cycleSeq = DG.Tweening.DOTween.Sequence();

        bool isLastCycle = (cycleIndex == ACTIVATION_TARGETS.Length - 1);

        // 1. A지점(225)까지 이동 (0.6s) -> 대기 (0.2s)
        cycleSeq.Append(element1.DOAnchorPos(targetPos1, 0.6f).SetEase(DG.Tweening.Ease.OutSine));
        cycleSeq.AppendInterval(0.2f);

        // 2. B지점(72)까지 빠르게 복귀 (0.1s)
        cycleSeq.Append(element1.DOAnchorPos(targetPos2, 0.1f).SetEase(DG.Tweening.Ease.OutQuad));

        // 3. 72 도달 즉시 콜백 실행 (핵심 로직)
        cycleSeq.AppendCallback(() =>
        {
            if (cycleIndex < ACTIVATION_TARGETS.Length)
            {
                int targetIndex = ACTIVATION_TARGETS[cycleIndex];

                StartFlashAndActivation(images, targetIndex, isLastCycle);

                // 4번 요소 활성화 시점과 동시에 5번 요소 복제 시작
                if (isLastCycle)
                {
                    StartSparkleAndHold(images);
                }
            }
        });

        // 4. 추가 대기 (0.5s)
        cycleSeq.AppendInterval(0.5f);

        // 5. 마지막 사이클인 경우, 3초간 정지 대기
        if (isLastCycle)
        {
            cycleSeq.AppendInterval(3.0f);
        }

        return cycleSeq;
    }

    private void StartFlashAndActivation(System.Collections.Generic.List<UnityEngine.UI.Image> images, int targetIndex, bool holdTargetElement)
    {
        if (images.Count <= FLASH_ELEMENT_INDEX) return;

        UnityEngine.UI.Image flashElement = images[FLASH_ELEMENT_INDEX];
        UnityEngine.UI.Image targetElement = images[targetIndex];

        // --- 6번 요소 플래시 (0.1s 인 -> 0.1s 아웃) ---
        DG.Tweening.Sequence flashSeq = DG.Tweening.DOTween.Sequence().SetTarget(flashElement);
        flashSeq.Append(flashElement.DOFade(1f, 0.1f));
        flashSeq.Append(flashElement.DOFade(0f, 0.1f));

        // --- 타겟 요소 활성화 및 페이드 아웃 ---
        targetElement.color = new UnityEngine.Color(targetElement.color.r, targetElement.color.g, targetElement.color.b, 1f);

        if (!holdTargetElement)
        {
            // 2번, 3번 요소는 0.2초 유지 후 0.3초에 걸쳐 알파 0으로 페이드 아웃
            DG.Tweening.Sequence targetSeq = DG.Tweening.DOTween.Sequence().SetTarget(targetElement);
            targetSeq.AppendInterval(0.2f);
            targetSeq.Append(targetElement.DOFade(0f, 0.3f));
        }
    }

    // ====================================================================
    // 5. 복제 및 반짝임 로직
    // ====================================================================

    private void StartSparkleAndHold(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count <= SPARKLE_ELEMENT_INDEX) return;

        // 반복 전에 이전 복사본 정리 (KillAllTweens이 담당하지만, 안전을 위해 남은 트윈 Kill)
        foreach (UnityEngine.GameObject copy in _activeCopies)
        {
            if (copy != null) copy.GetComponent<UnityEngine.UI.Image>().DOKill(true);
        }

        UnityEngine.Vector2[] positions5 = {
            new UnityEngine.Vector2(-400f, 278f),
            new UnityEngine.Vector2(-575f, -244f),
            new UnityEngine.Vector2(-189f, -167f)
        };

        foreach (UnityEngine.Vector2 pos in positions5)
        {
            UnityEngine.GameObject copy = UnityEngine.GameObject.Instantiate(images[SPARKLE_ELEMENT_INDEX].gameObject, images[SPARKLE_ELEMENT_INDEX].transform.parent);
            UnityEngine.UI.Image copyImage = copy.GetComponent<UnityEngine.UI.Image>();
            copy.GetComponent<UnityEngine.RectTransform>().anchoredPosition = pos;

            copyImage.color = new UnityEngine.Color(copyImage.color.r, copyImage.color.g, copyImage.color.b, 1f);
            _activeCopies.Add(copy);

            DG.Tweening.Sequence sparkleSeq = DG.Tweening.DOTween.Sequence().SetTarget(copyImage);

            sparkleSeq.AppendInterval(UnityEngine.Random.Range(0f, 0.5f));

            sparkleSeq.Append(copyImage.DOFade(0f, 0.05f).SetEase(DG.Tweening.Ease.Linear));
            sparkleSeq.Append(copyImage.DOFade(1f, 0.05f).SetEase(DG.Tweening.Ease.Linear));

            sparkleSeq.SetLoops(-1, DG.Tweening.LoopType.Restart);
        }
    }

    // ====================================================================
    // 6. 루프 리셋 로직
    // ====================================================================

    private void ResetAndLoop(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 1. 부드러운 정리 시퀀스 시작 (부자연스러운 전환 방지)
        UnityEngine.RectTransform element1 = images[1].GetComponent<UnityEngine.RectTransform>();
        UnityEngine.Vector2 startPos1 = new UnityEngine.Vector2(114f, element1.anchoredPosition.y);
        UnityEngine.UI.Image element4 = images[4];

        DG.Tweening.Sequence cleanupSeq = DG.Tweening.DOTween.Sequence().SetTarget(this);

        // Element 4 페이드 아웃과 Element 1 원위치 복귀 동시 진행 (0.5s)
        cleanupSeq.Append(element4.DOFade(0f, 0.5f));
        cleanupSeq.Join(element1.DOAnchorPos(startPos1, 0.5f));

        // 생성된 모든 복사본 페이드 아웃 및 파괴 준비
        foreach (UnityEngine.GameObject copy in _activeCopies)
        {
            if (copy != null)
            {
                UnityEngine.UI.Image copyImage = copy.GetComponent<UnityEngine.UI.Image>();
                copyImage.DOKill();
                cleanupSeq.Join(copyImage.DOFade(0f, 0.3f));
            }
        }

        // 2. 부드러운 정리 완료 후, 모든 트윈을 강제 종료하고 애니메이션 재시작
        cleanupSeq.OnComplete(() =>
        {
            KillAllTweens();
            ExecuteAnimation(images);
        });
    }

    // ====================================================================
    // 7. 보조 함수 (FQN 사용)
    // ====================================================================

    private void SetPosAndScale(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float x, float y, float scale)
    {
        if (index < imgs.Count)
        {
            UnityEngine.RectTransform rt = imgs[index].GetComponent<UnityEngine.RectTransform>();
            if (rt == null) return;

            rt.anchoredPosition = new UnityEngine.Vector2(x, y);
            rt.localScale = UnityEngine.Vector3.one * scale;
        }
    }

    private void SetAlpha(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float alpha)
    {
        if (index < imgs.Count)
        {
            UnityEngine.Color c = imgs[index].color;
            imgs[index].color = new UnityEngine.Color(c.r, c.g, c.b, alpha);
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
        cloneRect.localScale = Vector3.one * 0.3f;
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



public class CardAnimation_Num_110 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        // ====================================================================
        // 1. 초기 설정 (크기, 알파, 위치)
        // ====================================================================

        // 1. 크기(Scale) 설정
        for (int i = 1; i <= 11; i++)
        {
            if (i >= images.Count) break;

            images[i].rectTransform.localScale = Vector3.one * 0.4f;
            images[i].rectTransform.localRotation = Quaternion.identity;
        }

        // 2. 알파(Alpha) 설정
        SetAlphaZero(images, 1);
        SetAlphaZero(images, 2);

        // 3. 위치(Position) 설정
        float initialY_3 = -120f;

        // 3번: 0, -80
        SetPosition(images, 3, 0f, initialY_3);

        // 4, 5번: 329, -271
        SetPosition(images, 4, 329f, -271f);
        SetPosition(images, 5, 329f, -271f);

        // 6, 7번: -474, -322
        SetPosition(images, 6, -474f, -322f);
        SetPosition(images, 7, -474f, -322f);

        // 8, 9번: 525, -343
        SetPosition(images, 8, 525f, -343f);
        SetPosition(images, 9, 525f, -343f);

        // 10, 11번: -291, -257
        SetPosition(images, 10, -291f, -257f);
        SetPosition(images, 11, -291f, -257f);

        // ====================================================================
        // 2. 애니메이션 구현 (Tween)
        // ====================================================================

        const float jumpDuration = 0.7f;
        const float jumpHeight = 330f;
        const float halfDuration = jumpDuration / 2f; // 0.5초
        const float rotateAngle = -20f;

        // Z축 떨림 효과 상수
        const float tremorScale = 0f;
        const float tremorRotate = 10f;
        const float cycleTime = 0.08f;
        int loopCount = (int)(halfDuration / (cycleTime * 2));

        // 시퀀스 생성 및 무한 반복 설정
        Sequence mySequence = DOTween.Sequence()
            .SetLoops(-1, LoopType.Restart)
            .SetTarget(this);

        // 0. 바람 이펙트 시작
        mySequence.AppendCallback(() => StartWindEffect(images));


        // 1. 3번 카드 점프 (상승)
        mySequence.Append(images[3].rectTransform.DOAnchorPosY(initialY_3 + jumpHeight, halfDuration)
            .SetEase(Ease.OutSine));
        mySequence.Join(images[3].rectTransform.DOLocalRotate(new Vector3(0, 0, rotateAngle), halfDuration)
            .SetEase(Ease.OutSine));


        // 2. Z축 떨림 효과 (점프 상승과 동시 실행)

        // -Z 방향 떨림: 4, 8번
        CreateTremorSequenceInSequence(mySequence, images[4].rectTransform, -tremorScale, -tremorRotate, cycleTime, loopCount);
        CreateTremorSequenceInSequence(mySequence, images[8].rectTransform, -tremorScale, -tremorRotate, cycleTime, loopCount);

        // +Z 방향 떨림: 6, 10번
        CreateTremorSequenceInSequence(mySequence, images[6].rectTransform, tremorScale, tremorRotate, cycleTime, loopCount);
        CreateTremorSequenceInSequence(mySequence, images[10].rectTransform, tremorScale, tremorRotate, cycleTime, loopCount);


        // 3. 3번 카드 착지 (하강)
        mySequence.Append(images[3].rectTransform.DOAnchorPosY(initialY_3, halfDuration)
            .SetEase(Ease.InSine));
        mySequence.Join(images[3].rectTransform.DOLocalRotate(Vector3.zero, halfDuration)
            .SetEase(Ease.InSine));

        // 4. 반복 대기 시간 추가 (0.5초)
        mySequence.AppendInterval(0.5f);
    }

    private void StartWindEffect(List<Image> images)
    {
        if (images.Count <= 1) return;

        const float moveDuration = 0.5f;
        const float fadeDuration = 0.1f;

        // 0번 요소(배경)의 Sibling Index를 가져와 +1 한 값에 복제 요소를 배치 (바로 앞으로)
        int desiredSiblingIndex = images[0].rectTransform.GetSiblingIndex() + 1;

        // 1번 요소 복제 (초기 위치: 100, -200 | 이동 목표: 800, -200)
        if (images.Count > 1 && images[1] != null)
        {
            CreateMovingClone(images[1], 300f, -240f, 800f, -240f, moveDuration, fadeDuration, this, desiredSiblingIndex);
        }

        // 2번 요소 복제 (초기 위치: -100, -200 | 이동 목표: -800, -200)
        if (images.Count > 2 && images[2] != null)
        {
            CreateMovingClone(images[2], -300f, -240f, -800f, -240f, moveDuration, fadeDuration, this, desiredSiblingIndex);
        }
    }

    private void CreateMovingClone(Image originalImage, float startX, float startY, float targetX, float targetY, float duration, float fadeDuration, object targetObject, int desiredSiblingIndex)
    {
        // 복제본 생성
        GameObject cloneObj = GameObject.Instantiate(originalImage.gameObject, originalImage.transform.parent);
        RectTransform cloneRect = cloneObj.GetComponent<RectTransform>();
        CanvasGroup cloneCanvasGroup = cloneObj.GetComponent<CanvasGroup>();

        if (cloneCanvasGroup == null)
        {
            cloneCanvasGroup = cloneObj.AddComponent<CanvasGroup>();
        }

        // 초기 위치 설정
        cloneRect.anchoredPosition = new Vector2(startX, startY);
        cloneCanvasGroup.alpha = 0f;

        // ★ Sibling Index를 0번 요소 바로 다음(앞)으로 설정
        cloneRect.SetSiblingIndex(desiredSiblingIndex);

        // 시퀀스 구성
        Sequence cloneSequence = DOTween.Sequence().SetTarget(targetObject);

        // 1. 알파 페이드 인 (등장)
        cloneSequence.Append(cloneCanvasGroup.DOFade(1f, fadeDuration));

        // 2. X, Y축으로 이동
        cloneSequence.Append(cloneRect.DOAnchorPos(new Vector2(targetX, targetY), duration).SetEase(Ease.OutQuad));

        // 3. 이동 종료 시점에 알파 페이드 아웃 (사라짐)
        cloneSequence.Join(cloneCanvasGroup.DOFade(0f, fadeDuration).SetDelay(duration - fadeDuration));

        // 4. 애니메이션 완료 후 오브젝트 제거
        cloneSequence.OnComplete(() => {
            GameObject.Destroy(cloneObj);
        });
    }

    // 시퀀스 내에 떨림 시퀀스를 Join 시키는 Helper 함수
    private void CreateTremorSequenceInSequence(Sequence mainSequence, RectTransform element, float scaleOffset, float rotateOffset, float cycleTime, int loopCount)
    {
        Sequence tremorSequence = DOTween.Sequence()
            .SetTarget(this)
            .Append(element.DOScale(element.localScale.x + scaleOffset, cycleTime))
            .Join(element.DOLocalRotate(new Vector3(0, 0, rotateOffset), cycleTime).SetEase(Ease.InOutSine))
            .Append(element.DOScale(element.localScale.x, cycleTime))
            .Join(element.DOLocalRotate(Vector3.zero, cycleTime).SetEase(Ease.InOutSine))
            .SetLoops(loopCount, LoopType.Yoyo)
            .SetTarget(element.gameObject);

        mainSequence.Join(tremorSequence);
    }

    private void SetAlphaZero(List<Image> images, int index)
    {
        if (index < images.Count)
        {
            CanvasGroup cg = images[index].GetComponent<CanvasGroup>();
            if (cg == null) cg = images[index].gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
        }
    }

    private void SetPosition(List<Image> images, int index, float x, float y)
    {
        if (index < images.Count)
        {
            images[index].rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null) img.rectTransform.DOKill();
            }
        }
        DOTween.Kill(this);
    }
}




public class CardAnimation_Num_112 : CardAnimationBase
{
    // ⭐ 추가: 4번 요소의 무한 스케일 트윈을 제어할 멤버 변수
    private DG.Tweening.Tween _element4PulsingTween;
    // ⭐ 추가: 전체 애니메이션 루프 딜레이를 제어할 멤버 변수
    private DG.Tweening.Tween _repeatTween;

    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        KillAllTweens();
        InitElements(images);

        // 메인 애니메이션 실행
        StartMainSequence(images);
    }

    private void InitElements(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 0. 전체 요소 초기화 (위치, 회전, 스케일)
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.rotation = UnityEngine.Quaternion.identity;
            images[i].rectTransform.localScale = UnityEngine.Vector3.one;
            SetAlpha(images[i], 1f); // 기본적으로 보이게 설정
        }

        // --- 요청하신 개별 위치, 크기 및 알파값 세팅 ---

        // 1번 요소 (Index 1): (-652, -487, 0.5배)
        if (images.Count > 1)
        {
            SetRect(images[1], -652f, -487f, 0.5f);
        }

        // 2번 요소 (Index 2): (-300, -26, 0.2배, 알파 0)
        if (images.Count > 2)
        {
            SetRect(images[2], -300f, -26f, 0.2f);
            SetAlpha(images[2], 0f);
        }

        // 3번 요소 (Index 3): (93, 120, 0.213배, 알파 0)
        if (images.Count > 3)
        {
            SetRect(images[3], 93f, 120f, 0.213f);
            SetAlpha(images[3], 0f);
        }

        // 4번 요소 (Index 4): (657, -388, 0.57배, 알파 0)
        // ⭐ 초기 스케일을 0으로 설정하여 팝인 애니메이션 시작 준비
        if (images.Count > 4)
        {
            SetRect(images[4], 657f, -388f, 0f); // 크기 0으로 시작
            SetAlpha(images[4], 0f);
        }

        // 5번 요소 (Index 5): (160, -202, 0.34배, 알파 0)
        if (images.Count > 5)
        {
            SetRect(images[5], 160f, -202f, 0.34f);
            SetAlpha(images[5], 0f);
        }

        // 6번 요소 (Index 6): (490, 3, 0.34배, 알파 0)
        if (images.Count > 6)
        {
            SetRect(images[6], 490f, 3f, 0.34f);
            SetAlpha(images[6], 0f);
        }
    }

    // ===================================
    // ✨ 메인 시퀀스 함수: 바톤 터치 릴레이 + 동시 팝인 + 4번 요소 무한 맥동
    // ===================================

    private void StartMainSequence(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // 필요한 요소 참조
        UnityEngine.RectTransform rt1 = images[1].rectTransform;
        UnityEngine.UI.Image img2 = images[2];
        UnityEngine.RectTransform rt2 = img2.rectTransform;
        UnityEngine.UI.Image img3 = images[3];
        UnityEngine.RectTransform rt3 = img3.rectTransform;

        // ⭐추가된 요소 참조⭐
        UnityEngine.UI.Image img4 = images[4];
        UnityEngine.RectTransform rt4 = img4.rectTransform;
        UnityEngine.UI.Image img5 = images[5];
        UnityEngine.UI.Image img6 = images[6];

        // 타겟 좌표 및 스케일 계산
        UnityEngine.Vector2 targetPos3_Start = new UnityEngine.Vector2(93f, 120f);       // 2번의 목표 / 3번의 시작 위치
        UnityEngine.Vector2 targetPos4_End = new UnityEngine.Vector2(657f, -388f);       // 3번의 최종 목표 위치
        float targetScale4 = 0.57f;
        float overshootScale4 = targetScale4 * 1.1f; // 0.684f

        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence();

        // 1. A: 1번 Rot 0 -> 25 (1.0s)
        seq.Append(rt1.DORotate(new UnityEngine.Vector3(0, 0, 25f), 1.0f).SetEase(DG.Tweening.Ease.OutQuad));

        // 2. B: 1번 Rot 25 -> 0 (0.1s)
        seq.Append(rt1.DORotate(new UnityEngine.Vector3(0, 0, 0f), 0.1f).SetEase(DG.Tweening.Ease.InQuad));

        // 3. C, D: 2번 Alpha up (0.1s) & Move to Element 3's pos (1.0s, Linear)
        seq.Append(img2.DOFade(1f, 0.1f));
        DG.Tweening.Tween moveD = rt2.DOAnchorPos(targetPos3_Start, 1.0f).SetEase(DG.Tweening.Ease.Linear);
        seq.Join(moveD);

        // 4. E: Fade Out/In Handover (Step D 이동 1.0초 중 0.7초 시점, 즉 끝나기 0.3초 전)
        float handoverStartTime = seq.Duration() - 0.3f;
        seq.Insert(handoverStartTime, img2.DOFade(0f, 0.3f)); // 2번 요소 퇴장
        seq.Insert(handoverStartTime, img3.DOFade(1f, 0.3f)); // 3번 요소 등장

        // 5. G: 3번 요소 Move to Element 4's pos (1.0s, Linear)
        DG.Tweening.Tween moveG = rt3.DOAnchorPos(targetPos4_End, 1.0f).SetEase(DG.Tweening.Ease.Linear);
        seq.Append(moveG);

        // ===========================================
        // ⭐6. Final Pop Sequence with Infinite Pulsing⭐
        // ===========================================

        DG.Tweening.Sequence finalPopSeq = DG.Tweening.DOTween.Sequence();

        // H-1 & H-2: 4번 요소 Scale Pop (총 0.1s)
        finalPopSeq.Append(rt4.DOScale(overshootScale4, 0.05f).SetEase(DG.Tweening.Ease.OutQuad));
        finalPopSeq.Append(rt4.DOScale(targetScale4, 0.05f).SetEase(DG.Tweening.Ease.InQuad));

        // H-3: 4번 요소 알파 Fade In (0.1s) - 팝인 시작과 동시에
        finalPopSeq.Insert(0f, img4.DOFade(1f, 0.1f));

        // I, J: 5번, 6번 요소 알파 Fade In (0.2s) - 4번 팝인 시작과 동시에
        finalPopSeq.Insert(0f, img5.DOFade(1f, 0.2f));
        finalPopSeq.Insert(0f, img6.DOFade(1f, 0.2f));

        // ⭐ 4번 요소의 팝인 애니메이션 완료 후 무한 맥동 애니메이션 시작 ⭐
        finalPopSeq.OnComplete(() => {
            _element4PulsingTween = rt4.DOScale(overshootScale4, 0.5f) // 0.5초에 1.2배로 커짐
                .SetEase(DG.Tweening.Ease.InOutQuad)
                .SetLoops(-1, DG.Tweening.LoopType.Yoyo); // 무한 반복 (왔다갔다)
        });

        // 메인 시퀀스에 최종 팝 시퀀스 추가
        seq.Append(finalPopSeq);

        // ===========================================
        // ⭐7. 애니메이션 완료 후 정리 및 반복 로직 (추가됨)⭐
        // ===========================================

        seq.OnComplete(() =>
        {
            // 2초 대기
            _repeatTween = DG.Tweening.DOVirtual.DelayedCall(2f, () =>
            {
                // 모든 요소를 초기 상태로 복원하는 페이드 아웃 시퀀스 (0.5초)
                DG.Tweening.Sequence fadeOutSeq = DG.Tweening.DOTween.Sequence();

                // 1번부터 6번까지의 요소를 0.5초에 걸쳐 투명하게 만듦 (동시)
                for (int i = 2; i <= 6; i++)
                {
                    if (images.Count > i)
                    {
                        fadeOutSeq.Join(images[i].DOFade(0f, 0.5f));
                    }
                }

                // 페이드 아웃 완료 후 초기화 및 재시작
                fadeOutSeq.OnComplete(() =>
                {
                    // 4번 요소의 무한 트윈 정리 및 초기 상태로 복원
                    if (_element4PulsingTween != null)
                    {
                        _element4PulsingTween.Kill(true);
                        _element4PulsingTween = null;
                    }
                    // 모든 트윈이 정리된 상태에서 초기화 및 재귀 호출
                    ExecuteAnimation(images);
                });
            });
        });
    }

    // ===================================
    // 🛠️ 필수 헬퍼 함수
    // ===================================

    private void SetRect(UnityEngine.UI.Image img, float x, float y, float scale)
    {
        UnityEngine.RectTransform rt = img.rectTransform;
        rt.anchoredPosition = new UnityEngine.Vector2(x, y);
        rt.localScale = UnityEngine.Vector3.one * scale;
    }

    private void SetAlpha(UnityEngine.UI.Image img, float alpha)
    {
        img.color = new UnityEngine.Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    protected override void KillAllTweens()
    {
        // 1번부터 6번 요소 및 RectTransform의 모든 트윈을 정리
        if (animationImages != null)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (animationImages.Count > i && animationImages[i] != null)
                {
                    DG.Tweening.DOTween.Kill(animationImages[i]);
                    DG.Tweening.DOTween.Kill(animationImages[i].GetComponent<UnityEngine.RectTransform>());
                }
            }
        }

        // ⭐ 무한 트윈 및 반복 딜레이 정리 ⭐
        if (_element4PulsingTween != null)
        {
            _element4PulsingTween.Kill(true);
            _element4PulsingTween = null;
        }
        if (_repeatTween != null)
        {
            _repeatTween.Kill(true);
            _repeatTween = null;
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

        // 3-7번 요소들 원래 위치로 초기화 및 스케일 수정 (⭐수정됨)
        UnityEngine.Vector3 targetScale = UnityEngine.Vector3.one * 0.3f; // ⭐0.2배 스케일 정의

        images[3].rectTransform.anchoredPosition = UnityEngine.Vector2.zero;
        images[3].rectTransform.localScale = targetScale; // ⭐스케일 적용
        images[4].rectTransform.anchoredPosition = UnityEngine.Vector2.zero;
        images[4].rectTransform.localScale = targetScale; // ⭐스케일 적용
        images[5].rectTransform.anchoredPosition = UnityEngine.Vector2.zero;
        images[5].rectTransform.localScale = targetScale; // ⭐스케일 적용
        images[6].rectTransform.anchoredPosition = UnityEngine.Vector2.zero;
        images[6].rectTransform.localScale = targetScale; // ⭐스케일 적용
        images[7].rectTransform.anchoredPosition = UnityEngine.Vector2.zero;
        images[7].rectTransform.localScale = targetScale; // ⭐스케일 적용

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

            // 1번 요소 크기 복원 (⭐수정됨: 0.7f까지 커지게)
            images[1].rectTransform.DOScale(UnityEngine.Vector3.one * 0.7f, 0.05f).OnComplete(() =>
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



public class CardAnimation_Num_118 : CardAnimationBase
{
    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        KillAllTweens();

        // --------------------------------------------------------
        // [초기 세팅]
        // --------------------------------------------------------
        InitElements(images);

        // --------------------------------------------------------
        // [상시 애니메이션]
        // --------------------------------------------------------
        StartFloating9(images);
        StartSwaying3to8(images);

        // --------------------------------------------------------
        // [메인 시퀀스] (1번 말 달리기 & 2번 밀치기 & 먼지 생성)
        // --------------------------------------------------------
        StartMainSequence(images);
    }

    private void InitElements(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count > 1) images[1].rectTransform.anchoredPosition = new UnityEngine.Vector2(43f, -110f);
        if (images.Count > 2)
        {
            images[2].rectTransform.anchoredPosition = new UnityEngine.Vector2(883f, -20f);
            images[2].rectTransform.localScale = UnityEngine.Vector3.one * 0.3f;
        }
        if (images.Count > 3) images[3].rectTransform.anchoredPosition = new UnityEngine.Vector2(-13f, -27f);
        if (images.Count > 4) images[4].rectTransform.anchoredPosition = new UnityEngine.Vector2(27f, 12f);
        if (images.Count > 6) images[6].rectTransform.anchoredPosition = new UnityEngine.Vector2(1050f, 0f);

        int[] group10_12 = { 10, 11, 12 };
        foreach (int i in group10_12)
        {
            if (images.Count > i)
            {
                // 템플릿으로 쓰일 원본은 숨겨둠
                images[i].rectTransform.localScale = UnityEngine.Vector3.one * 0.3f;
                images[i].color = new UnityEngine.Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f);
            }
        }
    }

    private void StartMainSequence(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count <= 2) return;

        UnityEngine.RectTransform rt1 = images[1].GetComponent<UnityEngine.RectTransform>();
        UnityEngine.RectTransform rt2 = images[2].GetComponent<UnityEngine.RectTransform>();

        UnityEngine.Vector2 origin1 = new UnityEngine.Vector2(43f, -110f);
        UnityEngine.Vector2 origin2 = new UnityEngine.Vector2(883f, -20f);

        DG.Tweening.Sequence mainSeq = DG.Tweening.DOTween.Sequence();

        mainSeq.SetDelay(0.7f);

        // --- [Phase 1: 달리기 (2초)] ---
        mainSeq.AppendCallback(() => {
            // 1. 말 달리기 모션
            rt1.DOAnchorPosY(origin1.y + 50f, 0.5f)
               .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
               .SetEase(DG.Tweening.Ease.InOutSine);

            // 2. 먼지(10~12번) 랜덤 생성 시작 (2초 동안)
            SpawnDustLoop(images, 2.0f);
        });
        mainSeq.AppendInterval(2.0f);

        // --- [Phase 2: 돌진 & 밀치기 (0.5초)] ---
        mainSeq.AppendCallback(() => {
            DG.Tweening.DOTween.Kill(rt1); // 달리기 중지
            rt1.anchoredPosition = new UnityEngine.Vector2(rt1.anchoredPosition.x, origin1.y);
        });

        mainSeq.Append(rt1.DOAnchorPos(new UnityEngine.Vector2(240f, origin1.y), 0.5f).SetEase(DG.Tweening.Ease.OutQuad));
        mainSeq.Join(rt2.DOAnchorPos(new UnityEngine.Vector2(398f, origin2.y), 0.5f).SetEase(DG.Tweening.Ease.OutQuad));

        // --- [Phase 3: 1번 복귀] ---
        mainSeq.Append(rt1.DOAnchorPos(origin1, 0.5f).SetEase(DG.Tweening.Ease.OutBack));
        mainSeq.Join(rt1.DOShakeRotation(0.5f, new UnityEngine.Vector3(0, 0, 10f), 100, 90));

        // --- [Phase 4: 스턴 상태 (대기 1초)] ---
        mainSeq.AppendInterval(1.0f);

        // 2번 복귀 (1초)
        mainSeq.Append(rt2.DOAnchorPos(origin2, 1.0f).SetEase(DG.Tweening.Ease.InOutQuad));

        // 루프 재시작 전 초기화
        mainSeq.AppendCallback(() => {
            DG.Tweening.DOTween.Kill(rt1);
            rt1.anchoredPosition = origin1;
        });

        // 무한 반복
        mainSeq.SetLoops(-1);
    }

    // 먼지 생성 루프 함수
    private void SpawnDustLoop(System.Collections.Generic.List<UnityEngine.UI.Image> images, float duration)
    {
        if (images.Count <= 12) return;

        int spawnCount = 8;
        float interval = duration / spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            DG.Tweening.DOVirtual.DelayedCall(i * interval, () =>
            {
                CreateSingleDust(images);
            });
        }
    }

    // 개별 먼지 생성 및 애니메이션 함수
    private void CreateSingleDust(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        int rndIndex = UnityEngine.Random.Range(10, 13);
        UnityEngine.UI.Image template = images[rndIndex];

        UnityEngine.GameObject dustObj = UnityEngine.Object.Instantiate(template.gameObject, template.transform.parent);
        UnityEngine.RectTransform dustRt = dustObj.GetComponent<UnityEngine.RectTransform>();
        UnityEngine.UI.Image dustImg = dustObj.GetComponent<UnityEngine.UI.Image>();

        // 1. 초기 위치 랜덤 설정
        float randX = UnityEngine.Random.Range(-366f, 116f);
        float randY = UnityEngine.Random.Range(-325f, 154f);
        dustRt.anchoredPosition = new UnityEngine.Vector2(randX, randY);

        // 2. 초기 상태 설정
        dustRt.localScale = UnityEngine.Vector3.one * 0.3f;
        dustImg.color = new UnityEngine.Color(dustImg.color.r, dustImg.color.g, dustImg.color.b, 0f);

        // 3. 애니메이션 시퀀스
        DG.Tweening.Sequence dustSeq = DG.Tweening.DOTween.Sequence();

        // - 등장 (0.2초)
        dustSeq.Append(dustImg.DOFade(1f, 0.2f));

        // - 이동: [수정됨] -1000으로 이동하여 확실하게 화면 왼쪽으로 지나가게 함
        dustSeq.Join(dustRt.DOAnchorPosX(-1000f, 2.0f).SetEase(DG.Tweening.Ease.Linear));

        // - 소멸 (이동 끝나기 0.5초 전부터 Fade Out)
        dustSeq.Insert(1.5f, dustImg.DOFade(0f, 0.5f));

        // 4. 종료 후 삭제
        dustSeq.OnComplete(() => {
            UnityEngine.Object.Destroy(dustObj);
        });
    }

    private void StartFloating9(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count > 9)
        {
            UnityEngine.RectTransform rt9 = images[9].GetComponent<UnityEngine.RectTransform>();
            UnityEngine.Vector2 startPos = rt9.anchoredPosition;
            UnityEngine.Vector2 moveDir = UnityEngine.Random.insideUnitCircle.normalized;
            float moveDist = UnityEngine.Random.Range(20f, 50f);
            UnityEngine.Vector2 targetPos = startPos + (moveDir * moveDist);

            rt9.DOAnchorPos(targetPos, UnityEngine.Random.Range(1.5f, 2.5f))
               .SetEase(DG.Tweening.Ease.InOutSine)
               .SetLoops(-1, DG.Tweening.LoopType.Yoyo);
        }
    }

    private void StartSwaying3to8(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        int[] horizontalIndices = { 3, 4, 5, 6, 7, 8 };
        foreach (int i in horizontalIndices)
        {
            if (images.Count > i)
            {
                UnityEngine.RectTransform rt = images[i].GetComponent<UnityEngine.RectTransform>();
                float startX = rt.anchoredPosition.x;
                float dist = UnityEngine.Random.Range(10f, 40f);
                float dir = (UnityEngine.Random.value > 0.5f) ? 1f : -1f;
                float targetX = startX + (dist * dir);
                float duration = UnityEngine.Random.Range(1.5f, 2.5f);
                float delay = UnityEngine.Random.Range(0f, 1f);

                rt.DOAnchorPosX(targetX, duration)
                  .SetEase(DG.Tweening.Ease.InOutSine)
                  .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                  .SetDelay(delay);
            }
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    DG.Tweening.DOTween.Kill(img);
                    DG.Tweening.DOTween.Kill(img.GetComponent<UnityEngine.RectTransform>());
                }
            }
        }
    }
}


public class CardAnimation_Num_119 : CardAnimationBase
{
    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        KillAllTweens();

        // --------------------------------------------------------
        // [초기 세팅]
        // --------------------------------------------------------
        InitElements(images);

        // --------------------------------------------------------
        // [애니메이션 로직]
        // --------------------------------------------------------

        // 2번 요소 이동 및 흔들림 시작
        if (images.Count > 2)
        {
            // 2번 요소의 움직임과 함께, 1번 요소를 생성하기 위해 리스트 전체를 넘김
            StartElement2Movement(images);
        }

        // 3, 4, 5번 요소를 이용한 트레일(연기) 효과 시작
        if (images.Count > 5)
        {
            StartTrailEffect(images);
        }
    }

    private void StartElement2Movement(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        UnityEngine.RectTransform rt2 = images[2].GetComponent<UnityEngine.RectTransform>();

        // --- 1. X축 이동 및 크기 변화 시퀀스 ---
        DG.Tweening.Sequence moveSeq = DG.Tweening.DOTween.Sequence();

        float baseScale = 0.26f;

        // [이동] -1000까지 4.5초 동안 이동
        moveSeq.Append(rt2.DOAnchorPosX(-1000f, 4.5f).SetEase(DG.Tweening.Ease.Linear));

        // [이벤트 1] x=300 지점 통과 시점 (약 1.527초)
        moveSeq.InsertCallback(1.527f, () =>
        {
            PunchScale(rt2, baseScale);

            // 1번 요소 복사 생성
            if (images.Count > 1) SpawnElement1Effect(images[1], 300f);

            // [추가됨] 속도 1.5배 가속
            moveSeq.timeScale *= 1.65f;
        });

        // [이벤트 2] x=-230 지점 통과 시점 (약 2.739초)
        moveSeq.InsertCallback(2.739f, () =>
        {
            PunchScale(rt2, baseScale);

            // 1번 요소 복사 생성
            if (images.Count > 1) SpawnElement1Effect(images[1], -230f);

            // [추가됨] 속도 다시 1.5배 가속 (누적)
            moveSeq.timeScale *= 1.65f;
        });

        // [추가됨] 반복 텀 1초 대기
        moveSeq.AppendInterval(1.0f);

        // 초기 위치(968)로 복귀 및 속도 초기화
        moveSeq.AppendCallback(() =>
        {
            UnityEngine.Vector2 currentPos = rt2.anchoredPosition;
            rt2.anchoredPosition = new UnityEngine.Vector2(968f, currentPos.y);

            // [추가됨] 속도 초기화
            moveSeq.timeScale = 1.0f;
        });

        // 무한 반복
        moveSeq.SetLoops(-1);


        // --- 2. Y축 위아래 움직임 (독립적으로 실행) ---
        float startY = -212f;
        rt2.DOAnchorPosY(startY + 30f, 1.5f)
           .SetEase(DG.Tweening.Ease.InOutSine)
           .SetLoops(-1, DG.Tweening.LoopType.Yoyo);
    }

    // [신규] 1번 요소 생성 및 상승 애니메이션
    private void SpawnElement1Effect(UnityEngine.UI.Image template, float xPos)
    {
        // 1. 복제
        UnityEngine.GameObject cloneObj = UnityEngine.Object.Instantiate(template.gameObject, template.transform.parent);
        UnityEngine.RectTransform cloneRt = cloneObj.GetComponent<UnityEngine.RectTransform>();
        UnityEngine.UI.Image cloneImg = cloneObj.GetComponent<UnityEngine.UI.Image>();

        // 2. 초기 상태 설정
        // 위치: 같은 X축, Y는 -150
        cloneRt.anchoredPosition = new UnityEngine.Vector2(xPos, -150f);

        // 크기: 초기 세팅값(0.25)의 0.7배로 시작
        float originalScaleVal = 0.25f;
        cloneRt.localScale = UnityEngine.Vector3.one * (originalScaleVal * 0.7f);

        // 알파: 0에서 시작
        cloneImg.color = new UnityEngine.Color(cloneImg.color.r, cloneImg.color.g, cloneImg.color.b, 0f);

        // 3. 애니메이션 (0.4초)
        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence();

        // - 위로 올라가기 (-150 -> -50)
        seq.Append(cloneRt.DOAnchorPosY(100f, 0.4f).SetEase(DG.Tweening.Ease.OutBack));

        // - 알파값 1로 (서서히 등장)
        seq.Join(cloneImg.DOFade(1f, 0.4f));

        // - 크기 원복 (0.25로)
        seq.Join(cloneRt.DOScale(UnityEngine.Vector3.one * originalScaleVal, 0.4f));

        // 4. 마무리 (자연스럽게 사라지게 한 뒤 삭제)
        // 등장 후 잠시 보였다가 사라짐
        seq.Append(cloneImg.DOFade(0f, 0.3f).SetDelay(0.2f));
        seq.OnComplete(() => UnityEngine.Object.Destroy(cloneObj));
    }

    // 트레일 효과 (3,4,5번)
    private void StartTrailEffect(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        UnityEngine.RectTransform target = images[2].GetComponent<UnityEngine.RectTransform>();
        float spawnInterval = 0.35f;

        DG.Tweening.Sequence trailLoop = DG.Tweening.DOTween.Sequence();

        trailLoop.AppendCallback(() => SpawnSingleTrail(images[3], target));
        trailLoop.AppendInterval(spawnInterval);

        trailLoop.AppendCallback(() => SpawnSingleTrail(images[4], target));
        trailLoop.AppendInterval(spawnInterval);

        trailLoop.AppendCallback(() => SpawnSingleTrail(images[5], target));
        trailLoop.AppendInterval(spawnInterval);

        trailLoop.SetLoops(-1);
    }

    private void SpawnSingleTrail(UnityEngine.UI.Image templateImage, UnityEngine.RectTransform target)
    {
        UnityEngine.GameObject trailObj = UnityEngine.Object.Instantiate(templateImage.gameObject, templateImage.transform.parent);
        UnityEngine.RectTransform trailRt = trailObj.GetComponent<UnityEngine.RectTransform>();
        UnityEngine.UI.Image trailImg = trailObj.GetComponent<UnityEngine.UI.Image>();

        float randomY = UnityEngine.Random.Range(-50f, 100f);
        trailRt.anchoredPosition = target.anchoredPosition + new UnityEngine.Vector2(0f, randomY);

        trailImg.color = new UnityEngine.Color(trailImg.color.r, trailImg.color.g, trailImg.color.b, 0f);
        trailRt.localScale = UnityEngine.Vector3.one * -0.4f;

        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence();
        seq.Append(trailImg.DOFade(1f, 0.2f));
        seq.AppendInterval(0.2f);
        seq.Append(trailImg.DOFade(0f, 0.5f));
        seq.OnComplete(() => UnityEngine.Object.Destroy(trailObj));
    }

    private void PunchScale(UnityEngine.RectTransform target, float baseScale)
    {
        float targetScale = baseScale * 1.2f;
        DG.Tweening.Sequence scaleSeq = DG.Tweening.DOTween.Sequence();
        scaleSeq.Append(target.DOScale(UnityEngine.Vector3.one * targetScale, 0.1f));
        scaleSeq.Append(target.DOScale(UnityEngine.Vector3.one * baseScale, 0.1f));
    }

    private void InitElements(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.rotation = UnityEngine.Quaternion.identity;
            images[i].color = new UnityEngine.Color(images[i].color.r, images[i].color.g, images[i].color.b, 1f);
            images[i].rectTransform.localScale = UnityEngine.Vector3.one;
        }

        // 1번 (Index 1)
        if (images.Count > 1)
        {
            images[1].rectTransform.localScale = UnityEngine.Vector3.one * 0.25f;
            images[1].color = new UnityEngine.Color(images[1].color.r, images[1].color.g, images[1].color.b, 0f);
        }

        // 2번 (Index 2)
        if (images.Count > 2)
        {
            images[2].rectTransform.anchoredPosition = new UnityEngine.Vector2(968f, -212f);
            images[2].rectTransform.localScale = UnityEngine.Vector3.one * 0.26f;
        }

        // 3, 4, 5번 (Index 3, 4, 5)
        int[] group3_5 = { 3, 4, 5 };
        foreach (int i in group3_5)
        {
            if (images.Count > i)
            {
                images[i].rectTransform.localScale = UnityEngine.Vector3.one * 0.2f;
                images[i].color = new UnityEngine.Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f);
            }
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    DG.Tweening.DOTween.Kill(img);
                    DG.Tweening.DOTween.Kill(img.GetComponent<UnityEngine.RectTransform>());
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

public class CardAnimation_Num_130 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        KillAllTweens();

        // 1. 초기 상태 설정
        InitElements(images);

        // 2. 메인 시퀀스 실행 (무한 반복)
        StartMainSequence(images);
    }

    private void InitElements(List<Image> images)
    {
        // 전체 초기화
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.rotation = Quaternion.identity;
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 1f);
            images[i].rectTransform.localScale = Vector3.one;
        }

        // --- 개별 세팅 ---
        if (images.Count > 1) SetRect(images[1], 0, -107, 1f); // 1번
        if (images.Count > 3) SetRect(images[3], 308, -54, 0.27f); // 3번

        // 7번 -> 0번 위치로 이동 (초기에는 비활성화)
        if (images.Count > 7 && images.Count > 0)
        {
            images[7].rectTransform.anchoredPosition = images[0].rectTransform.anchoredPosition;
            images[7].rectTransform.localScale = Vector3.one;
            SetAlpha(images[7], 0f);
        }

        // 비활성화 그룹 초기화
        int[] group4_6 = { 4, 5, 6 };
        foreach (int i in group4_6) if (images.Count > i) SetAlpha(images[i], 0f);

        if (images.Count > 8) SetAlpha(images[8], 0f);

        int[] group9_11 = { 9, 10, 11 };
        foreach (int i in group9_11)
        {
            if (images.Count > i)
            {
                images[i].rectTransform.localScale = Vector3.one * 0.27f;
                SetAlpha(images[i], 0f);
            }
        }

        if (images.Count > 12) { SetRect(images[12], -413, 234, 0.43f); SetAlpha(images[12], 0f); }
        if (images.Count > 13) { SetRect(images[13], 66, -317, 0.31f); SetAlpha(images[13], 0f); }
        if (images.Count > 14) { SetRect(images[14], -495, -66, 0.2f); SetAlpha(images[14], 0f); }
        if (images.Count > 15) { SetAlpha(images[15], 0f); }
    }

    private void StartMainSequence(List<Image> images)
    {
        Sequence seq = DOTween.Sequence();

        // [최초 딜레이] 1.5초
        seq.SetDelay(1.5f);

        if (images.Count > 15)
        {
            Image img15 = images[15];

            // 1. 15번 등장 (0.1초)
            seq.Append(img15.DOFade(1f, 0.1f));

            // 2. [교체 트리거]
            seq.AppendCallback(() => SwapElementsVisibilityAndMove(images));

            // 3. 대기 (0.5초)
            seq.AppendInterval(0.5f);

            // 4. 15번 소멸 (3.5초)
            seq.Append(img15.DOFade(0f, 3.5f));

            // 5. 소멸 후 대기 (2.0초)
            seq.AppendInterval(2.0f);

            // 6. 복구 시작 (2.0초 동안)
            seq.AppendCallback(() => RevertToOriginalState(images));

            // 7. 복구되는 시간만큼 대기
            seq.AppendInterval(2.0f);

            // 8. [루프] 다시 처음부터
            seq.OnComplete(() =>
            {
                StartMainSequence(images);
            });
        }
    }

    private void SwapElementsVisibilityAndMove(List<Image> images)
    {
        // 끄기: 0, 1, 3
        int[] deactiveIndices = { 0, 1, 3 };
        foreach (int i in deactiveIndices)
        {
            if (images.Count > i) SetAlpha(images[i], 0f);
        }

        // 위치 이동: 9, 10, 11
        if (images.Count > 9) images[9].rectTransform.anchoredPosition = new Vector2(244, 168);
        if (images.Count > 10) images[10].rectTransform.anchoredPosition = new Vector2(405, -89);
        if (images.Count > 11) images[11].rectTransform.anchoredPosition = new Vector2(452, 81);

        // 켜기: 4~14 (7번 포함)
        int[] activeIndices = { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
        foreach (int i in activeIndices)
        {
            if (images.Count > i) SetAlpha(images[i], 1f);
        }

        // 플로팅 시작
        StartFloatingEffect(images);
    }

    private void RevertToOriginalState(List<Image> images)
    {
        float duration = 2.0f;

        // 1. 플로팅 멈춤 (9, 12, 13, 14)
        int[] floatingIndices = { 9, 12, 13, 14 };
        foreach (int i in floatingIndices)
        {
            if (images.Count > i) images[i].rectTransform.DOKill();
        }

        // 2. 다시 켜기 (0, 1, 3)
        int[] restoreIndices = { 0, 1, 3 };
        foreach (int i in restoreIndices)
        {
            if (images.Count > i) images[i].DOFade(1f, duration);
        }

        // 3. 다시 끄기 (4~14)
        int[] hideIndices = { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
        foreach (int i in hideIndices)
        {
            if (images.Count > i) images[i].DOFade(0f, duration);
        }

        // [수정됨] 9, 10, 11번 위치 복구 코드 제거됨 (제자리에서 사라짐)
    }

    private void StartFloatingEffect(List<Image> images)
    {
        int[] floatingIndices = { 9, 12, 13, 14 };

        foreach (int index in floatingIndices)
        {
            if (images.Count <= index) continue;

            RectTransform rt = images[index].rectTransform;
            Vector2 centerPos = rt.anchoredPosition;

            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float moveDist = Random.Range(10f, 20f);
            float duration = Random.Range(1.5f, 2.5f);
            Vector2 targetPos = centerPos + (randomDir * moveDist);

            rt.DOAnchorPos(targetPos, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(Random.Range(0f, 0.5f));
        }
    }

    private void SetRect(Image img, float x, float y, float scale)
    {
        RectTransform rt = img.rectTransform;
        rt.anchoredPosition = new Vector2(x, y);
        rt.localScale = Vector3.one * scale;
    }

    private void SetAlpha(Image img, float alpha)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    img.DOKill();
                    img.GetComponent<RectTransform>().DOKill();
                }
            }
        }
    }
}

public class CardAnimation_Num_131 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        KillAllTweens();

        // 1. 초기 위치, 크기, 회전 세팅
        InitElements(images);

        // 2. 3~5번 요소만 두둥실 플로팅 (6,7번은 떨림을 위해 제외)
        StartFloatingEffect(images);

        // 3. 2번, 6번, 7번 메인 루프 (떨림 & 교체)
        // 최초 1.0초 딜레이 후 시작
        DOVirtual.DelayedCall(1.0f, () => {
            StartShakeAndBlinkLoop(images);
        });
    }

    private void InitElements(List<Image> images)
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.rotation = Quaternion.identity;
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 1f);
            images[i].rectTransform.localScale = Vector3.one;
        }

        // 2번 (Index 2): 비활성화
        if (images.Count > 2) SetAlpha(images[2], 0f);

        if (images.Count > 3) SetRect(images[3], -500, -300, 0.33f);
        if (images.Count > 4) SetRect(images[4], -473, -23, 0.258f);
        if (images.Count > 5) SetRect(images[5], 535, 39, 0.29f);

        // 6번 (Index 6): Z축 80도 회전
        if (images.Count > 6)
        {
            SetRect(images[6], 143, 59, 0.5f);
            images[6].rectTransform.localRotation = Quaternion.Euler(0, 0, 80f);
        }

        // 7번 (Index 7): 비활성화, Z축 80도 회전
        if (images.Count > 7)
        {
            SetRect(images[7], 143, 59, 0.5f); // 6번과 위치 동일하게 맞춤 (겹치도록)
            images[7].rectTransform.localRotation = Quaternion.Euler(0, 0, 80f);
            SetAlpha(images[7], 0f);
        }
    }

    private void StartShakeAndBlinkLoop(List<Image> images)
    {
        if (images.Count <= 7) return;

        RectTransform rt2 = images[2].rectTransform;
        RectTransform rt7 = images[7].rectTransform;
        Image img2 = images[2];
        Image img6 = images[6];
        Image img7 = images[7];

        Sequence seq = DOTween.Sequence();

        // [Step 1] 상태 초기화 (켜기/끄기)
        seq.AppendCallback(() => {
            SetAlpha(img2, 1f); // 2번 켜기
            SetAlpha(img7, 1f); // 7번 켜기 (이펙트)
            SetAlpha(img6, 0f); // 6번 끄기 (원본)
        });

        // [Step 2] 떨림 애니메이션 (3.5초 동안 지속)
        // Append로 시작 시간을 맞추고 Join으로 동시에 실행
        seq.Append(rt2.DOShakeAnchorPos(3.5f, strength: 30f, vibrato: 50, randomness: 90, snapping: false, fadeOut: true));
        seq.Join(rt7.DOShakeAnchorPos(3.5f, strength: 30f, vibrato: 50, randomness: 90, snapping: false, fadeOut: true));

        // [Step 3] 페이드 아웃/인 (1.5초 시점에 시작하여 2초 동안 진행)
        // Insert를 사용하여 1.5초 시점에 끼워넣기
        seq.Insert(1.5f, img2.DOFade(0f, 2f)); // 2번 사라짐
        seq.Insert(1.5f, img7.DOFade(0f, 2f)); // 7번 사라짐
        seq.Insert(1.5f, img6.DOFade(1f, 2f)); // 6번 나타남

        // [Step 4] 휴식 (애니메이션 끝난 후 2초 대기)
        seq.AppendInterval(2.0f);

        // [Step 5] 무한 반복
        seq.SetLoops(-1);
    }

    private void StartFloatingEffect(List<Image> images)
    {
        // [수정됨] 6, 7번은 떨림 효과와 좌표 충돌이 나므로 플로팅에서 제외했습니다.
        // 3, 4, 5번만 두둥실 움직입니다.
        int[] independentIndices = { 3, 4, 5 };

        foreach (int index in independentIndices)
        {
            if (images.Count <= index) continue;
            ApplyFloating(images[index].rectTransform);
        }
    }

    private void ApplyFloating(RectTransform rt)
    {
        Vector2 centerPos = rt.anchoredPosition;
        Vector2 targetPos = centerPos + (Random.insideUnitCircle.normalized * Random.Range(10f, 20f));

        rt.DOAnchorPos(targetPos, Random.Range(1.5f, 2.5f))
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(Random.Range(0f, 1f));
    }

    private void SetRect(Image img, float x, float y, float scale)
    {
        RectTransform rt = img.rectTransform;
        rt.anchoredPosition = new Vector2(x, y);
        rt.localScale = Vector3.one * scale;
    }

    private void SetAlpha(Image img, float alpha)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    img.DOKill();
                    img.GetComponent<RectTransform>().DOKill();
                }
            }
        }
    }
}






public class CardAnimation_Num_132 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        KillAllTweens();
        InitElements(images);

        // 2. 메인 애니메이션 시퀀스 실행
        StartMainSequence(images);
    }

    private void InitElements(List<Image> images)
    {
        // 0. 전체 요소 초기화 (위치, 회전, 스케일)
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.rotation = Quaternion.identity;
            images[i].rectTransform.localScale = Vector3.one;
            // 알파값 설정 지시가 없으므로 생략
        }

        // --- 요청하신 개별 위치 및 스케일 세팅 ---
        // 스케일 지시가 없는 요소는 기본 스케일 1.0으로 설정합니다.

        if (images.Count > 1) SetRect(images[1], 0f, -526f, 1f); // 1번
        if (images.Count > 2) SetRect(images[2], -514f, 785f, 1f); // 2번
        if (images.Count > 3) SetRect(images[3], 513f, 807f, 1f); // 3번
        if (images.Count > 4) SetRect(images[4], -644f, -914f, 1f); // 4번
        if (images.Count > 5) SetRect(images[5], 629f, -879f, 1f); // 5번
    }

    // ===================================
    // ✨ 메인 시퀀스 함수: 웅웅거림 및 왕복 이동 적용
    // ===================================

    private void StartMainSequence(List<Image> images)
    {
        // 1. 1번 요소 크기 웅웅거림 (0.9배 ~ 1.1배)
        if (images.Count > 1)
        {
            ApplyPulsingScale(images[1].rectTransform, maxRatio: 1f, minRatio: 0.9f, duration: 4f, delay: 0.2f);
        }

        // 2. 2, 3, 4, 5번 요소 위아래 왕복 이동 (-50 ~ 50)
        float moveRange = 50f;

        // 요소별로 약간의 속도 및 딜레이 차이를 주어 단조로움을 피합니다.
        if (images.Count > 2) StartVerticalMove(images[2].rectTransform, moveRange, duration: 2.0f, delay: 0f);
        if (images.Count > 3) StartVerticalMove(images[3].rectTransform, moveRange, duration: 2.3f, delay: 0.2f);
        if (images.Count > 4) StartVerticalMove(images[4].rectTransform, moveRange, duration: 2.5f, delay: 0.4f);
        if (images.Count > 5) StartVerticalMove(images[5].rectTransform, moveRange, duration: 2.8f, delay: 0.6f);
    }

    // ===================================
    // ⚙️ 헬퍼 함수: 크기 웅웅거림 반복 (초기 대비 비율 적용)
    // ===================================

    /// <summary>
    /// 대상 요소에 웅웅거림(Pulsing) 크기 애니메이션을 적용합니다.
    /// maxRatio, minRatio는 InitElements에서 설정된 초기 스케일 대비 비율입니다.
    /// </summary>
    private void ApplyPulsingScale(RectTransform target, float maxRatio, float minRatio, float duration, float delay)
    {
        Vector3 baseScale = target.localScale;

        // 초기 스케일(baseScale) 대비 비율을 곱하여 목표 스케일을 정확히 계산
        Vector3 targetScaleMax = baseScale * maxRatio;
        Vector3 targetScaleMin = baseScale * minRatio;

        // 현재 크기에서 최소 크기까지 이동 후, Yoyo 루프로 최대-최소 반복
        target.DOScale(targetScaleMin, duration * 0.5f)
              .SetEase(Ease.InOutSine)
              .SetDelay(delay)
              .SetLoops(-1, LoopType.Yoyo);
    }

    // ===================================
    // ⚙️ 헬퍼 함수: 위아래 왕복 이동
    // ===================================

    /// <summary>
    /// 대상 요소를 초기 위치 대비 위아래로 반복 이동시킵니다.
    /// </summary>
    /// <param name="target">애니메이션 대상 RectTransform</param>
    /// <param name="moveRange">중심에서 최대 이동 거리 (총 이동 범위는 2배)</param>
    private void StartVerticalMove(RectTransform target, float moveRange, float duration, float delay)
    {
        Vector2 originalPos = target.anchoredPosition;

        // 초기 위치(originalPos.y)에서 위쪽(+moveRange)으로 이동
        target.DOAnchorPosY(originalPos.y + moveRange, duration)
              .SetEase(Ease.InOutSine)
              .SetDelay(delay)
              .SetLoops(-1, LoopType.Yoyo);
    }

    // ===================================
    // 🛠️ 필수 헬퍼 함수
    // ===================================

    private void SetRect(Image img, float x, float y, float scale)
    {
        RectTransform rt = img.rectTransform;
        rt.anchoredPosition = new Vector2(x, y);
        rt.localScale = Vector3.one * scale;
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    img.DOKill();
                    img.GetComponent<RectTransform>().DOKill();
                }
            }
        }
    }
}



public class CardAnimation_Num_133 : CardAnimationBase
{
    // 복사본 생성 로직이 삭제되었으므로 createdElements 변수를 제거합니다.

    protected override void ExecuteAnimation(List<Image> images)
    {
        KillAllTweens();
        InitElements(images);

        // 2. 메인 애니메이션 시퀀스 실행
        StartMainSequence(images);
    }

    private void InitElements(List<Image> images)
    {
        // 0. 전체 요소 초기화 (위치, 회전, 스케일)
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.rotation = Quaternion.identity;
            images[i].rectTransform.localScale = Vector3.one;
        }

        // --- 요청하신 개별 세팅 (Index 4, 5, 6, 7) ---

        // 1. 4번 요소: 알파값 0
        if (images.Count > 4)
        {
            SetRect(images[4], -272f, 0f, 0.6f);
            SetAlpha(images[4], 0f);
        }

        // 2. 5번 요소 (복사본 대체): (-272, 0, 0.6배, 알파 0)
        if (images.Count > 5)
        {
            SetRect(images[5], 272f, 0f, 0.6f);
            SetAlpha(images[5], 0f);
        }

        // 3. 6번 요소 (복사본 대체): (272, 0, 0.6배, 알파 0)
        if (images.Count > 6)
        {
            SetAlpha(images[6], 0f);
        }

        // 4. 7번 요소 (기존 5번): (0, -113, 0.2배)
        if (images.Count > 7)
        {
            SetRect(images[7], 0f, -113f, 0.2f);
        }
    }

    // ===================================
    // ✨ 메인 시퀀스 함수: 왕복 이동 및 알파값 깜빡임 적용
    // ===================================

    private void StartMainSequence(List<Image> images)
    {
        // 1. 2, 3, 4번 요소 위아래 왕복 이동 (-50 ~ 50)

        // 2번 요소 이동 (지연 시간 0초)
        if (images.Count > 2)
        {
            StartVerticalMove(images[2].rectTransform, moveRange: 50f, duration: 2.5f, delay: 0f);
        }

        // 3번 요소 이동 (지연 시간 0.5초)
        if (images.Count > 3)
        {
            StartVerticalMove(images[3].rectTransform, moveRange: 50f, duration: 2.7f, delay: 0.5f);
        }

        // 4번 요소 이동 (지연 시간 1.0초)
        if (images.Count > 4)
        {
            StartVerticalMove(images[4].rectTransform, moveRange: 50f, duration: 2.3f, delay: 1.0f);
        }

        // 2. 4, 5, 6번 요소 알파값 깜빡임 로직 (2초 초기 대기)

        // 4번 요소 (원본): 알파값 0 -> 1 -> 0
        if (images.Count > 4)
        {
            StartAlphaFlicker(images[4], targetAlpha: 0.6f, initialDelay: 0.1f);
        }

        // 5번 요소 (복사본 대체): 알파값 0 -> 0.6 -> 0
        if (images.Count > 5)
        {
            // 4번 요소와의 미세한 지연 적용 (0.2f)
            StartAlphaFlicker(images[5], targetAlpha: 0.6f, initialDelay: 0.25f);
        }

        // 6번 요소 (복사본 대체): 알파값 0 -> 0.6 -> 0
        if (images.Count > 6)
        {
            // 4번 요소와의 미세한 지연 적용 (0.4f)
            StartAlphaFlicker(images[6], targetAlpha: 1f, initialDelay: 0f);
        }
    }

    // ===================================
    // ⚙️ 헬퍼 함수: 위아래 왕복 이동
    // ===================================

    private void StartVerticalMove(RectTransform target, float moveRange, float duration, float delay)
    {
        Vector2 originalPos = target.anchoredPosition;

        target.DOAnchorPosY(originalPos.y + moveRange, duration)
              .SetEase(Ease.InOutSine)
              .SetDelay(delay)
              .SetLoops(-1, LoopType.Yoyo);
    }

    // ===================================
    // ⚙️ 헬퍼 함수: 4초 주기 알파값 깜빡임 (2초 초기 대기 적용)
    // ===================================

    /// <summary>
    /// 4초 주기로 알파값이 0 -> targetAlpha -> 0으로 깜빡이는 시퀀스를 무한 반복하며,
    /// 전체 시퀀스는 2.0초 대기 후 시작합니다.
    /// </summary>
    private void StartAlphaFlicker(Image img, float targetAlpha, float initialDelay)
    {
        // 전체 시퀀스에 대한 딜레이는 2.0초 + 개별 요소 지연을 합산합니다.
        Sequence sequence = DOTween.Sequence()
            .SetDelay(2.0f + initialDelay)
            .SetLoops(-1, LoopType.Restart);

        // 1. 알파값 올리기 (0 -> targetAlpha) - 0.1초
        sequence.Append(img.DOFade(targetAlpha, 0.1f).SetEase(Ease.Linear));

        // 2. 대기 - 1.9초 (4초 주기를 맞추기 위해)
        sequence.AppendInterval(1.9f);

        // 3. 알파값 내리기 (targetAlpha -> 0) - 2초
        sequence.Append(img.DOFade(0f, 2f).SetEase(Ease.Linear));

        sequence.Play();
    }

    // ===================================
    // 🛠️ 필수 헬퍼 함수
    // ===================================

    private void SetRect(Image img, float x, float y, float scale)
    {
        RectTransform rt = img.rectTransform;
        rt.anchoredPosition = new Vector2(x, y);
        rt.localScale = Vector3.one * scale;
    }

    private void SetAlpha(Image img, float alpha)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }

    protected override void KillAllTweens()
    {
        // 기존 요소의 트윈만 정리합니다.
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    img.DOKill();
                    img.GetComponent<RectTransform>().DOKill();
                }
            }
        }
    }
}

public class CardAnimation_Num_134 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {
        KillAllTweens();
        InitElements(images);

        // 2. 메인 애니메이션 시퀀스 실행 (웅웅거림)
        StartMainSequence(images);
    }

    private void InitElements(List<Image> images)
    {
        // 전체 요소 초기화 (위치, 회전, 스케일)
        for (int i = 0; i < images.Count; i++)
        {
            images[i].rectTransform.rotation = Quaternion.identity;
            images[i].rectTransform.localScale = Vector3.one;
        }

        // --- 요청하신 개별 세팅 ---
        // 1번 요소 (Index 1): 위치 설정
        if (images.Count > 1)
        {
            SetRect(images[1], 0f, -414f, 1f);
        }

        // 2번 요소 (Index 2): 위치 및 크기 설정
        if (images.Count > 2)
        {
            SetRect(images[2], -166f, -169f, 0.5f);
        }

        // 4번 요소 (Index 4): 위치 및 크기 설정
        if (images.Count > 4)
        {
            SetRect(images[4], 67f, -113f, 0.5f);
        }

        // 5번 요소 (Index 5): 위치 및 크기 설정
        if (images.Count > 5)
        {
            SetRect(images[5], 116f, -152f, 0.5f);
        }
    }

    // ===================================
    // ✨ 메인 시퀀스 함수: 웅웅거림 효과 적용
    // ===================================

    private void StartMainSequence(List<Image> images)
    {
        // 1. 1번 요소 (Index 1) 애니메이션: 1.15배 ~ 0.95배
        if (images.Count > 1)
        {
            ApplyPulsingScale(images[1].rectTransform, maxRatio: 1.15f, minRatio: 0.95f, duration: 3f, delay: 0.1f);
        }

        // 2. 2번 요소 (Index 2) 애니메이션: 1.05배 ~ 0.95배
        if (images.Count > 2)
        {
            ApplyPulsingScale(images[2].rectTransform, maxRatio: 1.05f, minRatio: 0.95f, duration: 1.25f, delay: 0.3f);
        }

        if (images.Count > 3)
        {
            ApplyPulsingScale(images[3].rectTransform, maxRatio: 1.02f, minRatio: 0.96f, duration: 3f, delay: 0.3f);
        }


        // 3. 4번 요소 (Index 4) 애니메이션: 1.05배 ~ 0.95배
        if (images.Count > 4)
        {
            ApplyPulsingScale(images[4].rectTransform, maxRatio: 1.05f, minRatio: 0.95f, duration: 1.4f, delay: 0.5f);
        }

        // 4. 5번 요소 (Index 5) 애니메이션: 1.05배 ~ 0.95배
        if (images.Count > 5)
        {
            ApplyPulsingScale(images[5].rectTransform, maxRatio: 1.05f, minRatio: 0.95f, duration: 1.3f, delay: 0.7f);
        }
    }

    // ===================================
    // ⚙️ 헬퍼 함수: 크기 웅웅거림 반복 (수정됨)
    // ===================================

    /// <summary>
    /// 대상 요소에 웅웅거림(Pulsing) 크기 애니메이션을 적용합니다.
    /// maxRatio, minRatio는 InitElements에서 설정된 초기 스케일 대비 비율입니다.
    /// </summary>
    private void ApplyPulsingScale(RectTransform target, float maxRatio, float minRatio, float duration, float delay)
    {
        Vector3 baseScale = target.localScale;

        // 초기 스케일(baseScale) 대비 비율을 곱하여 목표 스케일을 정확히 계산
        Vector3 targetScaleMax = baseScale * maxRatio;
        Vector3 targetScaleMin = baseScale * minRatio;

        // 현재 크기에서 최소 크기까지 이동 (duration의 절반 시간)
        // 이후 Yoyo 루프가 targetScaleMax와 targetScaleMin 사이를 반복합니다.
        target.DOScale(targetScaleMin, duration * 0.5f)
              .SetEase(Ease.InOutSine)
              .SetDelay(delay)
              .SetLoops(-1, LoopType.Yoyo);
    }

    // ===================================
    // 🛠️ 헬퍼 함수 (InitElements 동작을 위한 필수 함수)
    // ===================================

    private void SetRect(Image img, float x, float y, float scale)
    {
        RectTransform rt = img.rectTransform;
        rt.anchoredPosition = new Vector2(x, y);
        rt.localScale = Vector3.one * scale;
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            foreach (var img in animationImages)
            {
                if (img != null)
                {
                    img.DOKill();
                    img.GetComponent<RectTransform>().DOKill();
                }
            }
        }
    }
}




public class CardAnimation_Num_135 : CardAnimationBase
{
    private const float InitialRotZ = 0f;

    // 동적으로 생성된 요소들을 관리하기 위한 리스트
    private System.Collections.Generic.List<UnityEngine.GameObject> _activeCopies = new System.Collections.Generic.List<UnityEngine.GameObject>();

    protected override void ExecuteAnimation(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        // ====================================================================
        // 초기 설정
        // ====================================================================
        SetPosScaleRotate(images, 1, 164f, -564f, 0.75f, InitialRotZ);
        SetPosScaleRotate(images, 2, -255f, -50f, 0.5f, 0f);
        SetAlpha(images, 2, 0f);
        SetPosScaleRotate(images, 3, 531f, -232f, 0.22f, 0f);
        SetAlpha(images, 3, 0f);
        SetPosScaleRotate(images, 4, 0f, 0f, 0.36f, 0f);
        SetAlpha(images, 4, 0f); // 4번 요소도 페이드 인을 위해 초기 알파 0

        // ====================================================================
        // 애니메이션 시작
        // ====================================================================

        if (images.Count > 1)
        {
            StartComplexRotationLoop(images[1].GetComponent<UnityEngine.RectTransform>(), InitialRotZ, images);
        }
    }

    protected override void KillAllTweens()
    {
        if (animationImages != null)
        {
            // 1번 (회전), 2번 (페이드), 3번 (페이드) 트윈 정리
            if (animationImages.Count > 1) animationImages[1].GetComponent<UnityEngine.RectTransform>().DOKill(true);
            if (animationImages.Count > 2) animationImages[2].DOKill(true);
            if (animationImages.Count > 3) animationImages[3].DOKill(true);
            if (animationImages.Count > 4) animationImages[4].DOKill(true); // 원본 4번 요소의 트윈도 정리

            // 🌟 동적으로 생성된 4번 요소 복사본들 정리 및 파괴 🌟
            foreach (UnityEngine.GameObject copy in _activeCopies)
            {
                if (copy != null)
                {
                    copy.GetComponent<UnityEngine.UI.Image>().DOKill(true);
                    UnityEngine.GameObject.Destroy(copy);
                }
            }
            _activeCopies.Clear();
        }
    }

    // ====================================================================
    // Z축 복합 반복 회전 로직 (1번 요소 - 강한 움직임 적용)
    // ====================================================================

    private void StartComplexRotationLoop(UnityEngine.RectTransform rect, float initialRotZ, System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        const float ROT_DURATION_LONG = 0.6f;
        const float ROT_DURATION_SHORT = 0.05f;
        const float ROT_DURATION_RETURN = 0.6f;
        const float ROT_DELAY = 0.3f;

        const float NEG_PEAK = -12f; // 왼쪽 최대 각도
        const float POS_PEAK = 12f;  // 오른쪽 최대 각도

        DG.Tweening.Sequence seq = DG.Tweening.DOTween.Sequence().SetTarget(rect);

        // --- 첫 번째 스윙 (음의 방향/왼쪽) ---
        // 1.1: InitialRotZ -> -12도 (0.6s) - OutSine
        seq.Append(
            rect.DOLocalRotate(new UnityEngine.Vector3(0f, 0f, initialRotZ + NEG_PEAK), ROT_DURATION_LONG)
                .SetEase(DG.Tweening.Ease.OutSine)
        );

        // 1.2: -12도 -> -4도 (0.05s) - Linear (왼쪽 꺾이는 순간)
        seq.Append(
            rect.DOLocalRotate(new UnityEngine.Vector3(0f, 0f, initialRotZ + (NEG_PEAK + 8f)), ROT_DURATION_SHORT)
                .SetEase(DG.Tweening.Ease.Linear)
        );
        // 🌟 콜백: 왼쪽 꺾임 시 2번 요소 페이드 & 4번 요소 복사본 4개 생성 🌟
        seq.AppendCallback(() =>
        {
            HandleElement2Fade(images);
            CreateAndFadeElement4(images, -527f, 73f, 231f, 65f, 4);
        });

        // 1.3: -4도 -> 원래 로테이션 (0.6s) - OutQuad (중앙으로 스냅)
        seq.Append(
            rect.DOLocalRotate(new UnityEngine.Vector3(0f, 0f, initialRotZ), ROT_DURATION_RETURN)
                .SetEase(DG.Tweening.Ease.OutQuad)
        );

        // --- 두 번째 스윙 (양의 방향/오른쪽) ---
        // 1.4: InitialRotZ -> +12도 (0.6s) - OutSine
        seq.Append(
            rect.DOLocalRotate(new UnityEngine.Vector3(0f, 0f, initialRotZ + POS_PEAK), ROT_DURATION_LONG)
                .SetEase(DG.Tweening.Ease.OutSine)
        );

        // 1.5: +12도 -> +4도 (0.05s) - Linear (오른쪽 꺾이는 순간)
        seq.Append(
            rect.DOLocalRotate(new UnityEngine.Vector3(0f, 0f, initialRotZ + (POS_PEAK - 8f)), ROT_DURATION_SHORT)
                .SetEase(DG.Tweening.Ease.Linear)
        );
        // 🌟 콜백: 오른쪽 꺾임 시 3번 요소 페이드만 (4번 요소 생성 제거) 🌟
        seq.AppendCallback(() =>
        {
            HandleElement3Fade(images);
            // CreateAndFadeElement4(images, -527f, 73f, 231f, 65f, 4); // 이 부분 제거
        });

        // 1.6: +4도 -> 원래 로테이션 (0.6s) - OutQuad (중앙으로 스냅)
        seq.Append(
            rect.DOLocalRotate(new UnityEngine.Vector3(0f, 0f, initialRotZ), ROT_DURATION_RETURN)
                .SetEase(DG.Tweening.Ease.OutQuad)
        );

        // 1.7: 0.3초 대기
        seq.AppendInterval(ROT_DELAY);

        seq.SetLoops(-1, DG.Tweening.LoopType.Restart);
    }

    // ====================================================================
    // 2번 요소 페이드 애니메이션 (0.1s In -> 0.2s Hold -> 0.3s Out)
    // ====================================================================
    private void HandleElement2Fade(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count <= 2) return;
        UnityEngine.UI.Image element = images[2];

        element.DOKill(true); // 기존 트윈 정리

        DG.Tweening.Sequence fadeSeq = DG.Tweening.DOTween.Sequence().SetTarget(element);

        // 1. Fade In (0.1s)
        fadeSeq.Append(element.DOFade(1f, 0.1f).SetEase(DG.Tweening.Ease.Linear));

        // 2. Hold (0.2s)
        fadeSeq.AppendInterval(0.2f);

        // 3. Fade Out (0.3s)
        fadeSeq.Append(element.DOFade(0f, 0.3f).SetEase(DG.Tweening.Ease.Linear));
    }

    // ====================================================================
    // 3번 요소 페이드 애니메이션 (0.1s In -> 0.2s Hold -> 0.3s Out)
    // ====================================================================
    private void HandleElement3Fade(System.Collections.Generic.List<UnityEngine.UI.Image> images)
    {
        if (images.Count <= 3) return;
        UnityEngine.UI.Image element = images[3];

        element.DOKill(true); // 기존 트윈 정리

        DG.Tweening.Sequence fadeSeq = DG.Tweening.DOTween.Sequence().SetTarget(element);

        // 1. Fade In (0.1s)
        fadeSeq.Append(element.DOFade(1f, 0.1f).SetEase(DG.Tweening.Ease.Linear));

        // 2. Hold (0.2s)
        fadeSeq.AppendInterval(0.2f);

        // 3. Fade Out (0.3s)
        fadeSeq.Append(element.DOFade(0f, 0.3f).SetEase(DG.Tweening.Ease.Linear));
    }

    // ====================================================================
    // 4번 요소 복제 및 페이드 애니메이션
    // ====================================================================

    private void CreateAndFadeElement4(
        System.Collections.Generic.List<UnityEngine.UI.Image> images,
        float minX, float maxX, float minY, float maxY,
        int count
    )
    {
        // 4번 요소는 인덱스 4에 있다고 가정
        if (images.Count <= 4) return;

        UnityEngine.GameObject originalElement4 = images[4].gameObject;
        UnityEngine.Transform parentTransform = originalElement4.transform.parent;

        for (int i = 0; i < count; i++)
        {
            // 1. 4번 요소 복제 및 리스트에 추가
            UnityEngine.GameObject copy = UnityEngine.GameObject.Instantiate(originalElement4, parentTransform);
            UnityEngine.UI.Image copyImage = copy.GetComponent<UnityEngine.UI.Image>();
            copyImage.color = new UnityEngine.Color(copyImage.color.r, copyImage.color.g, copyImage.color.b, 0f); // 초기 알파 0
            _activeCopies.Add(copy);

            // 2. 무작위 위치 설정
            float randomX = UnityEngine.Random.Range(minX, maxX);
            float randomY = UnityEngine.Random.Range(minY, maxY);
            copy.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(randomX, randomY);

            // 3. 페이드 애니메이션 시퀀스 (0.1s In -> 0.1s Hold -> 0.3s Out)
            DG.Tweening.Sequence fadeSeq = DG.Tweening.DOTween.Sequence().SetTarget(copyImage);

            fadeSeq.Append(copyImage.DOFade(1f, 0.1f));          // Fade In (0.1s)
            fadeSeq.AppendInterval(0.1f);                       // Hold (0.1s)
            fadeSeq.Append(copyImage.DOFade(0f, 0.3f));          // Fade Out (0.3s)

            // 4. 애니메이션 완료 후 GameObject 파괴
            fadeSeq.OnComplete(() =>
            {
                if (copy != null)
                {
                    _activeCopies.Remove(copy); // 리스트에서 제거
                    UnityEngine.GameObject.Destroy(copy);
                }
            });
        }
    }

    // ====================================================================
    // 보조 함수 (FQN 사용)
    // ====================================================================

    private void SetPosScaleRotate(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float x, float y, float scale, float rotZ)
    {
        if (index < imgs.Count)
        {
            UnityEngine.RectTransform rt = imgs[index].GetComponent<UnityEngine.RectTransform>();
            if (rt == null) return;

            rt.anchoredPosition = new UnityEngine.Vector2(x, y);
            rt.localScale = UnityEngine.Vector3.one * scale;
            rt.localRotation = UnityEngine.Quaternion.Euler(0f, 0f, rotZ);
        }
    }

    private void SetAlpha(System.Collections.Generic.List<UnityEngine.UI.Image> imgs, int index, float alpha)
    {
        if (index < imgs.Count)
        {
            UnityEngine.Color c = imgs[index].color;
            imgs[index].color = new UnityEngine.Color(c.r, c.g, c.b, alpha);
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







public class CardAnimation_Num_999 : CardAnimationBase
{
    protected override void ExecuteAnimation(List<Image> images)
    {

    }
    protected override void KillAllTweens()
    {
    }
}
