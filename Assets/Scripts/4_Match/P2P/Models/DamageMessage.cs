using UnityEngine;

[System.Serializable]
public class DamageMessage
{
    public int targetPlayer;   // 피격된 플레이어 번호
    public int hp;             // 남은 HP
    public int attackPlayer;
    public bool hasSource;     // 좌표 포함 여부 (간단 가드)
    public Vector3 sourceWorldPos;
}