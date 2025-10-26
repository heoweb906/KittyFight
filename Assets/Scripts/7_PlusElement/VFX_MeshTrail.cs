using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicaCloth2;
public class VFX_MeshTrail : MonoBehaviour
{
    public float activeTime = 2f;

    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 2;

    [Header("Shader Related")]
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shadervarRefreshRate = 0.05f;


    private bool isTrailActive;
    private MeshRenderer[] meshRenderers;
    private GameObject[] objsMagicaCloth;

    private void Awake()
    {
        MagicaCloth[] magicaCloths = GetComponentsInChildren<MagicaCloth>();
        objsMagicaCloth = new GameObject[magicaCloths.Length];
        for (int i = 0; i < magicaCloths.Length; ++i)
        {
            objsMagicaCloth[i] = magicaCloths[i].gameObject;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ActiveateTrail(activeTime);
        }
    }

    public void ActiveateTrail(float activeTime)
    {
        if (isTrailActive) return;

        isTrailActive = true;
        StartCoroutine(ActiveateTrail_(activeTime));
    }

    IEnumerator ActiveateTrail_(float timeActive)
    {
        for (int i = 0; i < objsMagicaCloth.Length; ++i)
        {
            objsMagicaCloth[i].SetActive(false);
        }
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < meshRenderers.Length; ++i)
            {
                GameObject obj = new GameObject();
                obj.transform.SetParent(meshRenderers[i].transform.parent);
                obj.transform.localPosition = meshRenderers[i].transform.localPosition;
                obj.transform.localRotation = meshRenderers[i].transform.localRotation;
                obj.transform.localScale = meshRenderers[i].transform.localScale;
                obj.transform.SetParent(null);
                MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                MeshFilter mf = obj.AddComponent<MeshFilter>();
                MeshFilter originalMF = meshRenderers[i].GetComponent<MeshFilter>();
                if (originalMF != null)
                {
                    Mesh currentMesh = originalMF.mesh;
                    Mesh copiedMesh = Object.Instantiate(currentMesh);
                    mf.mesh = copiedMesh;
                    mr.material = mat;


                    StartCoroutine(AnimateMaterailFloat(mr.material, 0, shaderVarRate, shadervarRefreshRate));

                    Destroy(obj, meshDestroyDelay);
                }
            }
            yield return new WaitForSeconds(meshRefreshRate);
        }
        for (int i = 0; i < objsMagicaCloth.Length; ++i)
        {
            objsMagicaCloth[i].SetActive(true);
        }
        isTrailActive = false;
    }

    IEnumerator AnimateMaterailFloat(Material mat, float goal, float rate, float refrehRate)
    {
        float valueToAnimate = mat.GetFloat(shaderVarRef);

        while(valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refrehRate);
        }
    }


}