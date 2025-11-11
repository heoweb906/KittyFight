using UnityEngine;

public static class PlayerStateMessageBuilder
{
    public static string Build(
        Vector3 position,
        float rotationY,
        int playerNumber,
        string animEvent,
        bool walking,
        bool isGround,
        bool isRun,
        bool isHanging,
        float speedY,
        bool isShock
    )
    {
        var s = new PlayerState
        {
            player = playerNumber,
            position = position,
            rotationY = rotationY,
            anim = animEvent,
            walking = walking,
            isGround = isGround,
            isRun = isRun,
            isHanging = isHanging,
            speedY = speedY,
            isShock = isShock
        };
        return "[MOVE]" + JsonUtility.ToJson(s);
    }
}