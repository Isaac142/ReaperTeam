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

    public LayerMask groundLayer;

    //Calling on the CharacterController Component
    void Start()
    {
        controller = GetComponent<Rigidbody>();
    }

    //Calling the PlayerJumping function
    void Update()
    {
        //Movement();
        Grounded();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        h = Mathf.Abs(h);

        Debug.Log("H" + h);
            if (Input.GetKeyDown(KeyCode.A))
            {
                if(!isFacingLeft)
                {
                verticalSpeed = -verticalSpeed;
                }
            isFacingLeft = true;

                transform.eulerAngles = new Vector3(0, 180, 0);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if(isFacingLeft)
                {
                verticalSpeed = -verticalSpeed;
                }
            isFacingLeft = false;

                transform.eulerAngles = Vector3.zero;
            }

        transform.Translate(new Vector3(h * horizontalSpeed, 0, v * verticalSpeed) * Time.deltaTime);
        //transform.Rotate(Vector3.up * speed * Time.deltaTime);
        //transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime);

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    transform.Rotate(-Vector3.up * speed * Time.deltaTime);
        //}

        //if(Input.GetKey(KeyCode.A))
        //{
        //    transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime);
        //}
    }

    #region PlayerMovement
    //Creating the player jumping, and player movement function.
    //If the player is on ground then he is able to jump, depending on the jumpforce and gravity.
    //void Movement()
    //{
    //    //float h = Input.GetAxis("Horizontal");
    //    //float v = Input.GetAxis("Vertical");

    //    //transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime);

    //    if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
    //    {
    //        Jump();
    //    }


    //    //Flip();
    //    //if (Input.GetKeyDown(KeyCode.A))
    //    //{
    //    //    Flip();
    //    //}
    //    //if (Input.GetKeyDown(KeyCode.D))
    //    //{
    //    //    Flip();
    //    //}
    //}

    //void Jump()
    //{
    //    controller.AddForce(new Vector3(0, velocity, 0));
    //}

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
        //Switch the way the player is labelled as facing.
        //m_FacingRight = !m_FacingRight;

        //Transform for fire point
        //transform.Rotate(0f, 180f, 0f);

        //if(Input.GetKeyDown(KeyCode.A))
        //{
        //    transform.Rotate(0f, 180f, 0f);
        //}

            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    transform.Rotate(0f, 0f, 0f);
            //}

    }
    #endregion
}
