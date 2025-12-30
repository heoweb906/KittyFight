using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlindEggOverlayUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image[] eggs; // Egg1~3 (순서대로)

    [Header("Pop In")]
    [SerializeField] private float popScaleFrom = 0.2f;
    [SerializeField] private float popScaleTo = 1.15f;
    [SerializeField] private float popSettleTo = 1.0f;
    [SerializeField] private float popUpTime = 0.12f;
    [SerializeField] private float popSettleTime = 0.08f;
    [SerializeField] private float eggPopStagger = 0.06f; // 0.06초씩 순차 팝

    [Header("Fade Out")]
    [SerializeField] private float fadeOutTime = 0.35f;

    Coroutine routine;

    void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Play(float duration, int eggCount = 3)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(CoPlay(duration, eggCount));
    }

    IEnumerator CoPlay(float duration, int eggCount)
    {
        if (!canvasGroup) yield break;

        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;

        // eggCount만큼만 표시
        eggCount = Mathf.Clamp(eggCount, 0, eggs.Length);
        for (int i = 0; i < eggs.Length; i++)
        {
            bool on = i < eggCount;
            eggs[i].gameObject.SetActive(on);
            if (on)
            {
                eggs[i].rectTransform.localScale = Vector3.one * popScaleFrom;
                var c = eggs[i].color;
                c.a = 1f;
                eggs[i].color = c;
            }
        }

        // 등장 팝 애니메이션 (순차)
        for (int i = 0; i < eggCount; i++)
        {
            StartCoroutine(CoPopOne(eggs[i].rectTransform));
            yield return new WaitForSeconds(eggPopStagger);
        }

        // 유지
        if (duration > 0f)
            yield return new WaitForSeconds(duration);

        // 퇴장: 전체 알파 페이드아웃
        yield return CoFadeCanvas(1f, 0f, fadeOutTime);

        // 정리
        //gameObject.SetActive(false);
        routine = null;
    }

    IEnumerator CoPopOne(RectTransform rt)
    {
        // 1) 빠르게 커짐(오버슈트)
        yield return CoScale(rt, popScaleFrom, popScaleTo, popUpTime);

        // 2) 살짝 줄어들며 정착
        yield return CoScale(rt, popScaleTo, popSettleTo, popSettleTime);
    }

    IEnumerator CoScale(RectTransform rt, float from, float to, float time)
    {
        float t = 0f;
        rt.localScale = Vector3.one * from;

        while (t < time)
        {
            t += Time.unscaledDeltaTime; // UI는 보통 unscaled가 자연스러움(슬로우/일시정지 영향 최소화)
            float u = (time <= 0f) ? 1f : Mathf.Clamp01(t / time);

            // EaseOutBack 느낌을 간단히 흉내(오버슈트는 이미 popScaleTo로 처리)
            float eased = 1f - Mathf.Pow(1f - u, 3f); // cubic out
            float s = Mathf.Lerp(from, to, eased);
            rt.localScale = Vector3.one * s;

            yield return null;
        }

        rt.localScale = Vector3.one * to;
    }

    IEnumerator CoFadeCanvas(float from, float to, float time)
    {
        float t = 0f;
        canvasGroup.alpha = from;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float u = (time <= 0f) ? 1f : Mathf.Clamp01(t / time);

            // ease in-out
            float eased = u * u * (3f - 2f * u);
            canvasGroup.alpha = Mathf.Lerp(from, to, eased);

            yield return null;
        }

        canvasGroup.alpha = to;
    }
}