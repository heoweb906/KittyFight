using UnityEngine;

public class SK_WoolWall : Skill
{
    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;
        if (!objSkillEntity) return;

        var go = Instantiate(objSkillEntity, origin, Quaternion.identity);
        var ab = go.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        var owner = playerAbility.gameObject;
        var wallCols = go.GetComponentsInChildren<Collider>();
        var ownerCols = owner.GetComponentsInChildren<Collider>();

        foreach (var wc in wallCols)
        {
            if (!wc) continue;
            foreach (var oc in ownerCols)
            {
                if (!oc) continue;
                Physics.IgnoreCollision(wc, oc, true);
            }
        }

        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.RegisterRoundObject(go);
    }
}