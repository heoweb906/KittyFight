using UnityEngine;

public class PS_PlotArmor : Passive
{
    public override int PassiveId => 102;

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
        if (!IsAuthority) return;
        if (dmg <= 0) return;
        if (Random.value < ignoreChance)
        {
            dmg = 0;

            PlayFx(transform.position);
            SendProc(
                PassiveProcType.FxOnly,
                pos: transform.position,
                dir: Vector3.up,
                i0: 0,
                f0: 0f,
                targetPlayer: 0
            );

            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
        }

    }

    private void PlayFx(Vector3 pos)
    {
        if (!effectPrefab) return;

        GameObject instance = Instantiate(
            effectPrefab,
            pos,
            Quaternion.Euler(-90f, 0f, 0f)
        );

        ability.PlaySFX(audioClip);


        // [¼öÁ¤] ºÎ¸ð °ü°è ÇØÁ¦
        instance.transform.SetParent(null);
    }

    public override void RemoteExecute(PassiveProcMessage msg)
    {
        var pos = new Vector3(msg.px, msg.py, msg.pz);
        PlayFx(pos);
    }
}