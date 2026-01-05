using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_Blackhole : AB_HitboxBase
{
    [Header("Gravity")]
    [SerializeField] private float pullStrength = 13f;
    [SerializeField] private float pullRadius = 4f;

    [Header("Damage per second")]
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private int damagePerTick = 1;


    private float duration;

    private readonly Dictionary<PlayerHealth, float> timers = new Dictionary<PlayerHealth, float>();

    protected override void Awake()
    {
        StartEffect();
    }
    public void Init(PlayerAbility owner, float dur)
    {
        base.Init(owner);
        duration = dur;

        StartCoroutine(Co_Blackhole());
    }

    private IEnumerator Co_Blackhole()
    {
        float elapsed = 0f;




        while (elapsed < duration)
        {
            StepBlackhole(Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    private void StepBlackhole(float dt)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pullRadius, ~0);

        var targetMap = new Dictionary<PlayerHealth, PlayerAbility>();
        foreach (var col in hits)
        {
            var ability = col.GetComponentInParent<PlayerAbility>();
            var health = col.GetComponentInParent<PlayerHealth>();
            if (ability == null || health == null) continue;

            if (ability.playerNumber != MatchResultStore.myPlayerNumber)
                continue;
            if (!targetMap.ContainsKey(health))
            {
                targetMap.Add(health, ability);
            }
        }

        HashSet<PlayerHealth> inRange = new HashSet<PlayerHealth>(targetMap.Keys);

        foreach (var kvp in targetMap)
        {
            PlayerHealth health = kvp.Key;
            PlayerAbility ability = kvp.Value;

            // 끌어당김
            Rigidbody rb = ability.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (transform.position - ability.transform.position).normalized;
                rb.AddForce(dir * pullStrength, ForceMode.Acceleration);
            }

            // 틱 데미지 누적
            float acc = timers.TryGetValue(health, out var t) ? t : 0f;
            acc += dt;

            while (acc >= tickInterval)
            {
                acc -= tickInterval;
                health.TakeDamage(damagePerTick);
            }

            timers[health] = acc;
        }

        // 4) 범위 벗어난 플레이어 타이머 제거
        var keys = new List<PlayerHealth>(timers.Keys);
        foreach (var h in keys)
        {
            if (!inRange.Contains(h))
                timers.Remove(h);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 0.1f, 0.8f, 0.25f);
        Gizmos.DrawSphere(transform.position, pullRadius);
    }

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
    }
}