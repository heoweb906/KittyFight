using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 5;
    public float invincibleTime = 1.0f;
    public int playerNumber;

    private int currentHP;
    private bool isInvincible = false;

    private Renderer rend;
    private Color originalColor;

    private void Awake()
    {
        currentHP = maxHP;
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;

        InGameUIController.Instance?.InitializeHP(playerNumber, maxHP);
        InGameUIController.Instance?.UpdateHP(playerNumber, currentHP);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        InGameUIController.Instance?.UpdateHP(playerNumber, currentHP);

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

    public void RemoteSetHP(int hp)
    {
        currentHP = hp;
        InGameUIController.Instance?.UpdateHP(playerNumber, currentHP);
    }

    public void ResetHealth()
    {
        currentHP = maxHP;
        InGameUIController.Instance?.UpdateHP(playerNumber, currentHP);
    }
}