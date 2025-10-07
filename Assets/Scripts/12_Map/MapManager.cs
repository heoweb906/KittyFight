using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public int currentIndex = -1;



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
        for (int i = 0; i < gimicks.Count; ++i) gimicks[i].gameManger = gameManager;
        for (int i = 0; i < gimicks.Count; ++i) gimicks[i].mapManager = this;

        currentGimmick = null;
    }



    private void FixedUpdate()
    {
        if (currentGimmick != null)
            currentGimmick.OnGimmickUpdate();
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


    public void ChangeMapGimicIndex(int iGimicIndex)
    {
        currentIndex = iGimicIndex - 1;
    }


    public void ActivateGimmick()
    {
        if (currentGimmick != null) currentGimmick.OnGimicEnd();

        if (currentIndex < 0 || currentIndex >= gimicks.Count)
        {
            currentGimmick = null;
            currentIndex = -1;
            return;
        }

        currentGimmick = gimicks[currentIndex];
        currentGimmick.OnGimicStart();
    }



    public void StopCurrentGimmick()
    {
        if (currentGimmick != null)
        {
            currentGimmick.OnGimicEnd();
            currentGimmick = null;
            currentIndex = -1;
        }
    }
    #endregion
}
