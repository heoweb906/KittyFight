using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AB_Eggsplosion : AB_HitboxBase
{
    [Header("Blind")]
    [Min(0.01f)] public float blindnessDuration = 2.5f;

    [Header("µø¿€")]
    public bool destroyOnHit = true;

    [Header("¿Ã∆Â∆Æ")]
    public VFX_BasicProjectile particle_Line;

    public GameObject obj_Egg;
    public GameObject objEffect_Hit;
    public GameObject[] objs_Piece;


    void Start()
    {
        if (obj_Egg != null)
        {
            obj_Egg.transform.DORotate(new Vector3(360, 0f, 0), 1f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(UpdateType.Fixed);
        }
    }


    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var blind = victim.GetComponent<BlindStatus>();
        if (!blind) blind = victim.gameObject.AddComponent<BlindStatus>();

        blind.ApplyBlind(blindnessDuration);

        if (destroyOnHit && this)
        {
            OnDisappearEffect();
            Destroy(gameObject);
        }
    }


    protected override void OnEnvironmentHit(Collider other)
    {
        OnDisappearEffect();

        Destroy(gameObject);
    }

    private void OnDisappearEffect()
    {
        obj_Egg.transform.DOKill();

        particle_Line.StopTrailGeneration();
        GameObject obj = particle_Line.GetComponent<GameObject>();
        particle_Line.transform.SetParent(null);



        if (objEffect_Hit != null)
        {
            GameObject effect = Instantiate(objEffect_Hit, transform);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.Euler(0, 0, 0);
            effect.transform.localScale = new Vector3(2f, 2f, 2f);
            effect.transform.SetParent(null);
        }


        Explode(0.7f, 5f);
        foreach (GameObject piece in objs_Piece)
        {
            if (piece != null)
            {
                piece.transform.SetParent(null);
            }
        }

      
    }
}