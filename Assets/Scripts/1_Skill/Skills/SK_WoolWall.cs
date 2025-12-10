using UnityEngine;

public class SK_WoolWall : Skill
{
    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!playerAbility) return;
        if (!objSkillEntity) return;

        var go = Instantiate(objSkillEntity, origin, Quaternion.identity);
        Instantiate(
            effectPrefab,
            origin,
            Quaternion.identity,
            go.transform
        );

      


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

        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
    }
}