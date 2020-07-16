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
public enum MovingState { DEFAULT, PLARERIN, CANHOLD, HOLDING }
public class ItemMovement : ReaperJr
{
    [HideInInspector]
    public MovingState movingstate;
    public float mass = 1f; //object's mass
    public float massModifier = 2f;
    public float drag = 5f; //object's rigidbody's sliperness on ground
    [HideInInspector]
    public bool isLigther = false, canHold = false, playerIn = false, isHolding = false;

    public float relasingThreshold = 0.3f; //when curren - iniDistance > releasingThreshold, object is released.
    public float gravityFactor = 7f;
    public float pickUpDist = 1f;

    [VectorLabels("R", "B", "G")]
    public List<GameObject> emissionObj = new List<GameObject>();
    private List<Material> mats = new List<Material>();
    public Vector3 emiColorPlayerIn = Vector3.zero, emiColorCanHold = Vector3.zero, emiColorHold = Vector3.zero; //emissionc colour settings
    private Vector3 colliderSize = Vector3.zero;

    private Rigidbody objectRB;
    private GameObject placeHolder;

    private float iniDistance; //initial distance between character and object when the object is held. use for heavy object
    private float CurrDist; //current distance between character and object as the character is holding object. use for heavy object.

    private void Start()
    {
        isHolding = false;
        canHold = false;
        playerIn = false;

        if (mass < _GAME.playerMass)
            isLigther = true;
        else
            isLigther = false;

        if (GetComponent<Rigidbody>() != null)
        {
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

        StartCoroutine(DefaultState());
    }

    void Update()
    {
        if (_GAME.playerActive == false)
            return;

        EmissionControl();

        if (playerIn)
        {
            CanHold();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canHold)
                PickUp();
            else if (isHolding)
                StartCoroutine(Release());
            else
                return;
        }

        if (isHolding)
            HoldingEvents();
    }

    IEnumerator DefaultState()
    {
        yield return new WaitForSeconds(0.5f);
        if (objectRB != null)
        {
            if (objectRB.velocity.sqrMagnitude < 0.1f)
                objectRB.isKinematic = true;
            else
                objectRB.isKinematic = false;
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
        Vector3 topPoint = _PLAYER.transform.position + Vector3.up * (height - 0.01f);
        Vector3 bottomPoint = _PLAYER.transform.position + Vector3.up * 0.01f;
        float radius = _PLAYER.GetComponent<CapsuleCollider>().radius - 0.03f;
        //test if player is in front and close enough to the object
        if (Physics.CapsuleCast(topPoint, bottomPoint, radius, _PLAYER.transform.right, out hor, pickUpDist))
            inFront = (hor.transform == transform) ? true : false;

        //test if player is holding other object.
        if (!_GAME.isHolding)
        {
            canHold = (!onTop && inFront) ? true : false;
        }
        else
            canHold = false;

        if (canHold)
        {
            _UI.SetHints(1);
            _UI.hint1.text = "Press E key to Hold Object.";
        }
        else
            _UI.FadeInPanel(_UI.hintsPanel);
    }

    void PickUp()
    {
        isHolding = true;
        GameEvents.ReportScytheEquipped(false);
        ReaperJr._PLAYER.movable = (ReaperJr._PLAYER.isGrounded) ? true : false;
        _GAME.isHolding = true;

        if (transform.parent != null)
            transform.parent = null;
        ReaperJr._PLAYER.speedFactor -= mass * massModifier;
        ReaperJr._PLAYER.GetComponent<Rigidbody>().mass += mass * massModifier;
        this.transform.parent = ReaperJr._PLAYER.transform;

        if (isLigther)
        {
            transform.position = new Vector3(transform.position.x, _PLAYER.GetComponent<CapsuleCollider>().height / 2f + transform.position.y, transform.position.z); //can change to hand position
            transform.eulerAngles = Vector3.zero;
            _GAME.holdingLightObject = true;
        }
        else
            _GAME.holdingLightObject = false;

        if (objectRB != null)
        {
            objectRB.isKinematic = true;
            if (!isLigther)
            {
                GetComponent<BoxCollider>().isTrigger = false;
                ConstrainSetUp(); //constrain rotation at the mostly upwards axis.
                objectRB.mass = 0;
                objectRB.isKinematic = false;
                iniDistance = Vector3.Distance(transform.position, _PLAYER.transform.position);
            }
        }
    }

    void HoldingEvents()
    {
        if (!isLigther)
        {
            if (_GAME.scytheEquiped) //equip scythe releases heavy object
            {
                StartCoroutine(Release());
            }
        }

        if (objectRB != null) //dynamic events
        {
            if (!isLigther)
            {
                CurrDist = Vector3.Distance(transform.position, _PLAYER.transform.position); //if object rotat further enough, object falls.

                if (CurrDist - iniDistance >= relasingThreshold)
                {
                    StartCoroutine(Release());
                }
            }
        }

        _UI.SetHints(1);
        _UI.hint1.text = "Press E key to Release Object.";
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
        transform.parent = null;
        _GAME.holdingLightObject = false;

        if (objectRB != null)
        {
            objectRB.isKinematic = false;
            if (!isLigther)
            {
                objectRB.mass = mass;
                GetComponent<BoxCollider>().isTrigger = true;
            }
        }

        isHolding = false;
        yield return new WaitForSeconds(0.1f);
        _GAME.isHolding = false;

        StartCoroutine(DefaultState());
    }
}