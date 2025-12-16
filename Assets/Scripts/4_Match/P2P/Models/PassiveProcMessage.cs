using UnityEngine;

public enum PassiveProcType
{
    FxOnly = 0,        // 이펙트만 재생
    Spawn = 1,         // 오브젝트/이펙트 Instantiate 계열
    Value = 2,         // 수치(이속/쿨감 등) 적용 + 필요시 FX
    Custom = 99
}

[System.Serializable]
public class PassiveProcMessage
{
    // 누가 보냈는지 (루프 방지용)
    public int player;

    // 어떤 패시브가 발동했는지
    public int passiveId;

    // 메시지 성격(연출/스폰/수치 등)
    public int procType;

    // 위치/방향(필요할 때만 사용)
    public float px, py, pz;
    public float dx, dy, dz;

    // 범용 값 슬롯(패시브별로 1~2개면 대부분 커버됨)
    public int i0;
    public float f0;

    // 필요하면 대상(상대 플레이어 번호 등)을 넣을 수 있게 해둠
    public int targetPlayer;
}