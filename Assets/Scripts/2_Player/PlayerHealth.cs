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

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, null);
    }

    public void TakeDamage(int damage, PlayerAbility attacker)
    {
        if (isInvincible) return;

        int amount = Mathf.Max(0, damage);

        // 공격자
        attacker?.events?.EmitBeforeDealDamage(ref amount, this.gameObject);

        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);

        // 권위 판정: Ability.playerNumber 기준
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

        StartCoroutine(DamageEffectCoroutine());
    }

    private IEnumerator DamageEffectCoroutine()
    {
        isInvincible = true;
        if (rend != null) rend.material.color = Color.red;

        yield return new WaitForSeconds(invincibleTime);

        if (rend != null) rend.material.color = originalColor;
        isInvincible = false;
    }

    // 원격 HP 확정값 반영
    public void RemoteSetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
        if (currentHP <= 0)
        {
            Debug.Log("Lose");

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
}