using UnityEngine;

public class AB_FuryBlast : AB_HitboxBase
{
    [Header("���� ����")]
    public int damageToOthers = 20;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // �ֺ� ������ 2 ������
        victim.TakeDamage(damageToOthers, ownerAbility);
    }
}