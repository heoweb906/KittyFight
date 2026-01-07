using UnityEngine;

public class AB_WoolWall : AB_HitboxBase
{
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private GameObject stunEffectPrefab;

    [Header("사운드")]
    [SerializeField] private AudioClip destroySfxAudio;

    [Header("카메라 연출(피격 시)")]
    [SerializeField] private float hitShakeAmount = 0.3f;
    [SerializeField] private float hitShakeDuration = 0.7f;

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var stun = victim.GetComponent<StunStatus>();
        if (!stun) stun = victim.gameObject.AddComponent<StunStatus>();
        stun.ApplyStun(stunDuration);

        Instantiate(
            stunEffectPrefab,
            victim.transform.position,
            Quaternion.identity,
            victim.transform
        );

        ShakeOnHit(victim);

        if (this)
        {
            ownerAbility?.PlaySFX(destroySfxAudio);
            Destroy(gameObject);
        }
    }
    protected override void OnRemoteHit(PlayerHealth victim, Collider victimCollider)
    {
        ShakeOnHit(victim);

        if (this)
        {
            ownerAbility?.PlaySFX(destroySfxAudio);
            Destroy(gameObject);
        }
    }

    private void ShakeOnHit(PlayerHealth victim)
    {
        var gm = FindObjectOfType<GameManager>();
        if (gm == null || gm.cameraManager == null || victim == null) return;

        Vector3 dir = victim.transform.position - transform.position;
        dir.z = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector3.right;

        gm.cameraManager.ShakeCameraPunch(hitShakeAmount, hitShakeDuration, dir.normalized);
    }
}