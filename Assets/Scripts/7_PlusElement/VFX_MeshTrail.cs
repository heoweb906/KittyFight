using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicaCloth2;

public class VFX_MeshTrail : MonoBehaviour
{
    private GameObject Parent; // 캐릭터 본체 혹은 회전 기준 오브젝트
    public float activeTime = 2f;

    [Header("Mesh Settings")]
    public float trailScaleMultiplier = 1.0f;
    public Mesh[] customMeshes;

    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 2;

    [Header("Shader Related")]
    public Material mat_Red;
    public Material mat_Blue;
    public float shaderVarRate = 0.1f;
    public float shadervarRefreshRate = 0.05f;

    private bool isTrailActive;
    private MeshRenderer[] meshRenderers;
    private GameObject[] objsMagicaCloth;

    [Header("Rotation Settings")]
    public Vector3 vec3_1; // Y회전이 90일 때 적용
    public Vector3 vec3_2; // Y회전이 -90일 때 적용

    private void Awake()
    {
        if (Parent == null)
        {
            Parent = transform.root.gameObject;
        }

        MagicaCloth[] magicaCloths = GetComponentsInChildren<MagicaCloth>();
        objsMagicaCloth = new GameObject[magicaCloths.Length];
        for (int i = 0; i < magicaCloths.Length; ++i)
        {
            objsMagicaCloth[i] = magicaCloths[i].gameObject;
        }
    }

    public void ActiveateTrail(int iPlayerNum)
    {
        if (isTrailActive) return;

        isTrailActive = true;
        StartCoroutine(ActiveateTrail_(activeTime, iPlayerNum));
    }

    IEnumerator ActiveateTrail_(float timeActive, int iPlayerNum)
    {
        // 1. MagicaCloth 비활성화
        for (int i = 0; i < objsMagicaCloth.Length; ++i)
        {
            objsMagicaCloth[i].SetActive(false);
        }

        // [최적화] 루프 밖에서 미리 수집하여 렉 방지
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        // 2. Mesh Trail 생성 루프
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            // 2-1. 부모 컨테이너(Root) 생성 및 초기화
            GameObject trailRoot = new GameObject("Trail_Root_Group");
            trailRoot.transform.position = this.transform.position;
            trailRoot.transform.rotation = Quaternion.identity;
            trailRoot.transform.localScale = Vector3.one; // 부모 스케일 강제 고정

            for (int i = 0; i < meshRenderers.Length; ++i)
            {
                if (!meshRenderers[i].gameObject.activeInHierarchy) continue;

                GameObject obj = new GameObject("Trail_Part_" + i);

                // 위치와 회전을 먼저 설정
                obj.transform.position = meshRenderers[i].transform.position;
                obj.transform.rotation = meshRenderers[i].transform.rotation;

                // [중요] 부모 설정을 스케일 대입보다 먼저 수행
                obj.transform.SetParent(trailRoot.transform);

                // [중요] 부모 귀속 후 월드 스케일(lossyScale)을 복사하여 뻥튀기 방지
                obj.transform.localScale = meshRenderers[i].transform.lossyScale * trailScaleMultiplier;

                MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                MeshFilter mf = obj.AddComponent<MeshFilter>();
                MeshFilter originalMF = meshRenderers[i].GetComponent<MeshFilter>();

                if (originalMF != null)
                {
                    if (customMeshes != null && i < customMeshes.Length && customMeshes[i] != null)
                    {
                        mf.mesh = customMeshes[i];
                    }
                    else
                    {
                        mf.mesh = Object.Instantiate(originalMF.mesh);
                    }

                    if (iPlayerNum <= 1) mr.material = mat_Red;
                    else if (iPlayerNum == 2) mr.material = mat_Blue;

                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.receiveShadows = false;

                    StartCoroutine(AnimateMaterailAlpha(mr.material, 0, shaderVarRate, shadervarRefreshRate));
                }
            }

            // 2-4. Parent 회전에 따른 위치 보정
            if (Parent != null)
            {
                float parentY = Parent.transform.eulerAngles.y;
                if (parentY > 180) parentY -= 360;

                if (parentY >= 0)
                {
                    trailRoot.transform.Rotate(vec3_1);
                    trailRoot.transform.position += new Vector3(-0.2f, 0.05f, 0);
                }
                else
                {
                    trailRoot.transform.Rotate(vec3_2);
                    trailRoot.transform.position += new Vector3(0.2f, 0.05f, 0);
                }
            }

            Destroy(trailRoot, meshDestroyDelay);
            yield return new WaitForSeconds(meshRefreshRate);
        }

        // 3. MagicaCloth 재활성화
        for (int i = 0; i < objsMagicaCloth.Length; ++i)
        {
            if (objsMagicaCloth[i] != null) objsMagicaCloth[i].SetActive(true);
        }
        isTrailActive = false;
    }

    IEnumerator AnimateMaterailAlpha(Material mat, float goal, float rate, float refrehRate)
    {
        if (mat == null) yield break;

        Color colorToAnimate = mat.GetColor("_Color");
        float alpha = colorToAnimate.a;

        while (alpha > goal)
        {
            if (mat == null) yield break;

            alpha = Mathf.Max(goal, alpha - rate);
            colorToAnimate.a = alpha;
            mat.SetColor("_Color", colorToAnimate);

            yield return new WaitForSeconds(refrehRate);
        }
    }
}