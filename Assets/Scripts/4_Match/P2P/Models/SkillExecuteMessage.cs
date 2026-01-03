using UnityEngine;

[System.Serializable]
public class SkillExecuteMessage
{
    public int player;
    public int skillType;

    // origin
    public float ox, oy, oz;

    // direction
    public float dx, dy, dz;

    // RandomDraw Àü¿ë
    public bool isStateOnly;
    public int randomPickIndex;
}