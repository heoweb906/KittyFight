using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Skill
{
    public override void Execute(Vector3 origin, Vector3 direction)
    {
        Debug.Log("Fireball launched!");
    }
}
