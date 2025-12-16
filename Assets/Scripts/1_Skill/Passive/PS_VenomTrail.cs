using System.Collections;
using UnityEngine;

public class PS_VenomTrail : Passive
{
    public override int PassiveId => 116;

    [Header("독구름 프리팹 (AB_VenomTrail)")]
    public GameObject venomCloudPrefab;

    [Header("꼬리 생성 주기")]
    [Tooltip("투사체가 날아가는 동안, 몇 초마다 독구름을 생성할지")]
    public float spawnInterval = 0.3f;

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);
        e.OnHitboxSpawned += OnHitboxSpawned;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnHitboxSpawned -= OnHitboxSpawned;
        base.Unsubscribe(e);
    }

    private void OnHitboxSpawned(AB_HitboxBase hb)
    {
        if (!hb || venomCloudPrefab == null || ability == null)
            return;

        StartCoroutine(SpawnTrailRoutine(hb.transform));
    }

    private IEnumerator SpawnTrailRoutine(Transform projectile)
    {
        if (projectile == null) yield break;

        float timer = 0f;

        while (projectile != null)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                timer -= spawnInterval;
                SpawnCloud(projectile.position);
            }

            yield return null;
        }
    }

    private void SpawnCloud(Vector3 pos)
    {
        if (venomCloudPrefab == null || ability == null) return;
        pos.z = ability.transform.position.z;
        var cloud = Object.Instantiate(venomCloudPrefab, pos, Quaternion.identity);

        var hb = cloud.GetComponent<AB_HitboxBase>();
        if (hb != null)
        {
            hb.Init(ability);
        }
    }
}