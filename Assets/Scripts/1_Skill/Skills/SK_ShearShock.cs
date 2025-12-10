using UnityEngine;
public class SK_ShearShock : Skill
{
    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;
        var rot = Quaternion.LookRotation(direction, Vector3.up);
        Vector3 spawnPos = origin + rot * Vector3.right * -0.5f;
        var shock = Instantiate(objSkillEntity, spawnPos, rot);
        var ab = shock.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        var gm = FindObjectOfType<GameManager>();
        gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
    }
}