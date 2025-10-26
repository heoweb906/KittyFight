using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
public class SK_Repeek : Skill
{
    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (playerAbility == null) return;
        if (!playerAbility.TryGetLastActionType(out var lastType)) return;
        if (!objSkillEntity) return;
        var rot = Quaternion.LookRotation(direction, Vector3.up);
        Vector3 spawnPos = origin + rot * Vector3.right * 1f;
        var shock = Instantiate(objSkillEntity, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
        var target = playerAbility.GetSkill(lastType);
        if (target == null || target is SK_Repeek) return;
        float range = target.GetAimRange();
        Vector3 newOrigin, newDir;
        AttackUtils.GetAimPointAndDirection(playerAbility.transform, range, out newOrigin, out newDir);
        target.Execute(newOrigin, newDir);
    }
}