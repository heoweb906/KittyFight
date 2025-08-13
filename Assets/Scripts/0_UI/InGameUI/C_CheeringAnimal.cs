using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class C_CheeringAnimal : MonoBehaviour
{
    private RectTransform rectTransform_Origin;
    public RectTransform rectTransform_Target;
    private bool isFloating = false;
    private Vector2 originalPosition;

    void Start()
    {
        rectTransform_Origin = GetComponent<RectTransform>();
        originalPosition = rectTransform_Origin.anchoredPosition;
    }


    public void On()
    {
        Debug.Log("응원 시작!!!");

        gameObject.SetActive(true);

        if (rectTransform_Origin == null || rectTransform_Target == null) return;
        isFloating = false;
        rectTransform_Origin.DOKill();
        // 위치 이동 완료 후에 플로팅 시작
        rectTransform_Origin.DOAnchorPos(rectTransform_Target.anchoredPosition, 0.15f).SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                StartFloating();
            });
    }
    public void Off()
    {
        Debug.Log("응원 종료@@@");
        isFloating = false;
        rectTransform_Origin.DOKill();
        // 원래 위치로 돌아가기
        rectTransform_Origin.DOAnchorPos(originalPosition, 0.8f).SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }


    private void StartFloating()
    {
        if (!isFloating)
        {
            isFloating = true;
            FloatingLoop();
        }
    }


    private void FloatingLoop()
    {
        if (!isFloating) return;
        Vector2 targetPos = rectTransform_Target.anchoredPosition;
        float jumpHeight = Random.Range(30f, 60f);

        // 위로 점프
        rectTransform_Origin.DOAnchorPosY(targetPos.y + jumpHeight, Random.Range(0.25f, 0.4f))
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // 원래 위치로 돌아오기
                rectTransform_Origin.DOAnchorPosY(targetPos.y, Random.Range(0.25f, 0.4f))
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        FloatingLoop();
                    });
            });
    }
}