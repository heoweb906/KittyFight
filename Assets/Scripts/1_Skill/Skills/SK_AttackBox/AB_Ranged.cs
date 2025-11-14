using RayFire;
using UnityEngine;
using DG.Tweening;
public class AB_Ranged : AB_HitboxBase
{
    [Header("피해/제어")]
    public int damage = 20;

    [Header("동작")]
    public bool destroyOnHit = true;

    [Header("꾸미기용")]
    public Material mat;

    public VFX_BasicProjectile particle_Line;

    public GameObject obj_Bone;
    public GameObject particle_Destroy;
    public GameObject[] objs_Piece;

    [SerializeField] private Material p1Material;
    [SerializeField] private Material p2Material;

    void Start()
    {
        if (obj_Bone != null)
        {
            obj_Bone.transform.DORotate(new Vector3(0, 360f, 0), 1f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(UpdateType.Fixed);
        }
    }

    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        victim.TakeDamage(damage, ownerAbility, transform.position);

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
        obj_Bone.transform.DOKill();

        particle_Line.StopTrailGeneration();
        GameObject obj = particle_Line.GetComponent<GameObject>();
        particle_Line.transform.SetParent(null);

        if (particle_Destroy != null)
        {
            GameObject effect = Instantiate(particle_Destroy, transform.position, transform.rotation);
            effect.transform.localScale = Vector3.one * 2.5f;
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


    public void ChangeMaterial()
    {

        foreach (GameObject piece in objs_Piece)
        {
            if (piece != null)
            {
                Renderer renderer = piece.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = p2Material;
                }
            }
        }
    }


}