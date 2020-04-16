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
    public ItemMovement itemMovement;

    public float mass; //object's mass
    public float drag = 5f; //object's rigidbody's sliperness on ground
    public bool isHolding;
    public bool isLigther = false;
    public GameObject player;
    public float playerMass = 1f; //need to manully set.
    public float relasingThreshold = 0.3f; 
    public float gravityFactor = 7f;

    private Rigidbody objectRB;
    private Vector3 offset;
    public bool isGround = true;

    private float iniDistance;
    private float CurrDist;
    private bool canHold = false;
    
    private Vector3 size;
    private double groundDist;
    private float localDirX;
    private float localDirY;
    private float localDirZ;

    private void Start()
    {
        isHolding = false;
        canHold = false;
        size = GetComponent<Collider>().bounds.size;

        if (GetComponent<Rigidbody>() != null)
        {
            objectRB = GetComponent<Rigidbody>();
            objectRB.mass = mass;
            objectRB.drag = drag;
            objectRB.isKinematic = true;
        }

        if (mass < playerMass)
            isLigther = true;
        else
            isLigther = false;
    }

    void Update()
    {
        if (objectRB != null && !objectRB.isKinematic)
        {
            objectRB.velocity += Vector3.up * Physics.gravity.y * (gravityFactor - 1) * Time.deltaTime;
            IsGrounded();
            if (isGround)
                objectRB.isKinematic = true;
        }
        
        if (player == null)
            return;

        if (Input.GetMouseButtonDown(1)) //events happen on click event
        {
            CanHold();
            if (canHold) //preventing player standing on object and try to hold it.
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.transform.name == transform.name)
                    {
                        if (!GameManager.Instance.isHolding && !GameManager.Instance.scytheEquiped)
                        {
                            isHolding = true;
                            if (objectRB != null)
                                objectRB.isKinematic = false;
                        }
                        else if (GameManager.Instance.isHolding)
                            isHolding = false;
                        else
                            return;
                    }
                    else
                        return;
                }
            }
            else
                return;

            if (isHolding)
            {
                player.GetComponent<Rigidbody>().mass += mass;
                gameObject.transform.parent = player.transform;

                if (isLigther)
                {
                    transform.position = new Vector3(transform.position.x, player.transform.position.y + size.y / 2, transform.position.z); //can change to hand position
                }

                if (objectRB != null)
                {
                    objectRB.constraints = RigidbodyConstraints.FreezeAll;

                    if (isLigther)
                        GameManager.Instance.holdingLightObject = true;
                    else
                    {
                        iniDistance = Vector3.Distance(transform.position, player.transform.position);
                        GameManager.Instance.holdingLightObject = false;
                    }
                }
                else return;
            }
            else
            {
                player.GetComponent<Rigidbody>().mass -= mass;
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

                if (!isLigther)
                {
                    if (!isGround) //when the center of mass is not grounded, object is at higher risk of fall.
                    {
                        objectRB.constraints = RigidbodyConstraints.None; 
                        ConstrainSetUp(); 
                    }
                    else
                        objectRB.constraints = RigidbodyConstraints.FreezeAll;

                    CurrDist = Vector3.Distance(transform.position, player.transform.position);
                    if (CurrDist - iniDistance >= relasingThreshold)
                    {
                        isHolding = false;
                        player.GetComponent<Rigidbody>().mass -= mass;
                        transform.parent = null;
                        objectRB.constraints = RigidbodyConstraints.None;
                    }

                    if (GameManager.Instance.scytheEquiped)
                    {
                        isHolding = false;
                        player.GetComponent<Rigidbody>().mass -= mass;
                        transform.parent = null;
                        objectRB.constraints = RigidbodyConstraints.None;
                    }
                }
            }
            else
            {
                
            }
        }

        GameManager.Instance.isHolding = isHolding;
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

    void IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            groundDist = System.Math.Round(Vector3.Distance(transform.position, hit.point), 2);
        }
        if (groundDist == size.x / 2 || groundDist == size.y / 2 || groundDist == size.z / 2) //might have problem with irregular shaped collider.
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