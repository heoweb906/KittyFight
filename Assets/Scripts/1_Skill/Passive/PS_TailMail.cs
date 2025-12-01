using UnityEngine;

public class PS_TaiMail : Passive
{
    [Header("Tai Mail Settings")]
    [Tooltip("원거리 공격 쿨타임 감소량 (초 단위)")]
    public float cooldownReduction = 1.0f;

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);
        e.OnModifyCooldown += OnModifyCooldown;
        e.OnHitboxSpawned += OnHitboxSpawned;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnModifyCooldown -= OnModifyCooldown;
        e.OnHitboxSpawned -= OnHitboxSpawned;
        base.Unsubscribe(e);
    }

    private void OnModifyCooldown(SkillType slot, ref float seconds)
    {
        if (slot != SkillType.Ranged) return;
        float reduce = Mathf.Max(0f, cooldownReduction);
        seconds = Mathf.Max(0f, seconds - reduce);
    }

    private void OnHitboxSpawned(AB_HitboxBase hb)
    {
        var rb = hb.GetComponent<Rigidbody>();
        if (!rb) return;

        rb.useGravity = false;
    }
}