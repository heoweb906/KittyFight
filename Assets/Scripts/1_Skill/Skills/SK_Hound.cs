using UnityEngine;

public class SK_Hound : Skill
{
    [Header("슬로우 설정")]
    [SerializeField] private float slowDuration = 3f;
    [SerializeField, Range(0f, 100f)]
    private float slowPercent = 50f; 

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;

        var gm = FindObjectOfType<GameManager>();
        if (!gm) return;

        int selfNum = playerAbility.playerNumber;

        PlayerAbility enemy = null;

        if (gm.playerAbility_1 && gm.playerAbility_2)
        {
            enemy = (gm.playerAbility_1.playerNumber == selfNum)
                ? gm.playerAbility_2
                : gm.playerAbility_1;
        }

        if (enemy != null)
        {
            ApplySlow(enemy.gameObject);
        }
    }

    private void ApplySlow(GameObject target)
    {
        if (!target) return;

        var slow = target.GetComponent<SlowStatus>();
        if (!slow) slow = target.AddComponent<SlowStatus>();

        slow.ApplySlowPercent(slowPercent, slowDuration);
    }
}