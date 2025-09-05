using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public int player; // 1 or 2
    public Vector3 position;
    public float rotationY;
    public string anim;
    public bool walking;
    public double time;
}