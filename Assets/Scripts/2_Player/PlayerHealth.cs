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
        TakeDamage(damage, null);
    }

    public void TakeDamage(int damage, PlayerAbility attacker)
    {
        if (isInvincible) return;

        int amount = Mathf.Max(0, damage);

        // ������
        attacker?.events?.EmitBeforeDealDamage(ref amount, this.gameObject);

        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);

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

    private IEnumerator DamageEffectCoroutine()
    {
        isInvincible = true;
        if (rend != null) rend.material.color = Color.white;

        if (hitEffectPrefab != null)
        {
            Instantiate(
                hitEffectPrefab,
                transform.position,
                Quaternion.Euler(-90f, 0f, 0f)
            );
        }

        yield return new WaitForSeconds(invincibleTime);

        if (rend != null) rend.material.color = originalColor;
        isInvincible = false;
    }

    // ���� HP Ȯ���� �ݿ�
    public void RemoteSetHP(int hp)
    {
        int prev = currentHP;
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);

        if (currentHP < prev)
            hitEffectPending = true;

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
}