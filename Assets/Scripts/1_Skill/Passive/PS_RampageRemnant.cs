using System.Collections;
using UnityEngine;

public class PS_RampageRemnant : Passive
{
    [Header("폭발 프리팹")]
    public GameObject explosionPrefab;

    [Header("타이밍 / 설치 옵션")]
    [Tooltip("대쉬 종료 후 몇 초 뒤에 폭발을 깔지")]
    public float delayAfterDash = 1.0f;

    [Tooltip("대쉬 경로를 몇 개 구간으로 나눌지 (폭발 개수)")]
    public int segmentCount = 5;

    [Tooltip("폭발 Z좌표를 플레이어 Z로 고정할지 여부")]
    public bool lockZToOwner = true;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);
        e.OnDashFinished += HandleDashFinished;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnDashFinished -= HandleDashFinished;
        base.Unsubscribe(e);
    }

    private void HandleDashFinished(Vector3 start, Vector3 end)
    {
        if (!explosionPrefab || ability == null)
            return;

        ability.StartCoroutine(SpawnExplosionsRoutine(start, end));
    }

    private IEnumerator SpawnExplosionsRoutine(Vector3 start, Vector3 end)
    {
        int count = Mathf.Max(1, segmentCount);

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 1f : (float)i / (count - 1);
            Vector3 pos = Vector3.Lerp(start, end, t);
            SpawnOneEffect(pos);
        }
        if (delayAfterDash > 0f)
            yield return new WaitForSeconds(delayAfterDash);

        if (!explosionPrefab || ability == null)
            yield break;

        Vector3 diff = end - start;
        float dist = diff.magnitude;

        // 거의 안 움직였으면 끝 지점 하나만
        if (dist < 0.01f)
        {
            SpawnOneExplosion(end);
            yield break;
        }


        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 1f : (float)i / (count - 1);
            Vector3 pos = Vector3.Lerp(start, end, t);
            SpawnOneExplosion(pos);
        }
    }

    private void SpawnOneExplosion(Vector3 pos)
    {
        if (lockZToOwner && ability != null)
        {
            pos.z = ability.transform.position.z;
        }

        var go = Object.Instantiate(explosionPrefab, pos, Quaternion.identity);
        var hitbox = go.GetComponent<AB_HitboxBase>();
        if (hitbox != null && ability != null)
        {
            hitbox.Init(ability);
        }
    }

    private void SpawnOneEffect(Vector3 pos)
    {
        Instantiate(
           effectPrefab,
           pos,
           Quaternion.Euler(-90f, 0f, 0f)
       );
    }
}