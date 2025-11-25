using System.Collections;
using UnityEngine;

public class SK_MadDash : Skill
{
    [Header("대시 설정")]
    [SerializeField] private float dashSpeed = 16f;
    [SerializeField] private float maxDuration = 1.2f;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private LayerMask wallMask;

    [Header("근접 공격 설정")]
    [SerializeField] private GameObject meleeHitboxPrefab;
    [SerializeField] private float meleeInterval = 0.5f;

    private AbilityEvents events;
    private bool isRunning = false;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (isRunning) return;
        if (!playerAbility) return;

        var movement = playerAbility.GetComponent<PlayerMovement>();

        float dirX;
        if (Mathf.Abs(direction.x) > 0.001f)
        {
            dirX = Mathf.Sign(direction.x);
        }
        else
        {
            float forwardX = playerAbility.transform.forward.x;
            if (Mathf.Abs(forwardX) < 0.001f)
                forwardX = 1f;
            dirX = Mathf.Sign(forwardX);
        }

        if (movement != null && Mathf.Abs(dirX) > 0.001f)
        {
            movement.Move(new Vector2(dirX, 0f));
        }

        Vector3 dashDir = new Vector3(dirX, 0f, 0f);

        StartCoroutine(Co_MadDash(direction));
    }

    private IEnumerator Co_MadDash(Vector3 dashDir)
    {
        isRunning = true;

        GameObject owner = playerAbility.gameObject;

        var rb = owner.GetComponent<Rigidbody>();
        var controller = owner.GetComponent<PlayerController>();
        var movement = owner.GetComponent<PlayerMovement>();
        var selfHealth = owner.GetComponent<PlayerHealth>();
        events = playerAbility.events;

        int startHP = selfHealth != null ? selfHealth.CurrentHP : 0;
        bool canceledByHit = false;
        bool hitWall = false;

        if (rb)
        {
            var v = rb.velocity;
            v.x = 0f; v.z = 0f;
            rb.velocity = v;
        }
        if (controller) controller.enabled = false;
        if (movement) movement.enabled = false;

        float elapsed = 0f;
        float meleeTimer = 0f;

        while (elapsed < maxDuration)
        {
            if (!owner)
            {
                canceledByHit = true;
                break;
            }

            if (selfHealth != null && selfHealth.CurrentHP != startHP)
            {
                canceledByHit = true;
                break;
            }

            float dt = Time.deltaTime;
            elapsed += dt;
            meleeTimer += dt;

            // 벽 체크
            Vector3 pos = owner.transform.position;
            pos.z = playerAbility.transform.position.z;

            RaycastHit hit;
            if (Physics.Raycast(
                    pos,
                    dashDir,
                    out hit,
                    wallCheckDistance,
                    wallMask,
                    QueryTriggerInteraction.Ignore))
            {
                hitWall = true;
                break;
            }

            if (rb)
            {
                var v = rb.velocity;
                v.x = dashDir.x * dashSpeed;
                rb.velocity = v;
            }
            else
            {
                Vector3 move = dashDir * (dashSpeed * dt);
                Vector3 newPos = owner.transform.position + move;
                newPos.z = pos.z;
                owner.transform.position = newPos;
            }

            // 근접공격
            if (meleeTimer >= meleeInterval)
            {
                meleeTimer -= meleeInterval;
                SpawnMeleeHitbox(pos + dashDir*aimRange, dashDir);
            }

            yield return null;
        }

        if (rb)
        {
            var v = rb.velocity;
            v.x = 0f;
            rb.velocity = v;
        }
        if (controller) controller.enabled = true;
        if (movement) movement.enabled = true;


        isRunning = false;
    }

    private void SpawnMeleeHitbox(Vector3 origin, Vector3 direction)
    {
        if (!meleeHitboxPrefab) return;

        Vector3 spawnPos = origin;
        spawnPos.z = playerAbility.transform.position.z;

        Quaternion rot = Quaternion.LookRotation(direction);

        int damage = 0;
        var protoHB = meleeHitboxPrefab.GetComponent<AB_MeleeHitbox>();
        if (protoHB != null) damage = protoHB.damage;

        // TalonsEdge 등 패시브 보정
        events?.EmitMeleeDamageInt(ref damage);

        GameObject hitbox = Object.Instantiate(meleeHitboxPrefab, spawnPos, rot);

        var hb = hitbox.GetComponent<AB_MeleeHitbox>();
        if (hb != null) hb.damage = damage;

        var ab = hitbox.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);
    }
}