using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AB_FeatherShot : AB_HitboxBase
{
    [Header("����/����")]
    public int damage = 5;

    [Header("����")]
    public bool destroyOnHit = true;


    [Header("����Ʈ")]
    public VFX_BasicProjectile particle_Line;
    public GameObject particle_Destroy;
    public GameObject obj_Featfer;



    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage, ownerAbility, transform.position);
        if (destroyOnHit && this) Destroy(gameObject);
    }

    protected override void OnEnvironmentHit(Collider other)
    {
        OnDisappearEffect();

        Destroy(gameObject);
    }




    private void OnDisappearEffect()
    {
        particle_Line.StopTrailGeneration();
        GameObject obj = particle_Line.GetComponent<GameObject>();
        particle_Line.transform.SetParent(null);


        if (particle_Destroy != null)
        {
            GameObject effect = Instantiate(particle_Destroy, transform.position, transform.rotation);
            effect.transform.localScale = Vector3.one * 2f;
            effect.transform.SetParent(null);
        }
    }

    private void OnDestroy()
    {
        obj_Featfer.transform.SetParent(null);
        ProbPiece piece = obj_Featfer.GetComponent<ProbPiece>();
        piece.OnThisPiece();
    }
}