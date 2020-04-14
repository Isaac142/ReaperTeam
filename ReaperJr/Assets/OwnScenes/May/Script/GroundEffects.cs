using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEffects : MonoBehaviour
{
    //set the collider at ignore raycast layer

    public float slowtFactor = 1f;
    public float dragModify = 3f;
    public float delayTime = 1f;

    private enum GroundEffect {SLOW, SLIP, PITTRAP }
    private GroundEffect groundEffect;

    private void Start()
    {
        if (transform.tag == "Slow")
            groundEffect = GroundEffect.SLOW;

        if (transform.tag == "Slippery")
            groundEffect = GroundEffect.SLIP;

        if (transform.tag == "PitTrap")
            groundEffect = GroundEffect.PITTRAP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            switch (groundEffect)
            {
                case GroundEffect.SLOW:
                    if (other.GetComponent<PlayerMovement>() != null) // useful for current player movement script, this method will not affect jumping.
                    {
                        other.GetComponent<PlayerMovement>().addForce -= slowtFactor * 100;
                    }
                    else
                        other.GetComponent<Rigidbody>().mass += slowtFactor; //used for addForce type of character control
                    break;

                case GroundEffect.SLIP:
                    if (other.GetComponent<Rigidbody>() != null) //prevent bug, if character has no rigid body attached
                        other.GetComponent<Rigidbody>().drag -= dragModify; //used for addForce type of character control
                    break;

                case GroundEffect.PITTRAP:
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
                    if (other.GetComponent<PlayerMovement>() != null) // useful for current player movement script, this method will not affect jumping.
                    {
                        other.GetComponent<PlayerMovement>().addForce += slowtFactor * 100;
                    }

                    else
                        other.GetComponent<Rigidbody>().mass -= slowtFactor;
                    break;

                case GroundEffect.SLIP:
                    if (other.GetComponent<Rigidbody>() != null)
                        other.GetComponent<Rigidbody>().drag += dragModify;
                    break;

                case GroundEffect.PITTRAP:
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
