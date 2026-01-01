using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 

/// <summary>
/// 맵과 배경의 로딩 및 관리를 모두 전담하는 통합 클래스입니다.
/// </summary>
public class MapManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("맵 관리")]
    [Tooltip("Hierarchy 뷰의 맵 부모 오브젝트들을 순서대로 등록해주세요.")]
    public GameObject[] mapObjects;

    [Header("배경 관리")]
    [Tooltip("배경을 표시할 SpriteRenderer 컴포넌트를 할당하세요.")]
    public SpriteRenderer backgroundSpriteRenderer;
    [Tooltip("사용할 배경 스프라이트 목록")]
    public Sprite[] backgroundSprites;

    private MapLayout currentMapLayout;


    [Header("맵 기믹 관련")]
    // 수정필요
    public Image iamge_TestMapGimicColor;

    [SerializeField] private List<AbstractMapGimic> gimicks;
    [HideInInspector] private AbstractMapGimic currentGimmick;

    private int currentMapGimicIndex = -1;


    [Header("맵 기믹 연출")]
    public Material matScreen; 
    private readonly string propertyName = "_BorderThick";
    private readonly string colorPropName = "_Color";



    void Awake()
    {
        // 게임 시작 시 모든 맵을 일단 꺼둡니다.
        foreach (var map in mapObjects)
        {
            if (map != null)
            {
                map.SetActive(false);
            }
        }
    }


    private void Start()
    {
        for (int i = 0; i < gimicks.Count; ++i) gimicks[i].gameManager = gameManager;
        for (int i = 0; i < gimicks.Count; ++i) gimicks[i].mapManager = this;

        currentGimmick = null;
    }



    private void FixedUpdate()
    {
        if (MatchResultStore.myPlayerNumber == 1 && gameManager.BoolAcitveMapGimic && !gameManager.GetGameEnded() && currentGimmick != null)
        {
            currentGimmick.OnGimmickUpdate();
        }
    }



    public void PlayBorderAnimation(Color finalHitColor, Action onHitCallback = null)
    {
        matScreen.DOKill();
        matScreen.SetFloat(propertyName, 0.5f);
        matScreen.SetColor(colorPropName, Color.white);

        Sequence seq = DOTween.Sequence();

        // 1. [빌드업] 3회 반복
        for (int i = 0; i < 3; i++)
        {
            seq.Append(matScreen.DOFloat(0.47f, propertyName, 0.6f).SetEase(Ease.OutQuad));
            seq.Append(matScreen.DOFloat(0.5f, propertyName, 0.4f).SetEase(Ease.InQuad));
        }

        // 2. [타격 직전] 콜백 실행!
        // 여기서 경고음이나 함수가 실행됩니다.
        seq.AppendCallback(() =>
        {
            if (onHitCallback != null) onHitCallback.Invoke();
        });

        // 3. [피니시] 강하고 빠른 동작
        seq.Append(matScreen.DOFloat(0.45f, propertyName, 0.2f).SetEase(Ease.OutQuad));
        seq.Join(matScreen.DOColor(finalHitColor, colorPropName, 0.2f).SetEase(Ease.OutQuad));

        seq.Append(matScreen.DOFloat(0.5f, propertyName, 0.2f).SetEase(Ease.InQuad));
    }




    /// <summary>
    /// 지정된 인덱스의 맵을 활성화하고 나머지 맵들은 모두 비활성화합니다.
    /// </summary>
    public void ChangeMap(int mapIndex)
    {
        if (mapIndex < 0 || mapIndex >= mapObjects.Length) return;
      

        for (int i = 0; i < mapObjects.Length; i++)
        {
            if (mapObjects[i] == null) continue;
            bool shouldBeActive = (i == mapIndex);
            mapObjects[i].SetActive(shouldBeActive);

            if (shouldBeActive)
            {
                currentMapLayout = mapObjects[i].GetComponent<MapLayout>();
            }
        }
    }

    /// <summary>
    /// 지정된 인덱스의 스프라이트로 배경을 변경합니다.
    /// </summary>
    public void ChangeBackground(int bgIndex)
    {
        if (bgIndex < 0 || bgIndex >= backgroundSprites.Length) return;

        if (backgroundSpriteRenderer != null)
        {
            backgroundSpriteRenderer.sprite = backgroundSprites[bgIndex];

            // 8, 9, 10번 인덱스일 때 스케일 조절
            if (bgIndex >= 8 && bgIndex <= 10)
            {
                backgroundSpriteRenderer.transform.localScale = Vector3.one * 0.9f;
            }
            else
            {
                // 다른 배경으로 바뀔 때 원래 크기로 복구 (필수)
                backgroundSpriteRenderer.transform.localScale = Vector3.one;
            }
        }
    }

    /// <summary>
    /// 현재 활성화된 맵의 스폰 지점을 반환합니다.
    /// </summary>
    public Transform GetSpawnPoint(int playerNumber)
    {
        if (currentMapLayout == null)
        {
            return this.transform;
        }
        return playerNumber == 1 ? currentMapLayout.spawnPoint1 : currentMapLayout.spawnPoint2;
    }




    #region // 맵 기믹 컨트롤 관련

    public void SetMapGimicIndex(int iGimicIndex)
    {
        iGimicIndex--;

        currentMapGimicIndex = iGimicIndex;

        // 인덱스가 유효하면 미리 변수에 넣어둡니다 (실행은 안 함)
        if (iGimicIndex >= 0 && iGimicIndex < gimicks.Count)
        {
            currentGimmick = gimicks[iGimicIndex];
        }
        else
        {
            currentGimmick = null;
        }
    }
    public int GetMapGimicIndex()
    {
        return currentMapGimicIndex;
    }

    public void StartCurrentGimmick()
    {
        // 혹시 모르니 null 체크 후 시작
        if (currentGimmick != null)
        {
            currentGimmick.OnGimicStart();
        }
    }

    public void StopCurrentGimmick()
    {
        if (currentGimmick != null)
        {
            currentGimmick.OnGimicEnd();
            currentGimmick = null;
            currentMapGimicIndex = -1;
        }
    }
    #endregion



    public void ResetScreenEffect()
    {
        matScreen.DOKill(); 
        matScreen.DOFloat(0.5f, propertyName, 1.0f);
    }

    public void SetScreenColor(Color color)
    {
        matScreen.SetColor(colorPropName, color);
    }

    public void TweenScreenBorder(float duration, float targetThick)
    {
        // 중복 실행 방지를 위해 기존 트윈 제거
        matScreen.DOKill();

        // duration초에 걸쳐 targetThick까지 부드럽게 변경
        matScreen.DOFloat(targetThick, propertyName, duration);
    }




    public void ExecuteGimic_Rat(Packet_1_Rat packet) // Index 0
    {
        if (gimicks.Count > 0 && gimicks[0] is MapGimic_1_Rat ratGimic)
        {
            if (packet.targetHpSync > 0)
                ratGimic.ReceiveSyncHP(packet.targetHpSync);
        }
    }

    public void ExecuteGimic_Cow(Packet_2_Cow packet) // Index 1
    {
        if (gimicks.Count > 1 && gimicks[1] is MapGimic_2_Cow cowGimic)
        {
            cowGimic.ReceiveCowSync(packet.actionType);
        }
    }

    public void ExecuteGimic_Tiger(Packet_3_Tiger packet) // Index 2 (매우 중요: On/Off)
    {
        if (gimicks.Count > 2 && gimicks[2] is MapGimic_3_Tiger tiger)
        {
            tiger.ReceiveTigerSync(packet.isStart);
        }
    }

    public void ExecuteGimic_Rabbit(Packet_4_Rabbit packet) // Index 3 (매우 중요: On/Off)
    {
        if (gimicks.Count > 3 && gimicks[3] is MapGimic_4_Rabbit rabbit)
        {
            rabbit.ReceiveRabbitSync(packet.isStart);
        }
    }

    public void ExecuteGimic_Dragon(Packet_5_Dragon packet) // Index 4
    {
        if (gimicks.Count > 4 && gimicks[4] is MapGimic_5_Dragon dragon)
        {
            dragon.ReceiveSpawnSync(packet.x, packet.y, packet.z, packet.angleZ);
        }
    }

    public void ExecuteGimic_Snake(Packet_6_Snake packet) // Index 5
    {
        if (gimicks.Count > 5 && gimicks[5] is MapGimic_6_Snake snake)
        {
            snake.ReceiveSpawnSync(packet.x, packet.y, packet.z);
        }
    }

    public void ExecuteGimic_Horse(Packet_7_Horse packet) // Index 6 (매우 중요: On/Off)
    {
        if (gimicks.Count > 6 && gimicks[6] is MapGimic_7_Horse horse)
        {
            horse.ReceiveHorseSync(packet.isStart);
        }
    }

    public void ExecuteGimic_Sheep(Packet_8_Sheep packet) // Index 7
    {
        if (gimicks.Count > 7 && gimicks[7] is MapGimic_8_Sheep sheep)
        {
            sheep.ReceiveSpawnSync(packet.x, packet.y, packet.z);
        }
    }

    public void ExecuteGimic_Monkey(Packet_9_Monkey packet) // Index 8
    {
        if (gimicks.Count > 8 && gimicks[8] is MapGimic_9_Monkey monkey)
        {
            monkey.ReceiveMonkeySync();
        }
    }

    public void ExecuteGimic_Chicken(Packet_10_Chicken packet) // Index 9
    {
        if (gimicks.Count > 9 && gimicks[9] is MapGimic_10_Chicken chicken)
        {
            chicken.ReceiveSpawnSync(packet.spawnRatio, packet.randomRotationZ);
        }
    }

    public void ExecuteGimic_Dog(Packet_11_Dog packet) // Index 10 (매우 중요: On/Off)
    {
        if (gimicks.Count > 10 && gimicks[10] is MapGimic_11_Dog dog)
        {
            dog.ReceiveDogSync(packet.isActive);
        }
    }

    public void ExecuteGimic_Pig(Packet_12_Pig packet) // Index 11
    {
        if (gimicks.Count > 11 && gimicks[11] is MapGimic_12_Pig pig)
        {
            pig.ReceiveSpawnSync(packet.x, packet.y, packet.z);
        }
    }


    public void ExecuteScreenEffect(Packet_ScreenEffect packet)
    {
        switch (packet.effectType)
        {
            case 0: // Reset
                ResetScreenEffect();
                break;
            case 1: // SetColor
                SetScreenColor(new Color(packet.r, packet.g, packet.b));
                break;
            case 2: // Tween
                TweenScreenBorder(packet.duration, packet.targetThick);
                break;
        }
    }

}
