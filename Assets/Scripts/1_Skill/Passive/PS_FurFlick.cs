using UnityEngine;

public class PS_FurFlick : Passive
{
    [Header("발사 설정")]
    [Tooltip("깃털 투사체 프리팹")]
    public GameObject featherProjectilePrefab;

    [Tooltip("깃털 발사 주기 (초)")]
    public float interval = 3f;

    [Tooltip("플레이어 위치 기준, 얼마나 앞에 생성할지")]
    public float spawnOffset = 0.6f;

    [Tooltip("깃털 속도")]
    public float projectileSpeed = 20f;


    [Header("이펙트")]
    public GameObject objEffect_Use;

    private float timer = 0f;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnTick += OnTick;
        e.OnRoundStart += OnRoundStart;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
        e.OnRoundStart -= OnRoundStart;
    }

    private void OnRoundStart(int roundIndex)
    {
        timer = 0f;
    }

    private void OnTick(float dt)
    {
        if (ability == null) return;
        if (featherProjectilePrefab == null) return;
        if (interval <= 0f) return;

        timer += dt;
        if (timer < interval) return;
        timer -= interval;

      
        var opponent = FindOpponentAbility();
        if (opponent == null) return;

        Vector3 origin = ability.transform.position;
        Vector3 targetPos = opponent.transform.position;

        Vector3 dir = targetPos - origin;
        dir.z = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        FireOne(origin, dir);
    }

    private PlayerAbility FindOpponentAbility()
    {
        if (ability == null) return null;

        var gm = GameObject.FindObjectOfType<GameManager>();
        if (gm == null) return null;

        var pa1 = gm.playerAbility_1;
        var pa2 = gm.playerAbility_2;

        if (pa1 == null || pa2 == null)
            return null;

        if (ability == pa1) return pa2;
        if (ability == pa2) return pa1;

        if (ability.playerNumber == 1) return pa2;
        if (ability.playerNumber == 2) return pa1;

        return null;
    }

    private void FireOne(Vector3 origin, Vector3 dir)
    {
        Vector3 spawnPos = origin + dir * spawnOffset;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        var proj = Object.Instantiate(featherProjectilePrefab, spawnPos, rot);

        var ab = proj.GetComponent<AB_HitboxBase>();
        if (ab != null)
        {
            ab.Init(ability);
        }


        if (objEffect_Use != null)
        {
            GameObject effect = Instantiate(objEffect_Use, transform);
            effect.transform.localPosition = Vector3.zero;

            effect.transform.rotation = Quaternion.Euler(-90, 0, 0);

            effect.transform.localScale = new Vector3(2f, 2f, 2f);
            effect.transform.SetParent(null);
        }


        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.velocity = dir * projectileSpeed;
        }
    }
}