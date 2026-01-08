using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    public Canvas mainCanVas;
    public MatchManager matchManager;
    public TestPlayerComtroller scriptPlayerCharacter;
    public TitleLogoAssist titleLogoAssist;
    public CameraManager cameraManager;

    public List<GameObject> panels; // 인덱스로 관리
    private int currentIndex = -1;


    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    public AudioClip[] sfxClips;
    [SerializeField] private AudioSource bgmSource;
    public AudioClip[] bgmClips;

    


    [Header("Panel_InputNickName 관련")]
    public Image image_UpperArea;
    public Image image_LowerArea;
    public TMP_InputField nicknameInput;
    public Transform transformCenter;
    public GameObject text_WarningInfo;
    private bool isButtonCooldown = false;

    public RectTransform nicknameConfirmButtonRect;


    [Header("Panel_MatchingLoading 관련")]
    public Image iamge_UpperAreaMatching;
    public Image iamge_LowerAreaMatching;
    public MatchStartCollision matchStartCollision_1;           // 나중에 지정 매칭으로
    public MatchStartCollision matchStartCollision_2;
    public MatchStartCollision matchStartCollision_3; // 테스트용

    public RectTransform stopMatchingButtonRect;
    public GameObject[] successEffectObjs;
    public Button buttonCancel;

    public Image image_LoadingSpiner;
    private Tween currentSpinTween;


    [Header("SceneChnageAssist 관련")]
    public Image image_LeftBackGround;
    public Image image_RightBackGround;


    [Header("설정창 관련")]
    public GameObject obj_Gausian;
    public GameObject obj_PlayerPanel;
    public GameObject[] panelMenu;
    private int iPanelNum;    // 패널 번호
    private bool bOtherPanel;       // 닉네임 수정 중 or 매칭 중

    public Button_MainMenu[] buttonsMainMenu;






    void Start()
    {
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        OpenSceneChangePanel(image_LeftBackGround.rectTransform, image_RightBackGround.rectTransform, 0f);

        // 여기 false 추가 (소리 안 남)
        OpenInputnickNamePanel_Vertical(image_UpperArea.rectTransform, image_LowerArea.rectTransform, 0f, false);
        OpenInputnickNamePanel_Vertical(iamge_UpperAreaMatching.rectTransform, iamge_LowerAreaMatching.rectTransform, 0.2f, false);

        if (MatchResultStore.myNickname == null) matchManager.MyNickname = "Kitty";



        if (matchStartCollision_1 != null) matchStartCollision_1.mainMenuController = this; 
        if(matchStartCollision_2 != null) matchStartCollision_2.mainMenuController = this; 
        if(matchStartCollision_3 != null) matchStartCollision_3.mainMenuController = this;

        bOtherPanel = false;
        obj_Gausian.SetActive(false);

        iPanelNum = 0;
        scriptPlayerCharacter.bCanControl = false;


        Cursor.lockState = CursorLockMode.Confined;


        for (int i = 0; i < buttonsMainMenu.Length; ++i)
        {
            buttonsMainMenu[i].mainMenuController = this;
        }
    }


    private void Update()
    {
        if (titleLogoAssist.bTitleAssistFinish == true && bOtherPanel == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 0번이면 1번 열기
                if (iPanelNum == 0)
                {
                    ChangePanel(1);
                }
                else if (iPanelNum == 1)
                {
                    ChangePanel(0);
                }
                else if (iPanelNum >= 2)
                {
                    ChangePanel(1);
                }
            }
        }
    }


    public void ChangePanel(int _iPanelIdx)
    {
        ChangePanel(_iPanelIdx, true);
    }


    public void ChangePanel(int _iPanelIdx, bool bSoundOn = true)
    {
        if (panelMenu == null) return;
       
        // 1. 상태 변수 갱신
        iPanelNum = _iPanelIdx;

        // 2. 패널 활성화/비활성화 로직 (0번 비움, 1번부터 시작)
        for (int i = 1; i < panelMenu.Length; i++)
        {
            if (panelMenu[i] == null) continue;
            bool isActive = (iPanelNum > 0) && (i == iPanelNum);
            panelMenu[i].SetActive(isActive);
        }

        // 3. [요청하신 부분] 가우시안 배경 처리
        if (obj_Gausian != null)
        {
            if (_iPanelIdx == 1)
            {
                Cursor.lockState = CursorLockMode.None;

                obj_Gausian.SetActive(true);
                obj_PlayerPanel.SetActive(false);
                scriptPlayerCharacter.bCanControl = false;
                scriptPlayerCharacter.GetComponent<Rigidbody>().isKinematic = true;
            }
            else if (_iPanelIdx == 0)
            {
                Cursor.lockState = CursorLockMode.Confined;

                obj_Gausian.SetActive(false);
                obj_PlayerPanel.SetActive(true);
                scriptPlayerCharacter.bCanControl = true;
                scriptPlayerCharacter.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        if(bSoundOn) PlaySFX(sfxClips[1]);
    }






    // #. 부드러운 패널 교체
    public void SwitchPanel_ByButton(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= panels.Count || targetIndex == currentIndex)
            return;

        if (currentIndex >= 0)
        {
            panels[currentIndex].SetActive(false);
        }

        if (targetIndex == 0)
        {
            scriptPlayerCharacter.bCanControl = false;
        }
        else if (targetIndex == 1)
        {
            if(MatchResultStore.myNickname != null) SetNickName(MatchResultStore.myNickname);
            else SetNickName("KITTY"); // 닉네임 설정은 즉시 실행

            StartCoroutine(EnableControlRoutine(1.5f)); // 0.5초 딜레이 코루틴 시작
        }

        panels[targetIndex].SetActive(true);
        currentIndex = targetIndex;
    }

    // 딜레이를 처리할 코루틴 함수 추가
    private IEnumerator EnableControlRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        cameraManager.UpdateOriginalPosition();
        scriptPlayerCharacter.bCanControl = true;
        titleLogoAssist.bTitleAssistFinish = true;
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
        if (bOtherPanel) return;

        bOtherPanel = true;
        ChangePanel(0, false);


        // 0.3초 동안 문이 닫히는 애니메이션 실행 -> 끝난 후(Lambda) 내부 코드 실행
        CloseInputnickNamePanel_Vertical(image_UpperArea.rectTransform, image_LowerArea.rectTransform, 0.2f, () =>
        {

          

            // 애니메이션 종료 후 플레이어 이동
            // (SettingRoomNickName에서 other를 넘길 필요 없이, 이미 알고 있는 scriptPlayerCharacter 사용)
            if (scriptPlayerCharacter != null)
            {
                ResetPlayerPosition(scriptPlayerCharacter.gameObject);
            }
        });
    }

    public void CloseInputnickNamePanel_Vertical(RectTransform topImage, RectTransform bottomImage, float fDuration, System.Action onComplete = null)
    {
        PlaySFX(sfxClips[3]);

        topImage.gameObject.SetActive(true);
        bottomImage.gameObject.SetActive(true);

        float topImageHeight = topImage.rect.height;
        float bottomImageHeight = bottomImage.rect.height;
        float topClosedPosY = (topImageHeight / 2f);
        float bottomClosedPosY = -(bottomImageHeight / 2f);

        // topImage 애니메이션 끝에 OnComplete 연결
        topImage.DOAnchorPosY(topClosedPosY, fDuration).SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                // 애니메이션이 끝나면 콜백 실행
                onComplete?.Invoke();
            });

        bottomImage.DOAnchorPosY(bottomClosedPosY, fDuration).SetEase(Ease.OutQuart);
    }


    public void OnClickNicknameConfirmButton()
    {
        if (isButtonCooldown) return;
        StartCoroutine(ButtonCooldown());

        PlaySFX(sfxClips[1]);

        if (nicknameConfirmButtonRect == null)
        {
            Debug.LogError("닉네임 확인 버튼의 RectTransform이 연결되지 않았습니다.");
            ExecuteOffNickNameInputPanelLogic();
            return;
        }

        // 버튼 애니메이션 실행 후, 완료되면 ExecuteOffNickNameInputPanelLogic() 실행
        PlaySimpleAnimation(nicknameConfirmButtonRect, () =>
        {
            ExecuteOffNickNameInputPanelLogic();
        });
    }


    private void ExecuteOffNickNameInputPanelLogic()
    {
        // 이전 'if (isButtonCooldown) return;' 코드는 OnClickNicknameConfirmButton()으로 이동했습니다.

        string nickname = nicknameInput.text.Trim();

        // 공백이면 Kitty로 강제 설정
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = "Kitty";
            matchManager.MyNickname = nickname;
            MatchResultStore.myNickname = nickname;

            SetNickName(matchManager.MyNickname);
            scriptPlayerCharacter.bCanControl = true;
            OpenInputnickNamePanel_Vertical(image_UpperArea.rectTransform, image_LowerArea.rectTransform, 0.2f);
            return;
        }

        // 영어, 숫자 외 문자 체크
        bool hasInvalidChar = false;
        foreach (char c in nickname)
        {
            if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_'))
            {
                hasInvalidChar = true;
                break;
            }
        }

        if (hasInvalidChar)
        {
            // 유효성 검사 실패 시: 버튼 쿨다운 재시작(추가 방지), 경고 애니메이션
            StartCoroutine(ButtonCooldown());

            nicknameInput.text = "";
            nicknameInput.transform.DOShakePosition(0.5f, strength: 10f, vibrato: 20, randomness: 90f);
            text_WarningInfo.SetActive(true);
            DOVirtual.DelayedCall(3f, () => {
                text_WarningInfo.SetActive(false);
            });
            return;
        }

        // 정상 처리: 닉네임 설정 및 패널 닫기
        matchManager.MyNickname = nickname;
        SetNickName(matchManager.MyNickname);
        scriptPlayerCharacter.bCanControl = true;
        OpenInputnickNamePanel_Vertical(image_UpperArea.rectTransform, image_LowerArea.rectTransform, 0.2f);
    }

    private IEnumerator ButtonCooldown()
    {
        isButtonCooldown = true;
        yield return new WaitForSeconds(2f);
        isButtonCooldown = false;

        bOtherPanel = false;
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

        PlaySFX(sfxClips[3]);
    }

    public void OpenInputnickNamePanel_Vertical(RectTransform topImage, RectTransform bottomImage, float fDuration, bool bSound = true)
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

        // bSound가 true일 때만 소리 재생
        if (bSound) PlaySFX(sfxClips[3]);
    }







    public void StartMatching()
    {
        if (bOtherPanel) return;

        bOtherPanel = true;

        // 1. 패널 변경을 먼저 실행 (여기서 내부적으로 bCanControl = true가 됨)
        ChangePanel(0, false);

        // 2. 그 다음 확실하게 다시 잠금
        scriptPlayerCharacter.bCanControl = false;
        scriptPlayerCharacter.GetComponent<Rigidbody>().isKinematic = true;

        // ... 나머지 로직 ...
        matchManager.logText.text = "";
        CloseInputnickNamePanel_Vertical(iamge_UpperAreaMatching.rectTransform, iamge_LowerAreaMatching.rectTransform, 0.2f);

        panels[1].SetActive(false); // Player UI 비활성화

        DOVirtual.DelayedCall(0.8f, () =>
        { 
            OnSpinImage(image_LoadingSpiner);

            matchManager.OnMatchButtonClicked();
            SetNickName(matchManager.MyNickname);
        });
    }




    public void OnClickStopMatchingButton()
    {

        if (isButtonCooldown) return;
        StartCoroutine(ButtonCooldown());

        PlaySFX(sfxClips[1]);
   

        if (stopMatchingButtonRect == null)
        {
            Debug.LogError("매칭 취소 버튼의 RectTransform이 연결되지 않았습니다.");
            ExecuteStopMatchingLogic();
            return;
        }

        // 버튼 애니메이션 실행 후, 완료되면 실제 로직 함수를 실행하도록 콜백 전달
        PlaySimpleAnimation(stopMatchingButtonRect, () =>
        {
            ExecuteStopMatchingLogic();
        });
    }

    public void ExecuteStopMatchingLogic()
    {
        // 1. 매칭 취소 신호 및 플레이어 리셋 (애니메이션 전에 수행)
        matchManager.OnCancelMatchButtonClicked();
        scriptPlayerCharacter.transform.position = transformCenter.position;

        // Rigidbody를 Kinematic에서 해제하여 물리 동작을 복원
        scriptPlayerCharacter.GetComponent<Rigidbody>().isKinematic = false;

        OffSpinImage(image_LoadingSpiner, () =>
        {

            float delayBeforePanelOpen = 0.5f;

            DOVirtual.DelayedCall(delayBeforePanelOpen, () =>
            {

                // 2. 매칭 로딩 UI 닫기 애니메이션 시작 (패널 열림 - 0.3s)
                OpenInputnickNamePanel_Vertical(iamge_UpperAreaMatching.rectTransform, iamge_LowerAreaMatching.rectTransform, 0.3f);

                // 3. Player UI 활성화
                panels[1].SetActive(true);

                // 4. UI 닫힘 애니메이션이 끝날 즈음에 캐릭터 조작을 허용
                //    (0.3초 + 0.2초 지연 후)
                DOVirtual.DelayedCall(0.5f, () => {
                    scriptPlayerCharacter.bCanControl = true;
                });
            });
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


    public void SetNickName(string sName)
    {
        scriptPlayerCharacter.text_nickname.text = sName;
    }


    public void PlaySimpleAnimation(RectTransform targetRect, System.Action onCompleteCallback = null)
    {
        if (targetRect == null) return;

        // 현재 크기를 저장 (원래 크기 역할)
        Vector3 originalScale = targetRect.localScale;

        // 이전에 실행 중이던 DOTween 애니메이션 중지 (클릭 시마다 새로운 애니메이션이 덮어쓰도록)
        targetRect.DOKill();

        // 시퀀스 생성 및 애니메이션 정의
        Sequence seq = DOTween.Sequence();

        seq
            .Append(targetRect.DOScale(originalScale * 0.9f, 0.04f)) // 1. 축소
            .Append(targetRect.DOScale(originalScale * 1.2f, 0.1f))  // 2. 확대
            .Append(targetRect.DOScale(originalScale, 0.1f))        // 3. 복귀
            .OnComplete(() => {
                // 혹시 모를 오차 방지를 위해 원래 크기로 강제 설정
                targetRect.localScale = originalScale;

                // 전달받은 콜백 함수 실행
                onCompleteCallback?.Invoke();
            });
    }



    public void MatchSuccessfEffect()
    {
        buttonCancel.interactable = false; 
        OffSpinImage(image_LoadingSpiner); 
        for (int i = 0; i < successEffectObjs.Length; ++i) OffSpinGameObject(successEffectObjs[i]); 
    }


    private void OnSpinImage(Image targetImage, float fSpeedLap = 1f)
    {
        if (targetImage == null) return;

        RectTransform targetRect = targetImage.rectTransform;
        Vector2 targetSize = new Vector2(125f, 125f);

        // 이전 애니메이션 중지 및 초기 상태 설정
        targetRect.DOKill();

        currentSpinTween?.Kill();

        targetRect.sizeDelta = Vector2.zero;
        targetImage.gameObject.SetActive(true);

        currentSpinTween = targetRect.DORotate(new Vector3(0, 0, 360), 4f, RotateMode.FastBeyond360)
                                     .SetEase(Ease.Linear)
                                     .SetLoops(-1, LoopType.Incremental);

        DOTween.Sequence()
            // 1단계: 목표 크기의 1.2배까지 빠르게 확장
            .Append(targetRect.DOSizeDelta(targetSize * 1.2f, 0.2f * fSpeedLap).SetEase(Ease.OutQuad))

            // 2단계: 최종 목표 크기(300x300)로 되돌아와 정착
            .Append(targetRect.DOSizeDelta(targetSize, 0.1f * fSpeedLap).SetEase(Ease.InQuad))

            .OnComplete(() =>
            {
                // 애니메이션 완료 후 크기 오차 방지
                targetRect.sizeDelta = targetSize;
            });
    }

    private void OffSpinImage(Image targetImage, System.Action onCompleteCallback = null, float fSpeedLap = 1f)
    {
        if (targetImage == null) return;

        RectTransform targetRect = targetImage.rectTransform;
        Vector2 currentSize = targetRect.sizeDelta;

        targetRect.DOKill();
        currentSpinTween?.Kill();

        // fSpeedLap을 사용하여 애니메이션 시간을 0으로 설정
        float duration1 = 0.1f * fSpeedLap;
        float duration2 = 0.2f * fSpeedLap;

        // duration이 0이면 즉시 완료
        DOTween.Sequence()
            .Append(targetRect.DOSizeDelta(currentSize * 1.2f, duration1).SetEase(Ease.OutQuad))
            .Append(targetRect.DOSizeDelta(Vector2.zero, duration2).SetEase(Ease.InQuad))

            .OnComplete(() =>
            {
                // 애니메이션 완료 후 최종 정리
                targetRect.DOKill();
                currentSpinTween?.Kill();

                targetImage.gameObject.SetActive(false);
                targetRect.sizeDelta = Vector2.zero;
                targetRect.localRotation = Quaternion.identity;

                onCompleteCallback?.Invoke();
            });
    }

    private void OffSpinGameObject(GameObject targetObject, System.Action onCompleteCallback = null)
    {
        if (targetObject == null) return;

        RectTransform targetRect = targetObject.GetComponent<RectTransform>();
        if (targetRect == null)
        {
            Debug.LogError("대상 GameObject에 RectTransform 컴포넌트가 없습니다.");
            targetObject.SetActive(false);
            onCompleteCallback?.Invoke();
            return;
        }

        // 현재 실행 중인 DOTween 애니메이션 중지 (스피너 Tween은 이 함수에서 Kill하지 않음 - 필요한 경우 외부에서 처리)
        targetRect.DOKill();

        Vector3 currentScale = targetRect.localScale;

        DOTween.Sequence()
            // 1. 살짝 확대 (OffSpinImage와 유사한 효과)
            .Append(targetRect.DOScale(currentScale * 1.2f, 0.1f).SetEase(Ease.OutQuad))

            // 2. 0으로 축소
            .Append(targetRect.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad))

            .OnComplete(() =>
            {
                targetRect.DOKill();
                targetObject.SetActive(false);

                targetRect.localScale = currentScale;

                onCompleteCallback?.Invoke();
            });
    }





    public void SetCancelButtonVisibilityAndInteractable(bool isVisible, bool isInteractable)
    {
        if (buttonCancel == null) return;

        // 버튼의 GameObject 활성화/비활성화 (눈에 보이게/안 보이게)
        buttonCancel.gameObject.SetActive(isVisible);

        // 버튼의 상호작용 가능 여부 설정
        buttonCancel.interactable = isInteractable;
    }


    public void PlaySFX(AudioClip clip)
    {
        if (!clip || !sfxSource) return;
        sfxSource.PlayOneShot(clip);
    }


    public void PlayBGM(AudioClip clip)
    {
        if (!bgmSource) return;

        // clip이 있는데 이미 재생 중인 것과 같으면 리턴
        if (clip != null && bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.DOKill();

        float targetVolume = bgmSource.volume > 0.01f ? bgmSource.volume : 1f;

        Sequence bgmSeq = DOTween.Sequence();

        // 1. [Fade Out] 현재 재생 중이면 줄임
        if (bgmSource.isPlaying)
        {
            bgmSeq.Append(bgmSource.DOFade(0f, 1.0f).SetEase(Ease.Linear));
        }

        // 2. [Callback] 정지 및 교체
        bgmSeq.AppendCallback(() =>
        {
            bgmSource.Stop();

            if (clip != null)
            {
                bgmSource.clip = clip;
                bgmSource.loop = true;
                bgmSource.volume = 0f;
                bgmSource.Play();
            }
            else
            {
                bgmSource.clip = null; // clip이 null이면 비우고 끝 (재생 안 함)
            }
        });

        // 3. [Fade In] 새 음악이 있을 때만 볼륨 복구
        if (clip != null)
        {
            bgmSeq.Append(bgmSource.DOFade(targetVolume, 1.0f).SetEase(Ease.Linear));
        }
    }




}