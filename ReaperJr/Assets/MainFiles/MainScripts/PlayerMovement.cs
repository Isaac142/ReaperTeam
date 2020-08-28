using System.Collections;
using UnityEngine;

public class PlayerMovement : Singleton<PlayerMovement>
{
    #region Variables
    //public TrajectoryPredictor trajectory;

    Rigidbody controller;
    public Transform firePoint;
    public GameObject scythe;

    //[HideInInspector]
    public Vector3 startingPos;

    private ScytheController scytheController;
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
    [HideInInspector]
    public bool canCollect = false, keyCollect = true;
    
    private float timeToGround = 0;

    public Animator anim;

    public bool walkHack;
    private bool teleported = false;
    public GameObject teleportStart, teleportEnd;

    [HideInInspector]
    public bool dragging = false, pushing = false;

    #endregion

    #region Start
    //Calling on the CharacterController Component
    void Start()
    {
        //Debug.DrawRay(transform.position, transform.right, Color.red);
        controller = GetComponent<Rigidbody>();
        controller.mass = _GAME.playerMass;
        scytheController = scythe.GetComponentInParent<ScytheController>();

        bodyCollider = GetComponent<CapsuleCollider>();
        bodyCentre = bodyCollider.center;
        bodyHeight = bodyCollider.height;

        //replaced by animation later.
        bodyMesh = transform.GetChild(0);
        bodyScale = bodyMesh.localScale;
        bodyPos = bodyMesh.localPosition;
    }

    public void Restart()
    {
        transform.position = startingPos;
        teleported = false;
        canCollect = false;
        fallDist = 0f;
        teleportStart.SetActive(false);
        teleportEnd.SetActive(false);
    }
    #endregion

