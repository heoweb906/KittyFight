using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 5;
    public float invincibleTime = 1.0f;
    public int playerNumber;

    public PlayerHealthUI hpUI;

    private int currentHP;
    private bool isInvincible = false;

    private Renderer rend;
    private Color originalColor;

    private void Awake()
    {
        currentHP = maxHP;
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    public void InjectUI(PlayerHealthUI ui)
    {
        hpUI = ui;
        if (hpUI != null)
            hpUI.Initialize(maxHP);
        UpdateHPUI();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        currentHP -= damage;
        UpdateHPUI();

        P2PMessageSender.SendMessage(
            DamageMessageBuilder.Build(playerNumber, currentHP));

        if (currentHP <= 0)
        {
            Debug.Log("Lose");
            GameObject.FindObjectOfType<GameManager>().EndGame();
        }

        StartCoroutine(DamageEffectCoroutine());
    }

    private IEnumerator DamageEffectCoroutine()
    {
        isInvincible = true;
        rend.material.color = Color.red;

        yield return new WaitForSeconds(invincibleTime);

        rend.material.color = originalColor;
        isInvincible = false;
    }

    private void UpdateHPUI()
    {
        if (hpUI != null)
            hpUI.SetHP(currentHP);
    }
    public void RemoteSetHP(int hp)
    {
        currentHP = hp;
        UpdateHPUI();
    }
    public void ResetHealth()
    {
        currentHP = maxHP;
        UpdateHPUI();
    }
}