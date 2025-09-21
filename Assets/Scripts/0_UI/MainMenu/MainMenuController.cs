using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;

public class MainMenuController : MonoBehaviour
{
    public Canvas mainCanVas;
    public MatchManager matchManager;
    public TestPlayerComtroller scriptPlayerCharacter;
    public TitleLogoAssist titleLogoAssist;

    public List<GameObject> panels; // 인덱스로 관리
    private int currentIndex = -1;


    [Header("Panel_InputNickName 관련")]
    public Image image_UpperArea;
    public Image image_LowerArea;
    public TMP_InputField nicknameInput;
    public Transform transformCenter;


    [Header("Panel_MatchingLoading 관련")]
    public Image iamge_UpperAreaMatching;
    public Image iamge_LowerAreaMatching;
    public MatchStartCollision matchStartCollision_1;           // 나중에 지정 매칭으로
    public MatchStartCollision matchStartCollision_2;          


    [Header("SceneChnageAssist 관련")]
    public Image image_LeftBackGround;
    public Image image_RightBackGround;


    void Start()
    {
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        OpenSceneChangePanel(image_LeftBackGround.rectTransform, image_RightBackGround.rectTransform, 0f);
        OpenInputnickNamePanel_Vertical(image_UpperArea.rectTransform, image_LowerArea.rectTransform, 0f);
        OpenInputnickNamePanel_Vertical(iamge_UpperAreaMatching.rectTransform, iamge_LowerAreaMatching.rectTransform, 0.2f);

        matchStartCollision_1.mainMenuController = this;
        matchStartCollision_2.mainMenuController = this;
    }




    // #. 부드러운 패널 교체
    public void SwitchPanel_ByButton(int targetIndex) // 버튼용 함수
    {
        if (targetIndex < 0 || targetIndex >= panels.Count || targetIndex == currentIndex)
            return;
        if (currentIndex >= 0)
        {
            panels[currentIndex].SetActive(false);
        }


        if (targetIndex == 0) scriptPlayerCharacter.bCanControl = false;
        else if (targetIndex == 1) scriptPlayerCharacter.bCanControl = true;


        panels[targetIndex].SetActive(true);
        currentIndex = targetIndex;

        // StartCoroutine(SwitchRoutine(targetIndex));
    }

    //private IEnumerator SwitchRoutine(int targetIndex)
    //{
    //    CloseSceneChangePanel(image_LeftBackGround.rectTransform, image_RightBackGround.rectTransform, 0.15f);
    //    yield return new WaitForSeconds(0.8f); 

    //    if (currentIndex >= 0)
    //    {
    //        panels[currentIndex].SetActive(false);
    //    }
    //    panels[targetIndex].SetActive(true);
    //    currentIndex = targetIndex;
    //    // 이미지 열기
    //    OpenSceneChangePanel(image_LeftBackGround.rectTransform, image_RightBackGround.rectTransform, 0.2f);
    //}





