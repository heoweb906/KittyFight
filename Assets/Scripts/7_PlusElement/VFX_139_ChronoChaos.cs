using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VFX_139_ChronoChaos : MonoBehaviour
{
    [Header("Components")]
    public ParticleSystem ps_System;
    public SpriteRenderer sr_Renderer;
    public Sprite[] sprites;

    private Vector3 baseScale;
    private Sequence seq;

    private void Awake()
    {
        baseScale = transform.localScale;
        InitializeState();
    }

    // 초기화: 아무것도 안 보이는 상태
    private void InitializeState()
    {
        if (sr_Renderer != null)
        {
            Color c = sr_Renderer.color;
            c.a = 0f;
            sr_Renderer.color = c;
            transform.localScale = baseScale;
        }
        if (ps_System != null) ps_System.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Play()
    {
        if (sr_Renderer == null || sprites == null || sprites.Length < 2) return;

        // 기존 연출 중단 및 초기화
        seq.Kill();
        InitializeState();

        seq = DOTween.Sequence();

        // 1. 첫 번째 이미지 설정 및 등장 (0.1초)
        sr_Renderer.sprite = sprites[0];

        // 알파값 1 & 크기 커졌다 작아지기 (Pop 효과)
        seq.Append(sr_Renderer.DOFade(1f, 0.1f));
        seq.Join(transform.DOScale(baseScale * 1.2f, 0.05f).SetEase(Ease.OutQuad)); // 커짐
        seq.Append(transform.DOScale(baseScale, 0.05f).SetEase(Ease.InQuad));       // 복구

        // 2. 0.5초 대기
        seq.AppendInterval(0.2f);

        // 3. 두 번째 이미지 교체 & 파티클 재생 & Pop 효과
        seq.AppendCallback(() =>
        {
            sr_Renderer.sprite = sprites[1];
            if (ps_System != null) ps_System.Play();
        });
        seq.Append(transform.DOScale(baseScale * 1.2f, 0.05f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(baseScale, 0.05f).SetEase(Ease.InQuad));

        // 4. 0.2초 대기 후 페이드 아웃 (0.5초)
        seq.AppendInterval(0.2f);
        seq.Append(sr_Renderer.DOFade(0f, 0.5f));
    }
}