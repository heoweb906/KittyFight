using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_Finisher : Passive
{
    [Range(0f, 1f)] public float threshold = 0.10f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    protected override void Subscribe(AbilityEvents e)
        => e.OnBeforeDealDamage += OnBeforeDealDamage;
    protected override void Unsubscribe(AbilityEvents e)
        => e.OnBeforeDealDamage -= OnBeforeDealDamage;

    void OnBeforeDealDamage(ref int dmg, GameObject victimGO)
    {
        var hp = victimGO.GetComponentInParent<PlayerHealth>();
        if (!hp) return;

        int cur = hp.CurrentHP;
        int max = hp.MaxHP;

        if (max <= 0 || cur <= 0) return;
        dmg = max;

        Instantiate(
            effectPrefab,
            transform.position,
            Quaternion.Euler(-90f, 0f, 0f)
        );
    }
}