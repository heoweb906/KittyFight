using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Map_Iterator : MonoBehaviour
{
    [Header("Settings")]
    public GameObject movingObject;
    public Transform targetA;
    public Transform targetB;
    public float maxSpeed = 5f;
    public float acceleration = 2f;
    public float deceleration = 2f;
    public float stopDistance = 1f;
    private Transform currentTarget;
    private float currentSpeed = 0f;
    void Start()
    {
        if (movingObject == null || targetA == null || targetB == null) return;
        currentTarget = targetA;
    }
    void FixedUpdate()
    {
        if (movingObject == null || targetA == null || targetB == null) return;
        float distanceToTarget = Vector3.Distance(movingObject.transform.position, currentTarget.position);
        if (distanceToTarget > stopDistance)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);
        }
        else
        {
            currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.fixedDeltaTime, 0f);
        }
        movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, currentTarget.position, currentSpeed * Time.fixedDeltaTime);
        if (currentSpeed <= 0f && distanceToTarget <= stopDistance)
        {
            currentTarget = (currentTarget == targetA) ? targetB : targetA;
        }
    }
}