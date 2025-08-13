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
        Debug.Log("���� ����!!!");

        gameObject.SetActive(true);

        if (rectTransform_Origin == null || rectTransform_Target == null) return;
        isFloating = false;
        rectTransform_Origin.DOKill();
        // ��ġ �̵� �Ϸ� �Ŀ� �÷��� ����
        rectTransform_Origin.DOAnchorPos(rectTransform_Target.anchoredPosition, 0.15f).SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                StartFloating();
            });
    }
    public void Off()
    {
        Debug.Log("���� ����@@@");
        isFloating = false;
        rectTransform_Origin.DOKill();
        // ���� ��ġ�� ���ư���
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

        // ���� ����
        rectTransform_Origin.DOAnchorPosY(targetPos.y + jumpHeight, Random.Range(0.25f, 0.4f))
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // ���� ��ġ�� ���ƿ���
                rectTransform_Origin.DOAnchorPosY(targetPos.y, Random.Range(0.25f, 0.4f))
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        FloatingLoop();
                    });
            });
    }
}