using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    public GameObject scythe;
    public float timeToMove;

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

        transform.Translate(new Vector3(h * horizontalSpeed, 0, v * verticalSpeed) * Time.deltaTime);

        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(TeleportToScythe());
        }
    }

    #region PlayerMovement
    //Creating the player jumping, and player movement function.
    //If the player is on ground then he is able to jump, depending on the jumpforce and gravity.
    /*
     * 
    void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        
        if (Input.GetKeyDown(KeyCode.A))
        {
            Flip() ;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Flip();
        }
        
    }

    void Jump()
    {
        controller.AddForce(new Vector3(0, velocity, 0));
    }
    */

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

    /*
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        //Transform for fire point
        transform.Rotate(0f, 180f, 0f);

    }
    */
    #endregion

    #region Teleport
    IEnumerator TeleportToScythe()
    {
        //Get position of the player
        Vector3 positionOfPlayer = transform.position;
        //Get position of the scythe
        Vector3 positionOfScythe = scythe.transform.position;
        //Lerp position over time
        float timer = 0f;
        while (timer < timeToMove)
        {
            transform.position = Vector3.Lerp(positionOfPlayer, positionOfScythe, timer / timeToMove);
            timer += Time.deltaTime;
            yield return null;
        }
    }
    #endregion
}