using UnityEngine;

public class AB_Banana : AB_HitboxBase
{
    [Header("바나나 스펙")]
    [Tooltip("바나나에 피격 시 들어갈 피해량")]
    public int damage = 40;


    private bool installed = false;
    private Rigidbody rb;
    private Collider col;

    [Tooltip("날아가는 바나나")]
    [SerializeField] private GameObject flyingVisual;
    [Tooltip("바닥 바나나")]
    [SerializeField] private GameObject installedVisual;

    [Header("회전 설정")]
    [SerializeField] private float rotateSpeedX = 720f;

    [Tooltip("이펙트")]
    public GameObject obj_HitPlayer;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.angularVelocity = new Vector3(
                rotateSpeedX * Mathf.Deg2Rad,
                rotateSpeedX * Mathf.Deg2Rad,
                rotateSpeedX * Mathf.Deg2Rad
            );
        }

        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.RegisterRoundObject(this.gameObject);
    }

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        if (victim == null) return;
        victim.TakeDamage(damage, ownerAbility, transform.position);

        var ob = Instantiate(
            obj_HitPlayer,
            transform.position,
            Quaternion.identity
        );
        ob.transform.SetParent(null);

        Destroy(gameObject);
    }
    protected override void OnRemoteHit(PlayerHealth victim, Collider victimCollider)
    {
        var ob = Instantiate(
           obj_HitPlayer,
           transform.position,
           Quaternion.identity
        );
        ob.transform.SetParent(null);

        Destroy(gameObject);
    }

    protected override void OnEnvironmentHit(Collider other)
    {
        if (installed) return;
        installed = true;

        if (rb == null) rb = GetComponent<Rigidbody>();
        if (col == null) col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        if (col != null)
        {
            col.isTrigger = true;
        }

        if (flyingVisual != null)
            flyingVisual.SetActive(false);

        if (installedVisual != null)
            installedVisual.SetActive(true);

        var ob = Instantiate(
           obj_HitPlayer,
           transform.position,
           Quaternion.identity
        );
        ob.transform.SetParent(null);
    }
}