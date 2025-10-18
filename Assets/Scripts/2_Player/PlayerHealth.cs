using System.Collections;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP (유일 소유자)")]
    [SerializeField] private int maxHP = 90;
    [SerializeField] private float invincibleTime = 1.0f;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    public event Action<int, int> OnHPChanged; // (current, max)

    private int currentHP;
    private bool isInvincible = false;

    private Renderer rend;
    private Color originalColor;

    // Ability를 통해 playerNumber를 조회
    private PlayerAbility ability;
    public AbilityEvents events;


    private bool hitEffectPending;   // 데미지 점멸

    [Header("Effects")]
    [SerializeField] public GameObject hitEffectPrefab;
    private Vector3? pendingSourcePos;        // 원거리/근거리에서 넘겨주는 공격 소스 위치
    private Vector3? pendingPunchDir;

    [SerializeField] private Renderer[] flashTargets;   // group6_polySurface11, Right_Ear, Left_Ear
    [SerializeField] private Material flashMaterial;   // 순백 머테리얼 (Toony Colors Pro 2 등)
    [SerializeField] private float flashDuration = 0.2f;


    private void Awake()
    {
        currentHP = maxHP;

        rend = GetComponent<Renderer>();
        if (rend != null) originalColor = rend.material.color;

        ability = GetComponent<PlayerAbility>();

        // 초기값 알림
        OnHPChanged?.Invoke(currentHP, maxHP);

        if (!events && ability) events = ability.events;
        if (!events) events = GetComponent<AbilityEvents>();  // 보강
    }

    private void Update()
    {
        if (hitEffectPending && isActiveAndEnabled && gameObject.activeInHierarchy && !isInvincible)
        {
            hitEffectPending = false;
            StartCoroutine(DamageEffectCoroutine()); // 메인 스레드에서 안전하게 시작
        }
    }

    private void ShakeCameraPunch(float strength, Vector3 dir, float duration = 0.14f)
    {
        var gm = FindObjectOfType<GameManager>();
        var cam = gm?.cameraManager;
        if (cam == null) return;

        float mag = Mathf.Clamp01(strength);
        if (dir.sqrMagnitude < 1e-8f) dir = Vector3.right;

        cam.ShakeCameraPunch(mag, duration, dir);
    }

    private Vector3 ComputePunchDirFromSource(Vector3 sourceWorldPos)
    {
        Vector3 away = transform.position - sourceWorldPos; // 소스→나
        Vector3 dir = new Vector3(away.x, away.y, 0f);
        if (dir.sqrMagnitude > 1e-8f) return dir.normalized;

        // 폴백: 바라보는 좌/우로
        bool selfFacingRight = Vector3.Dot(transform.forward, Vector3.right) >= 0f;
        return selfFacingRight ? Vector3.right : Vector3.left;
    }


    public void TakeDamage(int damage)
    {
        pendingSourcePos = null;
        TakeDamage(damage, null);
    }

    public void TakeDamage(int damage, PlayerAbility attacker)
    {
        if (isInvincible) return;

        // 테스트용
        //pendingSourcePos = null;

        int amount = Mathf.Max(0, damage);

        // 공격자
        attacker?.events?.EmitBeforeDealDamage(ref amount, this.gameObject);
        if (amount <= 0) return;

        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
        ability.effect?.PlayDoubleShakeAnimation(5, 6); // 내 HP

        float selfShake = 0.15f + Mathf.FloorToInt(amount / 10) * 0.08f;

        Vector3 dir = pendingPunchDir
            ?? (Vector3.Dot(transform.forward, Vector3.right) >= 0f ? Vector3.right : Vector3.left);
        ShakeCameraPunch(selfShake, dir);
        pendingPunchDir = null;

        // 권위 판정: Ability.playerNumber 기준
        int pn = ability != null ? ability.playerNumber : 0;
        if (pn == MatchResultStore.myPlayerNumber)
        {
            P2PMessageSender.SendMessage(
                DamageMessageBuilder.Build(pn, currentHP, pendingSourcePos));
        }

        if (currentHP <= 0)
        {
            Debug.Log("Lose");
            FindObjectOfType<GameManager>()?.EndGame(MatchResultStore.myPlayerNumber);
        }

        hitEffectPending = true;
    }

    public void TakeDamage(int damage, PlayerAbility attacker, Vector3 sourceWorldPos)
    {
        pendingSourcePos = sourceWorldPos;
        pendingPunchDir = ComputePunchDirFromSource(sourceWorldPos);
        TakeDamage(damage, attacker);
    }

    private IEnumerator DamageEffectCoroutine()
    {
        isInvincible = true;

        // 하얗게 점멸
        //yield return
        StartCoroutine(WhiteFlashSwapOnce());

        Quaternion rot = ComputeHitEffectRotation();

        if (hitEffectPrefab)
            Instantiate(hitEffectPrefab, transform.position, rot);

        // 무적
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;

        // 사용한 값 정리
        pendingSourcePos = null;
    }

    // === [ADD] 머테리얼 스왑 코루틴 (sharedMaterials만 교체/복원) ===
    private IEnumerator WhiteFlashSwapOnce()
    {
        if (flashTargets == null || flashTargets.Length == 0 || flashMaterial == null)
            yield break;

        // 원복을 위해 각 렌더러의 sharedMaterials 배열을 그대로 백업
        var originals = new System.Collections.Generic.List<Material[]>();
        originals.Capacity = flashTargets.Length;

        for (int i = 0; i < flashTargets.Length; i++)
        {
            var r = flashTargets[i];
            if (!r) { originals.Add(null); continue; }

            var prev = r.sharedMaterials;                  // 그대로 보관 (자산 레퍼런스)
            originals.Add(prev);

            // 같은 길이로 전 슬롯을 flashMaterial로 채워서 교체
            int n = prev != null ? prev.Length : 1;
            var temp = new Material[n];
            for (int k = 0; k < n; k++) temp[k] = flashMaterial;
            r.sharedMaterials = temp;                      // ← 스왑 (자산 레퍼런스만 바꿈)
        }

        yield return new WaitForSeconds(flashDuration);

        // 원상복구
        for (int i = 0; i < flashTargets.Length; i++)
        {
            var r = flashTargets[i];
            if (!r || originals[i] == null) continue;
            r.sharedMaterials = originals[i];
        }
    }




    // 원격 HP 확정값 반영
    public void RemoteSetHP(int hp)
    {
        RemoteSetHP(hp, null);
    }

    public void RemoteSetHP(int hp, Vector3? sourceWorldPos)
    {
        int prev = currentHP;
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
        ability.effect?.PlayDoubleShakeAnimation(5, 6);

        if (currentHP < prev)
        {
            pendingSourcePos = sourceWorldPos;

            int dealt = prev - currentHP;
            float oppHitShake = 0.09f + Mathf.FloorToInt(dealt / 10f) * 0.08f;

            Vector3 dir = sourceWorldPos.HasValue
                ? ComputePunchDirFromSource(sourceWorldPos.Value)
                : (Vector3.Dot(transform.forward, Vector3.right) >= 0f ? Vector3.right : Vector3.left);

            ShakeCameraPunch(oppHitShake, dir);
            hitEffectPending = true;
        }

        if (currentHP <= 0)
        {
            int winnerPlayerNum = MatchResultStore.myPlayerNumber == 1 ? 2 : 1;
            FindObjectOfType<GameManager>()?.EndGame(winnerPlayerNum);
        }
    }

    public void ResetHealth()
    {
        currentHP = maxHP;
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    public void SetMaxHP(int newMax, bool keepCurrentRatio = false)
    {
        newMax = Mathf.Max(1, newMax);

        if (keepCurrentRatio)
        {
            float ratio = maxHP > 0 ? (float)currentHP / maxHP : 1f;
            currentHP = Mathf.Clamp(Mathf.RoundToInt(newMax * ratio), 0, newMax);
        }
        else
        {
            currentHP = Mathf.Clamp(currentHP, 0, newMax);
        }

        maxHP = newMax;
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    public void AddMaxHP(int delta, bool keepCurrentRatio = false)
    {
        SetMaxHP(maxHP + delta, keepCurrentRatio);
    }

    private Quaternion ComputeHitEffectRotation()
    {
        const float fixedY = 90f;
        const float fixedZ = -90f;
        float xAngle;

        if (pendingSourcePos.HasValue)
        {
            Vector3 away = transform.position - pendingSourcePos.Value;

            float x = away.x;
            float y = away.y;

            if (Mathf.Abs(x) > 1e-5f || Mathf.Abs(y) > 1e-5f)
            {
                xAngle = Mathf.Atan2(-y, x) * Mathf.Rad2Deg;
                if (xAngle < 0f) xAngle += 360f;  // 0~360°로 정규화
            }
            else
            {
                xAngle = 0f; // 같은 자리면 기본값(오른쪽)
            }
        }
        else
        {
            xAngle = 0f; // 소스 정보 없으면 기본(오른쪽)
        }

        return Quaternion.Euler(xAngle, fixedY, fixedZ);
    }
}