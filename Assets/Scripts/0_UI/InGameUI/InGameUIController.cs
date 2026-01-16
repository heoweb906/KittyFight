using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;
using TMPro;

public class InGameUIController : MonoBehaviour
{
    public static InGameUIController Instance { get; private set; }

    public Canvas canvasMain;
    public GameManager gameManager;

    [Header("Player1 UI")]
    public PlayerHealthHexUI hpUI_Player1;
    public SkillCooldownHexUI skillUI_Player1;     // Player1 - Melee
    public SkillCooldownHexUI skillUI2_Player1;    // Player1 - Ranged
    public SkillCooldownHexUI skillUI3_Player1;    // Player1 - Dash
    public SkillCooldownHexUI skillUI4_Player1;    // Player1 - Skill1
    public SkillCooldownHexUI skillUI5_Player1;    // Player1 - Skill2
    public SkillEffectAnimation effectPlayer1;
    public PassiveSlotUI passiveUI1_Player1; // 슬롯 0
    public PassiveSlotUI passiveUI2_Player1; // 슬롯 1
    public PassiveSlotUI passiveUI3_Player1; // 슬롯 2



    public Image image_UpperArea;       // 기본 가림막 (위)   
    public Image image_LowerArea;       // 기본 가림막 (아래) 
    // ingameUIController.CloseFadePanel_Vertical(ingameUIController.image_UpperArea.rectTransform, ingameUIController.image_LowerArea.rectTransform, 0f);   
    // ingameUIController.OpenFadePanel_Vertical(ingameUIController.image_UpperArea.rectTransform, ingameUIController.image_LowerArea.rectTransform, 0.5f);  

    public Image image_ReadyStart;
    public Sprite[] sprites_ReadyStart;


    [Header("Player2 UI")]
    public PlayerHealthHexUI hpUI_Player2;
    public SkillCooldownHexUI skillUI_Player2;     // Player2 - Melee
    public SkillCooldownHexUI skillUI2_Player2;    // Player2 - Ranged
    public SkillCooldownHexUI skillUI3_Player2;    // Player2 - Dash
    public SkillCooldownHexUI skillUI4_Player2;    // Player2 - Skill1 
    public SkillCooldownHexUI skillUI5_Player2;    // Player2 - Skill2 
    public SkillEffectAnimation effectPlayer2;
    public PassiveSlotUI passiveUI1_Player2; // 슬롯 0
    public PassiveSlotUI passiveUI2_Player2; // 슬롯 1
    public PassiveSlotUI passiveUI3_Player2; // 슬롯 2

    [Header("Game UI etc")]
    public GameTimer gameTimer;
    [SerializeField] private BlindEggOverlayUI blindEggOverlay;


    [Header("관리하는 UI 컨트롤러들")]
    public SkillCardController skillCardController;
    public ScoreBoardUIController scoreBoardUIController;
    public MapBoardController mapBoardController;
    public FinalEndingController finalEndingController;

    [Header("연출용")]
    public Image image_FadeOut_White;
    public bool bFinalEndingStart;

    public GameObject text_disconnectWarning;       // 연결 끊겼을 때 나옴 
    public TMP_Text tmp_disconnectWarning;          // 연결 끊겼을 때 나오는 텍스트

    [Header("사운드")]
    [SerializeField] private AudioSource sfxSource;
    public AudioClip[] sfxClips_InGameSystem;
    public AudioClip[] sfxClips_MapGimicSound;
    public AudioClip[] sfxClips_UI;

    [SerializeField] private AudioSource bgmSource;
    public AudioClip[] bgmClips;




    [Header("설정창 관련")]
    public GameObject obj_Gausian;
    public GameObject obj_PlayerPanel;
    public GameObject[] panelMenu;
    public int iPanelNum;    // 패널 번호
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(this.gameObject);

        //canvasMain = FindObjectOfType<Canvas>();
        //if (canvasMain == null) return;

        skillCardController.Initialize(this, canvasMain.transform);

        scoreBoardUIController = this.GetComponent<ScoreBoardUIController>();
        scoreBoardUIController.Initialize(this, canvasMain.transform);
        mapBoardController.Initialize(this, canvasMain.transform);
        finalEndingController.Initialize(this, canvasMain.transform);

        bFinalEndingStart = false;

        obj_Gausian.SetActive(false);
        iPanelNum = 0;

        Cursor.lockState = CursorLockMode.Confined;

        DOTween.useSafeMode = true;

