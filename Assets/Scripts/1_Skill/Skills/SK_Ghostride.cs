using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_Ghostride : Skill
{
    [SerializeField] private float duration = 5f;
    [SerializeField] private float speedMultiplier = 1.8f;
    [SerializeField] private GameObject[] visualObjects;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    private bool cancelRequested = false;
    private bool hideVisual = false;


    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        Instantiate(
            effectPrefab,
            origin,
            Quaternion.Euler(-90, 0, 0)
        );

        if (!playerAbility) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 4);
        StartCoroutine(ResetAttackAnimState());

        hideVisual = !(playerAbility.playerNumber == MatchResultStore.myPlayerNumber);
        //hideVisual = true;

        if ((visualObjects == null || visualObjects.Length == 0) && hideVisual)
        {
            var all = playerAbility.GetComponentsInChildren<Transform>(true);
            var list = new List<GameObject>();

            foreach (var t in all)
            {
                if (t != null && t.CompareTag("Visual"))
                    list.Add(t.gameObject);
            }

            visualObjects = list.ToArray();
        }

        var owner = playerAbility.gameObject;

        var gm = FindObjectOfType<GameManager>();
        if (playerAbility.playerNumber == MatchResultStore.myPlayerNumber)
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
        }
        else
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeDuration * 0.5f, direction);
        }


        StartCoroutine(Co_Ghostride(owner));
    }

    private IEnumerator Co_Ghostride(GameObject owner)
    {
        cancelRequested = false;

        var ability = playerAbility;
        var health = owner.GetComponent<PlayerHealth>();

        float originalSpeed = ability.moveSpeed;
        int startHP = health != null ? health.CurrentHP : 0;

        ability.moveSpeed = originalSpeed * speedMultiplier;

        if (hideVisual && visualObjects != null)
        {
            for (int i = 0; i < visualObjects.Length; i++)
            {
                if (visualObjects[i])
                    visualObjects[i].SetActive(false);
            }
        }

        float elapsed = 0f;
        float dur = Mathf.Max(0.01f, duration);

        while (elapsed < dur)
        {
            if (cancelRequested) break;

            if (health != null && health.CurrentHP != startHP)
                break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (ability)
            ability.moveSpeed = originalSpeed;

        if (hideVisual && visualObjects != null)
        {
            for (int i = 0; i < visualObjects.Length; i++)
            {
                if (visualObjects[i])
                    visualObjects[i].SetActive(true);
            }
        }

        Instantiate(
            effectPrefab,
            owner.transform.position,
            Quaternion.Euler(-90, 0, 0)
        );
    }
    public void NotifyAttack()
    {
        cancelRequested = true;
    }

    private IEnumerator ResetAttackAnimState()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        anim.SetBool("isAttack", false);
    }
}