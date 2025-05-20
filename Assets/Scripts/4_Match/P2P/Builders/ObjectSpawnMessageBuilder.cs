using UnityEngine;

public static class ObjectSpawnMessageBuilder
{
    public static string Build(Vector3 position, int player)
    {
        SpawnMessage msg = new SpawnMessage
        {
            x = position.x,
            y = position.y,
            z = position.z,
            player = player
        };

        return "[SPAWN]" + JsonUtility.ToJson(msg);
    }
}