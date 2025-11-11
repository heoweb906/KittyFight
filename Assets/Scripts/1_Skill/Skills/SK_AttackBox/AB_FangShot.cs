using RayFire;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AB_FangShot : AB_HitboxBase
{
    [Header("독(DoT) 파라미터")]
    public float duration = 3f;
    public float tickInterval = 1f;
    public int damagePerTick = 7;

    [Header("동작")]
    public bool destroyOnHit = true;

    [Header("이펙트")]
    public GameObject objEffect_Use;
    public GameObject objEffect_Hit;
    public GameObject[] objs_Piece;


    protected override void StartEffect()
    {
        if (objEffect_Use != null)
        {
            GameObject effect = Instantiate(objEffect_Use, transform);
            effect.transform.localPosition = Vector3.zero;
            // effect.transform.localRotation = Quaternion.Euler(00, 0, 0);
            effect.transform.localScale = new Vector3(2f, 2f, 2f);
            effect.transform.SetParent(null);
        }
    }



    protected override void ApplyEffects(PlayerHealth victim, Collider victimCollider)
    {
        var poison = victim.GetComponent<PoisonDoT>();
        if (!poison) poison = victim.gameObject.AddComponent<PoisonDoT>();
        poison.ApplyPoison(duration, tickInterval, damagePerTick);

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
        if (objEffect_Hit != null)
        {
            GameObject effect = Instantiate(objEffect_Hit, transform);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.Euler(0, 0, 0);
            effect.transform.localScale = new Vector3(5f, 5f, 5f);
            effect.transform.SetParent(null);
        }


        Explode(0.5f, 5f);
        foreach (GameObject piece in objs_Piece)
        {
            if (piece != null)
            {
                piece.transform.SetParent(null);
            }
        }



    }



}