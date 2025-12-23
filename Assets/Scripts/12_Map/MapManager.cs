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




    public void ExecuteGimic_Rat(Packet_1_Rat packet)
    {
        // 현재 활성화된 기믹이 '쥐'인지 확인만 하고
        if (currentGimmick is MapGimic_1_Rat ratGimic)
        {
            // 데이터만 던져준다. (끝)
            // 만약 패킷에 다른 변수가 더 있다면 여기서 함수 인자로 더 넘겨주면 됨
            if (packet.targetHpSync > 0)
            {
                ratGimic.ReceiveSyncHP(packet.targetHpSync);
            }
        }
    }


    public void ExecuteGimic_Cow(Packet_2_Cow packet)
    {
        if (currentGimmick is MapGimic_2_Cow cowGimic)
        {
            // 다른 애들처럼 함수 호출로 값만 전달
            cowGimic.ReceiveCowSync(packet.actionType);
        }
    }


    public void ExecuteGimic_Tiger(Packet_3_Tiger packet)
    {
        if (currentGimmick is MapGimic_3_Tiger tiger)
        {
            // 패킷의 isStart 값에 따라 분기 처리
            tiger.ReceiveTigerSync(packet.isStart);
        }
    }

    public void ExecuteGimic_Rabbit(Packet_4_Rabbit packet)
    {
        if (currentGimmick is MapGimic_4_Rabbit rabbit)
        {
            rabbit.ReceiveRabbitSync(packet.isStart);
        }
    }

    public void ExecuteGimic_Dragon(Packet_5_Dragon packet)
    {
        if (currentGimmick is MapGimic_5_Dragon dragon)
        {
            dragon.ReceiveSpawnSync(packet.x, packet.y, packet.z, packet.angleZ);
        }
    }


    public void ExecuteGimic_Snake(Packet_6_Snake packet)
    {
        if (currentGimmick is MapGimic_6_Snake snake)
        {
            snake.ReceiveSpawnSync(packet.x, packet.y, packet.z);
        }
    }


    public void ExecuteGimic_Horse(Packet_7_Horse packet)
    {
        if (currentGimmick is MapGimic_7_Horse horse)
        {
            horse.ReceiveHorseSync(packet.isStart);
        }
    }

    public void ExecuteGimic_Sheep(Packet_8_Sheep packet)
    {
        if (currentGimmick is MapGimic_8_Sheep sheep)
        {
            sheep.ReceiveSpawnSync(packet.x, packet.y, packet.z);
        }
    }


    public void ExecuteGimic_Monkey(Packet_9_Monkey packet)
    {
        if (currentGimmick is MapGimic_9_Monkey monkey)
        {
            monkey.ReceiveMonkeySync();
        }
    }

    public void ExecuteGimic_Chicken(Packet_10_Chicken packet)
    {
        if (currentGimmick is MapGimic_10_Chicken chicken)
        {
            // 비율과 회전값을 넘겨줍니다.
            chicken.ReceiveSpawnSync(packet.spawnRatio, packet.randomRotationZ);
        }
    }


    public void ExecuteGimic_Dog(Packet_11_Dog packet)
    {
        if (currentGimmick is MapGimic_11_Dog dog)
        {
            dog.ReceiveDogSync(packet.isActive);
        }
    }

    public void ExecuteGimic_Pig(Packet_12_Pig packet)
    {
        if (currentGimmick is MapGimic_12_Pig pig)
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
