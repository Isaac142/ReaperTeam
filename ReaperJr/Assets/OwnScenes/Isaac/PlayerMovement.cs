using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody controller;

    public float speed = 1f;

    public float velocity = 10f;

    public bool isGrounded;

    public float distanceGround = 1.2f;

    public LayerMask groundLayer;

    private bool m_FacingRight = true;


    //Calling on the CharacterController Component
    void Start()
    {
        controller = GetComponent<Rigidbody>();
    }

    //Calling the PlayerJumping function
    void Update()
    {
        Movement();
        Grounded();
    }

    //Creating the player jumping, and player movement function.
    //If the player is on ground then he is able to jump, depending on the jumpforce and gravity.
    void Movement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Flip();
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    Flip();
        //}
    }

    void Jump()
    {
        controller.AddForce(new Vector3(0, velocity, 0));
    }

    void Grounded()
    {
        RaycastHit hit;
        Vector3 dir = new Vector3(0, -1, 0);

        if (Physics.Raycast(transform.position, dir, out hit, distanceGround))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        //Transform for fire point
        transform.Rotate(0f, 180f, 0f);

    }
}
