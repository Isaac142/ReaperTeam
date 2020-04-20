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

    private float iniDistance; //initial distance between character and object when the object is held. use for heavy object
    private float CurrDist; //current distance between character and object as the character is holding object. use for heavy object.
    
    private float localDirX;
    private float localDirY;
    private float localDirZ;

    bool IsGrounded() //test if the object is on ground.
    {
        return (Physics.Raycast(transform.position, Vector3.down, GetComponent<Collider>().bounds.size.y / 2));
    }

    private void Start()
    {
        isHolding = false;
        canHold = false;
        if (mass < GameManager.Instance.playerMass)
            isLigther = true;
        else
            isLigther = false;

        if (GetComponent<Rigidbody>() != null)
        {
            objectRB = GetComponent<Rigidbody>();
            objectRB.mass = mass;
            objectRB.drag = drag;
        }
    }

    private void FixedUpdate()
    {
        if (objectRB != null) //let object with rigid body fall naturally
        {
            objectRB.velocity += Vector3.up * Physics.gravity.y * (gravityFactor - 1) * Time.deltaTime;

            if (!objectRB.isKinematic)
            {
                if (IsGrounded() && !isHolding)
                    objectRB.isKinematic = true;
                else
                    objectRB.isKinematic = false;
            }
        }
    }

    void Update()
    {
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
                if (hit.collider.transform.position == transform.position)
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
                player.GetComponent<Rigidbody>().mass += mass;
                gameObject.transform.parent = player.transform;

                if (isLigther)
                {
                    transform.position = new Vector3(transform.position.x, player.transform.position.y + GetComponent<Collider>().bounds.size.y / 2, transform.position.z); //can change to hand position
                    GameManager.Instance.holdingLightObject = true;
                }
                else
                    GameManager.Instance.holdingLightObject = false;

                if (objectRB != null)
                {
                    if (!isLigther)
                        iniDistance = Vector3.Distance(transform.position, player.transform.position);
                }
                else return;
            }
            else
            {
                player.GetComponent<PlayerMovement>().speedFactor += mass;
                player.GetComponent<Rigidbody>().mass = GameManager.Instance.playerMass;
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
            if (isHolding)
            {
                objectRB.isKinematic = false;
                objectRB.mass = 0f;
                objectRB.constraints = RigidbodyConstraints.FreezeAll;

                if (!isLigther)
                {
                    if (!IsGrounded()) //when the center of mass is not grounded, object is at higher risk of fall.
                    {
                        objectRB.mass = mass;
                        objectRB.constraints = RigidbodyConstraints.None;
                        ConstrainSetUp(); //constrain rotation at the mostly upwards axis.
                        CurrDist = Vector3.Distance(transform.position, player.transform.position); //if object rotat further enough, object falls.
                    }
                    
                    if (CurrDist - iniDistance >= relasingThreshold)
                    {
                        isHolding = false;
                        GameManager.Instance.isHolding = false;
                        GameManager.Instance.canHold = true;
                        player.GetComponent<PlayerMovement>().speedFactor += mass;
                        player.GetComponent<Rigidbody>().mass = GameManager.Instance.playerMass;
                        transform.parent = null;
                        objectRB.constraints = RigidbodyConstraints.None;
                    }

                    if (GameManager.Instance.scytheEquiped) //equip scythe releases heavy object
                    {
                        isHolding = false;
                        GameManager.Instance.isHolding = false;
                        GameManager.Instance.canHold = true;
                        player.GetComponent<PlayerMovement>().speedFactor += mass;
                        player.GetComponent<Rigidbody>().mass = GameManager.Instance.playerMass;
                        transform.parent = null;
                        objectRB.constraints = RigidbodyConstraints.None;
                    }
                }
            }

            else
                objectRB.mass = mass;
        }
    }

    void CanHold() //Testing if the player is standing on the object
    {
        RaycastHit hit;
        if(Physics.Raycast(player.transform.position, Vector3.down, out hit))
        {
            if (hit.collider.transform.position == transform.position)
                canHold = false;
            else
                canHold = true;
        }
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