    // #. 게임 종료 함수
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
  // 빌드된 게임에서 실행 중일 때
  Application.Quit();
#endif
    }




    public void CloseSceneChangePanel(RectTransform leftImage, RectTransform rightImage, float fDuration)
    {
        leftImage.gameObject.SetActive(true);
        rightImage.gameObject.SetActive(true);
        float leftImageWidth = leftImage.rect.width;
        float rightImageWidth = rightImage.rect.width;
        float leftClosedPosX = -(leftImageWidth / 2f);
        float rightClosedPosX = (rightImageWidth / 2f);
        leftImage.DOAnchorPosX(leftClosedPosX, fDuration).SetEase(Ease.OutQuart);
        rightImage.DOAnchorPosX(rightClosedPosX, fDuration).SetEase(Ease.OutQuart);
    }
    public void OpenSceneChangePanel(RectTransform leftImage, RectTransform rightImage, float fDuration)
    {
        RectTransform canvasRect = mainCanVas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float imageWidth = leftImage.rect.width;
        float leftTargetX = -(canvasWidth / 2f) - (imageWidth / 2f);
        float rightTargetX = (canvasWidth / 2f) + (imageWidth / 2f);
        leftImage.DOAnchorPosX(leftTargetX, fDuration).SetEase(Ease.InQuint)
            .OnComplete(() => {
                leftImage.gameObject.SetActive(false);
                rightImage.gameObject.SetActive(false);
            });
        rightImage.DOAnchorPosX(rightTargetX, fDuration).SetEase(Ease.InQuint);
    }



    public void OnNickNameInputPanel()
    {
        CloseInputnickNamePanel_Vertical(image_UpperArea.rectTransform, image_LowerArea.rectTransform, 0.15f);
    }
    public void OffNickNameInputPanel(int iPanelNum)         // 닉네임 결정
    {
        matchManager.MyNickname = nicknameInput.text;

        scriptPlayerCharacter.bCanControl = true;

        // SwitchPanel_ByButton(iPanelNum);

        OpenInputnickNamePanel_Vertical(image_UpperArea.rectTransform, image_LowerArea.rectTransform, 0.2f);
    }

    public void CloseInputnickNamePanel_Vertical(RectTransform topImage, RectTransform bottomImage, float fDuration)
    {
        topImage.gameObject.SetActive(true);
        bottomImage.gameObject.SetActive(true);
        float topImageHeight = topImage.rect.height;
        float bottomImageHeight = bottomImage.rect.height;
        float topClosedPosY = (topImageHeight / 2f);
        float bottomClosedPosY = -(bottomImageHeight / 2f);
        topImage.DOAnchorPosY(topClosedPosY, fDuration).SetEase(Ease.OutQuart);
        bottomImage.DOAnchorPosY(bottomClosedPosY, fDuration).SetEase(Ease.OutQuart);
    }
    public void OpenInputnickNamePanel_Vertical(RectTransform topImage, RectTransform bottomImage, float fDuration)
    {
        RectTransform canvasRect = mainCanVas.GetComponent<RectTransform>();
        float canvasHeight = canvasRect.rect.height;
        float imageHeight = topImage.rect.height;
        float topTargetY = (canvasHeight / 2f) + (imageHeight / 2f);
        float bottomTargetY = -(canvasHeight / 2f) - (imageHeight / 2f);
        topImage.DOAnchorPosY(topTargetY, fDuration).SetEase(Ease.InQuint)
            .OnComplete(() => {
                topImage.gameObject.SetActive(false);
                bottomImage.gameObject.SetActive(false);
            });
        bottomImage.DOAnchorPosY(bottomTargetY, fDuration).SetEase(Ease.InQuint);
    }







    public void StartMatching()
    {
        scriptPlayerCharacter.bCanControl = false;

        scriptPlayerCharacter.GetComponent<Rigidbody>().isKinematic = true;

        CloseInputnickNamePanel_Vertical(iamge_UpperAreaMatching.rectTransform, iamge_LowerAreaMatching.rectTransform, 0.15f);
        panels[1].SetActive(false); // Player UI
        DOVirtual.DelayedCall(0.2f, () => {
            matchManager.OnMatchButtonClicked();
        });
    }
    public void StopMatching()
    {
        matchManager.OnCancelMatchButtonClicked();
        scriptPlayerCharacter.transform.position = transformCenter.position;

        scriptPlayerCharacter.GetComponent<Rigidbody>().isKinematic = false;

        OpenInputnickNamePanel_Vertical(iamge_UpperAreaMatching.rectTransform, iamge_LowerAreaMatching.rectTransform, 0.15f);
        panels[1].SetActive(true); // Player UI
        DOVirtual.DelayedCall(0.2f, () => {
            scriptPlayerCharacter.bCanControl = true;
        });
    }



    public void ResetPlayerPosition(GameObject obj)
    {
        scriptPlayerCharacter.bCanControl = false;


        obj.transform.position = transformCenter.position;
        obj.transform.eulerAngles = new Vector3(0f, 90f, 0f);
        Rigidbody playerRb = obj.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        StartCoroutine(DelayedAction());
    }


    private IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(0.3f);
        titleLogoAssist.ChangeVirtualCamera(0);
    }




}