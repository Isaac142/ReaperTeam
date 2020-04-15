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
    public float mass;
    public ItemMovement itemMovement;
    public bool isHolding;
    public bool isLigther = false;
    public GameObject player;
    public float playerMass = 1f;
    public float relasingThreshold = 0.3f; 
    public float gravityFactor = 7f;
    public Rigidbody objectRB;

    private float iniDistance;
    private float CurrDist;

    private void Start()
    {
        itemMovement.enabled = false;
        isHolding = false;
        if (GetComponent<Rigidbody>() != null)
            objectRB = GetComponent<Rigidbody>();
        objectRB.mass = mass;
        objectRB.isKinematic = true;

        if (mass < playerMass)
            isLigther = true;
        else
            isLigther = false;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == transform.name)
                {
                    if (!GameManager.Instance.isHolding && !GameManager.Instance.scytheEquiped)
                        isHolding = true;
                    else
                        isHolding = false;
                }
                else
                    return;
            }

            if(isHolding)
            {
                player.GetComponent<Rigidbody>().mass += mass;
                gameObject.transform.parent = player.transform;

                if(isLigther)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + player.transform.position.y, transform.position.z); //can change to hand position
                }

                if(objectRB != null)
                {
                    if (isLigther)
                    {
                        objectRB.constraints = RigidbodyConstraints.FreezeAll;
                        GameManager.Instance.holdingLightObject = true;
                    }
                    else
                    {
                        objectRB.constraints = RigidbodyConstraints.FreezeRotationY;
                        iniDistance = Vector3.Distance(transform.position, player.transform.position);
                    }
                }
            }
            else
            {
                player.GetComponent<Rigidbody>().mass -= mass;
                transform.parent = null;
                objectRB.constraints = RigidbodyConstraints.None;
            }
        }

        if(objectRB != null)
        {
            objectRB.velocity += Vector3.up * Physics.gravity.y * (gravityFactor - 1) * Time.deltaTime;
            objectRB.isKinematic = false;

            if (isHolding)
            {
                if (!isLigther)
                {
                    CurrDist = Vector3.Distance(transform.position, player.transform.position);
                    if(CurrDist - iniDistance >= relasingThreshold)
                    {
                        isHolding = false;
                        player.GetComponent<Rigidbody>().mass -= mass;
                        transform.parent = null;
                        objectRB.constraints = RigidbodyConstraints.None;
                    }

                    if(GameManager.Instance.scytheEquiped)
                    {
                        isHolding = false;
                        player.GetComponent<Rigidbody>().mass -= mass;
                        transform.parent = null;
                        objectRB.constraints = RigidbodyConstraints.None;
                    }
                }
            }
        }

        GameManager.Instance.isHolding = isHolding;
    }
}
// note: working on check if the object is stationary, if it is, isKinematic = true.