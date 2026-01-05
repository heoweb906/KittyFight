using UnityEngine;

public class PS_BunnyBounce : Passive
{
    public override int PassiveId => 110;

    [Header("점프 충격파 프리팹")]
    public GameObject objEntity;

    [Header("스폰 위치/수명 설정")]
    [Tooltip("발밑 기준 Y 오프셋 (살짝 아래/위로 조정용)")]
    public float spawnYOffset = 0.0f;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;


    protected override void Subscribe(AbilityEvents e)
    {
        base.Subscribe(e);
        e.OnJump += OnJump;
    }

    protected override void Unsubscribe(AbilityEvents e)
    {
        e.OnJump -= OnJump;
        base.Unsubscribe(e);
    }

    private void OnJump()
    {
        if (!IsAuthority) return;
        if (objEntity == null || ability == null) return;

        Vector3 pos = ability.transform.position;
        pos.y += spawnYOffset;

        SpawnHitboxAndFx(pos, ability);
        SendProc(
            PassiveProcType.Spawn,
            pos: pos,
            dir: Vector3.up
        );

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration);
    }

    private void SpawnHitboxAndFx(Vector3 pos, PlayerAbility ownerAbility)
    {
        var go = Instantiate(objEntity, pos, Quaternion.identity);

        var hb = go.GetComponent<AB_HitboxBase>();
        if (hb != null) hb.Init(ownerAbility);

        if (effectPrefab != null)
        {
            GameObject eff = Instantiate(effectPrefab, pos, Quaternion.Euler(-90f, 0f, 0f));
            eff.transform.SetParent(null); // 부모 관계 해제
        }
        ability.PlaySFX(audioClip);

    }

    public override void RemoteExecute(PassiveProcMessage msg)
    {
        if (objEntity == null || ability == null) return;

        Vector3 pos = new Vector3(msg.px, msg.py, msg.pz);

        SpawnHitboxAndFx(pos, ability);
    }
}