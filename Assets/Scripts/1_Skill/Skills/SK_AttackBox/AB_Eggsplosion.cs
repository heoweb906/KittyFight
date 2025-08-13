using UnityEngine;

public class AB_Eggsplosion : AB_HitboxBase
{
    public float blindnessDuration = 2.5f;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        // 시야 방해 부착 (내가 맞았을 때만 실행됨)
        var blindness = victim.gameObject.AddComponent<BlindStatus>();
        blindness.ApplyBlind(blindnessDuration);

        // 데미지 없음 → TakeDamage 미호출
    }
}