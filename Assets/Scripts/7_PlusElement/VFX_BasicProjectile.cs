using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
public class VFX_BasicProjectile : MonoBehaviour
{
    LineRenderer lineRenderer;
    Queue<Vector3> positions = new Queue<Vector3>();

    [Header("Trail Settings")]
    public int trailLength = 15;
    public float startWidth = 0.1f;
    public float endWidth = 0.01f;
    public Color startColor = Color.white;
    public Color endColor = Color.white;

    [Header("Smoothness")]
    public float minDistance = 0.1f;
    public float fadeTime = 1.0f;

    Vector3 lastPosition;
    float timeSinceLastMove = 0f;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
        lineRenderer.numCapVertices = 5;
        lineRenderer.numCornerVertices = 5;

        lastPosition = transform.position;
        UpdateGradient();
    }

    void UpdateGradient()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(startColor.a, 0.0f), new GradientAlphaKey(endColor.a, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    void Update()
    {
        if (lineRenderer == null) return;

        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        UpdateGradient();

        bool isMoving = Vector3.Distance(transform.position, lastPosition) > minDistance;

        if (isMoving)
        {
            positions.Enqueue(transform.position);
            lastPosition = transform.position;
            timeSinceLastMove = 0f;
        }
        else
        {
            timeSinceLastMove += Time.deltaTime;
            if (timeSinceLastMove > fadeTime / trailLength && positions.Count > 0)
            {
                positions.Dequeue();
                timeSinceLastMove = 0f;
            }
        }

        if (positions.Count > trailLength)
        {
            positions.Dequeue();
        }
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
}