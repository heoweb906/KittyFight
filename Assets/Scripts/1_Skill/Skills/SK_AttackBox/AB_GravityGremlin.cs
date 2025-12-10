using UnityEngine;

public class AB_GravityGremlin : AB_HitboxBase
{
    [Header("Blackhole")]
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float blackholeDuration = 6f;

    [Header("Camera FX")]
    [SerializeField] private float shakeStrength = 0.25f;
    [SerializeField] private float shakeDuration = 0.35f;

    private bool exploded = false;

    public void InitProjectile(
        PlayerAbility owner,
        GameObject blackholePrefab,
        float blackholeDuration,
        float shakeStrength,
        float shakeDuration)
    {
        base.Init(owner);

        this.blackholePrefab = blackholePrefab;
        this.blackholeDuration = blackholeDuration;
        this.shakeStrength = shakeStrength;
        this.shakeDuration = shakeDuration;
    }

    protected override void OnEnvironmentHit(Collider other)
    {
        if (exploded) return;
        exploded = true;

        // 충돌 지점 추출
        Vector3 hitPos = other.ClosestPoint(transform.position);

        // 블랙홀 생성
        if (blackholePrefab != null && ownerAbility != null)
        {
            GameObject bhObj = Instantiate(blackholePrefab, hitPos, Quaternion.identity);
            var blackhole = bhObj.GetComponent<AB_Blackhole>();
            if (blackhole != null)
            {
                blackhole.Init(ownerAbility, blackholeDuration);
            }
        }

        // 카메라 쉐이크
        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeStrength, shakeDuration);

        // 투사체 제거
        Destroy(gameObject);
    }

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
    }
}