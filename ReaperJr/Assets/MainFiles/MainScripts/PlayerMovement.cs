using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody controller;

    //public float speed = 1f;
    private ThrowableScythe scytheScript; // added by May, used to control teleporting
    public float velocity = 10f;
    public float distanceGround = 1.2f;

    public bool isGrounded;
    private bool m_FacingRight = true;

    public float horizontalSpeed = 10f, verticalSpeed = 5f;
    public float addForce = 500f; //added by May
    public bool isFacingLeft;
    public bool isFacingFront;

    public bool isDiagonal;
    public bool isVertical;
    public bool isHorizontal;

    public LayerMask groundLayer;

    public GameObject scythe;
    public float timeToMove;

    //Calling on the CharacterController Component
    void Start()
    {
        controller = GetComponent<Rigidbody>();
        scytheScript = GetComponent<ThrowableScythe>();
    }

    //Calling the PlayerJumping function
    void Update()
    {
        Movement();
        Grounded();

        if (Input.GetMouseButtonDown(0) && scytheScript.isTrown == true)
        {
            StartCoroutine(TeleportToScythe());
        }
    }

    #region PlayerMovement
    //Creating the player jumping, and player movement function.
    //If the player is on ground then he is able to jump, depending on the jumpforce and gravity.
    void Movement()
    {
        isDiagonal = false;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        h = Mathf.Abs(h);
        v = Mathf.Abs(v);

        Debug.Log("H" + h);
        Debug.Log("V" + v);
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    if (!isFacingLeft)
        //    {
        //        verticalSpeed = -verticalSpeed;
        //    }
        //    isFacingLeft = true;

        //    transform.eulerAngles = new Vector3(0, 180, 0);
        //}

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    if (isFacingLeft)
        //    {
        //        verticalSpeed = -verticalSpeed;
        //    }
        //    isFacingLeft = false;

        //    transform.eulerAngles = Vector3.zero;
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    if (!isFacingFront)
        //    {
        //        horizontalSpeed = -horizontalSpeed;
        //    }
        //    isFacingFront = true;
        //    transform.eulerAngles = new Vector3(0, 275, 0);
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    if (isFacingFront)
        //    {
        //        horizontalSpeed = -horizontalSpeed;
        //    }
        //    isFacingFront = false;
        //    transform.eulerAngles = new Vector3(0, 90, 0);
        //}

        if (Input.GetKey(KeyCode.W))
        {
            transform.eulerAngles = new Vector3(0, 270, 0);
            isVertical = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.eulerAngles = new Vector3(0, 90, 0);
            isVertical = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            isHorizontal = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            isHorizontal = true;
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 45, 0);
            isDiagonal = true;
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 135, 0);
            isDiagonal = true;
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 225, 0);
            isDiagonal = true;
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 315, 0);
            isDiagonal = true;
        }

        //transform.Translate(new Vector3(h * horizontalSpeed, 0, v * verticalSpeed) * Time.deltaTime);
        float speed = 0f;

        if (isDiagonal)
        {
            isHorizontal = false;
            isVertical = false;
            speed = (h + v) * horizontalSpeed / 2f;
        }

        if(isHorizontal)
        {
            speed = h * horizontalSpeed;
        }

        if (isVertical)
        {
            speed = v * horizontalSpeed;
        }

        if (isGrounded) // added by May --> change movement control to addForce, when changinng ground condition, just need to alter rigidbody componenet 
        {
            controller.AddForce(transform.right * speed * addForce * Time.deltaTime);
        }
        else
            transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime);

        isVertical = false;
        isHorizontal = false;
        isDiagonal = false;

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
