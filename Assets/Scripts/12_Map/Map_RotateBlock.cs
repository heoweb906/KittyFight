using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Map_RotateBlock : MonoBehaviour
{
    [Header("Settings")]
    public float rotateSpeed = 50f;
    void FixedUpdate()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.fixedDeltaTime);
    }
}