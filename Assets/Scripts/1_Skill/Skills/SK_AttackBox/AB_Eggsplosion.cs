using UnityEngine;

public class AB_Eggsplosion : AB_HitboxBase
{
    public float blindnessDuration = 2.5f;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // �þ� ���� ���� (���� �¾��� ���� �����)
        var blindness = victim.gameObject.AddComponent<BlindStatus>();
        blindness.ApplyBlind(blindnessDuration);

        // ������ ���� �� TakeDamage ��ȣ��
    }
}