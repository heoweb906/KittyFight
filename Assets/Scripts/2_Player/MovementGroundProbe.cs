using UnityEngine;

public class MovementGroundProbe : MonoBehaviour
{
    [Header("Probe")]
    public LayerMask groundMask;
    public float castOriginOffsetY = 0.1f;   // 시작점 살짝 위
    public float castRadius = 0.25f;         // 캐릭터 폭에 맞춰 조정
    public float castDistance = 0.8f;        // 캐릭터 키에 맞춰 0.6~1.0
    public float maxSlopeAngle = 45f;        // 허용 경사각

    public bool IsGrounded { get; private set; }
    public bool OnSlope { get; private set; }
    public Vector3 GroundNormal { get; private set; } = Vector3.up;
    public float GroundSlopeAngle { get; private set; } // 0 = 평지

    private RaycastHit hit;

    public void Refresh(Transform body)
    {
        Vector3 origin = body.position + Vector3.up * castOriginOffsetY;

        IsGrounded = false;
        OnSlope = false;
        GroundNormal = Vector3.up;
        GroundSlopeAngle = 0f;

        if (Physics.SphereCast(
                origin, castRadius, Vector3.down,
                out hit, castDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            GroundNormal = hit.normal;
            GroundSlopeAngle = Vector3.Angle(Vector3.up, GroundNormal);
            IsGrounded = true;
            OnSlope = GroundSlopeAngle > 0.01f && GroundSlopeAngle < maxSlopeAngle + 0.01f;
        }
    }
}