using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/*Script is using on movable Objects
 * Right mouse click on object to hold/release it.
 * if the mass of object is less than player's mass, player carries it.
 * when the object has rigidbody component, if the mass of object is greater than player's mass, player pushes is, and the object is able to fall from edge.
 * player can teleport while carries light object.
 * equip scythe when hold heavy object will relase the object.
 * character CAN NOT jump if holding heavy object.
*/
public class ItemMovement : ReaperJr
{
    public float mass = 1f; //object's mass
    public float massModifier = 2f;
    public float drag = 5f; //object's rigidbody's sliperness on ground
    [HideInInspector]
    public bool isLigther = false, canHold = false, playerIn = false, isHolding = false;
    private bool hasRB = false;

    public float relasingThreshold = 0.3f; //when curren - iniDistance > releasingThreshold, object is released.
    public float gravityFactor = 7f;
    public float pickUpDist = 1f;

    [VectorLabels("R", "B", "G")]
    public List<GameObject> emissionObj = new List<GameObject>();
    private List<Material> mats = new List<Material>();
    public Vector3 emiColorPlayerIn = Vector3.zero, emiColorCanHold = Vector3.zero, emiColorHold = Vector3.zero; //emissionc colour settings
    private Vector3 colliderSize = Vector3.zero;

    [HideInInspector]
    public Rigidbody objectRB;
    private GameObject placeHolder;

    private float iniDistance; //initial distance between character and object when the object is held. use for heavy object
    private float CurrDist; //current distance between character and object as the character is holding object. use for heavy object.

    public bool isKeyItem = false;

    private GameObject centerMarker;
    public bool justReleased = false;
    private GameObject oriParent;

    private void Start()
    {
        if(transform.parent != null)
        oriParent = transform.parent.gameObject;
        isHolding = false;
        canHold = false;
        playerIn = false;

        if (mass < _GAME.playerMass)
            isLigther = true;
        else
            isLigther = false;

        if (GetComponent<Rigidbody>() != null)
        {
            hasRB = true;
            objectRB = GetComponent<Rigidbody>();
            objectRB.mass = mass;
            objectRB.drag = drag;
            objectRB.isKinematic = false;
        }

        if (!emissionObj.Contains(this.gameObject))
            emissionObj.Add(this.gameObject);
        foreach (GameObject obj in emissionObj)
        {
            mats.Add(obj.GetComponent<Renderer>().material);
        }

        DefaultState();
        if (!isLigther && hasRB)
        {
            centerMarker = new GameObject("Center");
            centerMarker.transform.parent = this.transform;
            centerMarker.transform.position = GetComponent<BoxCollider>().center + transform.position;
        }
    }

    void Update()
    {
        if (_GAME.gameState != GameState.INGAME)
            return;
        if (_PLAYER.isCrouching == true)
            return;

        EmissionControl();

        if (playerIn)
        {
            CanHold();
        }

        if (isHolding)
        {
            HoldingEvents();
            if (hasRB && !isLigther)
                AutoDrop();
        }
    }

