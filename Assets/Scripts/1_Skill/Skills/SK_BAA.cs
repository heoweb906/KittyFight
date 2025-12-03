using UnityEngine;

public class SK_BAA : Skill
{
    [SerializeField] private float stunDuration = 2f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private GameObject stunEffectPrefab;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;

        Instantiate(
            effectPrefab,
            playerAbility.gameObject.transform.position,
            Quaternion.identity,
            playerAbility.gameObject.transform
        );

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

        ApplyStun(playerAbility.gameObject, stunDuration, false);
        if (enemy != null)
            ApplyStun(enemy.gameObject, stunDuration, true);
    }

    private void ApplyStun(GameObject target, float duration, bool playShockAnim)
    {
        if (!target) return;

        var stun = target.GetComponent<StunStatus>();
        if (!stun) stun = target.AddComponent<StunStatus>();
        stun.ApplyStun(duration, playShockAnim);

        Instantiate(
            stunEffectPrefab,
            target.transform.position,
            Quaternion.identity,
            target.transform
        );
    }
}