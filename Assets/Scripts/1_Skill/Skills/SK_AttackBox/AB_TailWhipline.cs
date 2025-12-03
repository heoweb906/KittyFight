using UnityEngine;

public class AB_TailWhipline : AB_HitboxBase
{
    [Header("스턴 수치")]
    public float stunDuration = 0.3f;

    [Header("피해/제어")]
    public int damage = 20;

    [Header("이펙트")]
    public GameObject objEffect;
    

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage, ownerAbility);

        Debug.Log("작동작동작동작동");


        var stun = victim.GetComponent<StunStatus>();
        if (!stun) stun = victim.gameObject.AddComponent<StunStatus>();

        stun.ApplyStun(stunDuration);

        Destroy(gameObject);
    }
}
