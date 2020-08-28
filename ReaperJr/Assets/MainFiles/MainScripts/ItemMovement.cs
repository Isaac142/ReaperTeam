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
    public bool canHold = false, playerIn = false, isHolding = false;
    private bool hasRB = false;

    public float relasingThreshold = 0.3f; //when curren - iniDistance > releasingThreshold, object is released.
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
    private  bool justReleased = false;
    private GameObject oriParent;

    [HideInInspector]
    public enum ObjectType { HEAVY, LIGHT}
    [HideInInspector]
    public ObjectType weight;
    bool canHoldTest = true;

    private void Start()
    {
        if(transform.parent != null)
        oriParent = transform.parent.gameObject;
        isHolding = false;
        canHold = false;
        playerIn = false;

        if (mass < _GAME.playerMass)
            weight = ObjectType.LIGHT;
        else
            weight = ObjectType.HEAVY;

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

        switch(weight)
        {
            case ObjectType.HEAVY:
                if(hasRB)
                {
                    centerMarker = new GameObject("Center");
                    centerMarker.transform.parent = this.transform;
                    centerMarker.transform.position = GetComponent<BoxCollider>().center + transform.position;
                }
                break;
        }
    }

    void Update()
    {
        if (_GAME.gameState != GameState.INGAME)
        {
            if (isHolding)
                StartCoroutine(Release());
            return;
        }

        if (_PLAYER.isCrouching == true)
            return;

        EmissionControl();

        if (playerIn && !_GAME.isHolding && canHoldTest)
        {
            CanHold();

            if (canHold && _UI.currCollectInfo == HintForItemCollect.DEFAULT)
            {
                if (_UI.currMovingInfo != HintForMovingBoxes.CANHOLD)
                    GameEvents.ReportMovableHintShown(HintForMovingBoxes.CANHOLD);
                if (isKeyItem && _UI.currInteractInfo != HintForInteraction.KEYITEM)
                    GameEvents.ReportInteractHintShown(HintForInteraction.KEYITEM);
            }
            else
            {
                GameEvents.ReportMovableHintShown(HintForMovingBoxes.DEFAULT);
                GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
            }
        }

        if (isHolding)
        {
            HoldingEvents();

            switch(weight)
            {
                case ObjectType.HEAVY:
                    if(hasRB)
                        AutoDrop();
                    break;
            }
        }
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

    void DefaultState()
    {
        if (oriParent != null)
            transform.parent = oriParent.transform;
        else
            Destroy(this.gameObject);
        if (hasRB)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            objectRB.constraints = RigidbodyConstraints.None;
        }
    }

    void CanHold()
    {
        bool onTop = false;
        bool inFront = false;
        RaycastHit ver;
        //test if the player is standing on the object
        if (Physics.Raycast(_PLAYER.transform.position, Vector3.down, out ver)) //test if player is on top of the object
            onTop = (ver.collider.transform == transform) ? true : false;

            RaycastHit hor;
            //test if player is in front and close enough to the object
            if (Physics.Raycast(_PLAYER.transform.position + Vector3.up * 0.2f, _PLAYER.transform.right, out hor, pickUpDist))
                inFront = (hor.transform == transform) ? true : false;
            
            //test if player is holding other object.
            if (!_GAME.isHolding && !_PLAYER.isCrouching)
                canHold = (!onTop && inFront) ? true : false;
        if (canHold)
            GameEvents.ReportOnCanHoldTest(true);
        else
            GameEvents.ReportOnCanHoldTest(false);
    }

    void PickUp()
    {
        canHold = false;
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

        if(objectRB != null)
            Destroy(objectRB);

        switch (weight)
        {
            case ObjectType.HEAVY:
                _GAME.holdingLightObject = false;

                if(objectRB != null)
                    iniDistance = Vector3.Distance(centerMarker.transform.position, _PLAYER.transform.position) + relasingThreshold;
                GameEvents.ReportMovableHintShown(HintForMovingBoxes.HEAVYOBJNOTE);
                break;

            case ObjectType.LIGHT:
                transform.position = new Vector3(transform.position.x, _PLAYER.GetComponent<CapsuleCollider>().height / 2f + (GetComponent<Collider>().bounds.min.y), transform.position.z); //can change to hand position
                transform.eulerAngles = Vector3.zero;
                _GAME.holdingLightObject = true;
                GameEvents.ReportMovableHintShown(HintForMovingBoxes.RELEASING);
                break;
        }

        if (isKeyItem && _UI.currCollectInfo == HintForItemCollect.DEFAULT)
            GameEvents.ReportInteractHintShown(HintForInteraction.KEYITEM);
    }

    void HoldingEvents()
    {
        switch(weight)
        {
            case ObjectType.HEAVY:
                if (_GAME.scytheEquiped) //equip scythe releases heavy object
                    StartCoroutine(Release());
                break;
        }
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
            StartCoroutine(Release());
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
            if (objectRB == null)
            {
                objectRB = this.gameObject.AddComponent<Rigidbody>();
                objectRB.mass = mass;
                objectRB.drag = drag;
            }

            objectRB.isKinematic = false;

            switch (weight)
            {
                case ObjectType.HEAVY:
                    GetComponent<BoxCollider>().isTrigger = true;
                    break;
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
        justReleased = true;
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
                switch (weight)
                {
                    case ObjectType.HEAVY:
                        objectRB.isKinematic = true;
                        break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectRB != null)
            objectRB.isKinematic = false;
        GameEvents.ReportOnCanHoldTest(false);
    }

    private void OnEnable()
    {
        GameEvents.OnMovingObject += OnMovingObject;
        GameEvents.OnCanHold += OnCanHold;
    }

    private void OnDisable()
    {
        GameEvents.OnMovingObject -= OnMovingObject;
        GameEvents.OnCanHold -= OnCanHold;
    }

    void OnMovingObject (bool holding)
    {
        GameEvents.ReportOnCanHoldTest(false);
        //if (canHold || isHolding)
        holding = _GAME.isHolding;

        if (canHold && !holding)
        {
            PickUp();
        }

        if (isHolding && holding)
        {
            StartCoroutine(Release());
            iniDistance = 0f;
        }
    }

    void OnCanHold(bool canHoldReport) // ensure the character can only pick up one object.
    {
        if (canHoldReport)
        {
            if (canHoldTest && !canHold)
                canHoldTest = false;
        }
        else
        {
            if(!canHoldTest)
                canHoldTest = true;
        }
    }
}