using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ChipPlayerController : MonoBehaviour
{
    /*
     * I want to create a similar controller to the NPC Chip
     * This should move the same way, except controlled by WASD
     * Also need to shoot to destroy the Chip before it gets virus score before you
     * You also can clean virus, the aim is to do it faster than NPC
     * 
     */
    private Rigidbody2D rb;

    // use the turn speed for its rotation scanning
    public float turnSpeed = 30f;

    public float moveSpeed = 5f;
    public float rayDistance = 6f;
    public LayerMask rayCastOn;

    public Transform explosion;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameController.Instance.AddScore();
        }
        // shoot the other chip for some reason
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.DrawRay( transform.position, transform.up * rayDistance, Color.white);
            RaycastHit2D hit = Physics2D.Raycast( transform.position, transform.up, rayDistance, rayCastOn);
            if (hit.collider != null)
            {
                Debug.Log($"Hit {hit.collider.name}");
                // draw red if hit
                Debug.DrawRay( transform.position, transform.up * rayDistance, Color.red);
                Transform _explosion = Instantiate(explosion, hit.collider.transform.position, Quaternion.identity);
                Destroy(hit.collider.gameObject);
                Destroy(_explosion.gameObject, 3f);
              
            }
        }
    }

    private void FixedUpdate()
    {
        float turn = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            turn = turnSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            turn = -turnSpeed;
        }

        rb.angularVelocity = turn;

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.up * (moveSpeed * Time.fixedDeltaTime), ForceMode2D.Force);
        }
        else
        {
            rb.velocity *= 0.9f;
        }

        if (rb.velocity.magnitude > moveSpeed)
            rb.velocity = rb.velocity.normalized * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // is it a virus?
        if (other.CompareTag("virus"))
        {
            Destroy(other.gameObject);
        }
    }
    
}