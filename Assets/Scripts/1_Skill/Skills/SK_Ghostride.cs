using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_Ghostride : Skill
{
    [SerializeField] private float duration = 5f;
    [SerializeField] private float speedMultiplier = 1.8f;
    [SerializeField] private GameObject[] visualObjects;

    [Header("Ghostride Materials (by playerNumber 1/2)")]
    [SerializeField] private Material normalP1;
    [SerializeField] private Material normalP2;
    [SerializeField] private Material transparentP1;
    [SerializeField] private Material transparentP2;

    [Header("Effects")]
    [SerializeField] private GameObject effectPrefab;

    private bool cancelRequested = false;
    private bool hideVisual = false;

    [Header("카메라 연출")]
    public float shakeAmount;
    public float shakeDuration;

    [SerializeField] private float attackAnimDuration = 0.5f;

    private Renderer[] visualRenderers;

    public override void Execute(Vector3 origin, Vector3 direction)
    {
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, origin, Quaternion.Euler(-90, 0, 0));
        }

        if (!playerAbility) return;

        anim.SetTrigger("Attack");
        anim.SetBool("isAttack", true);
        anim.SetInteger("AttackType", 4);
        StartCoroutine(ResetAttackAnimState());

        bool isMine = playerAbility.playerNumber == MatchResultStore.myPlayerNumber;
        hideVisual = !isMine;

        if (visualObjects == null || visualObjects.Length == 0)
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

        if (isMine)
        {
            CacheVisualRenderers();
        }

        var owner = playerAbility.gameObject;

        var gm = FindObjectOfType<GameManager>();
        if (isMine)
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount, shakeDuration, direction);
        }
        else
        {
            gm?.cameraManager?.ShakeCameraPunch(shakeAmount * 0.5f, shakeDuration * 0.5f, direction);
        }

        StartCoroutine(Co_Ghostride(owner, isMine));
    }

    private IEnumerator Co_Ghostride(GameObject owner, bool isMine)
    {
        cancelRequested = false;

        var ability = playerAbility;
        var health = owner.GetComponent<PlayerHealth>();

        float originalSpeed = ability.moveSpeed;
        int startHP = health != null ? health.CurrentHP : 0;

        ability.moveSpeed = originalSpeed * speedMultiplier;

        if (isMine)
        {
            ApplyTransparentMaterialByPlayerNumber();
        }
        else if (hideVisual && visualObjects != null)
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

        if (isMine)
        {
            RestoreNormalMaterialByPlayerNumber();
        }
        else if (hideVisual && visualObjects != null)
        {
            for (int i = 0; i < visualObjects.Length; i++)
            {
                if (visualObjects[i])
                    visualObjects[i].SetActive(true);
            }
        }

        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, owner.transform.position, Quaternion.Euler(-90, 0, 0));
        }
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

    private void CacheVisualRenderers()
    {
        if (visualRenderers != null && visualRenderers.Length > 0)
            return;

        if (visualObjects == null || visualObjects.Length == 0)
            return;

        var list = new List<Renderer>();
        for (int i = 0; i < visualObjects.Length; i++)
        {
            if (!visualObjects[i]) continue;

            var rs = visualObjects[i].GetComponentsInChildren<Renderer>(true);
            if (rs != null && rs.Length > 0)
                list.AddRange(rs);
        }

        visualRenderers = list.ToArray();
    }

    private Material GetNormalMat()
    {
        int pn = playerAbility != null ? playerAbility.playerNumber : 1;
        return (pn == 2) ? normalP2 : normalP1;
    }

    private Material GetTransparentMat()
    {
        int pn = playerAbility != null ? playerAbility.playerNumber : 1;
        return (pn == 2) ? transparentP2 : transparentP1;
    }

    private void ApplyTransparentMaterialByPlayerNumber()
    {
        var mat = GetTransparentMat();
        if (mat == null) return;

        CacheVisualRenderers();
        if (visualRenderers == null) return;

        for (int i = 0; i < visualRenderers.Length; i++)
        {
            if (visualRenderers[i])
                visualRenderers[i].sharedMaterial = mat;
        }
    }

    private void RestoreNormalMaterialByPlayerNumber()
    {
        var mat = GetNormalMat();
        if (mat == null) return;

        if (visualRenderers == null) return;

        for (int i = 0; i < visualRenderers.Length; i++)
        {
            if (visualRenderers[i])
                visualRenderers[i].sharedMaterial = mat;
        }
    }

    private void OnDisable()
    {
        RestoreNormalMaterialByPlayerNumber();
    }
}