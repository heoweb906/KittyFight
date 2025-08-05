using UnityEngine;

public static class ProjectileMessageBuilder
{
    public static string Build(Vector3 pos, Vector3 dir, float speed, int player)
    {
        var msg = new ProjectileMessage
        {
            x = pos.x,
            y = pos.y,
            z = pos.z,
            dx = dir.x,
            dy = dir.y,
            dz = dir.z,
            speed = speed,
            player = player
        };

        return "[PROJECTILE]" + JsonUtility.ToJson(msg);
    }
}