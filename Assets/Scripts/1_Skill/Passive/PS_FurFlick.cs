using UnityEngine;

public class PS_FurFlick : Passive
{
    public override int PassiveId => 131;

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
    private bool roundActive = false;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;


    private const int PROC_FIRE = 1;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnTick += OnTick;
        e.OnRoundStart += OnRoundStart;
        e.OnRoundEnd += OnRoundEnd;

        roundActive = false;
        timer = 0f;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnTick -= OnTick;
        e.OnRoundStart -= OnRoundStart;
        e.OnRoundEnd -= OnRoundEnd;
    }

    private void OnRoundStart(int roundIndex)
    {
        if (!IsAuthority) return;
        roundActive = true;
        timer = 0f;
    }
    private void OnRoundEnd(int roundIndex)
    {
        if (!IsAuthority) return;
        roundActive = false;
        timer = -999f;
    }

    private void OnTick(float dt)
    {
        if (!IsAuthority) return;
        if (!roundActive) return;
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

        if (dir.sqrMagnitude < 0.0001f) return;
        dir.Normalize();

        FireOne(origin, dir);
        SendProc(
            PassiveProcType.Spawn,
            pos: origin,
            dir: dir,
            i0: PROC_FIRE,
            f0: 0f
        );
    }

    private PlayerAbility FindOpponentAbility()
    {
        if (ability == null) return null;

        var gm = GameObject.FindObjectOfType<GameManager>();
        if (gm == null) return null;

        var pa1 = gm.playerAbility_1;
        var pa2 = gm.playerAbility_2;

        if (pa1 == null || pa2 == null) return null;

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
            ab.Init(ability);

        if (objEffect_Use != null)
        {
            GameObject effect = Instantiate(objEffect_Use, transform);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.rotation = Quaternion.Euler(-90, 0, 0);
            effect.transform.localScale = new Vector3(2f, 2f, 2f);
            effect.transform.SetParent(null);
        }

        ability.PlaySFX(audioClip);

        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.velocity = dir * projectileSpeed;
        }

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }

    public override void RemoteExecute(PassiveProcMessage msg)
    {
        if (ability == null) return;
        if (featherProjectilePrefab == null) return;
        if (msg.i0 != PROC_FIRE) return;

        Vector3 origin = new Vector3(msg.px, msg.py, msg.pz);
        Vector3 dir = new Vector3(msg.dx, msg.dy, msg.dz);

        if (dir.sqrMagnitude < 0.0001f) return;
        dir.Normalize();

        FireOne(origin, dir);
    }
}