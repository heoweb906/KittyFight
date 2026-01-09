using UnityEngine;

public class HealBox : AB_HitboxBase
{
    [Header("Heal Settings")]
    public int healAmount = 30;

    protected override void Awake()
    {
        base.Awake();
        // 맵 아이템이므로 중립 상태
        bMiddleState = true;
    }

    protected override void ApplyEffects(PlayerHealth victimHealth, Collider victimCollider)
    {
        // 1. PlayerHealth가 있는 오브젝트에서 PlayerAbility를 먼저 찾습니다.
        // (PlayerAbility가 메인 매니저 역할을 하므로 이를 통해 접근)
        PlayerAbility victimAbility = victimHealth.GetComponent<PlayerAbility>();

        if (victimAbility != null)
        {
            // 2. PlayerAbility가 들고 있는 Health 프로퍼티를 통해 접근
            if (victimAbility.Health != null)
            {
                victimAbility.Health.Heal(healAmount);

                this.gameObject.SetActive(false);
            }
        }
    }
}