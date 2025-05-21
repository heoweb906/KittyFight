using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public int assignedPlayerNumber; // 1 or 2
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public GameObject spawnPrefab;

    public int myPlayerNumber;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetMyPlayerNumber(int number)
    {
        myPlayerNumber = number;

        if (rb == null) rb = GetComponent<Rigidbody>();

        if (myPlayerNumber != assignedPlayerNumber)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }
    }

    void Update()
    {
        if (myPlayerNumber != assignedPlayerNumber) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);
        transform.Translate(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 1.5f, 0);

            GameObject spawned = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
            Destroy(spawned, 2f);

            string msg = ObjectSpawnMessageBuilder.Build(spawnPos, myPlayerNumber);
            P2PMessageSender.SendMessage(msg);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
