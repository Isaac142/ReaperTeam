using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float forwardForce = 1500;
    public float forwardSpeed = 5f;
    public float jumpForce = 50f;
    public float gModifier = 7f; // gravite modifier when character is in air
    public float dashForce = 3f;
    public float turnSpeed = 60f;
    public float collectableDist = 5f;
    private float keyDown;
    private Vector3 mousePos;

    private Rigidbody rb;

    public LayerMask groundLayer;   

    private bool IsGrounded()
    { return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), -transform.up, 0.1f, groundLayer); }
    private bool dash = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;

        Movement();
        Jump();
        Crouch();
        Dash();
        if(Input.GetMouseButtonDown(1))
        { Collect(); }
    }
    
    void Movement()
    {           
        float xMove = Input.GetAxis("Horizontal");
        Vector3 move = xMove * transform.right;
        Vector3 characterRotation = transform.localEulerAngles;


        if(Input.GetKey(KeyCode.W))
        {
            transform.RotateAround (transform.position, transform.up, -turnSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.RotateAround(transform.position, transform.up, turnSpeed * Time.deltaTime);
        }

        if (dash)
        {
            rb.AddForce(transform.right * dashForce, ForceMode.Impulse);
            dash = false;
        }

        if (IsGrounded())
        {
            rb.AddForce(move * forwardForce * Time.deltaTime);           
        }
        else
        {
            transform.Translate((xMove * Vector3.right) * forwardSpeed * Time.deltaTime);
            rb.velocity += Vector3.up * Physics.gravity.y * (gModifier - 1) * Time.deltaTime;            
        }       
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Crouch()
    {
        CapsuleCollider bodyCollider = GetComponent<CapsuleCollider>();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            bodyCollider.center = new Vector3(0f, 0.5f, 0f);
            bodyCollider.height = 0.5f;

            //visual, replace by animation
            transform.localScale = new Vector3(1f, 0.5f, 1f);
        }
        else
        {
            bodyCollider.center = new Vector3(0f, 1f, 0f);
            bodyCollider.height = 2f;

            //visual, replace by animation
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void Dash()
    {
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {           
            float betweenPress = Time.time - keyDown;

            if (betweenPress <= 0.3f)
            {
                dash = true;
            }
            keyDown = Time.time;
        }
    }

    void Collect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag != "Untagged")
            {
                if (Vector3.Distance(transform.position, hit.transform.position) < collectableDist) //calculate if the collectable is with in the collectable distance.
                {
                    if (hit.transform.tag == "Soul")
                    {
                        Destroy(hit.transform.gameObject);                       
                        //do something --> collected amount, visual clue...
                    }

                    //if there's other collectables
                }
            }
        }
    }
}
