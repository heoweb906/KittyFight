using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalEndingController : MonoBehaviour
{
    public InGameUIController InGameUiController { get; set; }

    public GameObject obj_FinalPanel;


    [Header("사용하는 요소들")]
    public GameObject objPlate;
    public GameObject obj_CatRedFigure;
    public GameObject obj_CatBlueFigure;

    public GameObject obj_WinnnerCatImage;
    public GameObject obj_CatRedWinnerImage;
    public GameObject obj_CatBlueWinnerImage;

    public GameObject obj_TextArray;
    public TMP_Text text_PlayerName;



    [Header("UI 발사 설정")]
    public RectTransform obj_Particle;
    public RectTransform[] spawnPoints; // [수정] 배열로 변경
    public RectTransform particleParent;

    [Header("부채꼴 및 이동 설정")]
    public int particleCount = 7;
    [Range(0f, 180f)]
    public float arcAngle = 120f;
    public float launchSpeed = 500f;
    public float launchSpeedRandomness = 200f; // [추가] 랜덤 속도 범위
    public float dropDuration = 1.5f;
    public float jumpHeight = 300f;
    public float jumpHeightRandomness = 150f; // [추가] 랜덤 속도 범위
    public float dropYOffset;
    public float dropYOffsetRandomness; // [추가] 랜덤 속도 범위

    [Header("회전 연출")]
    public float rotationSpeed = 360f;

    [Header("색상 설정")]
    [Tooltip("여기에 등록된 색상 중 하나가 랜덤으로 Image_IN에 적용됩니다.")]
    public Color[] particleColors;



    [Header("FadeInOut")]
    public Image iamge_Left;
    public Image iamge_Right;
    public RectTransform startPoint1;
    public RectTransform startPoint2;
    public RectTransform targetPoint;
    private RectTransform image1Rect;
    private RectTransform image2Rect;


    public void Initialize(InGameUIController temp, Transform parent)
    {
        InGameUiController = temp;
    }


    private void Awake()
    {
        obj_FinalPanel.SetActive(false);
        obj_CatRedFigure.SetActive(false);
        obj_CatBlueFigure.SetActive(false);

        obj_WinnnerCatImage.SetActive(false);
        obj_CatRedWinnerImage.SetActive(false);
        obj_CatBlueWinnerImage.SetActive(false);

        obj_TextArray.SetActive(false);

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = new RectTransform[] { GetComponent<RectTransform>() };
        }

        if (particleParent == null && spawnPoints.Length > 0)
            particleParent = spawnPoints[0].parent as RectTransform;

        if (iamge_Left != null) image1Rect = iamge_Left.GetComponent<RectTransform>();
        if (iamge_Right != null) image2Rect = iamge_Right.GetComponent<RectTransform>();
    }


    public void ShowFinalEnding(int _iWinnerPlayerNum)
    {
        InGameUiController.bFinalEndingStart = true;

        InGameUiController.PlayBGM(InGameUiController.bgmClips[3]);


        // [추가] 승리한 플레이어 번호와 내 번호를 비교하여 닉네임 설정
        if (text_PlayerName != null)
        {
            if (_iWinnerPlayerNum == MatchResultStore.myPlayerNumber)
                text_PlayerName.text = MatchResultStore.myNickname;
            else
                text_PlayerName.text = MatchResultStore.opponentNickname;
        }

        FadePanelOnOff(true, () =>
        {
            obj_FinalPanel.SetActive(true);

            DOVirtual.DelayedCall(1.0f, () =>
            {
                FadePanelOnOff(false, () =>
                {
                    // [수정] 매개변수가 있는 함수는 이렇게 람다식으로 감싸야 합니다.
                    DOVirtual.DelayedCall(0.5f, () => PlayMainSequence(_iWinnerPlayerNum));
                });
            });
        });
    }

    private void PlayMainSequence(int _winnerPlayerNum)
    {
        RectTransform rtPanel = obj_FinalPanel.GetComponent<RectTransform>();
        RectTransform rtPlate = objPlate.GetComponent<RectTransform>();

        bool isRedWinForBg = (_winnerPlayerNum == 1);
        if (obj_CatRedWinnerImage != null) obj_CatRedWinnerImage.SetActive(isRedWinForBg);
        if (obj_CatBlueWinnerImage != null) obj_CatBlueWinnerImage.SetActive(!isRedWinForBg);

        // ---------------------------------------------------------

        Sequence seq = DOTween.Sequence();

        // [순서 변경 1] 애니메이션을 먼저 정의합니다. (0초부터 즉시 시작됨)
        seq.Append(rtPanel.DOAnchorPosY(-4400f, 2.5f).SetEase(Ease.InOutQuad));
        rtPlate.localRotation = Quaternion.Euler(0, 0, 300f);
        seq.Join(rtPlate.DORotate(Vector3.zero, 3.5f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));


        // [순서 변경 2] 소리 예약을 애니메이션 정의 후에 Insert합니다.
        // InsertCallback은 절대 시간(0초 기준)을 사용하므로, 애니메이션 순서에 영향을 주지 않고 끼어듭니다.

        // ▼▼▼▼▼ 시간 조절은 여기서 하세요 ▼▼▼▼▼
        float soundStartDelay = 1f;  // 패널 움직이고 0.8초 뒤에 소리 시작
        float soundInterval = 0.55f;    // 소리 간격
                                       // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        for (int i = 0; i < 3; i++)
        {
            float playTime = soundStartDelay + (i * soundInterval);

            // InsertCallback: 0초를 기준으로 playTime초에 실행 (애니메이션과 병렬 실행됨)
            seq.InsertCallback(playTime, () =>
            {
                if (InGameUiController != null)
                    InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[4]);
            });
        }


        // 2. 대기 (여기서부턴 위 애니메이션(2.5초)이 끝난 뒤에 이어짐)
        seq.AppendInterval(0.8f);

        // 3. 떨림
        seq.Append(rtPlate.DOShakeAnchorPos(2.5f, 4f, 40, 90, false, false).SetEase(Ease.InExpo));

        // 4. 고양이 등장 (떨림 직후 콜백)
        seq.AppendCallback(() =>
        {
            bool isRedWin = (_winnerPlayerNum == 1);
            if (obj_CatRedFigure != null) obj_CatRedFigure.SetActive(isRedWin);
            if (obj_CatBlueFigure != null) obj_CatBlueFigure.SetActive(!isRedWin);
        });

        // 5. 띠요아 (커지는 연출) 직전에 5번 소리 재생
        seq.AppendCallback(() =>
        {
            if (InGameUiController != null)
                InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[5]);
        });

        seq.Append(rtPlate.DOScale(1.02f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad));

        // 6. 위로 이동 (-7700)
        seq.AppendInterval(1.1f);
        seq.Append(rtPanel.DOAnchorPosY(-7700f, 1.0f).SetEase(Ease.InBack));

        // =========================================================
        // 7. 이동 후 1초 대기 -> 팝업 연출
        // =========================================================

        // 7-1. 1초 대기
        seq.AppendInterval(0.6f);

        // 7-2. 활성화 및 크기 0으로 초기화
        seq.AppendCallback(() =>
        {
            if (obj_WinnnerCatImage != null)
            {
                obj_WinnnerCatImage.SetActive(true);
                obj_WinnnerCatImage.transform.localScale = Vector3.zero;
            }

            if (obj_TextArray != null)
            {
                obj_TextArray.SetActive(true);
                obj_TextArray.transform.localScale = Vector3.zero;
            }
        });

        // 7-3. 1단계: 커지기 (Overshoot)
        float popDuration = 0.2f;

        seq.AppendCallback(() => StartFirecracker());

        if (obj_WinnnerCatImage != null)
            seq.Append(obj_WinnnerCatImage.transform.DOScale(11f, popDuration).SetEase(Ease.OutQuad));

        if (obj_TextArray != null)
            seq.Join(obj_TextArray.transform.DOScale(1.25f, popDuration).SetEase(Ease.OutQuad));

        float settleDuration = 0.1f;

        if (obj_WinnnerCatImage != null)
            seq.Append(obj_WinnnerCatImage.transform.DOScale(9.5f, settleDuration).SetEase(Ease.OutQuad));

        if (obj_TextArray != null)
            seq.Join(obj_TextArray.transform.DOScale(1.0f, settleDuration).SetEase(Ease.OutQuad));

        seq.AppendInterval(2.0f);

        // 8-2. 커튼 닫기 및 다음 동작 예약
        seq.AppendCallback(() =>
        {
            FadePanelOnOff_2(true, () =>
            {
                DOVirtual.DelayedCall(2.0f, () =>
                {
                    Debug.Log("모든 엔딩 종료. 로비로 이동!");
                    if (InGameUIController.Instance != null && InGameUIController.Instance.gameManager != null)
                    {
                        InGameUiController.PlayBGM(null);
                        InGameUIController.Instance.gameManager.ReturnToTrainingByDisconnect();
                    }
                });
            });
        });
    }

    public void FadePanelOnOff(bool _bbb, System.Action onComplete = null)
    {
        if (_bbb)
        {
            if (image1Rect != null && targetPoint != null)
            {
                image1Rect.DOAnchorPos(targetPoint.anchoredPosition, 0.85f).SetEase(Ease.InQuint);
                image2Rect.DOAnchorPos(targetPoint.anchoredPosition, 0.85f).SetEase(Ease.InQuint)
                    .OnComplete(() => onComplete?.Invoke());
            }
        }
        else
        {
            // [수정됨] 커튼 열기 로직에도 OnComplete 추가
            if (image1Rect != null && startPoint1 != null)
                image1Rect.DOAnchorPos(startPoint1.anchoredPosition, 0.75f).SetEase(Ease.OutQuint);

            if (image2Rect != null && startPoint2 != null)
                image2Rect.DOAnchorPos(startPoint2.anchoredPosition, 0.75f).SetEase(Ease.OutQuint)
                    .OnComplete(() => onComplete?.Invoke()); // <-- 여기가 핵심, 이거 없으면 다음 동작 안 함
        }
    }

    public void FadePanelOnOff_2(bool _bbb, System.Action onComplete = null)
    {
        if (_bbb)
        {
            if (image1Rect != null && targetPoint != null)
            {
                image1Rect.DOAnchorPos(targetPoint.anchoredPosition, 1.2f).SetEase(Ease.InQuint);
                image2Rect.DOAnchorPos(targetPoint.anchoredPosition, 1.2f).SetEase(Ease.InQuint)
                    .OnComplete(() => onComplete?.Invoke());
            }
        }
        else
        {
            // [수정됨] 커튼 열기 로직에도 OnComplete 추가
            if (image1Rect != null && startPoint1 != null)
                image1Rect.DOAnchorPos(startPoint1.anchoredPosition, 1.2f).SetEase(Ease.OutQuint);

            if (image2Rect != null && startPoint2 != null)
                image2Rect.DOAnchorPos(startPoint2.anchoredPosition, 1.2f).SetEase(Ease.OutQuint)
                    .OnComplete(() => onComplete?.Invoke()); // <-- 여기가 핵심, 이거 없으면 다음 동작 안 함
        }
    }







    public void StartFirecracker()
    {
        if (obj_Particle == null || particleParent == null) return;

        // [수정] 모든 스폰 포인트를 돌면서 발사
        foreach (RectTransform currentSpawnPoint in spawnPoints)
        {
            if (currentSpawnPoint == null) continue;

            SpawnFromPoint(currentSpawnPoint);
        }

        InGameUiController.PlaySFX(InGameUiController.sfxClips_InGameSystem[10]);
    }

    private void SpawnFromPoint(RectTransform point)
    {
        float startAngle = -arcAngle / 2f;
        float angleStep = (particleCount > 1) ? (arcAngle / (particleCount - 1)) : 0;
        Vector3 upDirection = point.up;

        for (int i = 0; i < particleCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, currentAngle);
            Vector3 launchDirection = spreadRotation * upDirection;

            // 1. 파티클 생성
            RectTransform spawnedRT = Instantiate(obj_Particle, particleParent);
            spawnedRT.position = point.position;
            spawnedRT.localRotation = Quaternion.identity;
            spawnedRT.localScale = Vector3.one;

            // 2. 색상 변경 (Image_IN)
            if (particleColors != null && particleColors.Length > 0)
            {
                Transform inImageTransform = spawnedRT.Find("Image_IN");
                if (inImageTransform != null)
                {
                    Image inImage = inImageTransform.GetComponent<Image>();
                    if (inImage != null)
                    {
                        inImage.color = particleColors[Random.Range(0, particleColors.Length)];
                    }
                }
            }

            // 3. 속도 랜덤 계산 (기본 속도 +- 랜덤범위)
            float randomSpeed = launchSpeed + Random.Range(-launchSpeedRandomness, launchSpeedRandomness);

            // 4. 애니메이션 시작 (랜덤 속도 전달)
            AnimateParticle(spawnedRT, launchDirection.normalized, randomSpeed);
        }
    }

    // [수정] speed 파라미터 추가
    private void AnimateParticle(RectTransform targetRT, Vector3 direction, float speed)
    {
        Vector2 startAnchoredPos = targetRT.anchoredPosition;
        float targetPosX = startAnchoredPos.x + (direction.x * speed * dropDuration);

        // [수정됨] 원본 변수(dropYOffset)는 건드리지 않고, 
        // 이번 파티클에만 적용할 임시 변수(currentDropY)를 만들어 사용합니다.
        float currentDropY = dropYOffset - Random.Range(0f, dropYOffsetRandomness);

        float targetPosY = startAnchoredPos.y + currentDropY;

        Sequence seq = DOTween.Sequence();

        seq.Insert(0, targetRT.DOAnchorPosX(targetPosX, dropDuration).SetEase(Ease.Linear));

        float jumpDuration = dropDuration * 0.4f;

        // 점프 높이 랜덤 계산 (여기는 이미 잘 작성하셨습니다)
        float randomJump = jumpHeight + Random.Range(-jumpHeightRandomness, jumpHeightRandomness);

        seq.Insert(0, targetRT.DOAnchorPosY(startAnchoredPos.y + randomJump, jumpDuration).SetEase(Ease.OutQuad));
        seq.Insert(jumpDuration, targetRT.DOAnchorPosY(targetPosY, dropDuration - jumpDuration).SetEase(Ease.InQuad));

        float randomRotation = Random.Range(-rotationSpeed, rotationSpeed) * dropDuration;
        seq.Insert(0, targetRT.DORotate(new Vector3(0, 0, randomRotation), dropDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        seq.OnComplete(() =>
        {
            Destroy(targetRT.gameObject, 1.0f);
        });
    }


}
