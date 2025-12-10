using UnityEngine;

public class SK_Rend : Skill
{
    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;


    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (!objSkillEntity) return;

        var rot = Quaternion.LookRotation(direction, Vector3.up);
        var go = Instantiate(objSkillEntity, origin, rot);

        var ab = go.GetComponent<AB_HitboxBase>();
        if (ab != null) ab.Init(playerAbility);

        else if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeAmount, direction);
        }
        else
        {
            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeAmount * 0.5f, direction);
        }

    }
}