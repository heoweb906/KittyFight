using DG.Tweening;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UserData
{
    public bool isFirstRun = true;
}

public class FirstRunManager : MonoBehaviour
{
    public TitleLogoAssist titleLogoAssist;
    [SerializeField] private MainMenuController mainMenuController;

    private string saveFilePath;

    [Header("연출에 사용할 것들")]
    public GameObject obj_BackBoard;
    public GameObject obj_Plate;
    public Image image_Plate;
    public Sprite[] sprites_Plate;
    public Image sprite_QuestionMark;
    public TMP_Text text_Description;

    [Header("Plate 이동 설정")]
    public float plateMoveSpeed = 2f;
    private bool bIsPlateMoving = false;

    // [수정] 떨리기 전 원래 위치를 저장할 변수
    private Vector2 originalPlatePos;



    [Header("사운드")]
    public AudioClip[] audioclips_Dialogue;




    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "userdata.json");
    }

    private void Start()
    {
        // PlayOpeningCutscene();

        CheckFirstRun();
    }






    private void CheckFirstRun()
    {
        UserData data = LoadUserData();

        if (data.isFirstRun)
        {
            PlayOpeningCutscene();

            data.isFirstRun = false;
            SaveUserData(data);
        }
        else
        {
            Debug.Log("[System] 기존 유저. 바로 로비로 진입합니다.");
            EnterLobby();
        }
    }

    private void SaveUserData(UserData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    private UserData LoadUserData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<UserData>(json);
        }
        else
        {
            return new UserData();
        }
    }

    // --- 연출 관련 메서드 ---


    private void PlayOpeningCutscene()
    {
        Debug.Log("[System] 오프닝 컷씬 시작");
        WriteDescription("");

        Vector3 originalPlateScale = Vector3.one;

        // [안전장치] 시작 시점 위치/크기 저장
        if (image_Plate != null)
        {
            originalPlatePos = image_Plate.rectTransform.anchoredPosition;
            originalPlateScale = image_Plate.rectTransform.localScale;
        }

        // --- [T+0.0] : 초기 설정 ---
        if (obj_BackBoard != null) obj_BackBoard.SetActive(true);
        bIsPlateMoving = true;

        if (titleLogoAssist != null) titleLogoAssist.MoveToTarget(0);

        // ★ [핵심 수정] Sequence를 사용하여 "등속 운동" -> "마지막 감속" 연결
        if (image_Plate != null)
        {
            image_Plate.rectTransform.localRotation = Quaternion.identity;

            Sequence rotateSeq = DOTween.Sequence();

            // 1단계: 15초 동안 10바퀴를 '일정한 속도(Linear)'로 맹렬히 돕니다.
            // (15초, -3600도)
            rotateSeq.Append(image_Plate.rectTransform
                .DORotate(new Vector3(0, 0, -360 * 3), 15.0f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear));


            rotateSeq.Append(image_Plate.rectTransform
                .DORotate(new Vector3(0, 0, -360 * 1f), 5.0f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad));
        }

        // --- [T+1.5] : 문 열기 ---
        DOVirtual.DelayedCall(1.5f, () =>
        {
            if (titleLogoAssist != null) titleLogoAssist.MoveToStart(1.5f);
        });

        // --- [T+3.0] : 백보드 이동 (4초) ---
        float backBoardMoveTime = 4.0f;
        DOVirtual.DelayedCall(3.0f, () =>
        {
            if (obj_BackBoard != null)
            {
                obj_BackBoard.transform.DOLocalMoveY(-1300f, backBoardMoveTime).SetEase(Ease.InOutSine);
            }
        });


        mainMenuController.PlayBGM(mainMenuController.bgmClips[0]);


        // --- 연출 스케줄링 ---
        float currentTime = 4.0f + backBoardMoveTime; // 7.0초

        DOVirtual.DelayedCall(currentTime, () =>
        {
            WriteDescription("On the first day of each year, a guardian is chosen to protect the year");
            mainMenuController.PlaySFX(audioclips_Dialogue[0]);
        });
        currentTime += 2.75f;
        DOVirtual.DelayedCall(currentTime, () => WriteDescription(""));
        currentTime += 0.75f;


        DOVirtual.DelayedCall(currentTime, () =>
        {
            WriteDescription("We call them the [Zodiacs]");
            mainMenuController.PlaySFX(audioclips_Dialogue[1]);
        });
        currentTime += 2f;
        DOVirtual.DelayedCall(currentTime, () => WriteDescription(""));
        currentTime += 0.75f;


        DOVirtual.DelayedCall(currentTime, () =>
        {
            WriteDescription("The Zodiac consisted of twelve animals");
            mainMenuController.PlaySFX(audioclips_Dialogue[2]);
        });
        currentTime += 2f;
        DOVirtual.DelayedCall(currentTime, () => WriteDescription(""));
        currentTime += 0.75f;


        DOVirtual.DelayedCall(currentTime, () =>
        {
            WriteDescription("Until now...");
            mainMenuController.PlaySFX(audioclips_Dialogue[3]);
        });
        currentTime += 1.5f;
        DOVirtual.DelayedCall(currentTime, () => WriteDescription(""));
        currentTime += 1f; // 여기서 총 시간 약 18.5초 지점 (회전 종료 직후)


        // --- [Plate 낙하 연출] ---
        float dration_1 = 3f;
        DOVirtual.DelayedCall(currentTime, () =>
        {
            if (image_Plate != null)
            {
                originalPlatePos = image_Plate.rectTransform.anchoredPosition;
                float startY = originalPlatePos.y;
                float targetY = -5940f;

                image_Plate.rectTransform.DOScale(1.0f, dration_1).SetEase(Ease.OutQuad);

                DOVirtual.Float(startY, targetY, dration_1 - 0.5f, (currentY) =>
                {
                    if (image_Plate != null)
                    {
                        float shakeX = UnityEngine.Random.Range(-5f, 5f);
                        float shakeY = UnityEngine.Random.Range(-5f, 5f);
                        image_Plate.rectTransform.anchoredPosition = new Vector2(originalPlatePos.x + shakeX, currentY + shakeY);
                    }
                })
                .SetEase(Ease.InOutSine)
                .SetId("PlateShake");
            }
        });


        // [T+17.0 + @] 고양이 등장 (떨림 종료)
        currentTime += dration_1;
        DOVirtual.DelayedCall(currentTime, () =>
        {
            DOTween.Kill("PlateShake");

            if (image_Plate != null)
            {
                image_Plate.rectTransform.anchoredPosition = new Vector2(originalPlatePos.x, -5940f);
            }

            if (sprites_Plate.Length > 1) image_Plate.sprite = sprites_Plate[1];
            sprite_QuestionMark.gameObject.SetActive(false);

            Sequence popSeq = DOTween.Sequence();
            popSeq.Append(image_Plate.rectTransform.DOScale(1.2f, 0.1f).SetEase(Ease.OutQuad));
            popSeq.Append(image_Plate.rectTransform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack));
        });


        currentTime += 1f;


        DOVirtual.DelayedCall(currentTime, () =>
        {
            WriteDescription("This year, it has been decided that the Cat will join as the 13th Zodiac!");
            mainMenuController.PlaySFX(audioclips_Dialogue[4]);
        });
        currentTime += 2.75f;


        DOVirtual.DelayedCall(currentTime, () => WriteDescription(""));


        // --- [복귀 애니메이션] ---
        float duration_2 = 2.0f;
        currentTime += 1.0f;

        DOVirtual.DelayedCall(currentTime, () =>
        {
            if (image_Plate != null)
            {
                image_Plate.rectTransform.DOAnchorPos(originalPlatePos, duration_2).SetEase(Ease.InOutSine);
                image_Plate.rectTransform.DOScale(originalPlateScale, duration_2).SetEase(Ease.InOutSine);
            }
        });


        // --- [마지막 단계] ---
        currentTime += duration_2;

        DOVirtual.DelayedCall(currentTime, () =>
        {
            WriteDescription("On the first day of each year, a guardian is chosen to protect the year");
            mainMenuController.PlaySFX(audioclips_Dialogue[5]);

            DOVirtual.DelayedCall(2.0f, () =>
            {
                titleLogoAssist.MoveToTarget(0.85f);
                DOVirtual.DelayedCall(2.0f, EnterLobby);
            });
        });
    }




    private void EnterLobby()
    {
        Debug.Log(">>> 로비 진입 <<<");

        obj_BackBoard.SetActive(false);

        if (titleLogoAssist != null)
        {
            mainMenuController.PlayBGM(mainMenuController.bgmClips[1]);
            titleLogoAssist.StartStep1();
        }
    }

    private void WriteDescription(string _text)
    {
        if (text_Description != null)
        {
            text_Description.text = _text;
        }
    }

}