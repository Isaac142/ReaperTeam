using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    Rigidbody controller;
    public GameObject scythe;

    private ThrowableScythe scytheScript;
    public float jumpForce = 20f;
    public float distanceGround;
    [HideInInspector]
    public bool isJumping = false, isCrouching = false, isGrounded = false, movable = true;
    public float speedFactor = 7f, addForce = 600f, gModifier = 5f; // gravity modifier when character is in air
    public float jumpBufferTime = 0.2f, cayoteTime = 0.05f;
    private float distToGround = 0f, lastPos = 0f, timeInAir = 0f;
    public float timeToMove;
    public float collectableDist = 3f;
    public LayerMask collectableLayer;
    public LayerMask visionTestLayers;

    private float speed = 0f;
    private float recordYPos = 0f;
    [HideInInspector]
    public float fallDist = 0f;

    private bool isDiagonal = false, isVertical = false, isHorizontal = false, facingF = false, facingR = false, facingL = false, facingB = false;
    private bool groundedCheck = false, isCrouched = false;

    private CapsuleCollider bodyCollider;
    private Transform bodyMesh;
    private Vector3 bodyCentre = Vector3.zero, bodyScale = Vector3.zero, bodyPos = Vector3.zero; //bodyScale and bodyPos will be replaced by animation
    private float bodyHeight = 0f;
    [HideInInspector]
    public enum FacingDirection { LEFT, RIGHT, FRONT, BACK, FRONTLEFT, FRONTRIGHT, BACKLEFT, BACKRIGHT }
    [HideInInspector]
    public FacingDirection facingDirection;
    
    private float timeToGround = 0;
    #endregion

    #region Start
    //Calling on the CharacterController Component
    void Start()
    {
        Debug.DrawRay(transform.position, transform.right, Color.red);
        controller = GetComponent<Rigidbody>();
        controller.mass = GameManager.Instance.playerMass;
        scytheScript = GetComponent<ThrowableScythe>();

        bodyCollider = GetComponent<CapsuleCollider>();
        bodyCentre = bodyCollider.center;
        bodyHeight = bodyCollider.height;

        //replaced by animation later.
        bodyMesh = transform.GetChild(0);
        bodyScale = bodyMesh.localScale;
        bodyPos = bodyMesh.localPosition;
    }
    #endregion

    #region Update
    //Calling the PlayerJumping function
    void Update()
    {
        if (GameManager.Instance.playerActive == false)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && !isCrouching) // unable to jump while crouching
        {
            distToGround = 0f;
            isJumping = true;
            if (isGrounded)
                Jump();
        }    

        if (Input.GetMouseButtonDown(0) && scytheScript.isThrown == true)
        {
            if (GameManager.Instance.Energy >= GameManager.Instance.teleportingEnergy)
            {
                StartCoroutine(TeleportToScythe());
                GameManager.Instance.Energy -= GameManager.Instance.teleportingEnergy;
                GameManager.Instance.onCD = true;
                GameManager.Instance.CDTimer = 0;
            }
        }

        if (Input.GetMouseButtonDown(1) && !GameManager.Instance.isHolding)
            Collect();

        EquipScythe();

        if (GameManager.Instance.scytheEquiped)
            scythe.SetActive(true);
        else
            scythe.SetActive(false);

        isCrouching = Input.GetKey(KeyCode.LeftShift) ? true : false; //crouching
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isCrouched = true;
        if(isCrouched && !isCrouching)
        {
            if (Physics.Raycast(transform.position, Vector3.up, bodyHeight))
                isCrouching = true;
            else
            {
                isCrouching = false;
                isCrouched = false;
            }
        }
    }

    private void FixedUpdate() //prevent character walking into walls.
    {
        Grounded();
        FallDistCalculate();
        JumpBufferCayoteTime();
        if(movable)
            Movement();
        DirectionSwitch();
        Crouch();
    }
    #endregion

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
        //float timeToLand = 0f;
        if (!GameManager.Instance.isHolding || GameManager.Instance.holdingLightObject) // character can not jump if it's holding heavy objects.
        {
            controller.velocity = Vector3.up * jumpForce; //using velocity instead of addforce to ensure each jump reaches the same height.
            isJumping = false;
            distToGround = 0;
        }
    }
    #endregion

    #region Crouch
    void Crouch()
    {
        if (isCrouching)
        {
            bodyCollider.center = new Vector3(0f, bodyCentre.y / 2f, 0f);
            bodyCollider.height = bodyHeight / 2f;

            //visual, replace by animation
            bodyMesh.localScale = new Vector3(bodyScale.x, bodyScale.y / 2f, bodyScale.z);
            bodyMesh.localPosition = new Vector3(bodyPos.x, bodyPos.y / 2f, bodyPos.z);
        }
        else
        {
            bodyCollider.center = bodyCentre;
            bodyCollider.height = bodyHeight;

            //visual, replace by animation
            bodyMesh.localScale = bodyScale;
            bodyMesh.localPosition = bodyPos;
        }
    }
    #endregion

    #region Grounded Testing
    void Grounded()
    {
        Vector3 dir = new Vector3(0, -1, 0);
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + bodyCentre.y, transform.position.z), dir, distanceGround))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            controller.AddForce(Physics.gravity * gModifier);
        }
    }
    #endregion

    #region Collecting
    void Collect()
    {
        RaycastHit[] hits;

        Vector3 centre = transform.position + Vector3.up * collectableDist + transform.right * collectableDist;
        hits = Physics.BoxCastAll(centre, new Vector3(collectableDist, collectableDist, collectableDist), transform.right, transform.rotation, 0f, collectableLayer);

        for (int i = 0; i < hits.Length; i ++)
        {
            if (hits[i].transform.tag != "Untagged")
            {
                if (Physics.Linecast(transform.position, hits[i].transform.position, visionTestLayers))
                    return;
                else
                {
                    if (hits[i].transform.tag == "Soul")
                    {
                        GameManager.Instance.scytheEquiped = true;
                        Destroy(hits[i].transform.gameObject);
                        GameManager.Instance.totalSoulNo -= 1;
                        //do something --> collected amount, visual clue...
                    }

                    if (hits[i].transform.tag == "FakeSoul")
                    {
                        GameManager.Instance.scytheEquiped = true;
                        GameManager.Instance.dead = true;
                        //do something --> collected amount, visual clue...
                    }

                    if (hits[i].transform.tag == "HiddenItem")
                    {
                        GameManager.Instance.Timer += GameManager.Instance.rewardTime;
                        Destroy(hits[i].transform.gameObject);
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
                GameManager.Instance.scytheEquiped = !GameManager.Instance.scytheEquiped;
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

    #region JumpBuffering&CayoteTime
    void JumpBufferCayoteTime()
    {
        if (!isJumping && !isGrounded) // cayote time
                timeInAir += Time.deltaTime;

        if (!isGrounded && isJumping)
        {
            timeToGround += Time.deltaTime;
            if (timeInAir < cayoteTime)
                Jump();
        }

        if (isGrounded && isJumping)
        {
            if (timeToGround >0f && timeToGround <= jumpBufferTime)
            {
                Jump();
                timeToGround = 0f;
            }
            else
            {
                isJumping = false;
                timeInAir = 0f;
                timeToGround = 0f;
            }
        }
    }
    #endregion

    #region FallDistanceCalculation
    void FallDistCalculate()
    {
        bool groundedCheck = false;
        if(!isGrounded)
        {
            if (recordYPos > transform.position.y)
                fallDist += recordYPos - transform.position.y;
        }
        recordYPos = transform.position.y;

        if (isGrounded != groundedCheck)
        {
            groundedCheck = isGrounded;
            if (isGrounded)
            {
                if (fallDist >= GameManager.Instance.maxSafeFallDist)
                    GameManager.Instance.dead = true;

                fallDist = 0f;
            }
        }
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
