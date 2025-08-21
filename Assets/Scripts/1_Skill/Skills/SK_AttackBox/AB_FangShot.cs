using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AB_FangShot : AB_HitboxBase
{
    [Header("독(DoT) 파라미터")]
    public float duration = 3f;
    public float tickInterval = 1f;
    public int damagePerTick = 7;

    [Header("동작")]
    public bool destroyOnHit = true;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // PoisonDoT가 붙어있지 않으면 추가하고, 수치 적용
        var poison = victim.GetComponent<PoisonDoT>();
        if (!poison) poison = victim.gameObject.AddComponent<PoisonDoT>();

        // PoisonDoT 쪽에서 중첩/갱신 전략을 처리
        poison.ApplyPoison(duration, tickInterval, damagePerTick);

        if (destroyOnHit && this) Destroy(gameObject);
    }
}