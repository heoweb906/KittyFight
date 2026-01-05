using System.Collections;
using UnityEngine;

public class SK_MadDash : Skill
{
    [Header("연타 설정")]
    [SerializeField] private float attackDuration = 2.5f;
    [SerializeField] private float meleeInterval = 0.5f;

    [Header("근접 공격 설정")]
    [SerializeField] private GameObject meleeHitboxPrefab1;
    [SerializeField] private GameObject meleeHitboxPrefab2;

    private AbilityEvents events;
    private bool isRunning = false;
    private int effectIndex = 0;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [Header("사운드")]
    public AudioClip[] audioClips; 

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (isRunning) return;
        if (!playerAbility) return;

        float dirX;
        if (Mathf.Abs(direction.x) > 0.001f) dirX = Mathf.Sign(direction.x);
        else
        {
            float forwardX = playerAbility.transform.forward.x;
            if (Mathf.Abs(forwardX) < 0.001f) forwardX = 1f;
            dirX = Mathf.Sign(forwardX);
        }

        Vector3 attackDir = new Vector3(dirX, 0f, 0f);

        StartCoroutine(Co_MadDash(attackDir, direction));
    }

    private IEnumerator Co_MadDash(Vector3 attackDir, Vector3 direction)
    {
        isRunning = true;

        GameObject owner = playerAbility.gameObject;
        var selfHealth = owner.GetComponent<PlayerHealth>();
        events = playerAbility.events;

        int startHP = selfHealth != null ? selfHealth.CurrentHP : 0;

        anim.SetBool("isAttack", true);

        float elapsed = 0f;
        float nextSwing = 0f;

        while (elapsed < attackDuration)
        {
            if (!owner) break;

            if (selfHealth != null && selfHealth.CurrentHP != startHP)
                break;

            elapsed += Time.deltaTime;

            if (elapsed >= nextSwing)
            {
                nextSwing += meleeInterval;

                Vector3 pos = owner.transform.position;
                pos.z = playerAbility.transform.position.z;

                SpawnMeleeHitbox(pos + attackDir * aimRange, attackDir);

                var gm = FindObjectOfType<GameManager>();
                if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
                {
                    gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
                }
                else
                {
                    gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeDuration * 0.5f, direction);
                }

                anim.SetTrigger("Attack");
            }

            yield return null;
        }

        anim.SetBool("isAttack", false);
        isRunning = false;
    }

    private void SpawnMeleeHitbox(Vector3 origin, Vector3 direction)
    {
        if (!meleeHitboxPrefab1) return;
        if (!meleeHitboxPrefab2) return;

        Vector3 spawnPos = origin;
        spawnPos.z = playerAbility.transform.position.z;

        Quaternion rot = Quaternion.LookRotation(direction);

        int damage = 0;
        GameObject hitbox;

        if (effectIndex == 0)
        {
            var protoHB = meleeHitboxPrefab1.GetComponent<AB_MeleeHitbox>();
            if (protoHB != null) damage = protoHB.damage;

            hitbox = Object.Instantiate(meleeHitboxPrefab1, spawnPos, rot);

            anim.SetInteger("AttackType", 1);
            effectIndex = 1;
        }
        else
        {
            var protoHB = meleeHitboxPrefab2.GetComponent<AB_MeleeHitbox>();
            if (protoHB != null) damage = protoHB.damage;

            hitbox = Object.Instantiate(meleeHitboxPrefab2, spawnPos, rot);

            anim.SetInteger("AttackType", 2);
            effectIndex = 0;
        }


        if (audioClips != null && audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, audioClips.Length);
            playerAbility.PlaySFX(audioClips[randomIndex]);
        }

        // 패시브 보정
        events?.EmitMeleeDamageInt(ref damage);

        var hb = hitbox.GetComponent<AB_MeleeHitbox>();
        if (hb != null) hb.damage = damage;

        var ab = hitbox.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);
    }
}