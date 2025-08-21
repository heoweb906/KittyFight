using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AB_FangShot : AB_HitboxBase
{
    [Header("��(DoT) �Ķ����")]
    public float duration = 3f;
    public float tickInterval = 1f;
    public int damagePerTick = 7;

    [Header("����")]
    public bool destroyOnHit = true;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // PoisonDoT�� �پ����� ������ �߰��ϰ�, ��ġ ����
        var poison = victim.GetComponent<PoisonDoT>();
        if (!poison) poison = victim.gameObject.AddComponent<PoisonDoT>();

        // PoisonDoT �ʿ��� ��ø/���� ������ ó��
        poison.ApplyPoison(duration, tickInterval, damagePerTick);

        if (destroyOnHit && this) Destroy(gameObject);
    }
}