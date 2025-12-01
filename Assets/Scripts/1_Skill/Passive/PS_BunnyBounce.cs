using UnityEngine;

public class PS_BunnyBounce : Passive
{
    [Header("점프 충격파 프리팹")]
    public GameObject objEntity;

    [Header("스폰 위치/수명 설정")]
    [Tooltip("발밑 기준 Y 오프셋 (살짝 아래/위로 조정용)")]
    public float spawnYOffset = 0.0f;

    private Transform feetAnchor;

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
        if (objEntity == null || ability == null) return;

        Vector3 pos = ability.transform.position;
        pos.y += spawnYOffset;

        var go = Object.Instantiate(objEntity, pos, Quaternion.identity);

        var hb = go.GetComponent<AB_HitboxBase>();
        if (hb != null)
        {
            hb.Init(ability);
        }
    }
}