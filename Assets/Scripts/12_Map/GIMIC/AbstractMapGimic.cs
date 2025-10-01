using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMapGimic : MonoBehaviour
{
    public GameManager gameManger;
    protected bool bIsActive = false;
    
    public virtual void OnGimicStart()
    {
        bIsActive = true;
    }


    public virtual void OnGimmickUpdate() { }


    public virtual void OnGimicEnd()
    {
        bIsActive = false;
    }

}
