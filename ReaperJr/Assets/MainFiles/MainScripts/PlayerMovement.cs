﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody controller;

    //public float speed = 1f;
    private ThrowableScythe scytheScript; // added by May, used to control teleporting
    public float jumpForce = 20f;
    public float distanceGround;

    public bool isGrounded;

    public float speedFactor = 7f;
    public float addForce = 600f; //added by May
    public float gModifier = 5f; // gravity modifier when character is in air

    private bool isDiagonal;
    private bool isVertical;
    private bool isHorizontal;
    private bool facingF = false, facingR = false, facingL = false, facingB = false;

    public GameObject scythe;
    public float timeToMove;

    public float collectableDist = 3f;
    private float speed = 0f;

    public enum FacingDirection { LEFT, RIGHT, FRONT, BACK, FRONTLEFT, FRONTRIGHT, BACKLEFT, BACKRIGHT}
    public FacingDirection facingDirection;


    //Calling on the CharacterController Component
    void Start()
    {
        controller = GetComponent<Rigidbody>();
        controller.mass = GameManager.Instance.playerMass;
        scytheScript = GetComponent<ThrowableScythe>();
    }

    //Calling the PlayerJumping function
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
                Jump();
        }

        if (Input.GetMouseButtonDown(0) && scytheScript.isThrown == true && GameManager.Instance.Energy >= GameManager.Instance.teleportingEnergy)
        {
            StartCoroutine(TeleportToScythe());
            GameManager.Instance.Energy -= GameManager.Instance.teleportingEnergy;
        }

        if(Input.GetMouseButtonDown(1))
        {
            Collect();
        }

        EquipScythe();
        if (GameManager.Instance.scytheEquiped)
            scythe.SetActive(true);
        else
            scythe.SetActive(false);

        //RotatePlayer();
    }

    private void FixedUpdate() //prevent character walking into walls.
    {
        Grounded();
        Movement();
        DirectionSwitch();
    }

    #region FacingDirectionSwitch
    void DirectionSwitch()
    {
        if (Input.GetKey(KeyCode.W))
            facingDirection = FacingDirection.BACK;

        if (Input.GetKey(KeyCode.S))
            facingDirection = FacingDirection.FRONT;

        if (Input.GetKey(KeyCode.D))
            facingDirection = FacingDirection.RIGHT;

        if (Input.GetKey(KeyCode.A))
            facingDirection = FacingDirection.LEFT;

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            facingDirection = FacingDirection.FRONTRIGHT;

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            facingDirection = FacingDirection.FRONTLEFT;

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            facingDirection = FacingDirection.BACKLEFT;

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            facingDirection = FacingDirection.BACKRIGHT;
        
            facingB = false;
            facingF = false;
            facingL = false;
            facingR = false;
            isHorizontal = false;
            isVertical = false;
            isDiagonal = true;

        if (transform.eulerAngles == new Vector3(0, 270, 0))
        {
            facingB = true;
            isVertical = true;
            isDiagonal = false;
        }

        if (transform.eulerAngles == new Vector3(0, 90, 0))
        {
            facingF = true;
            isVertical = true;
            isDiagonal = false;
        }

        if (transform.eulerAngles == new Vector3(0, 0, 0))
        {
            facingR = true;
            isHorizontal = true;
            isDiagonal = false;
        }

        if (transform.eulerAngles == new Vector3(0, 180, 0))
        {
            facingL = true;
            isHorizontal = true;
            isDiagonal = false;
        }        
    }
    #endregion

    #region PlayerMovement
    //Creating the player jumping, and player movement function.
    void Movement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (GameManager.Instance.isHolding && !GameManager.Instance.holdingLightObject)
        #region MovingHeavyObject
        {
            speed = speedFactor;
            if (facingF)
            {
                transform.eulerAngles = new Vector3(0, 90, 0);

                if (GameManager.Instance.onSpecialGround)
                    controller.AddForce((transform.forward * h - transform.right * v) * speed * addForce * Time.deltaTime);
                else
                    transform.Translate((-transform.right * h - transform.forward * v) * speed * Time.deltaTime);
            }

            if (facingB)
            {
                transform.eulerAngles = new Vector3(0, 270, 0);

                if (GameManager.Instance.onSpecialGround)
                    controller.AddForce((-transform.forward * h + transform.right * v) * speed * addForce * Time.deltaTime);
                else
                    transform.Translate((-transform.right * h - transform.forward * v) * speed * Time.deltaTime);
            }

            if (facingR)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);

                if (GameManager.Instance.onSpecialGround)
                    controller.AddForce((transform.forward * v + transform.right * h) * speed * addForce * Time.deltaTime);
                else
                    transform.Translate((transform.right * h + transform.forward * v) * speed * Time.deltaTime);
            }

            if (facingL)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);

                if (GameManager.Instance.onSpecialGround)
                    controller.AddForce((-transform.forward * v - transform.right * h) * speed * addForce * Time.deltaTime);
                else
                    transform.Translate((transform.right * h + transform.forward * v) * speed * Time.deltaTime);
            }
        }
        #endregion
        else
        #region NormalMovement
        {
            h = Mathf.Abs(h);
            v = Mathf.Abs(v);

            switch (facingDirection)
            {
                case FacingDirection.FRONT:
                    transform.eulerAngles = new Vector3(0, 90, 0);
                    break;

                case FacingDirection.BACK:
                    transform.eulerAngles = new Vector3(0, 270, 0);
                    break;

                case FacingDirection.LEFT:
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    break;

                case FacingDirection.RIGHT:
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    break;

                case FacingDirection.FRONTLEFT:
                    transform.eulerAngles = new Vector3(0, 135, 0);
                    break;

                case FacingDirection.FRONTRIGHT:
                    transform.eulerAngles = new Vector3(0, 45, 0);
                    break;

                case FacingDirection.BACKLEFT:
                    transform.eulerAngles = new Vector3(0, 225, 0);
                    break;

                case FacingDirection.BACKRIGHT:
                    transform.eulerAngles = new Vector3(0, 315, 0);
                    break;
            }

            if (isDiagonal)
                speed = (h + v) * speedFactor / 2f;

            if (isHorizontal)
                speed = h * speedFactor;

            if (isVertical)
                speed = v * speedFactor;

            if (GameManager.Instance.onSpecialGround)
                controller.AddForce(transform.right * speed * addForce * Time.deltaTime);
            else
                transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime);
        }
        #endregion
    }
    #endregion

    #region Jump
    void Jump()
    {
            if (!GameManager.Instance.isHolding || GameManager.Instance.holdingLightObject) // character can not jump if it's holding heavy objects.
        controller.AddForce(new Vector3(0, jumpForce, 0),ForceMode.Impulse);
    }
    #endregion

    #region Grounded Testing
    void Grounded()
    {
        Vector3 dir = new Vector3(0, -1, 0);
        if (Physics.Raycast(transform.position, dir, distanceGround))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            controller.velocity += Vector3.up * Physics.gravity.y * (gModifier - 1) * Time.deltaTime; //modifying gravity
        }        
    }
    #endregion

    #region Collecting
    void Collect()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        //{
        Vector3 topPoint = controller.transform.position + controller.GetComponent<CapsuleCollider>().center + Vector3.up * (controller.GetComponent<CapsuleCollider>().height + 0.5f) / 2f;
        Vector3 bottomPoint = controller.transform.position + controller.GetComponent<CapsuleCollider>().center - Vector3.up * (controller.GetComponent<CapsuleCollider>().height - 0.01f) / 2f;
        float radius = controller.GetComponent<CapsuleCollider>().radius;
        if (Physics.CapsuleCast(topPoint, bottomPoint, radius, controller.transform.right, out hit, collectableDist))
        { if (hit.transform.tag != "Untagged")
            {
                if (Vector3.Distance(transform.position, hit.transform.position) <= collectableDist) //calculate if the collectable is with in the collectable distance.
                {
                    if (hit.transform.tag == "Soul")
                    {
                        GameManager.Instance.scytheEquiped = true;
                        Destroy(hit.transform.gameObject);
                        //do something --> collected amount, visual clue...
                    }

                    if (hit.transform.tag == "FakeSoul")
                    {
                        GameManager.Instance.scytheEquiped = true;
                        GameManager.Instance.dead = true;
                        //do something --> collected amount, visual clue...
                    }

                    if(hit.transform.tag == "HiddenItem")
                    {
                        GameManager.Instance.Timer += GameManager.Instance.rewardTime;
                        Destroy(hit.transform.gameObject);
                    }
                    //if there's other collectables
                }
            }
        }
    }
    #endregion

    #region EquipScythe
    void EquipScythe()
    {
        if (!scytheScript.isThrown)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                GameManager.Instance.scytheEquiped = !GameManager.Instance.scytheEquiped;
            }
        }
        else return;
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
        scytheScript.ResetScythe();
    }
    #endregion

    #region RotatePlayer(Charged Throw)
    /*
    void RotatePlayer()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Rotating player");
            if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                transform.eulerAngles = Vector3.zero;
            }
        }
    }
    */
    #endregion
}
