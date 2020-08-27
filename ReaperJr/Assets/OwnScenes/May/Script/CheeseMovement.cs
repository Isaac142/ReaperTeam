using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseMovement : ReaperJr
{
    public float mass = 1f; //object's mass
    public float massModifier = 2f;
    public float drag = 5f; //object's rigidbody's sliperness on ground
    [HideInInspector]
    public bool isLigther = true, canHold = false, playerIn = false, isHolding = false;
    private bool hasRB = false;
    public float pickUpDist = 1f;

    [VectorLabels("R", "B", "G")]
    public List<GameObject> emissionObj = new List<GameObject>();
    private List<Material> mats = new List<Material>();
    public Vector3 emiColorPlayerIn = Vector3.zero, emiColorCanHold = Vector3.zero, emiColorHold = Vector3.zero; //emissionc colour settings
    private Vector3 colliderSize = Vector3.zero;

    [HideInInspector]
    public Rigidbody objectRB;
    private GameObject placeHolder;

    private GameObject centerMarker;
    private GameObject oriParent;

    private void Start()
    {
        if (transform.parent != null)
            oriParent = transform.parent.gameObject;
        isHolding = false;
        canHold = false;
        playerIn = false;
        isLigther = true;

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

        if (playerIn && !_GAME.isHolding)
        {
            CanHold();
        }

        if (isHolding)
            HoldingEvents();
    }

    void DefaultState()
    {
        transform.parent = oriParent.transform;
        if (hasRB)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            objectRB.constraints = RigidbodyConstraints.None;
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

        if (canHold && _UI.currCollectInfo == HintForItemCollect.DEFAULT)
        {
            if (_UI.currMovingInfo != HintForMovingBoxes.CANHOLD)
                GameEvents.ReportMovableHintShown(HintForMovingBoxes.CANHOLD);
            if (_UI.currInteractInfo != HintForInteraction.KEYITEM)
                GameEvents.ReportInteractHintShown(HintForInteraction.KEYITEM);
        }
        else
        {
            if (_UI.currMovingInfo != HintForMovingBoxes.DEFAULT)
                GameEvents.ReportMovableHintShown(HintForMovingBoxes.DEFAULT);
            if (_UI.currInteractInfo != HintForInteraction.DEFAULT)
                GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
        }

    }

    void PickUp()
    {
        isHolding = true;
        GameEvents.ReportScytheEquipped(false);
        _GAME.isHolding = true;

        if (transform.parent != null)
            transform.parent = null;
        ReaperJr._PLAYER.speedFactor -= mass * massModifier;
        ReaperJr._PLAYER.GetComponent<Rigidbody>().mass += mass * massModifier;
        _PLAYER.jumpForce -= mass * 20f;
        this.transform.parent = ReaperJr._PLAYER.transform;

        transform.position = new Vector3(transform.position.x, _PLAYER.GetComponent<CapsuleCollider>().height / 2f + (GetComponent<Collider>().bounds.min.y), transform.position.z); //can change to hand position
        transform.eulerAngles = Vector3.zero;
        _GAME.holdingLightObject = true;

        if (objectRB != null)
            Destroy(objectRB);
    }

    void HoldingEvents()
    {
        GameEvents.ReportMovableHintShown(HintForMovingBoxes.RELEASING);
        GameEvents.ReportInteractHintShown(HintForInteraction.KEYITEM);
    }

    IEnumerator Release()
    {
        ReaperJr._PLAYER.speedFactor += mass * massModifier;
        _PLAYER.GetComponent<Rigidbody>().mass -= mass * massModifier;
        _PLAYER.jumpForce += mass * 20f;
        transform.parent = null;
        _GAME.holdingLightObject = false;

        if (hasRB)
        {
            if (objectRB == null)
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
        GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
        yield return new WaitForSeconds(0.5f);
        _GAME.isHolding = false;

        DefaultState();
    }
    private void OnEnable()
    {
        GameEvents.OnMovingObject += OnMovingObject;
    }

    private void OnDisable()
    {
        GameEvents.OnMovingObject -= OnMovingObject;
    }

    void OnMovingObject(bool holding)
    {
        holding = _GAME.isHolding;

        if (canHold && !holding)
        {
            PickUp();
        }

        if (isHolding && holding)
        {
            StartCoroutine(Release());
        }
    }
}
