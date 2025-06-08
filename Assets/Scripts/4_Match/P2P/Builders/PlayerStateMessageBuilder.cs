using UnityEngine;

public static class PlayerStateMessageBuilder
{
    public static string Build(Vector3 position, float rotationY, int player, string anim)
    {
        PlayerState state = new PlayerState
        {
            position = position,
            rotationY = rotationY,
            player = player,
            anim = anim
        };
        return "[MOVE]" + JsonUtility.ToJson(state);
    }
}