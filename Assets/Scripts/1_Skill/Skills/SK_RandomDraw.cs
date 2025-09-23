using UnityEngine;

public class SK_RandomDraw : Skill
{
    private void Awake()
    {
        coolTime = 4.5f;
    }

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;

    }
}