    void DefaultState()
    {
        transform.parent = oriParent.transform;
        if (hasRB)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            objectRB.constraints = RigidbodyConstraints.None;
        }
        if (justReleased)
            StartCoroutine(JustRelaseReset());
    }

    void EmissionControl()
    {
        for (int i = 0; i < mats.Count; i++)
        {
            mats[i].DisableKeyword("_EMISSION");
            if (playerIn)
            {
                mats[i].EnableKeyword("_EMISSION");
                mats[i].SetColor("_EmissionColor", new Color(emiColorPlayerIn.x / 255f, emiColorPlayerIn.y / 255f, emiColorPlayerIn.z / 255f));
            }

            if (canHold)
            {
                mats[i].EnableKeyword("_EMISSION");
                mats[i].SetColor("_EmissionColor", new Color(emiColorCanHold.x / 255f, emiColorCanHold.y / 255f, emiColorCanHold.z / 255f));
            }

            if (isHolding)
            {
                mats[i].EnableKeyword("_EMISSION");
                mats[i].SetColor("_EmissionColor", new Color(emiColorHold.x / 255f, emiColorHold.y / 255f, emiColorHold.z / 255f));
            }
        }
    }

    void CanHold()
    {
        bool onTop = false;
        bool inFront = false;
        CapsuleCollider playerCollider = _PLAYER.GetComponent<CapsuleCollider>();
        float height = playerCollider.height;
        RaycastHit ver;
        //test if the player is standing on the object
        if (Physics.Raycast(_PLAYER.transform.position, Vector3.down, out ver)) //test if player is on top of the object
            onTop = (ver.collider.transform.position == transform.position) ? true : false;

        RaycastHit hor;
        Vector3 topPoint = _PLAYER.transform.position + Vector3.up * (height * 0.6f);
        Vector3 bottomPoint = _PLAYER.transform.position + Vector3.up * 0.01f;
        float radius = _PLAYER.GetComponent<CapsuleCollider>().radius - 0.03f;
        //test if player is in front and close enough to the object
        if (Physics.CapsuleCast(topPoint, bottomPoint, radius, _PLAYER.transform.right, out hor, pickUpDist))
            inFront = (hor.transform == transform) ? true : false;

        //test if player is holding other object.
        if (!_GAME.isHolding && !_PLAYER.isCrouching)
        {
            canHold = (!onTop && inFront) ? true : false;
        }
        else
            canHold = false;

        if (canHold)
        {
            GameEvents.ReportMovableHintShown(HintForMovingBoxes.CANHOLD);
            if (isKeyItem && _UI.currCollectInfo == HintForItemCollect.DEFAULT)
                GameEvents.ReportInteractHintShown(HintForInteraction.KEYITEM);
        }
        else
        {
            GameEvents.ReportMovableHintShown(HintForMovingBoxes.DEFAULT);
            if (isKeyItem)
                GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
        }

    }

    void PickUp()
    {
        isHolding = true;
        justReleased = false;
        GameEvents.ReportScytheEquipped(false);
        ReaperJr._PLAYER.movable = (ReaperJr._PLAYER.isGrounded) ? true : false;
        _GAME.isHolding = true;

        if (transform.parent != null)
            transform.parent = null;
        ReaperJr._PLAYER.speedFactor -= mass * massModifier;
        ReaperJr._PLAYER.GetComponent<Rigidbody>().mass += mass * massModifier;
        _PLAYER.jumpForce -= mass * 20f;
        this.transform.parent = ReaperJr._PLAYER.transform;

        if (isLigther)
        {
            transform.position = new Vector3(transform.position.x, _PLAYER.GetComponent<CapsuleCollider>().height / 2f + (GetComponent<Collider>().bounds.min.y), transform.position.z); //can change to hand position
            transform.eulerAngles = Vector3.zero;
            _GAME.holdingLightObject = true;
        }
        else
            _GAME.holdingLightObject = false;

        if (objectRB != null)
        {
            Destroy(objectRB);
            if(!isLigther)
                iniDistance = Vector3.Distance(centerMarker.transform.position, _PLAYER.transform.position) + relasingThreshold;          
        }
    }

    void HoldingEvents()
    {
        if (!isLigther)
        {
            GameEvents.ReportMovableHintShown(HintForMovingBoxes.HEAVYOBJNOTE);

            if (_GAME.scytheEquiped) //equip scythe releases heavy object
                StartCoroutine(Release());
        }
        else
            GameEvents.ReportMovableHintShown(HintForMovingBoxes.RELEASING);

        if (isKeyItem && _UI.currCollectInfo == HintForItemCollect.DEFAULT)
            GameEvents.ReportInteractHintShown(HintForInteraction.KEYITEM);
    }

    void AutoDrop()
    {
        CurrDist = Vector3.Distance(centerMarker.transform.position, _PLAYER.transform.position); //if object rotat further enough, object falls.

        if (!Physics.Raycast(centerMarker.transform.position, Vector3.down, (centerMarker.transform.position.y - _PLAYER.transform.position.y) + 0.05f))
        {
            if (_PLAYER.pushing)
            {
                if (objectRB == null)
                {
                    objectRB = this.gameObject.AddComponent<Rigidbody>();
                    GetComponent<BoxCollider>().isTrigger = false;
                    ConstrainSetUp(); //constrain rotation at the mostly upwards axis.
                    objectRB.mass = mass;
                    objectRB.drag = drag;
                    objectRB.isKinematic = false;
                }
            }

            if(_PLAYER.dragging)
                Destroy(objectRB);
        }

        if (CurrDist > iniDistance)
        {
            justReleased = true;
            StartCoroutine(Release());
        }
    }

    void ConstrainSetUp() //testing the local upward axis and set up constrains.
    {
        float localDirX = 0f;
        float localDirY = 0f;
        float localDirZ = 0f;

        localDirX = Mathf.Abs(Vector3.Dot(Vector3.up, transform.right));
        localDirY = Mathf.Abs(Vector3.Dot(Vector3.up, transform.up));
        localDirZ = Mathf.Abs(Vector3.Dot(Vector3.up, transform.forward));
        if (localDirX > localDirY && localDirX > localDirZ)
            objectRB.constraints = RigidbodyConstraints.FreezeRotationX;
        if (localDirY > localDirX && localDirY > localDirZ)
            objectRB.constraints = RigidbodyConstraints.FreezeRotationY;
        if (localDirZ > localDirY && localDirZ > localDirX)
            objectRB.constraints = RigidbodyConstraints.FreezeRotationZ;
    }

    IEnumerator Release()
    {
        ReaperJr._PLAYER.movable = true;
        ReaperJr._PLAYER.speedFactor += mass * massModifier;
        _PLAYER.GetComponent<Rigidbody>().mass -= mass * massModifier;
        _PLAYER.jumpForce += mass * 20f;
        transform.parent = null;
        _GAME.holdingLightObject = false;

        if (hasRB)
        {
            if(objectRB == null)
                objectRB = this.gameObject.AddComponent<Rigidbody>();
            if (!isLigther)
            {
                objectRB.mass = mass;
                objectRB.drag = drag;
                GetComponent<BoxCollider>().isTrigger = true;
                objectRB.isKinematic = false;
            }
        }

        isHolding = false;
        GameEvents.ReportScytheEquipped(true);
        GameEvents.ReportMovableHintShown(HintForMovingBoxes.DEFAULT);
        if (isKeyItem)
            GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
        yield return new WaitForSeconds(0.5f);
        _GAME.isHolding = false;

        DefaultState();
    }

    IEnumerator JustRelaseReset()
    {
        yield return new WaitForSeconds(2f);
        justReleased = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( objectRB != null && other.GetComponent<EnemyPatrol>() != null)
        {
            if (other.GetComponent<EnemyPatrol>().tag == "Enemy" || other.GetComponent<EnemyPatrol>().tag == "Dummy")
                objectRB.isKinematic = true;
        }

        if (objectRB != null && !justReleased && !isHolding)
        {
            if (other.tag == "Player" || other.tag == "Scythe")
            {
                if (!isLigther)
                    objectRB.isKinematic = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectRB != null)
            objectRB.isKinematic = false;
    }

    private void OnEnable()
    {
        GameEvents.OnMovingObject += OnMovingObject; 
    }

    private void OnDisable()
    {
        GameEvents.OnMovingObject -= OnMovingObject;
    }

    void OnMovingObject (bool holding)
    {
        if (canHold || isHolding)
            holding = _GAME.isHolding;
       
        if(canHold && !holding)
        {
            PickUp();
        }

        if(isHolding && holding)
        {
            StartCoroutine(Release());
            iniDistance = 0f;
        }
    }
}