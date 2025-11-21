using UnityEngine;

public static class AttackUtils
{
    public static void GetAimPointAndDirection(
        Transform from, float maxRange, out Vector3 aimPoint, out Vector3 direction)
    {
        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(Camera.main.transform.position.z - from.position.z);
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(mp);

        direction = (worldClick - from.position).normalized;
        if (direction.sqrMagnitude < 0.001f)
            direction = from.forward;

        aimPoint = from.position + direction * maxRange;
    }

    public static Vector3 GetWorldClickOnSameZ(Transform from)
    {
        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(Camera.main.transform.position.z - from.position.z);
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(mp);
        worldClick.z = from.position.z;
        return worldClick;
    }
}