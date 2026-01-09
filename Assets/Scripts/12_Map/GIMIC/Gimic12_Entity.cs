using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimic12_Entity : MonoBehaviour
{
    [Header("Settings")]
    public GameObject targetObject; // 힐링 프리펩 
    public float delayTime;
    public float fDestroyDelay;

    public GameObject effect; // 힐링 프리펩 

    [Header("사운드")]
    [HideInInspector] public GameManager manager;
    public AudioClip sfxClip;

    void Start()
    {
        StartCoroutine(Co_SpawnProcess());
    }

    private IEnumerator Co_SpawnProcess()
    {
        yield return new WaitForSeconds(delayTime);

        effect.transform.SetParent(null);


        if (targetObject != null)
        {
            targetObject.SetActive(true);

            GameObject gas = targetObject;

            // [중요 2] 히트박스 찾기 (본체 혹은 자식에 있을 수 있음)
            var hitbox = gas.GetComponent<AB_HitboxBase>();
            if (hitbox == null) hitbox = gas.GetComponentInChildren<AB_HitboxBase>();

            if (hitbox != null)
            {
                // [핵심] 중립 상태 설정 -> 소유자가 없어도 누구든 때릴 수 있게 됨
                hitbox.bMiddleState = true;

                // 소유자는 없음(null)으로 설정
                hitbox.Init(null);
            }


            var gm = FindObjectOfType<GameManager>();
            gm?.cameraManager?.ShakeCameraPunch(0.6f, 0.3f);
        }

        if(manager.playerAbility_1 != null && sfxClip != null) manager.playerAbility_1.PlaySFX(sfxClip);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);

      
    }
}
