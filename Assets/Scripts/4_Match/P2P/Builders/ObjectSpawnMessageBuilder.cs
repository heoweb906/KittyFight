using UnityEngine;

public static class ObjectSpawnMessageBuilder
{
    public static string Build(Vector3 position, Quaternion rotation, int player)
    {
        SpawnMessage msg = new SpawnMessage
        {
            x = position.x,
            y = position.y,
            z = position.z,

            qx = rotation.x,
            qy = rotation.y,
            qz = rotation.z,
            qw = rotation.w,

            player = player
        };

        return "[SPAWN]" + JsonUtility.ToJson(msg);
    }
}