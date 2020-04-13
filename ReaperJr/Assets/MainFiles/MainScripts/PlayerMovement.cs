using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody controller;

    //public float speed = 1f;
    private ThrowableScythe scytheScript; // added by May, used to control teleporting
    public float velocity = 10f;
    public float distanceGround;

    public bool isGrounded;

    public float horizontalSpeed = 10f, verticalSpeed = 5f;

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
    void Movement()
    {
        isDiagonal = false;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        h = Mathf.Abs(h);
        v = Mathf.Abs(v);

        Debug.Log("H" + h);
        Debug.Log("V" + v);

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
