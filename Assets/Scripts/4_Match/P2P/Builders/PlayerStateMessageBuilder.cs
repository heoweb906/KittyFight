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
        float speedY
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
            speedY = speedY
        };
        return "[MOVE]" + JsonUtility.ToJson(s);
    }
}