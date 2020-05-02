using System.Collections;
using UnityEngine;

/*Script is using on movable Objects
 * Right mouse click on object to hold/release it.
 * if the mass of object is less than player's mass, player carries it.
 * when the object has rigidbody component, if the mass of object is greater than player's mass, player pushes is, and the object is able to fall from edge.
 * player can teleport while carries light object.
 * equip scythe when hold heavy object will relase the object.
 * character CAN NOT jump if holding heavy object.
*/
public class ItemMovement : MonoBehaviour
{
    public float mass = 1f; //object's mass
    public float drag = 5f; //object's rigidbody's sliperness on ground
    public bool isLigther = false;
    public bool canHold = false;
    public bool playerIn = false;
    private bool isHolding = false;

    public GameObject player;
    public float relasingThreshold = 0.3f; //when curren - iniDistance > releasingThreshold, object is released.
    public float gravityFactor = 7f;
    public float pickUpDist = 1f;

    [VectorLabels("R", "B", "G")]
    public Vector3 emiColorPlayerIn = Vector3.zero, emiColorCanHold = Vector3.zero, emiColorHold = Vector3.zero; //emissionc colour settings
    private Vector3 colliderSize = Vector3.zero;

    private Rigidbody objectRB;
    private GameObject placeHolder;

    private float iniDistance; //initial distance between character and object when the object is held. use for heavy object
    private float CurrDist; //current distance between character and object as the character is holding object. use for heavy object.

    public bool stableObject = false;

    bool IsFlat() //test if the object is flat on ground.
    {
        return (Mathf.Abs(Vector3.Dot(Vector3.up, transform.right)) == 1f || Mathf.Abs(Vector3.Dot(Vector3.up, transform.up)) == 1f || Mathf.Abs(Vector3.Dot(Vector3.up, transform.forward)) == 1f);
    }
    bool IsGrounded() //test if the object is on ground.
    {
        return (Physics.Raycast(transform.position, Vector3.down, GetComponent<Collider>().bounds.size.y / 2f + 0.1f));
    }

    private void Start()
    {
        isHolding = false;
        canHold = false;
        playerIn = false;

        if (mass < GameManager.Instance.playerMass)
            isLigther = true;
        else
            isLigther = false;

        if (GetComponent<Rigidbody>() != null)
        {
            objectRB = GetComponent<Rigidbody>();
            objectRB.mass = mass;
            objectRB.drag = drag;
            objectRB.isKinematic = false;
            if (stableObject)
                objectRB.isKinematic = true;
        }
    }

    private void FixedUpdate()
    {
        if (objectRB != null && !objectRB.isKinematic) //let object with rigid body fall naturally
        {
            objectRB.velocity += Vector3.up * Physics.gravity.y * (gravityFactor - 1) * Time.deltaTime;
            if (!isHolding && IsFlat() && IsGrounded())
                objectRB.isKinematic = true;
        }
    }

