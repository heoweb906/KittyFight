using UnityEngine;

public class PS_FluffyHindrance : Passive
{
    [Header("슬로우 설정")]
    public float slowDuration = 4f;
    [Range(0.01f, 1f)]
    public float slowMultiplier = 0.5f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    protected override void Subscribe(AbilityEvents e)
    {
        e.OnDealtDamage += OnDealtDamage;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnDealtDamage -= OnDealtDamage;
    }

    private void OnDealtDamage()
    {
        var victimAbility = FindOpponentAbility();
        if (victimAbility == null) return;

        var debuff = victimAbility.GetComponent<FluffySlowDebuff>();
        if (debuff == null)
            debuff = victimAbility.gameObject.AddComponent<FluffySlowDebuff>();

        debuff.Apply(victimAbility, slowDuration, slowMultiplier, effectPrefab);
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

        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);

        return null;
    }
}