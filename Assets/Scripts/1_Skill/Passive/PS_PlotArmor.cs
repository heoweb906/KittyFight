using UnityEngine;

public class PS_PlotArmor : Passive
{
    [Header("ÁãÅÐ °©¿Ê ¼³Á¤")]
    [Range(0f, 1f)]
    [Tooltip("ÇÇ°Ý ¹«½Ã È®·ü (0~1)")]
    public float ignoreChance = 0.25f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    [Header("Ä«¸Þ¶ó ¿¬Ãâ")]
    public float shakeAmount;
    public float shakeDuration;

    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);
        e.OnBeforeTakeDamage += OnBeforeTakeDamage;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnBeforeTakeDamage -= OnBeforeTakeDamage;
        base.Unsubscribe(e);
    }

    private void OnBeforeTakeDamage(ref int dmg, GameObject attackerObj)
    {
        if (dmg <= 0) return;
        if (Random.value < ignoreChance)
        {
            dmg = 0;
            Instantiate(
                effectPrefab,
                transform.position,
                Quaternion.Euler(-90f, 0f, 0f)
            );
        }

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }
}