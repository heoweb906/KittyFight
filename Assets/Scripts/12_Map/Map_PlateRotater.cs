using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_PlateRotater : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] blocks;
    public Transform[] waypoints;
    public float moveSpeed = 5f;

    private float totalPathLength;
    private float[] blockPositions;

    void Start()
    {
        if (blocks.Length == 0 || waypoints.Length < 2) return;

        CalculatePathLength();
        InitializeBlockPositions();
        StartCoroutine(MoveBlocks());
    }

    void CalculatePathLength()
    {
        totalPathLength = 0f;
        for (int i = 0; i < waypoints.Length; i++)
        {
            int nextIndex = (i + 1) % waypoints.Length;
            totalPathLength += Vector3.Distance(waypoints[i].position, waypoints[nextIndex].position);
        }
    }

    void InitializeBlockPositions()
    {
        blockPositions = new float[blocks.Length];
        float spacing = totalPathLength / blocks.Length;

        for (int i = 0; i < blocks.Length; i++)
        {
            blockPositions[i] = i * spacing;
        }
    }

    IEnumerator MoveBlocks()
    {
        while (true)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blockPositions[i] = (blockPositions[i] + moveSpeed * Time.deltaTime) % totalPathLength;
                blocks[i].transform.position = GetPositionOnPath(blockPositions[i]);
            }
            yield return null;
        }
    }

    Vector3 GetPositionOnPath(float distance)
    {
        float currentDistance = 0f;

        for (int i = 0; i < waypoints.Length; i++)
        {
            int nextIndex = (i + 1) % waypoints.Length;
            float segmentLength = Vector3.Distance(waypoints[i].position, waypoints[nextIndex].position);

            if (distance <= currentDistance + segmentLength)
            {
                float t = (distance - currentDistance) / segmentLength;
                return Vector3.Lerp(waypoints[i].position, waypoints[nextIndex].position, t);
            }

            currentDistance += segmentLength;
        }

        return waypoints[0].position;
    }
}