    void Update()
    {
        if (player == null) //if there's no character around, stop reading script here --> optimising performance.
            return;

        if (!isHolding && objectRB != null)
            objectRB.isKinematic = (IsFlat() && IsGrounded()) ? true : false;

        CanHold();

        if (GetComponent<Renderer>() != null)
        #region EmissionControl
        {
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            if (playerIn)
                GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(emiColorPlayerIn.x / 255f, emiColorPlayerIn.y / 255f, emiColorPlayerIn.z / 255f));
            if (canHold)
                GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(emiColorCanHold.x / 255f, emiColorCanHold.y / 255f, emiColorCanHold.z / 255f));
            if (isHolding)
                GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(emiColorHold.x / 255f, emiColorHold.y / 255f, emiColorHold.z / 255f));
        }
        #endregion

        if (Input.GetMouseButtonDown(1)) //events happen on click event
        {
            if (canHold && !GameManager.Instance.scytheEquiped) //preventing player standing on object and try to hold it.
            {
                player.GetComponent<PlayerMovement>().movable = (player.GetComponent<PlayerMovement>().isGrounded) ? true : false;
                isHolding = true;
                GameManager.Instance.canHold = false;
                GameManager.Instance.isHolding = true;
            }

            else if (isHolding)
            {
                isHolding = false;
                GameManager.Instance.canHold = true;
                GameManager.Instance.isHolding = false;
                player.GetComponent<PlayerMovement>().movable = true;
            }
            else
                return;

            if (isHolding)
            {
                if (transform.parent != null)
                    transform.parent = null;
                player.GetComponent<PlayerMovement>().speedFactor -= mass;
                player.GetComponent<Rigidbody>().mass += mass;
                gameObject.transform.parent = player.transform;

                if (isLigther)
                {
                    transform.position = new Vector3(transform.position.x, player.transform.position.y + GetComponent<Collider>().bounds.size.y, transform.position.z); //can change to hand position
                    transform.eulerAngles = Vector3.zero;
                    GameManager.Instance.holdingLightObject = true;
                }
                else
                    GameManager.Instance.holdingLightObject = false;

                if (objectRB != null)
                {
                    objectRB.isKinematic = false;
                    objectRB.mass = 0;
                    CreateCollider();
                    if (!isLigther)
                        iniDistance = Vector3.Distance(transform.position, player.transform.position);
                }
            }
            else
            {
                player.GetComponent<PlayerMovement>().speedFactor += mass;
                player.GetComponent<Rigidbody>().mass = GameManager.Instance.playerMass;
                transform.parent = null;
                GameManager.Instance.holdingLightObject = false;

                if (objectRB != null)
                {
                    DeleteCollider();
                    objectRB.mass = mass;
                    objectRB.constraints = RigidbodyConstraints.None;
                }
                else
                    return;
            }
        }

        if (isHolding)
        {
            if (!isLigther)
            {
                if (GameManager.Instance.scytheEquiped) //equip scythe releases heavy object
                {
                    isHolding = false;
                    GameManager.Instance.isHolding = false;
                    GameManager.Instance.canHold = true;
                    GameManager.Instance.holdingLightObject = false;
                    player.GetComponent<PlayerMovement>().speedFactor += mass;
                    player.GetComponent<Rigidbody>().mass = GameManager.Instance.playerMass;
                    transform.parent = null;
                }
            }

            if (objectRB != null) //dynamic events
            {
                objectRB.isKinematic = false;
                objectRB.mass = 0f;
                objectRB.constraints = RigidbodyConstraints.FreezeAll;
                if (placeHolder == null)
                    CreateCollider();
                transform.GetComponent<Collider>().isTrigger = true;
                placeHolder.GetComponent<Collider>().isTrigger = false;

                if (!isLigther)
                {
                    if (!IsGrounded()) //when the center of mass is not grounded, object is at higher risk of fall.
                    {
                        placeHolder.GetComponent<Collider>().isTrigger = true;
                        transform.GetComponent<Collider>().isTrigger = false;
                        objectRB.mass = mass;
                        objectRB.constraints = RigidbodyConstraints.None;
                        ConstrainSetUp(); //constrain rotation at the mostly upwards axis.
                        CurrDist = Vector3.Distance(transform.position, player.transform.position); //if object rotat further enough, object falls.
                        objectRB.AddForce(Vector3.down * 3f, ForceMode.Impulse);
                    }

                    if (CurrDist - iniDistance >= relasingThreshold)
                    {
                        DeleteCollider();
                        isHolding = false;
                        GameManager.Instance.isHolding = false;
                        GameManager.Instance.canHold = true;
                        GameManager.Instance.holdingLightObject = false;
                        player.GetComponent<PlayerMovement>().speedFactor += mass;
                        player.GetComponent<Rigidbody>().mass = GameManager.Instance.playerMass;
                        transform.parent = null;
                        objectRB.constraints = RigidbodyConstraints.None;
                        objectRB.mass = mass;
                    }

                    if (GameManager.Instance.scytheEquiped)
                    {
                        DeleteCollider();
                        objectRB.constraints = RigidbodyConstraints.None;
                        objectRB.mass = mass;
                    }
                }
            }
        }
    }

    void CanHold() //Testing if the player is standing on the object
    {
        bool onTop = false;
        bool inFront = false;
        CapsuleCollider playerCollider = player.GetComponent<CapsuleCollider>();
        RaycastHit ver;
        if (Physics.Raycast(player.transform.position, Vector3.down, out ver))
            onTop = (ver.collider.transform.position == transform.position) ? true : false;

        RaycastHit hor;
        //if (Physics.Raycast(player.transform.position, player.transform.right, out hor, pickUpDist))
        Vector3 topPoint = player.transform.position + player.GetComponent<CapsuleCollider>().center + Vector3.up * (player.GetComponent<CapsuleCollider>().height - 0.01f) / 2f;
        Vector3 bottomPoint = player.transform.position + player.GetComponent<CapsuleCollider>().center - Vector3.up * (player.GetComponent<CapsuleCollider>().height - 0.01f) / 2f;
        float radius = player.GetComponent<CapsuleCollider>().radius - 0.05f;
        if (Physics.CapsuleCast(topPoint, bottomPoint, radius, player.transform.right, out hor, pickUpDist))
            inFront = (hor.transform == transform) ? true : false;

        if (GameManager.Instance.canHold)
        {
            canHold = (!onTop && inFront) ? true : false;
        }
        else
            canHold = false;
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

    void CreateCollider()
    {
        placeHolder = new GameObject("MovablePlaceHolder");
        placeHolder.transform.position = transform.position;
        placeHolder.transform.parent = player.transform;
        objectRB.GetComponent<Collider>().isTrigger = true;
        Collider collider = CopyComponent<Collider>(objectRB.transform.GetComponent<Collider>(), placeHolder);
        colliderSize = collider.bounds.size;
        colliderSize = GetComponent<Collider>().bounds.size;
        placeHolder.transform.localScale = objectRB.transform.localScale;
    }

    T CopyComponent<T>(T original, GameObject destination) where T : Component //code from: https://answers.unity.com/questions/458207/copy-a-component-at-runtime.html?_ga=2.140673330.1201919783.1588314976-45500112.1566373877
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    void DeleteCollider()
    {
        if (placeHolder != null)
        {
            Destroy(placeHolder);
            objectRB.GetComponent<Collider>().isTrigger = false;
        }
    }
}