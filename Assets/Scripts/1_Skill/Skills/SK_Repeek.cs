using UnityEngine;

public class SK_Repeek : Skill
{
    private void Awake()
    {
        coolTime = 6.0f;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (playerAbility == null) return;
        if (!playerAbility.TryGetLastActionType(out var lastType)) return;

        var target = playerAbility.GetSkill(lastType);
        if (target == null || target is SK_Repeek) return;

        float range = target.GetAimRange();
        Vector3 newOrigin, newDir;
        AttackUtils.GetAimPointAndDirection(playerAbility.transform, range, out newOrigin, out newDir);

        target.Execute(newOrigin, newDir);
    }
}