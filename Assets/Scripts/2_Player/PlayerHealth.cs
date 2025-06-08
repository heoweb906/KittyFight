using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 5;
    public float invincibleTime = 1.0f;
    public TMP_Text hpText;
    public int playerNumber;

    private int currentHP;
    private bool isInvincible = false;

    private Renderer rend;
    private Color originalColor;

    public Transform hpUITransform;

    private void Awake()
    {
        currentHP = maxHP;
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
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
        if (hpText != null)
            hpText.text = currentHP.ToString();
    }
    public void RemoteSetHP(int hp)
    {
        currentHP = hp;
        UpdateHPUI();
    }

    private void LateUpdate()
    {
        if (hpUITransform != null)
        {
            hpUITransform.rotation = Quaternion.LookRotation(hpUITransform.position - Camera.main.transform.position);
        }
    }
}