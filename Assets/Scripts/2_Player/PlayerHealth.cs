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

    private void ShakeCamera(float strength)
    {
        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCamera(Mathf.Clamp01(strength), 0.2f);
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

        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
        ability.effect?.PlayDoubleShakeAnimation(5, 6); // �� HP

        // ���� ����: Ability.playerNumber ����
        int pn = ability != null ? ability.playerNumber : 0;
        if (pn == MatchResultStore.myPlayerNumber)
        {
            P2PMessageSender.SendMessage(
                DamageMessageBuilder.Build(pn, currentHP));
        }

        if (currentHP <= 0)
        {
            Debug.Log("Lose");
            FindObjectOfType<GameManager>()?.EndGame(MatchResultStore.myPlayerNumber);
        }

        //StartCoroutine(DamageEffectCoroutine());
        hitEffectPending = true;


        float selfShake = 0.15f + Mathf.FloorToInt(amount/10) * 0.08f;
        ShakeCamera(selfShake);
    }

    public void TakeDamage(int damage, PlayerAbility attacker, Vector3 sourceWorldPos)
    {
        pendingSourcePos = sourceWorldPos;
        TakeDamage(damage, attacker);
    }

    private IEnumerator DamageEffectCoroutine()
    {
        isInvincible = true;

        // �Ͼ�� ����
        // yield return

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
        int prev = currentHP;
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
        ability.effect?.PlayDoubleShakeAnimation(5, 6); // ��� HP

        if (currentHP < prev)
        {
            hitEffectPending = true;
            pendingSourcePos = null; // �׽�Ʈ��
        }

        if (currentHP <= 0)
        {
            Debug.Log("Lose");

            int winnerPlayerNum = MatchResultStore.myPlayerNumber == 1 ? 2 : 1;
            FindObjectOfType<GameManager>()?.EndGame(winnerPlayerNum);
        }

        int dealt = Mathf.Max(0, prev - currentHP);
        if (dealt > 0)
        {
            float oppHitShake = 0.09f + Mathf.FloorToInt(dealt/10f) * 0.08f;
            ShakeCamera(oppHitShake);
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
        float fy;

        if (pendingSourcePos.HasValue)
        {
            // �ҽ� ���� "�ݴ�" ������ X ��ȣ�� ����/�� �Ǵ�
            Vector3 awayDir = (transform.position - pendingSourcePos.Value);
            fy = (awayDir.x >= 0f) ? 90f : -90f;      // ������=+90, ����=-90
        }
        else
        {
            // �ҽ� ������: "���� ���� ������ �ݴ�"
            bool selfRight = Vector3.Dot(transform.forward, Vector3.right) >= 0f;
            fy = selfRight ? -90f : 90f; // �ݴ�� ����
        }

        return Quaternion.Euler(-9f, fy, -90f);     // X=-90, Z=-90 ����
    }
}