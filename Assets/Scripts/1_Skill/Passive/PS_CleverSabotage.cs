using UnityEngine;
public class PS_CleverSabotage : Passive
{
    [Header("쿨타임 증가 설정")]
    [Tooltip("패시브 1개당 상대 스킬 쿨타임에 더해질 시간(초)")]
    public float extraCooldownPerStack = 1.5f;

    public override void OnEquip(PlayerAbility a)
    {
        base.OnEquip(a);
        ModifyOpponentStack(+1);
    }

    public override void OnUnequip()
    {
        ModifyOpponentStack(-1);
        base.OnUnequip();
    }

    private void ModifyOpponentStack(int deltaStack)
    {
        if (ability == null || deltaStack == 0) return;

        var opponentAbility = FindOpponentAbility();
        if (opponentAbility == null) return;

        var debuff = opponentAbility.GetComponent<CleverSabotageDebuff>();
        if (debuff == null && deltaStack > 0)
        {
            debuff = opponentAbility.gameObject.AddComponent<CleverSabotageDebuff>();
            debuff.Init(opponentAbility, extraCooldownPerStack);
        }

        if (debuff == null) return;

        if (deltaStack > 0)
            debuff.AddStack();
        else
            debuff.RemoveStack();
    }

    private PlayerAbility FindOpponentAbility()
    {
        if (ability == null) return null;

        var gm = GameObject.FindObjectOfType<GameManager>();
        if (gm == null) return null;

        var pa1 = gm.playerAbility_1;
        var pa2 = gm.playerAbility_2;

        if (pa1 == null || pa2 == null)
            return null;

        if (ability == pa1) return pa2;
        if (ability == pa2) return pa1;

        if (ability.playerNumber == 1) return pa2;
        if (ability.playerNumber == 2) return pa1;

        return null;
    }
}