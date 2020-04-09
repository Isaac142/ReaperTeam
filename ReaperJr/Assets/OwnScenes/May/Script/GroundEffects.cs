using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEffects : MonoBehaviour
{
    //set the collider at ignore raycast layer

    public float slowtFactor = 1f;
    public float dragModify = 3f;
    public float delayTime = 1f;

    private enum GroundEffect {SLOW, SLIP, TRAP }
    private GroundEffect groundEffect;

    private void Start()
    {
        if (transform.tag == "Slow")
            groundEffect = GroundEffect.SLOW;

        if (transform.tag == "Slippery")
            groundEffect = GroundEffect.SLIP;

        if (transform.tag == "Trap")
            groundEffect = GroundEffect.TRAP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            switch (groundEffect)
            {
                case GroundEffect.SLOW:

                    other.GetComponent<Rigidbody>().mass += slowtFactor; //used for addForce type of character control
                    break;

                case GroundEffect.SLIP:
                    other.GetComponent<Rigidbody>().drag -= dragModify; //used for addForce type of character control
                    break;

                case GroundEffect.TRAP:
                    StartCoroutine("FloorDisappear");
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (groundEffect)
            {
                case GroundEffect.SLOW:

                    other.GetComponent<Rigidbody>().mass -= slowtFactor; //used for addForce type of character control
                    break;

                case GroundEffect.SLIP:
                    other.GetComponent<Rigidbody>().drag += dragModify; //used for addForce type of character control
                    break;

                case GroundEffect.TRAP:
                    transform.GetChild(0).gameObject.SetActive(true);
                    break;
            }
        }
    }

    IEnumerator FloorDisappear()
    {
        yield return new WaitForSeconds(delayTime);

        transform.GetChild(0).gameObject.SetActive(false);
    }
}
