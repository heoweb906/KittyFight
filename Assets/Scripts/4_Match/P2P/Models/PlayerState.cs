using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public int player; // 1 or 2
    public Vector3 position;
    public float rotationY;

    public string anim; // jump ÀÌÆåÆ®¿ë
    public bool walking;

    public bool isGround;
    public bool isRun;
    public bool isHanging;
    public float speedY;
    public bool isShock;
}