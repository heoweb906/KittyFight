using UnityEngine;

public class SK_GravityGremlin : Skill
{
    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 14f;

    [Header("Blackhole Settings")]
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float blackholeDuration = 6f;

    [Header("Camera FX")]
    [SerializeField] private float shakeStrength = 0.25f;
    [SerializeField] private float shakeDuration = 0.35f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;
        if (!objSkillEntity) return;
        if (!blackholePrefab) return;

        direction.z = 0f;
        origin.z = playerAbility.transform.position.z;

        if (direction.sqrMagnitude < 0.001f)
            direction = playerAbility.transform.right;

        direction.Normalize();

        // 투사체 생성
        GameObject proj = Instantiate(
            objSkillEntity,
            origin,
            Quaternion.LookRotation(direction, Vector3.up)
        );

        var projHitbox = proj.GetComponent<AB_GravityGremlin>();
        if (projHitbox == null)
        {
            projHitbox = proj.AddComponent<AB_GravityGremlin>();
        }

        projHitbox.InitProjectile(
            playerAbility,
            blackholePrefab,
            blackholeDuration,
            shakeStrength,
            shakeDuration
        );

        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = direction * projectileSpeed;
        }
    }
}