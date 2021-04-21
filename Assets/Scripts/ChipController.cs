using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class ChipController : MonoBehaviour
{
    public enum ChipStates {SCAN, CLEAN}
    public ChipStates chipstate = ChipStates.SCAN;
    private Rigidbody2D rb;
    // use the turn speed for its rotation scanning
    public float turnSpeed = 30f;
    public float rayDistance = 6f;
    // how fast can it move
    public float moveSpeed = 5f;
    public Transform rayFrom;    // detect ray from here
    public LayerMask rayCastOn;  // raycast on this layer!

    // make array of directions to scan
    private Vector2[] directions;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        directions = MultiAngleVector(12);
     
    }

    Vector2[] MultiAngleVector(int num)
    {
        Vector2[] _directions = new Vector2[num];
        for (int i = 0; i < num; i++)
        {
            float angle = i * 360 / num;
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);
            Vector2 d = new Vector2(x,y);
            _directions[i] = d;
        }

        return _directions;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (chipstate)
        {
            case ChipStates.SCAN:
                Scan();
                break;
            case ChipStates.CLEAN:
                Clean();
                break;
        }

        
    }

    void Clean()
    {
        rb.AddForce(transform.up * (moveSpeed * Time.fixedDeltaTime));
        if(rb.velocity.magnitude > moveSpeed)
            rb.velocity = rb.velocity.normalized * moveSpeed;
        
        Debug.DrawRay( rayFrom.position, transform.up * rayDistance, Color.white);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // is it a virus?
        if (other.CompareTag("virus"))
        {
            // find where on the grid this is
            Destroy(other.gameObject);
            rb.velocity = Vector2.zero; // stop any movement
            chipstate = ChipStates.SCAN; // resume scan
        }
    }
 
    void Scan()
    {
        rb.angularVelocity = turnSpeed;
        
        //Vector3 forward = transform.TransformDirection(Vector3.up) * rayDistance;
        int dirNumber = Time.frameCount % directions.Length;
        Vector2 forward = directions[dirNumber] * rayDistance;
        rb.velocity = forward * 0.3f;
        // test for any hits
        RaycastHit2D hit = Physics2D.Raycast( rayFrom.position, forward, rayDistance, rayCastOn);
        if (hit.collider != null)
        {
            Debug.Log($"Hit {hit.collider.name}");
            // draw red if hit
            Debug.DrawRay( rayFrom.position, forward, Color.red);
            rb.angularVelocity = 0;
          
            chipstate = ChipStates.CLEAN;
        }
        else
        {
            // draws green if no hit
            Debug.DrawRay( rayFrom.position, forward, Color.green);
        }
    }
}
