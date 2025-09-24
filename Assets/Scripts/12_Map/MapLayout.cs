using UnityEngine;

/// <summary>
/// 맵 프리팹 또는 씬의 맵 오브젝트에 부착하여,
/// 해당 맵의 주요 데이터(스폰 지점 등)를 관리하는 컴포넌트입니다.
/// </summary>
public class MapLayout : MonoBehaviour
{
    [Header("플레이어 스폰 포인트")]
    public Transform spawnPoint1;
    public Transform spawnPoint2;
}