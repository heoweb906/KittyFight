using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening; 

public class SkillCard_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SkillCardController skillCardController { get; set; }
    public bool bCanInteract = false;

    [Header("UI ����")]
    public Image image_BackGround;
    public Image image_SkillIcon;
    public TextMeshProUGUI text_SkillName;
    public TextMeshProUGUI text_SkilDescription;

    [Header("��ų")]
    public SkillCard_SO skillCard_SO;

    // ���� �� ������
    private Vector3 originalScale;
    private Color originalBGColor;

    // Ʈ�� ����
    private float tweenDuration = 0.08f;
    private float scaleFactor = 0.95f;



    private void Awake()
    {
        originalScale = transform.localScale;
        originalBGColor = image_BackGround.color;
    }






    /// <summary>
    /// SO ������ ������� UI ��� �ʱ�ȭ
    /// </summary>
    public void ApplyData(SkillCard_SO data)
    {
        if (data == null) return;

        skillCard_SO = data;

        // Background �̹��� ����
        if (data.image_BackGround != null && data.image_BackGround.sprite != null)
            image_BackGround.sprite = data.image_BackGround.sprite;

        // Skill Icon ����
        if (data.image_SkillIcon != null && data.image_SkillIcon.sprite != null)
            image_SkillIcon.sprite = data.image_SkillIcon.sprite;

        // �ؽ�Ʈ ����
        text_SkillName.text = data.sSkillName;
        text_SkilDescription.text = data.sSkillDescription;
    }




    // ���콺 �ø�
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!bCanInteract) return;

        // ���� Ʈ�� ����
        transform.DOKill();
        image_BackGround.DOKill();

        // �ε巯�� ũ�� ���
        transform.DOScale(originalScale * scaleFactor, tweenDuration);

        // ��� ä�� ���߱�(ȸ����� ����)
        Color targetColor = Color.Lerp(originalBGColor, Color.gray, 0.5f);
        image_BackGround.DOColor(targetColor, tweenDuration);

        // Debug.Log($"[SkillCard_UI] Enter '{text_SkillName.text}'");
    }


    // ���콺 Ŀ�� ����
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!bCanInteract) return;

        // ���� Ʈ�� ����
        transform.DOKill();
        image_BackGround.DOKill();

        // ���� ũ��, �÷��� ����
        transform.DOScale(originalScale, tweenDuration);
        image_BackGround.DOColor(originalBGColor, tweenDuration);

        // Debug.Log($"[SkillCard_UI] Exit '{text_SkillName.text}'");
    }





    // ���콺 Ŀ�� Ŭ��
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!bCanInteract) return;
        skillCardController.SetAllCanInteract(false);



        // Debug.Log($"[SkillCard_UI] Clicked '{text_SkillName.text}'");

        // Ŭ���ϸ� Exit ȿ�� ����
        OnPointerExit(eventData);

        // Ŭ�� �ִϸ��̼�: ��ġ �� HideAll ȣ��
        transform.DOKill();
        Sequence clickSeq = DOTween.Sequence();
        clickSeq.Append(transform.DOPunchScale(originalScale * 0.1f, tweenDuration + 0.5f));
        clickSeq.Append(transform.DOScale(originalScale, tweenDuration));
        clickSeq.OnComplete(() =>
        {
            // �ִϸ��̼��� ���� �ڿ��� ���� ���� ����
            skillCardController.HideAll();
        });
    }
}
