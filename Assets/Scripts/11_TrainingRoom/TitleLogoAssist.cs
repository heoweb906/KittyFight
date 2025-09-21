using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TitleLogoAssist : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public CameraManager cameraManager;
    public TestPlayerComtroller testPlayerComtroller;

    [Header("Ÿ��Ʋ �ΰ�")]
    public SpriteRenderer image_Logo;

    [Header("ī�޶� ���� / �ǻ�ü ��ġ")]
    public GameObject[] objs_VirtualCamera; 
    // 0 - �Ϸ���Ʈ, �߾� ��
    // 1 - ���� ��
    // 2 - ������
    public Transform[] transforms_Subject;
    // 0 - ī�޶� ���� ��ġ
    // 1 - �߾� ��

    public Door_CameraChange[] doors;

    private int currentStep = 0; // 0: ���, 1: �ΰ� ���̵���, 2: ���, 3: ī�޶� �̵�

    private void Awake()
    {
        image_Logo.DOFade(0f, 0f);
        ChangeVirtualCamera(0);
        objs_VirtualCamera[0].transform.position = transforms_Subject[0].position;
        testPlayerComtroller.bCanControl = false;

        for (int i = 0; i < doors.Length; ++i) doors[i].titleLogoAssist = this;
    }
    private void Start()
    {
        StartStep1();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SkipCurrentStep();
        }
    }


    private void SkipCurrentStep()
    {
        // DOTween.KillAll() �����ϰ� �� ��ũ��Ʈ�� Ʈ���׸� ����
        image_Logo.DOKill();
        objs_VirtualCamera[0].transform.DOKill();
        DOTween.Kill(this); // �� MonoBehaviour�� ������ DOVirtual ����
        if (currentStep == 1)
        {
            image_Logo.color = new Color(image_Logo.color.r, image_Logo.color.g, image_Logo.color.b, 1f);
            currentStep = 2;
            DOVirtual.DelayedCall(3f, StartStep3);
        }
        else if (currentStep == 2)
        {
            StartStep3();
        }
        else if (currentStep == 3)
        {
            objs_VirtualCamera[0].transform.position = transforms_Subject[1].position;
            DOVirtual.DelayedCall(0.2f, () => mainMenuController.SwitchPanel_ByButton(1));
        }
    }


    // �ΰ��� ���İ��� 1�̵�, �׸��� ���
    private void StartStep1()
    {
        currentStep = 1;
        image_Logo.DOFade(1f, 3f).SetDelay(1f).OnComplete(() => {
            currentStep = 2;
            DOVirtual.DelayedCall(3f, StartStep3);
        });
    }

    // ī�޶� �Ʒ� ������� �̵�
    private void StartStep3()
    {
        currentStep = 3;
        objs_VirtualCamera[0].transform.DOMove(transforms_Subject[1].position, 10f).OnComplete(() => {

            DOVirtual.DelayedCall(0.2f, () => mainMenuController.SwitchPanel_ByButton(1));
        });
    }



    public void ChangeVirtualCamera(int targetIndex)
    {
        for (int i = 0; i < objs_VirtualCamera.Length; i++)
        {
            if (i != targetIndex)
            {
                objs_VirtualCamera[i].SetActive(false);
            }
            else
            {
                objs_VirtualCamera[i].SetActive(true);
                cameraManager.objCamaera = objs_VirtualCamera[i];
            }
        }
    }
}