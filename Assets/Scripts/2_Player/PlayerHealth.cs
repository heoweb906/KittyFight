using System.Collections;
using System.Collections.Generic; // List 사용을 위해 명시
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP (유일 소유자)")]
    [SerializeField] private int maxHP = 90;
    [SerializeField] private float invincibleTime = 0.3f;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    public event Action<int, int> OnHPChanged; // (current, max)

    private int currentHP;
    private bool isInvincible = false;
    private bool skInvincible = false; // LazyMode 용
    private bool skipNextDamageEffect = false;

    private Renderer rend;
    // private Color originalColor; // 사용하지 않으므로 제거하거나 유지해도 무방

    // Ability를 통해 playerNumber를 조회
    private PlayerAbility ability;
    public AbilityEvents events;

    private Animator anim;

    private bool hitEffectPending;    // 데미지 점멸

    [Header("Audio")]
    [SerializeField] private AudioClip[] hitSfxClips;   // 맞을 때 (랜덤 3개)
    [SerializeField] private AudioClip deathSfx;        // 죽을 때

    [Header("Effects")]
    [SerializeField] public GameObject hitEffectPrefab;
    private Vector3? pendingSourcePos;        // 원거리/근거리에서 넘겨주는 공격 소스 위치
    private Vector3? pendingPunchDir;

    [SerializeField] private Renderer[] flashTargets;   // group6_polySurface11, Right_Ear, Left_Ear
    [SerializeField] private Material flashMaterial;   // 순백 머테리얼 (Toony Colors Pro 2 등)
    [SerializeField] private float flashDuration = 0.2f;

    [Header("보조 변수들")]
    public bool bDogGimickOn = false;

    // [ADD] 원래 머테리얼을 영구 보존할 리스트
    private List<Material[]> cachedOriginalMaterials = new List<Material[]>();
    // [ADD] 현재 실행 중인 점멸 코루틴을 추적하여 중복 실행 방지
    private Coroutine currentFlashRoutine;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        currentHP = maxHP;

        rend = GetComponent<Renderer>();
        // if (rend != null) originalColor = rend.material.color; // 머테리얼 교체 방식에서는 불필요

        ability = GetComponent<PlayerAbility>();

        // 초기값 알림
        OnHPChanged?.Invoke(currentHP, maxHP);

        if (!events && ability) events = ability.events;
        if (!events) events = GetComponent<AbilityEvents>();  // 보강

        // [ADD] 게임 시작 시, 순수한 원래 머테리얼들을 미리 캐싱(저장)해둡니다.
        // 나중에 절대 이 리스트는 수정하지 않고, 복구용으로만 씁니다.
        if (flashTargets != null)
        {
            foreach (var r in flashTargets)
            {
                if (r != null)
                {
                    // sharedMaterials 배열 자체를 복사해서 저장
                    cachedOriginalMaterials.Add(r.sharedMaterials);
                }
                else
                {
                    cachedOriginalMaterials.Add(null);
                }
            }
        }
    }

    // [ADD] 오브젝트가 꺼질 때(비활성화) 강제로 머테리얼을 복구 (안전장치)
    private void OnDisable()
    {
        RestoreOriginalMaterials();
    }

    private void Update()
    {
        if (hitEffectPending && isActiveAndEnabled && gameObject.activeInHierarchy && !isInvincible)
        {
            hitEffectPending = false;
            anim.SetBool("isDamage", true);
            anim.SetTrigger("TakeDamage");
            StartCoroutine(DamageEffectCoroutine()); // 메인 스레드에서 안전하게 시작
        }
    }

    // ... (중간 생략: ShakeCameraPunch, ComputePunchDirFromSource, TakeDamage 등 기존 로직 동일) ...
    // ... TakeDamage, ForceDamage, Heal 함수들은 수정할 필요 없음 ...
    // 다만 코드 길이상 생략하고 아래쪽의 수정된 부분만 붙여넣으시면 됩니다.
    // 기존 TakeDamage 등의 로직은 그대로 유지하세요.

    // 편의를 위해 TakeDamage 메서드들 생략... 
    // 기존 코드 그대로 사용하시면 됩니다.

    // --------------------------------------------------------------------------
    // [수정된 부분] 데미지 이펙트 및 머테리얼 점멸 처리
    // --------------------------------------------------------------------------

    private void ShakeCameraPunch(float strength, Vector3 dir, float duration = 0.6f)
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
        if (isInvincible || skInvincible) return;

        int amount = Mathf.Max(0, damage);
        if (bDogGimickOn) amount = Mathf.RoundToInt(damage * 1.3f);

        attacker?.events?.EmitBeforeDealDamage(ref amount, this.gameObject);
        if (amount <= 0) return;

        events?.EmitBeforeTakeDamage(ref amount, attacker ? attacker.gameObject : null);

        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
        ability.effect?.PlayDoubleShakeAnimation(5, 6);

        if (ability != null && hitSfxClips != null && hitSfxClips.Length > 0)
        {
            int idx = UnityEngine.Random.Range(0, hitSfxClips.Length);
            ability.PlaySFX(hitSfxClips[idx]);
        }

        float selfShake = 0.3f + Mathf.FloorToInt(amount / 10) * 0.08f;
        Vector3 dir = pendingPunchDir ?? (Vector3.Dot(transform.forward, Vector3.right) >= 0f ? Vector3.right : Vector3.left);
        ShakeCameraPunch(selfShake, dir);
        pendingPunchDir = null;

        int pn = ability != null ? ability.playerNumber : 0;
        int attackerPlayerNum = (attacker != null) ? attacker.playerNumber : 0;
        if (pn == MatchResultStore.myPlayerNumber)
        {
            P2PMessageSender.SendMessage(DamageMessageBuilder.Build(pn, currentHP, attackerPlayerNum, pendingSourcePos, maxHP));
        }

        if (currentHP <= 0)
        {
            if (ability != null && deathSfx != null) ability.PlaySFX(deathSfx);
            Debug.Log("Lose");
            FindObjectOfType<GameManager>()?.EndGame(MatchResultStore.myPlayerNumber);
        }

        if (!skipNextDamageEffect) hitEffectPending = true;
        else skipNextDamageEffect = false;
    }

    public void TakeDamage(int damage, PlayerAbility attacker, Vector3 sourceWorldPos)
    {
        pendingSourcePos = sourceWorldPos;
        pendingPunchDir = ComputePunchDirFromSource(sourceWorldPos);
        TakeDamage(damage, attacker);
    }

    public void ForceDamage(int damage, PlayerAbility attacker = null)
    {
        isInvincible = false;
        skInvincible = false;
        skipNextDamageEffect = true;
        TakeDamage(damage, attacker);
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        int prev = currentHP;
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        if (currentHP == prev) return;
        OnHPChanged?.Invoke(currentHP, maxHP);
        ability.effect?.PlayDoubleShakeAnimation(5, 6);
        int pn = ability != null ? ability.playerNumber : 0;
        if (pn == MatchResultStore.myPlayerNumber)
        {
            P2PMessageSender.SendMessage(DamageMessageBuilder.Build(pn, currentHP, 0, null, maxHP));
        }
    }

    // ... RemoteSetHP, ResetHealth, SetMaxHP 등등 기존 함수 유지 ...
    public void RemoteSetHP(int hp) { RemoteSetHP(hp, null); }
    public void RemoteSetHP(int hp, Vector3? sourceWorldPos)
    {
        int prev = currentHP;
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
        ability.effect?.PlayDoubleShakeAnimation(5, 6);

        if (currentHP < prev)
        {
            pendingSourcePos = sourceWorldPos;
            if (ability != null && hitSfxClips != null && hitSfxClips.Length > 0)
            {
                int idx = UnityEngine.Random.Range(0, hitSfxClips.Length);
                ability.PlaySFX(hitSfxClips[idx]);
            }
            int dealt = prev - currentHP;
            events?.EmitTookDamage(dealt);
            float oppHitShake = 0.09f + Mathf.FloorToInt(dealt / 10f) * 0.08f;
            Vector3 dir = sourceWorldPos.HasValue ? ComputePunchDirFromSource(sourceWorldPos.Value) : (Vector3.Dot(transform.forward, Vector3.right) >= 0f ? Vector3.right : Vector3.left);
            ShakeCameraPunch(oppHitShake, dir);
            hitEffectPending = true;
        }
        if (currentHP <= 0)
        {
            if (ability != null && deathSfx != null) ability.PlaySFX(deathSfx);
            int winnerPlayerNum = MatchResultStore.myPlayerNumber == 1 ? 2 : 1;
            FindObjectOfType<GameManager>()?.EndGame(winnerPlayerNum);
        }
    }
    public void ResetHealth() { currentHP = maxHP; OnHPChanged?.Invoke(currentHP, maxHP); }
    public void SetMaxHP(int newMax, bool keepCurrentRatio = false) { /* 기존 내용 유지 */ }
    public void AddMaxHP(int delta, bool keepCurrentRatio = false) { /* 기존 내용 유지 */ }
    private Quaternion ComputeHitEffectRotation() { /* 기존 내용 유지 */ return Quaternion.identity; }
    public void SetSkillInvincible(float duration) { StartCoroutine(Co_SkillInvincible(duration)); }
    private IEnumerator Co_SkillInvincible(float duration) { skInvincible = true; yield return new WaitForSeconds(duration); skInvincible = false; }


    private IEnumerator DamageEffectCoroutine()
    {
        isInvincible = true;

        // [수정] 점멸 효과 실행
        StartFlashEffect();

        // 히트 이펙트 생성
        Quaternion rot = ComputeHitEffectRotation(); // 이 함수는 아래에 있어야 합니다 (기존 코드에 있음)
        if (hitEffectPrefab)
        {
            GameObject effectObj = Instantiate(hitEffectPrefab, transform.position, rot);
            effectObj.transform.localScale *= 1.5f;
        }

        // 무적 시간 대기
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;

        // 정리
        pendingSourcePos = null;
        anim.SetBool("isDamage", false);
    }

    // [ADD] 점멸 시작 진입점
    private void StartFlashEffect()
    {
        // 1. 이미 돌고 있는 점멸 코루틴이 있다면 즉시 중단
        if (currentFlashRoutine != null)
        {
            StopCoroutine(currentFlashRoutine);
        }

        // 2. 새로운 점멸 코루틴 시작
        currentFlashRoutine = StartCoroutine(WhiteFlashSwapRoutine());
    }

    // [MODIFIED] 안전한 머테리얼 스왑 로직
    private IEnumerator WhiteFlashSwapRoutine()
    {
        if (flashTargets == null || flashTargets.Length == 0 || flashMaterial == null)
            yield break;

        // 1. 하얀색(Flash Material)으로 교체
        for (int i = 0; i < flashTargets.Length; i++)
        {
            var r = flashTargets[i];
            if (!r) continue;

            // Awake에서 저장한 원본의 개수만큼 flashMaterial 배열 생성
            int matCount = cachedOriginalMaterials[i].Length;
            var temp = new Material[matCount];
            for (int k = 0; k < matCount; k++) temp[k] = flashMaterial;

            r.sharedMaterials = temp;
        }

        // 2. 대기
        yield return new WaitForSeconds(flashDuration);

        // 3. 원본으로 복구
        RestoreOriginalMaterials();

        currentFlashRoutine = null;
    }

    // [ADD] 머테리얼 복구 함수 (코루틴 종료 시 & OnDisable 시 사용)
    private void RestoreOriginalMaterials()
    {
        if (flashTargets == null || cachedOriginalMaterials == null) return;

        for (int i = 0; i < flashTargets.Length; i++)
        {
            var r = flashTargets[i];
            // 인덱스 범위 체크 및 null 체크
            if (r != null && i < cachedOriginalMaterials.Count && cachedOriginalMaterials[i] != null)
            {
                r.sharedMaterials = cachedOriginalMaterials[i];
            }
        }
    }

    // ComputeHitEffectRotation 함수가 잘려있다면 기존 함수 사용하시면 됩니다.
    // 기존에 있던 private Quaternion ComputeHitEffectRotation() ... 코드는 그대로 두세요.
}