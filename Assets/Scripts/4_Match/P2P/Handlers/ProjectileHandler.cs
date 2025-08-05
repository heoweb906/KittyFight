using UnityEngine;

public class ProjectileHandler : IP2PMessageHandler
{
    private readonly GameObject projectilePrefab;
    private readonly int myPlayerNumber;

    public ProjectileHandler(GameObject opponentPlayer)
    {
        var attack = opponentPlayer.GetComponent<PlayerAttack>();
        projectilePrefab = attack.projectilePrefab;

        myPlayerNumber = MatchResultStore.myPlayerNumber;
    }

    public bool CanHandle(string msg) => msg.StartsWith("[PROJECTILE]");

    public void Handle(string msg)
    {
        var data = JsonUtility.FromJson<ProjectileMessage>(msg.Substring(12));
        if (data.player == myPlayerNumber) return;

        Debug.Log(data);

        Vector3 pos = new Vector3(data.x, data.y, data.z);
        Vector3 dir = new Vector3(data.dx, data.dy, data.dz);
        Quaternion rot = Quaternion.LookRotation(dir);

        GameObject proj = Object.Instantiate(projectilePrefab, pos, rot);
        proj.GetComponent<Rigidbody>().velocity = dir * data.speed;

        InGameUIController.Instance?.StartSkillCooldown(data.player, 2);
    }
}
