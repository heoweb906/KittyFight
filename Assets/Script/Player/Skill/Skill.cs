using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float fCoolTime;

    public virtual void Activate(PlayerAbility _ability)
    {
        Debug.Log("Base Skill Activated");
    }
}