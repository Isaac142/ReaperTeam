using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody controller;

    public float speed = 1f;
    public float velocity = 10f;
    public float distanceGround = 1.2f;

    public bool isGrounded;
    private bool m_FacingRight = true;

    public float horizontalSpeed = 10f, verticalSpeed = 5f;
    public bool isFacingLeft;
    public bool isFacingFront;

    public LayerMask groundLayer;

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

    #region PlayerMovement
    //Creating the player jumping, and player movement function.
    //If the player is on ground then he is able to jump, depending on the jumpforce and gravity.
    void Movement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        h = Mathf.Abs(h);

        Debug.Log("H" + h);
        Debug.Log("V" + v);
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!isFacingLeft)
            {
                verticalSpeed = -verticalSpeed;
            }
            isFacingLeft = true;

            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (isFacingLeft)
            {
                verticalSpeed = -verticalSpeed;
            }
            isFacingLeft = false;

            transform.eulerAngles = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!isFacingFront)
            {
                horizontalSpeed = -horizontalSpeed;
            }
            isFacingFront = true;
            transform.eulerAngles = new Vector3(0, 275, 0);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (isFacingFront)
            {
                horizontalSpeed = -horizontalSpeed;
            }
            isFacingFront = false;
            transform.eulerAngles = new Vector3(0, 90, 0);
        }

        if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 45, 0);
        }

        if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 135, 0);
        }

        if (Input.GetKeyDown(KeyCode.W) && Input.GetKeyDown(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 225, 0);
        }

        if (Input.GetKeyDown(KeyCode.W) && Input.GetKeyDown(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 315, 0);
        }

        transform.Translate(new Vector3(h * horizontalSpeed, 0, v * verticalSpeed) * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
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
    #endregion
}
