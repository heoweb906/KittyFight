using UnityEngine;

public class SK_HopHop : Skill
{
    [Header("HopHop 설정")]
    [Tooltip("일반 점프 대비 배수 (기본 4배)")]
    public float jumpMultiplier = 4f;

    private Rigidbody rb;
    private PlayerMovement movement;
    private PlayerAbility ability;

    private void Awake()
    {
        if (playerAbility != null)
            CacheComponents(playerAbility);
    }

    private void CacheComponents(PlayerAbility a)
    {
        ability = a;
        rb = a.GetComponent<Rigidbody>();
        movement = a.GetComponent<PlayerMovement>();
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (ability == null && playerAbility != null)
        {
            CacheComponents(playerAbility);
        }

        if (ability == null || rb == null) return;

        Vector3 hitboxPos = origin + Vector3.down * 0.4f;
        Quaternion hitboxRot = Quaternion.identity;
        var hitbox = Instantiate(objSkillEntity, hitboxPos, hitboxRot);
        var ab = hitbox.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        Vector3 v = rb.velocity;
        v.y = ability.jumpForce * jumpMultiplier;
        rb.velocity = v;
    }
}