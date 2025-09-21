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

    [Header("타이틀 로고")]
    public SpriteRenderer image_Logo;

    [Header("카메라 종류 / 피사체 위치")]
    public GameObject[] objs_VirtualCamera; 
    // 0 - 일러스트, 중앙 방
    // 1 - 우측 방
    // 2 - 좌측방
    public Transform[] transforms_Subject;
    // 0 - 카메라 최초 위치
    // 1 - 중앙 방

    public Door_CameraChange[] doors;

    private int currentStep = 0; // 0: 대기, 1: 로고 페이드인, 2: 대기, 3: 카메라 이동

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
        // DOTween.KillAll() 제거하고 이 스크립트의 트위닝만 정리
        image_Logo.DOKill();
        objs_VirtualCamera[0].transform.DOKill();
        DOTween.Kill(this); // 이 MonoBehaviour와 연관된 DOVirtual 정리
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


    // 로고의 알파값이 1이됨, 그리고 대기
    private void StartStep1()
    {
        currentStep = 1;
        image_Logo.DOFade(1f, 3f).SetDelay(1f).OnComplete(() => {
            currentStep = 2;
            DOVirtual.DelayedCall(3f, StartStep3);
        });
    }

    // 카메라가 아래 방향까지 이동
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