    #region Update
    //Calling the PlayerJumping function
    void Update()
    {
        if (_GAME.gameState != GameState.INGAME)
            return;

        if (Input.GetButtonDown("Jump") && !isCrouching) // unable to jump while crouching
        {
            _AUDIO.Play(this.GetComponent<AudioSource>(), "Jump", 10);
            distToGround = 0f;
            isJumping = true;
            if (isGrounded)
                Jump();
        }    

        if (Input.GetMouseButtonDown(0) && !scytheController.holdingScythe)
        {
            if (_GAME.Energy >= _GAME.teleportingEnergy)
            {
                anim.SetTrigger("Teleport");
                _AUDIO.Play(this.GetComponent<AudioSource>(), "Teleport1", 10);
                StartCoroutine(TeleportToScythe());
                _GAME.Energy -= _GAME.teleportingEnergy;
                _GAME.onCD = true;
                _GAME.CDTimer = 0;
            }
        }

        if(canCollect && !_GAME.isHolding)
            Collect();

        EquipScythe();

        if (_GAME.scytheEquiped)
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

        if (_UI.currMovingInfo != HintForMovingBoxes.DEFAULT)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameEvents.ReportOnMovingObject(_GAME.isHolding);
            }
        }
        
        anim.SetBool("Holding", _GAME.isHolding);
        anim.SetBool("Carrying", _GAME.holdingLightObject);
        anim.SetBool("Drag", dragging);
        anim.SetBool("Push", pushing);
        anim.SetBool("WalkHack", walkHack);

        if (walkHack)
        {
            if (dragging || pushing)
                _AUDIO.Play(this.GetComponent<AudioSource>(), "BoxMove", 10);
        }
    }

    private void FixedUpdate() //prevent character walking into walls.
    {
        if (_GAME.gameState != GameState.INGAME)
            return;
        if (_GAME.isPaused)
            return;
        Grounded();
        if(!teleported || !_GAME.isInvincible)
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
        if(!_GAME.isHolding)
        {
            dragging = false;
            pushing = false;
        }

        float h = Input.GetAxis("Horizontal"), v = Input.GetAxis("Vertical");


        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
            walkHack = true;
        else
            StartCoroutine(ResetMovement());

        if (_GAME.isHolding && !_GAME.holdingLightObject)
        #region MovingHeavyObject
        {
            speed = speedFactor;
            pushing = false;
            dragging = false;

            if(!movable)
            {
                h = 0;
                v = 0;
            }

            if (facingF)
            {
                h = 0; //stop side way walk
                if (v < 0)
                    pushing = true;
                if (v > 0)
                    dragging = true;
                transform.eulerAngles = new Vector3(0, 90, 0);

                if (_GAME.onSpecialGround)
                    controller.AddForce((transform.forward * h - transform.right * v) * speed * addForce * Time.deltaTime);
                else
                    transform.Translate((-transform.right * h - transform.forward * v) * speed * Time.deltaTime);
            }

            if (facingB)
            {
                h = 0; //stop side way walk
                if (v > 0)
                    pushing = true;
                if (v < 0)
                    dragging = true;

                transform.eulerAngles = new Vector3(0, 270, 0);

                if (_GAME.onSpecialGround)
                    controller.AddForce((-transform.forward * h + transform.right * v) * speed * addForce * Time.deltaTime);
                else
                    transform.Translate((-transform.right * h - transform.forward * v) * speed * Time.deltaTime);
            }

            if (facingR)
            {
                v = 0; //stop side way walk
                if (h > 0)
                    pushing = true;
                if (h < 0)
                    dragging = true;
                transform.eulerAngles = new Vector3(0, 0, 0);

                if (_GAME.onSpecialGround)
                    controller.AddForce((transform.forward * v + transform.right * h) * speed * addForce * Time.deltaTime);
                else
                    transform.Translate((transform.right * h + transform.forward * v) * speed * Time.deltaTime);
            }

            if (facingL)
            {
                v = 0; //stop side way walk
                if (h < 0)
                    pushing = true;
                if (h > 0)
                    dragging = true;
                transform.eulerAngles = new Vector3(0, 180, 0);

                if (_GAME.onSpecialGround)
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

            if (_GAME.onSpecialGround)
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
        if (!_GAME.isHolding || _GAME.holdingLightObject) // character can not jump if it's holding heavy objects.
        {
            anim.SetTrigger("Jump");
            anim.SetBool("Air", !isGrounded);
            controller.velocity = Vector3.up * jumpForce; //using velocity instead of addforce to ensure each jump reaches the same height.
            isJumping = false;
            distToGround = 0;
        }
    }
    #endregion

    #region Crouch
    void Crouch()
    {

        anim.SetBool("Crouch", isCrouching);
        if (isCrouching)
        {
            bodyCollider.center = new Vector3(0f, bodyCentre.y / 2f, 0f);
            bodyCollider.height = bodyHeight / 2f;
            bodyMesh.localScale = new Vector3(bodyScale.x, bodyScale.y / 2f, bodyScale.z);
            bodyMesh.localPosition = new Vector3(bodyPos.x, bodyPos.y / 2f, bodyPos.z);
        }
        else
        {
            bodyCollider.center = bodyCentre;
            bodyCollider.height = bodyHeight;
            
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
        if(_UI.currCollectInfo != HintForItemCollect.FAKESOULWARNING)
            GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
        RaycastHit[] hits;

        Vector3 centre = transform.position + Vector3.up * collectableDist + transform.right * collectableDist;
        hits = Physics.BoxCastAll(centre, new Vector3(collectableDist, collectableDist, collectableDist), transform.right, transform.rotation, 0f, collectableLayer);

        for (int i = 0; i < hits.Length; i ++)
        {
            if (hits[i].transform.tag != "Untagged")
            {
                if (Physics.Linecast(transform.position + Vector3.up * 0.5f, hits[i].transform.position, visionTestLayers))
                {
                    GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
                    return;
                }
                else
                {
                    if (hits[i].transform.tag == "HiddenItem")
                        GameEvents.ReportCollectHintShown(HintForItemCollect.COLLECTITEMS);

                    else if (hits[i].transform.tag == "KeyItem")
                    {
                        if (keyCollect)
                            GameEvents.ReportCollectHintShown(HintForItemCollect.COLLECTITEMS);
                        else
                            GameEvents.ReportCollectHintShown(HintForItemCollect.KEYLIMIT);
                    }

                    else
                    {
                        if (_UI.currCollectInfo != HintForItemCollect.FAKESOULWARNING)
                            GameEvents.ReportCollectHintShown(HintForItemCollect.COLLECTSOULS);
                    }

                    if (Input.GetMouseButtonDown(1) && !_GAME.isHolding)
                    {
                        anim.SetTrigger("Collect");

                        if (hits[i].transform.tag == "Soul")
                        {
                            _AUDIO.Play(this.GetComponent<AudioSource>(), "SoulCollect", 10);
                            GameEvents.ReportScytheEquipped(true);
                            hits[i].transform.GetComponent<SoulType>().isCollected = true;
                            GameEvents.ReportSoulCollected(hits[i].transform.GetComponent<SoulType>());
                            _GAME.totalSoulNo -= 1;
                        }

                        if (hits[i].transform.tag == "FakeSoul")
                        {
                            GameEvents.ReportScytheEquipped(true);
                            if (hits[i].transform.GetComponent<EnemyPatrol>() != null)
                                GameEvents.ReportOnFakeSoulChasing(hits[i].transform.GetComponent<EnemyPatrol>());
                            else if (hits[i].transform.GetComponent<FakeSoulController>() != null)
                                GameEvents.ReportOnFakeSoulCollected(hits[i].transform.GetComponent<FakeSoulController>());
                            else
                                GameEvents.ReportGameStateChange(GameState.DEAD);
                        }

                        if (hits[i].transform.tag == "HiddenItem")
                        {
                            _AUDIO.Play(this.GetComponent<AudioSource>(), "SpecialCollect", 10);
                            _GAME.Timer += _GAME.rewardTime;
                            Destroy(hits[i].transform.gameObject);
                            GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
                            GameEvents.ReportOnTimeChange(true, 2f);
                        }

                        if (hits[i].transform.tag == "KeyItem" && keyCollect)
                        {
                            if (!hits[i].transform.GetComponent<KeyItem>().isCollected)
                            {
                                _AUDIO.Play(this.GetComponent<AudioSource>(), "SpecialCollect", 10);
                                hits[i].transform.GetComponent<KeyItem>().isCollected = true;
                                GameEvents.ReportKeyItemCollected(hits[i].transform.GetComponent<KeyItem>());
                            }
                        }
                    }
                }
            }

            else
                GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
        }
    }
    #endregion

    #region EquipScythe
    void EquipScythe()
    {
        if (_GAME.scytheEquiped && !_GAME.scytheaThrown)
        {
            scythe.transform.parent = firePoint;
            scythe.transform.localEulerAngles = Vector3.zero;
            scythe.transform.localPosition = Vector3.zero;
            teleportEnd.SetActive(false);
            scythe.transform.position = scythe.transform.position;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !_GAME.scytheaThrown)
        {
            _GAME.scytheEquiped = !_GAME.scytheEquiped;
            GameEvents.ReportScytheEquipped(_GAME.scytheEquiped);
        }
        else return;
    }
    #endregion

    #region Teleport
    IEnumerator TeleportToScythe()
    {
        teleportStart.SetActive(false);
        teleportEnd.SetActive(true);
        teleported = true;
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
            //yield return null;
        }
        yield return new WaitForSeconds(timeToMove);
        teleported = false;
        teleportEnd.SetActive(false);
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
                if (fallDist >= _GAME.maxSafeFallDist)
                {
                    GameEvents.ReportOnFallDeath(true);
                    GameEvents.ReportGameStateChange(GameState.DEAD);
                }

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

    public IEnumerator ResetMovement()
    {
        yield return new WaitForSeconds(0.1f);
        walkHack = false;
    }
}
