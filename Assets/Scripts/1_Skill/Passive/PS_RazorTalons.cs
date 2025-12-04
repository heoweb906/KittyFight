using UnityEngine;

public class PS_RazorTalons : Passive
{
    [Header("범위 증가 설정")]
    [Tooltip("근접 범위 증가율 (0.4 = 40% 증가)")]
    [Range(0f, 2f)]
    public float rangeIncreaseRate = 0.4f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);
        e.OnMeleeHitboxSpawned += OnMeleeHitboxSpawned;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnMeleeHitboxSpawned -= OnMeleeHitboxSpawned;
        base.Unsubscribe(e);
    }

    private void OnMeleeHitboxSpawned(AB_MeleeHitbox hb)
    {
        if (hb == null) return;

        float factor = 1f + rangeIncreaseRate;   // 기본 1.4배

        var tr = hb.transform;
        Vector3 s = tr.localScale;
        
        s.x *= factor;
        s.y *= factor;

        tr.localScale = s;

        Instantiate(
            effectPrefab,
            hb.transform.position,
            hb.transform.rotation
        );
    }
}