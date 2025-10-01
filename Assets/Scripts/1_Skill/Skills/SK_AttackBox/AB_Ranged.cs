using RayFire;
using UnityEngine;
using System.Collections;
public class AB_Ranged : AB_HitboxBase
{
    [Header("피해/제어")]
    public int damage = 20;
    [Header("꾸미기용")]
    public Material mat;
    public VFX_BasicProjectile particle_Line;
    public RayfireRigid ray;
    public RayfireBomb rayBomb;
    public GameObject obj_Bone;
    public GameObject particle_Destroy;
    void Update()
    {
        if (obj_Bone != null)
        {
            obj_Bone.transform.Rotate(Vector3.up * 360f * Time.deltaTime);
        }
    }
    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage, ownerAbility);
    }
    protected override void OnEnvironmentHit(Collider other)
    {
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<Collider>());
        particle_Line.StopTrailGeneration();
        if (particle_Destroy != null)
        {
            GameObject effect = Instantiate(particle_Destroy, transform.position, transform.rotation);
            effect.transform.localScale = Vector3.one * 1.5f;
        }
        if (rayBomb != null) rayBomb.Explode(1f);
        ray.Demolish();
        if (ray.HasFragments)
        {
            int ignorePlayerLayer = LayerMask.NameToLayer("IgnorePlayer");
            foreach (var fragment in ray.fragments)
            {
                if (fragment != null && fragment.gameObject != null)
                {
                    SetLayerRecursively(fragment.gameObject, ignorePlayerLayer);
                }
            }
        }
        //StartCoroutine(DestroyFragments());
        Destroy(gameObject, 2f);
    }
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    IEnumerator DestroyFragments()
    {
        yield return new WaitForSeconds(1.4f);
        if (ray.HasFragments)
        {
            foreach (var fragment in ray.fragments)
            {
                if (fragment != null && fragment.gameObject != null)
                {
                    Destroy(fragment.gameObject);
                }
            }
        }
    }
}