using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimic8_Entity : MonoBehaviour
{
    [Header("Settings")]
    public GameObject spawnPrefab; 
    public float spawnInterval = 3.0f; 

    void Start()
    {
        StartCoroutine(Co_SpawnRoutine());
    }

    private IEnumerator Co_SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawnPrefab != null)
            {
                GameObject obj = Instantiate(spawnPrefab, transform.position, Quaternion.identity);

                var hitbox = obj.GetComponent<AB_HitboxBase>();
                if (hitbox == null) hitbox = obj.GetComponentInChildren<AB_HitboxBase>();

                if (hitbox != null)
                {
                    hitbox.bMiddleState = true;

                    // 소유자는 없음(null)으로 설정
                    hitbox.Init(null);
                }
            }
        }
    }
}