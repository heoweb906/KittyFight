using System.Collections;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP (���� ������)")]
    [SerializeField] private int maxHP = 90;
    [SerializeField] private float invincibleTime = 1.0f;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    public event Action<int, int> OnHPChanged; // (current, max)

    private int currentHP;
    private bool isInvincible = false;

    private Renderer rend;
    private Color originalColor;

    // Ability�� ���� playerNumber�� ��ȸ
    private PlayerAbility ability;
    public AbilityEvents events;


    private bool hitEffectPending;   // ������ ����

    [Header("Effects")]
    [SerializeField] public GameObject hitEffectPrefab;
    private Vector3? pendingSourcePos;        // ���Ÿ�/�ٰŸ����� �Ѱ��ִ� ���� �ҽ� ��ġ
    private Vector3? pendingPunchDir;

    [SerializeField] private Renderer[] flashTargets;   // group6_polySurface11, Right_Ear, Left_Ear
    [SerializeField] private Material flashMaterial;   // ���� ���׸��� (Toony Colors Pro 2 ��)
    [SerializeField] private float flashDuration = 0.2f;


    private void Awake()
    {
        currentHP = maxHP;

        rend = GetComponent<Renderer>();
        if (rend != null) originalColor = rend.material.color;

        ability = GetComponent<PlayerAbility>();

        // �ʱⰪ �˸�
        OnHPChanged?.Invoke(currentHP, maxHP);

        if (!events && ability) events = ability.events;
        if (!events) events = GetComponent<AbilityEvents>();  // ����
    }

    private void Update()
    {
        if (hitEffectPending && isActiveAndEnabled && gameObject.activeInHierarchy && !isInvincible)
        {
            hitEffectPending = false;
            StartCoroutine(DamageEffectCoroutine()); // ���� �����忡�� �����ϰ� ����
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
        Vector3 away = transform.position - sourceWorldPos; // �ҽ��泪
        Vector3 dir = new Vector3(away.x, away.y, 0f);
        if (dir.sqrMagnitude > 1e-8f) return dir.normalized;

        // ����: �ٶ󺸴� ��/���
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

        // �׽�Ʈ��
        //pendingSourcePos = null;

        int amount = Mathf.Max(0, damage);

        // ������
        attacker?.events?.EmitBeforeDealDamage(ref amount, this.gameObject);
        if (amount <= 0) return;

        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
        ability.effect?.PlayDoubleShakeAnimation(5, 6); // �� HP

        float selfShake = 0.15f + Mathf.FloorToInt(amount / 10) * 0.08f;

        Vector3 dir = pendingPunchDir
            ?? (Vector3.Dot(transform.forward, Vector3.right) >= 0f ? Vector3.right : Vector3.left);
        ShakeCameraPunch(selfShake, dir);
        pendingPunchDir = null;

        // ���� ����: Ability.playerNumber ����
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

        // �Ͼ�� ����
        //yield return
        StartCoroutine(WhiteFlashSwapOnce());

        Quaternion rot = ComputeHitEffectRotation();

        if (hitEffectPrefab)
            Instantiate(hitEffectPrefab, transform.position, rot);

        // ����
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;

        // ����� �� ����
        pendingSourcePos = null;
    }

    // === [ADD] ���׸��� ���� �ڷ�ƾ (sharedMaterials�� ��ü/����) ===
    private IEnumerator WhiteFlashSwapOnce()
    {
        if (flashTargets == null || flashTargets.Length == 0 || flashMaterial == null)
            yield break;

        // ������ ���� �� �������� sharedMaterials �迭�� �״�� ���
        var originals = new System.Collections.Generic.List<Material[]>();
        originals.Capacity = flashTargets.Length;

        for (int i = 0; i < flashTargets.Length; i++)
        {
            var r = flashTargets[i];
            if (!r) { originals.Add(null); continue; }

            var prev = r.sharedMaterials;                  // �״�� ���� (�ڻ� ���۷���)
            originals.Add(prev);

            // ���� ���̷� �� ������ flashMaterial�� ä���� ��ü
            int n = prev != null ? prev.Length : 1;
            var temp = new Material[n];
            for (int k = 0; k < n; k++) temp[k] = flashMaterial;
            r.sharedMaterials = temp;                      // �� ���� (�ڻ� ���۷����� �ٲ�)
        }

        yield return new WaitForSeconds(flashDuration);

        // ���󺹱�
        for (int i = 0; i < flashTargets.Length; i++)
        {
            var r = flashTargets[i];
            if (!r || originals[i] == null) continue;
            r.sharedMaterials = originals[i];
        }
    }




    // ���� HP Ȯ���� �ݿ�
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
                if (xAngle < 0f) xAngle += 360f;  // 0~360�Ʒ� ����ȭ
            }
            else
            {
                xAngle = 0f; // ���� �ڸ��� �⺻��(������)
            }
        }
        else
        {
            xAngle = 0f; // �ҽ� ���� ������ �⺻(������)
        }

        return Quaternion.Euler(xAngle, fixedY, fixedZ);
    }
}