        PlayBGM(bgmClips[0]);
    }

    private void Update()
    {
        if (!bFinalEndingStart)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (iPanelNum == 0 && !gameManager.gameEnded)
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

    private void FixedUpdate()
    {
        HPBarUpdate();
    }
    public void HPBarUpdate()
    {
        float randomValue = Random.Range(0.3f, 0.5f);

        if (hpUI_Player1 == null || hpUI_Player1.fillImage == null) return;
        if (hpUI_Player2 == null || hpUI_Player2.fillImage == null) return;

        if (gameManager.playerAbility_1 == null || gameManager.playerAbility_1.Health == null) return;
        if (gameManager.playerAbility_2 == null || gameManager.playerAbility_2.Health == null) return;

        // Player1
        int cur = gameManager.playerAbility_1.Health.CurrentHP;
        int max = gameManager.playerAbility_1.Health.MaxHP;
        float t = (max > 0) ? Mathf.Clamp01((float)cur / max) : 0f;

        hpUI_Player1.fillImage.fillAmount = randomValue;
        hpUI_Player1.fillImage.fillAmount = t;

        // Player2
        int cur_2 = gameManager.playerAbility_2.Health.CurrentHP;
        int max_2 = gameManager.playerAbility_2.Health.MaxHP;
        float t_2 = (max_2 > 0) ? Mathf.Clamp01((float)cur_2 / max_2) : 0f;

        hpUI_Player2.fillImage.fillAmount = randomValue;
        hpUI_Player2.fillImage.fillAmount = t_2;
    }




    public void ChangePanel(int _iPanelIdx)
    {
        if (panelMenu == null) return;
        iPanelNum = _iPanelIdx;

        for (int i = 1; i < panelMenu.Length; i++)
        {
            if (panelMenu[i] == null) continue;
            bool isActive = (iPanelNum > 0) && (i == iPanelNum);
            panelMenu[i].SetActive(isActive);
        }

        if (obj_Gausian != null)
        {
            if (_iPanelIdx == 1)
            {
                Cursor.lockState = CursorLockMode.None;


                obj_Gausian.SetActive(true);
                obj_PlayerPanel.SetActive(false);
            }
            else if (_iPanelIdx == 0)
            {
                Cursor.lockState = CursorLockMode.Confined;

                obj_Gausian.SetActive(false);
                obj_PlayerPanel.SetActive(true);
 
            }
        }
    }



    public void StartGameTimer(float duration)
    {
        gameTimer?.SetDuration(duration);

        
    }

    public void TickGameTimer()
    {
        if (gameTimer != null && gameTimer.Tick(Time.deltaTime))
        {
            // GameObject.FindObjectOfType<GameManager>()?.EndGame();
            FindObjectOfType<GameManager>()?.EndByTimer();
        }
    }
    public void ShowBlindEggs(float duration, int eggCount = 3)
    {
        blindEggOverlay?.Play(duration, eggCount);
    }

    // 게임 시작 시 각 UI 위젯에 abilityRef/slot 할당 (GameManager에서 호출)
    public void WireSkillUIs(PlayerAbility player1Ability, PlayerAbility player2Ability)
    {
        // Player1 (왼쪽)
        hpUI_Player1?.Bind(player1Ability);
        skillUI_Player1?.Bind(player1Ability, SkillType.Melee);
        skillUI2_Player1?.Bind(player1Ability, SkillType.Ranged);
        skillUI3_Player1?.Bind(player1Ability, SkillType.Dash);
        skillUI4_Player1?.Bind(player1Ability, SkillType.Skill1);
        skillUI5_Player1?.Bind(player1Ability, SkillType.Skill2);
        passiveUI1_Player1?.Bind(player1Ability, 0);
        passiveUI2_Player1?.Bind(player1Ability, 1);
        passiveUI3_Player1?.Bind(player1Ability, 2);

        // Player2 (오른쪽)
        hpUI_Player2?.Bind(player2Ability);
        skillUI_Player2?.Bind(player2Ability, SkillType.Melee);
        skillUI2_Player2?.Bind(player2Ability, SkillType.Ranged);
        skillUI3_Player2?.Bind(player2Ability, SkillType.Dash);
        skillUI4_Player2?.Bind(player2Ability, SkillType.Skill1);
        skillUI5_Player2?.Bind(player2Ability, SkillType.Skill2);
        passiveUI1_Player2?.Bind(player2Ability, 0);
        passiveUI2_Player2?.Bind(player2Ability, 1);
        passiveUI3_Player2?.Bind(player2Ability, 2);


        if (player1Ability != null) player1Ability.effect = effectPlayer1;
        if (player2Ability != null) player2Ability.effect = effectPlayer2;
    }






    // #. 허재승이 추가한 함수들
    public void ComeToTheEndGame(int winnerPlayerNum)
    {
        if (iPanelNum != 0)
        {
            ChangePanel(0);
        }

        scoreBoardUIController.CloseScorePanel(winnerPlayerNum, winnerPlayerNum == 1 ? ++gameManager.IntScorePlayer_1 : ++gameManager.IntScorePlayer_2 , true);
        scoreBoardUIController.OnOffCheering(true);

        StartCoroutine(OpenScorePanelAfterDelay(winnerPlayerNum));
    }


    private IEnumerator OpenScorePanelAfterDelay(int winnerPlayerNum)
    {
        yield return new WaitForSeconds(1.2f);

        ChangeReadyStartSprite(0);
        int winPlayerCurrentScore = winnerPlayerNum == 1 ? gameManager.IntScorePlayer_1 : gameManager.IntScorePlayer_2;
        int iLosePlayerNum = winnerPlayerNum == 1 ? 2 : 1;



        if (gameManager.IntScorePlayer_1 >= 11 || gameManager.IntScorePlayer_2 >= 11)
        {
            // [추가] BGM 끄기 (Fade Out)
            PlayBGM(null);

            yield return new WaitForSeconds(1.2f);
            int winnerNum = (gameManager.IntScorePlayer_1 >= 11) ? 1 : 2;
            finalEndingController.ShowFinalEnding(winnerNum);
            yield break;
        }

        // 스킬 획득 부분
        if (winPlayerCurrentScore % 2 == 0)     
        { 
            MovePlayerImageToCenter(iLosePlayerNum);
            yield break; 
        } 
        
       
        gameManager.ResetGame();
     
        yield return new WaitForSeconds(0.6f);    
 
        // scoreBoardUIController.OpenScorePanel();  
    }


    // #. 패배한 플레이어를 화면 중앙으로 오도록 배치하는 함수
    private void MovePlayerImageToCenter(int iLosePlayerNum)
    {
        scoreBoardUIController.OnOffCheering(false);

        C_ScoreImageElement targetPlayer = iLosePlayerNum == 1 ?
            scoreBoardUIController.scoreImageElement_Player1 :
            scoreBoardUIController.scoreImageElement_Player2;

        Vector2 targetPlayerImagePos = targetPlayer.rectTransform_PlayerImage.anchoredPosition;
        Vector2 targetBackgroundPos = targetPlayer.rectTransform_BackGround.anchoredPosition;

        float playerImageWorldPosX = targetBackgroundPos.x + targetPlayerImagePos.x;
        float offsetX = -playerImageWorldPosX;

        scoreBoardUIController.scoreImageElement_Player1.rectTransform_BackGround.DOAnchorPosX(
     scoreBoardUIController.scoreImageElement_Player1.rectTransform_BackGround.anchoredPosition.x + offsetX, 0.7f)
     .SetEase(Ease.OutQuart);
        scoreBoardUIController.scoreImageElement_Player2.rectTransform_BackGround.DOAnchorPosX(
           scoreBoardUIController.scoreImageElement_Player2.rectTransform_BackGround.anchoredPosition.x + offsetX, 0.7f)
           .SetEase(Ease.OutQuart)
                   .OnComplete(() =>
                   {
                       DOVirtual.DelayedCall(1.5f, () =>
                       {
                          
                           int iWinnerScore = iLosePlayerNum == 1 ? gameManager.IntScorePlayer_2 : gameManager.IntScorePlayer_1;
                           bool bActivePassive = (iWinnerScore == 2 || iWinnerScore == 6);

                           // 패배한 플레이어만 권한을 가지고 스킬카드 생성하도록 수정
                           if (iLosePlayerNum == MatchResultStore.myPlayerNumber)
                           {
                               skillCardController.ShowSkillCardList(iLosePlayerNum, bActivePassive);
                           }
                           else
                           {
                               skillCardController.iAuthorityPlayerNum = iLosePlayerNum; 
                           }
                       });
                   });
    }



    public void ChangeReadyStartSprite(int iIdx)
    {
        // 0 = Ready;
        // 1 = Start;

        GameObject targetGO = image_ReadyStart.gameObject;
        targetGO.SetActive(true);

        image_ReadyStart.sprite = sprites_ReadyStart[iIdx];
    }


    public void PlayStartPriteAnimation(RectTransform targetRect)
    {
        if (targetRect == null) return;

        // 현재 크기를 저장 (원래 크기 역할)
        Vector3 originalScale = targetRect.localScale;

        // 이전에 실행 중이던 DOTween 애니메이션 중지
        targetRect.DOKill();

        // 트윈 동작을 순서대로 연결하여 애니메이션 흐름을 만듭니다.
        DOTween.Sequence()
            .Append(targetRect.DOScale(originalScale * 0.9f, 0.04f)) // 첫 번째 동작: 축소
            .Append(targetRect.DOScale(originalScale * 1.35f, 0.2f))  // 두 번째 동작: 확대
            .Append(targetRect.DOScale(0f, 0.15f))        // 세 번째 동작: 크기 0으로 사라짐
            .OnComplete(() => {

                targetRect.gameObject.SetActive(false);
                targetRect.localScale = originalScale;
            }).SetLink(targetRect.gameObject, LinkBehaviour.KillOnDisable);
    }

    public void CloseFadePanel_Vertical(RectTransform topImage, RectTransform bottomImage, float fDuration)
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

    public void OpenFadePanel_Vertical(RectTransform topImage, RectTransform bottomImage, float fDuration)
    {
        RectTransform canvasRect = canvasMain.GetComponent<RectTransform>();
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