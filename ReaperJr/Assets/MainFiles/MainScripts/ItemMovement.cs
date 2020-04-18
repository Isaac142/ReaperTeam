using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Script is using on movable Objects
 * Right mouse click on object to hold/release it.
 * if the mass of object is less than player's mass, player carries it.
 * when the object has rigidbody component, if the mass of object is greater than player's mass, player pushes is, and the object is able to fall from edge.
 * player can teleport while carries light object.
 * equip scythe when hold heavy object will relase the object.
*/
public class ItemMovement : MonoBehaviour
{
    public float mass = 1f; //object's mass
    public float drag = 5f; //object's rigidbody's sliperness on ground
    public bool isLigther = false;
    public bool canHold = false;
    private bool isHolding = false;

    public GameObject player;
    public float relasingThreshold = 0.3f; //when curren - iniDistance > releasingThreshold, object is released.
    public float gravityFactor = 7f;

    public Vector3 emiColorSet = Vector3.zero;
    public Vector3 emiColorHold = Vector3.zero;

    private Rigidbody objectRB;
    private bool isGround = true;

    private float iniDistance; //initial distance between character and object when the object is held. use for heavy object
    private float CurrDist; //current distance between character and object as the character is holding object. use for heavy object.
    
    private Vector3 colliderSize;
    private double groundDist;

    private float localDirX;
    private float localDirY;
    private float localDirZ;

    private bool checkGroundCondition; //used to test if the character has entered special ground.

    private void Start()
    {
        isHolding = false;
        canHold = true;
        colliderSize = GetComponent<Collider>().bounds.size;

        if (GetComponent<Rigidbody>() != null)
        {
            objectRB = GetComponent<Rigidbody>();
            objectRB.mass = mass;
            objectRB.drag = drag;
            objectRB.isKinematic = true;
        }
    }

    void Update()
    {
        if (objectRB != null) //let object with rigid body fall naturally
        {
            objectRB.velocity += Vector3.up * Physics.gravity.y * (gravityFactor - 1) * Time.deltaTime;
            IsGrounded();
            if (isGround)
                objectRB.isKinematic = true;
            else
                objectRB.isKinematic = false;
        }

        if (player == null) //if there's no character around, stop reading script here --> optimising performance.
            return;

        if (GameManager.Instance.canHold)
            CanHold();
        else
            canHold = false;

        if (GetComponent<Renderer>() != null)
        #region EmissionControl
        {
            if (canHold)
            {
                GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(emiColorSet.x, emiColorSet.y, emiColorSet.z));
            }
            else if (isHolding)
            {
                GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(emiColorHold.x, emiColorHold.y, emiColorHold.z));
            }
            else
                GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
        #endregion

        if (Input.GetMouseButtonDown(1)) //events happen on click event
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform.name == transform.name)
                {
                    if (!GameManager.Instance.isHolding && !GameManager.Instance.scytheEquiped)
                    {
                        if (canHold) //preventing player standing on object and try to hold it.
                        {
                            isHolding = true;
                            GameManager.Instance.canHold = false;
                            GameManager.Instance.isHolding = true;
                        }
                    }
                    else if (GameManager.Instance.isHolding)
                    {
                        isHolding = false;
                        GameManager.Instance.canHold = true;
                        GameManager.Instance.isHolding = false;
                    }
                    else
                        return;
                }
            }
            else
                return;

            if (isHolding)
            {
                player.GetComponent<PlayerMovement>().speedFactor -= mass;
                gameObject.transform.parent = player.transform;

                if (isLigther)
                {
                    transform.position = new Vector3(transform.position.x, player.transform.position.y + colliderSize.y / 2, transform.position.z); //can change to hand position
                    GameManager.Instance.holdingLightObject = true;
                }
                else
                    GameManager.Instance.holdingLightObject = false;

                if (objectRB != null)
                {
                    objectRB.isKinematic = false;
                    objectRB.constraints = RigidbodyConstraints.FreezeAll;

                    if (!isLigther)
                        iniDistance = Vector3.Distance(transform.position, player.transform.position);
                }
                else return;
            }
            else
            {
                player.GetComponent<PlayerMovement>().speedFactor += mass;
                transform.parent = null;
                GameManager.Instance.holdingLightObject = false;

                if (objectRB != null)
                {
                    objectRB.constraints = RigidbodyConstraints.None;
                }
                else
                    return;
            }
        }

        if (objectRB != null) //dynamic events
        {
            if(GameManager.Instance.onSpecialGround != checkGroundCondition) //on entre/exit special ground event.
            {
                checkGroundCondition = GameManager.Instance.onSpecialGround;
                if(GameManager.Instance.onSpecialGround)
                {
                    player.GetComponent<Rigidbody>().mass += mass;
                    player.GetComponent<PlayerMovement>().speedFactor += mass;

                    objectRB.mass = 0;
                }
                else
                {
                    player.GetComponent<Rigidbody>().mass -= mass;
                    player.GetComponent<PlayerMovement>().speedFactor -= mass;
                    objectRB.mass = mass;
                }
            }

            if (isHolding)
            {
                if (!isLigther)
                {
                    if (!isGround) //when the center of mass is not grounded, object is at higher risk of fall.
                    {
                        objectRB.constraints = RigidbodyConstraints.None; 
                        ConstrainSetUp(); //constrain rotation at the mostly upwards axis.
                    }

                    CurrDist = Vector3.Distance(transform.position, player.transform.position); //if object rotat further enough, object falls.
                    if (CurrDist - iniDistance >= relasingThreshold)
                    {
                        isHolding = false;
                        GameManager.Instance.isHolding = false;
                        GameManager.Instance.canHold = true;
                        player.GetComponent<PlayerMovement>().speedFactor += mass;
                        transform.parent = null;
                        objectRB.constraints = RigidbodyConstraints.None;
                    }

                    if (GameManager.Instance.scytheEquiped) //equip scythe releases heavy object
                    {
                        isHolding = false;
                        GameManager.Instance.isHolding = false;
                        GameManager.Instance.canHold = true;
                        player.GetComponent<PlayerMovement>().speedFactor += mass;
                        transform.parent = null;
                        objectRB.constraints = RigidbodyConstraints.None;
                    }
                }
            }
        }
    }

    void CanHold() //Testing if the player is standing on the object
    {
        RaycastHit hit;
        if(Physics.Raycast(player.transform.position, Vector3.down, out hit))
        {
            if (hit.collider.transform.name == transform.name)
                canHold = false;
            else
                canHold = true;
        }
    }   

    void IsGrounded() //test if the object is on ground.
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            groundDist = System.Math.Round(Vector3.Distance(transform.position, hit.point), 2);
        }
        if (groundDist == colliderSize.x / 2 || groundDist == colliderSize.y / 2 || groundDist == colliderSize.z / 2) //might have problem with irregular shaped collider.
        {
            isGround = true;
        }
        else
            isGround = false;
    }

    void ConstrainSetUp () //testing the local upward axis and set up constrains.
    {
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
}