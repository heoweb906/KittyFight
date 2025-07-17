using UnityEngine;

public class ObjectSpawnHandler : IP2PMessageHandler
{
    readonly GameObject hitboxPrefab;
    readonly float life;
    readonly int myPlayerNumber;

    public ObjectSpawnHandler(GameObject localPlayer,
                              GameObject opponentPlayer)
    {
        hitboxPrefab = opponentPlayer.GetComponent<PlayerAttack>().hitboxPrefab;
        life = opponentPlayer.GetComponent<PlayerAbility>().AttackHitboxDuration;
        myPlayerNumber = MatchResultStore.myPlayerNumber;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[SPAWN]");

    public void Handle(string msg)
    {
        var data = JsonUtility.FromJson<SpawnMessage>(msg.Substring(7));
        if (data.player == myPlayerNumber) return;

        Vector3 pos = new Vector3(data.x, data.y, data.z);
        Quaternion rot = new Quaternion(data.qx, data.qy, data.qz, data.qw);

        Object.Destroy(Object.Instantiate(hitboxPrefab, pos, rot), life);
        InGameUIController.Instance?.StartSkillCooldown(data.player, 1);
    }
}