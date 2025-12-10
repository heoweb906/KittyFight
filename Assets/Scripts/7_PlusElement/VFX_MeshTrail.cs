using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicaCloth2;

public class VFX_MeshTrail : MonoBehaviour
{
    private GameObject Parent; // [필수] 캐릭터 본체 혹은 회전 기준이 되는 오브젝트 연결
    public float activeTime = 2f;

    [Header("Mesh Settings")]
    public float trailScaleMultiplier = 1.0f;
    public Mesh[] customMeshes;

    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 2;

    [Header("Shader Related")]
    public Material mat_Red;
    public Material mat_Blue;
    public string shaderVarRef;
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
        // [수정] Parent가 Inspector에서 할당되지 않았다면, 최상위 부모(Root)를 자동으로 할당
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
        // 1. MagicaCloth 비활성화 (캐릭터 본체의 Cloth 효과를 잠시 끕니다)
        for (int i = 0; i < objsMagicaCloth.Length; ++i)
        {
            objsMagicaCloth[i].SetActive(false);
        }

        // 2. Mesh Trail 생성 루프
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;
            meshRenderers = GetComponentsInChildren<MeshRenderer>();

            // 2-1. 부모 컨테이너(Root) 생성
            GameObject trailRoot = new GameObject("Trail_Root_Group");

            // 2-2. Root 초기 위치 설정 (현재 위치 기준)
            trailRoot.transform.position = this.transform.position;
            trailRoot.transform.rotation = Quaternion.identity;

            for (int i = 0; i < meshRenderers.Length; ++i)
            {
                GameObject obj = new GameObject("Trail_Part_" + i);

                // 위치, 회전, 스케일 동기화 및 설정
                obj.transform.position = meshRenderers[i].transform.position;
                obj.transform.rotation = meshRenderers[i].transform.rotation;
                obj.transform.localScale = meshRenderers[i].transform.localScale * trailScaleMultiplier;

                // 2-3. 생성한 파츠를 trailRoot의 자식으로 넣기
                obj.transform.SetParent(trailRoot.transform);

                MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                MeshFilter mf = obj.AddComponent<MeshFilter>();
                MeshFilter originalMF = meshRenderers[i].GetComponent<MeshFilter>();

                if (originalMF != null)
                {
                    // Mesh 복사 및 할당 (Custom Mesh 또는 원본 Mesh 복사)
                    if (customMeshes != null && i < customMeshes.Length && customMeshes[i] != null)
                    {
                        mf.mesh = customMeshes[i];
                    }
                    else
                    {
                        mf.mesh = Object.Instantiate(originalMF.mesh);
                    }

                    // Material 할당 (자동으로 인스턴스 생성됨)
                    if (iPlayerNum <= 1) mr.material = mat_Red;
                    if (iPlayerNum == 2) mr.material = mat_Blue;

                    // ✨ 그림자 제거 설정 ✨
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.receiveShadows = false;

                    // Material 페이드 아웃 애니메이션 시작 (이 스크립트에서 코루틴 실행)
                    StartCoroutine(AnimateMaterailAlpha(mr.material, 0, shaderVarRate, shadervarRefreshRate));
                }
            }

            // 2-4. Parent의 Y축 회전에 따른 잔상 위치 및 회전 조정
            if (Parent != null)
            {
                float parentY = Parent.transform.eulerAngles.y;

                // 유니티 각도 보정 (0~360 -> -180~180)
                if (parentY > 180) parentY -= 360;

                if (parentY >= 0)
                {
                    // Case A: Y회전이 양수 (주로 오른쪽을 바라볼 때)
                    trailRoot.transform.Rotate(vec3_1);
                    trailRoot.transform.position += new Vector3(-0.2f, 0.05f, 0);
                }
                else
                {
                    // Case B: Y회전이 음수 (주로 왼쪽을 바라볼 때)
                    trailRoot.transform.Rotate(vec3_2);
                    trailRoot.transform.position += new Vector3(0.2f, 0.05f, 0);
                }
            }

            // 2-5. 일정 시간 후 부모 컨테이너 통째로 삭제
            Destroy(trailRoot, meshDestroyDelay);

            // 2-6. 다음 잔상 생성을 위해 대기
            yield return new WaitForSeconds(meshRefreshRate);
        }

        // 3. MagicaCloth 재활성화 (애니메이션 종료 후)
        for (int i = 0; i < objsMagicaCloth.Length; ++i)
        {
            objsMagicaCloth[i].SetActive(true);
        }
        isTrailActive = false;
    }


    IEnumerator AnimateMaterailAlpha(Material mat, float goal, float rate, float refrehRate)
    {
        if (mat == null) yield break;

        // 셰이더 변수 대신, 머테리얼의 _Color 속성을 사용합니다.
        Color colorToAnimate = mat.GetColor("_Color");
        float alpha = colorToAnimate.a;

        while (alpha > goal)
        {
            if (mat == null) yield break;

            alpha = Mathf.Max(goal, alpha - rate); // 0보다 작아지지 않게 방지
            colorToAnimate.a = alpha;

            mat.SetColor("_Color", colorToAnimate); // 알파값이 수정된 Color 적용

            yield return new WaitForSeconds(refrehRate);
        }
    }
}