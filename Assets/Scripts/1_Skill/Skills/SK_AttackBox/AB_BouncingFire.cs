using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AB_BouncingFire : AB_HitboxBase
{
    [Header("Bouncing 설정")]
    [SerializeField] private int maxBounces = 5;
    [SerializeField] private float bounceSpeedMultiplier = 1.7f;
    [SerializeField] private int damage = 40;

    [Header("자동 삭제 설정")]
    [SerializeField] private float stopSpeedThreshold = 0.2f;
    [SerializeField] private float minAliveTime = 0.3f; 
    [SerializeField] private float lowSpeedDurationToDestroy = 0.7f;

    [Header("노멀 계산용 레이캐스트")]
    [SerializeField] private float rayBackOffset = 0.25f;
    [SerializeField] private float rayForwardDistance = 0.6f;

    private int remainingBounces;
    private Rigidbody rb;

    private Vector3 lastVelocity;

    private float timeAlive = 0f;
    private float lowSpeedTime = 0f;

    [Header("이펙트")]
    public GameObject objEffect_Hit;
    public float fEffectSize;
    public GameObject[] objs_Piece;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;
    

    protected override void Awake()
    {
        base.Awake();
        remainingBounces = maxBounces;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        timeAlive += Time.fixedDeltaTime;

        lastVelocity = rb.velocity;

        float sqrSpeed = rb.velocity.sqrMagnitude;
        float sqrThreshold = stopSpeedThreshold * stopSpeedThreshold;

        if (sqrSpeed < sqrThreshold)
        {
            lowSpeedTime += Time.fixedDeltaTime;
        }
        else
        {
            lowSpeedTime = 0f;
        }

        if (timeAlive > minAliveTime && lowSpeedTime > lowSpeedDurationToDestroy)
        {
            OnDisappearEffect();
            Destroy(gameObject);
        }
    }

    protected override void OnRemoteHit(PlayerHealth victim, Collider victimCollider)
    {
        OnDisappearEffect();
        Destroy(gameObject);
    }


    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        if (victim == null) return;

        victim.TakeDamage(damage, ownerAbility);
        OnDisappearEffect();
        Destroy(gameObject);
    }

    protected override void OnEnvironmentHit(Collider other)
    {
        if (rb == null)
        {
            OnDisappearEffect();
            Destroy(gameObject);
            return;
        }

        if (remainingBounces <= 0)
        {
            OnDisappearEffect();
            Destroy(gameObject);
            return;
        }

        // 충돌 직전 속도
        Vector3 incoming = lastVelocity;
        if (incoming.sqrMagnitude < 0.0001f)
            incoming = rb.velocity;

        if (incoming.sqrMagnitude < 0.0001f)
        {
            OnDisappearEffect();
            Destroy(gameObject);
            return;
        }

        Vector3 dir = incoming.normalized;

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);

        float radius = 0.2f;
        var sphereCol = GetComponent<SphereCollider>();
        if (sphereCol != null)
        {
            float maxScale = Mathf.Max(
                transform.lossyScale.x,
                transform.lossyScale.y,
                transform.lossyScale.z
            );
            radius = sphereCol.radius * maxScale;
        }
        else
        {
            radius = Mathf.Max(
                other.bounds.extents.x,
                other.bounds.extents.y
            );
        }

        Vector3 rayOrigin = transform.position - dir * rayBackOffset;
        float rayDist = rayBackOffset + rayForwardDistance;

        RaycastHit hitInfo;
        Vector3 normal;

        if (Physics.SphereCast(
                rayOrigin,
                radius,
                dir,
                out hitInfo,
                rayDist,
                environmentMask,
                QueryTriggerInteraction.Ignore))
        {
            normal = hitInfo.normal;
        }
        else
        {
            Vector3 diff = transform.position - other.bounds.center;
            diff.z = 0f;

            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                normal = new Vector3(Mathf.Sign(diff.x), 0f, 0f);
            else
                normal = new Vector3(0f, Mathf.Sign(diff.y), 0f);

            if (normal.sqrMagnitude < 0.5f)
                normal = Vector3.up;
        }

        Vector3 reflectedDir = Vector3.Reflect(dir, normal).normalized;

        if (normal.y > 0.6f)
        {
            if (reflectedDir.y < 0.25f)
            {
                reflectedDir.y = 0.25f;
                reflectedDir.z = 0f;
                reflectedDir.Normalize();
            }
        }

        // 속도 1.7배
        float newSpeed = incoming.magnitude * bounceSpeedMultiplier;
        if (!float.IsFinite(newSpeed) || newSpeed < 0.01f)
        {
            OnDisappearEffect();
            Destroy(gameObject);
            return;
        }

        rb.velocity = reflectedDir * newSpeed;
        remainingBounces--;
    }


    private void OnDisappearEffect()
    {
        if (objEffect_Hit != null)
        {
            GameObject effect = Instantiate(objEffect_Hit, transform);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.Euler(0, 0, 0);
            effect.transform.localScale = new Vector3(fEffectSize, fEffectSize, fEffectSize);
            effect.transform.SetParent(null);
        }

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);

        Explode(0.5f, 5f);
        foreach (GameObject piece in objs_Piece)
        {
            if (piece != null)
            {
                piece.transform.SetParent(null);
            }
        }



    }

}