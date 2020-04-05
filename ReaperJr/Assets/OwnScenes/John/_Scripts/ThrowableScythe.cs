using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableScythe : MonoBehaviour
{


    #region Variables

    private bool isHolding = false;

    private bool launched = false;
    public float chargeSpeed = 0.1f;
    const float min = 0f, max = 1f;
    float throwPower = 100f;
    public float chargeValue;

    float normalThrowTimer = 0f, normalThrowThreshold = 0.5f;

    public Rigidbody scythe;                    // The scythe object
    public float throwForce = 50;               // Amount of force to apply when throwing
    public Transform target;                    // the target; which is the player's hand.
    public Transform curve_point;               // The middle point between the scythe and the player's hand, to give it a curve
    private Vector3 old_pos;                    // Last position of the scythe before returning it, to use in the Bezier Quadratic Curve formula
    private bool isReturning = false;           // Is the scythe returning? To update the calculations in the Update method
    private float time = 0.0f;                  // Timer to link to the Bezier formual, Beginnning = 0, End = 1

    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            isHolding = true;
            normalThrowTimer += Time.deltaTime;
            if (chargeValue < max)
            {
                chargeValue += chargeSpeed * Time.deltaTime;
                if (chargeValue > max)
                {
                    chargeValue = max;
                }
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            if (normalThrowTimer < normalThrowThreshold)
            {
                ThrowScythe();
                scythe.GetComponent<ParabolaController>().FollowParabola();
            }

            if (chargeValue > 0.2f)
            {
                Fire(chargeValue * throwPower);
            }
            chargeValue = min;
            normalThrowTimer = 0f;
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            ReturnScythe();
        }

        // If the scythe is returning
        if (isReturning)
        {
            // Returning calcs
            if (time < 1.0f)
            {
                // Update its position by using the Bezier formula based on the current time
                scythe.position = getBQCPoint(time, old_pos, curve_point.position, target.position);
                // Reset its rotation (from current to the targets rotation) with 50 units/s
                scythe.rotation = Quaternion.Slerp(scythe.transform.rotation, target.rotation, 50 * Time.deltaTime);
                // Increase our timer, if you want the scythe to return faster, then increase "time" more
                // With something like:
                // time += Timde.deltaTime * 2;
                // It will return as twice as fast
                time += Time.deltaTime;
            }
            else
            {
                // Otherwise, if it is 1 or more, we reached the target so reset
                ResetScythe();
            }
        }
    }
    #endregion

    #region Charge
    void Fire(float power)
    {
        Debug.Log("This much power: " + power);
        ThrowScythe();
    }
    #endregion

    #region Throw scythe
    void ThrowScythe()
    {
        // The scythe isn't returning
        isReturning = false;
        // Deatach it form its parent
        scythe.transform.parent = null;
        // Set isKinematic to false, so we can apply force to it
        scythe.isKinematic = false;
        // Add force to the forward axis of the camera
        // We used TransformDirection to conver the axis from local to world
        scythe.AddForce(Camera.main.transform.TransformDirection(Vector3.right) * throwForce, ForceMode.Impulse);
        scythe.AddTorque(scythe.transform.TransformDirection(Vector3.back) * 100, ForceMode.Impulse);
    }
    #endregion

    #region Return Scythe
    void ReturnScythe()
    {
        // We are returning the scythe; so it is in its first point where time = 0
        time = 0.0f;
        // Save its last position to refer to it in the Bezier formula
        old_pos = scythe.position;
        // Now we are returning the scythe, to start the calculations in the Update method
        isReturning = true;
        // Reset its velocity
        scythe.velocity = Vector3.zero;
        // Set isKinematic to true, so now we control its position directly without being affected by force
        scythe.isKinematic = true;
    }
    #endregion

    #region Reset Scythe
    void ResetScythe()
    {
        // Scythe has reached, so it is not returning anymore
        isReturning = false;
        // Attach back to its parent, in this case it will attach it to the player (or where you attached the script to)
        scythe.transform.parent = transform;
        // Set its position to the target's
        scythe.position = target.position;
        // Set its rotation to the target's
        scythe.rotation = target.rotation;
    }

    // Bezier Quadratic Curve formula
    Vector3 getBQCPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // "t" is always between 0 and 1, so "u" is other side of t
        // If "t" is 1, then "u" is 0
        float u = 1 - t;
        // "t" square
        float tt = t * t;
        // "u" square
        float uu = u * u;
        // this is the formula in one line
        // (u^2 * p0) + (2 * u * t * p1) + (t^2 * p2)
        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }
    #endregion
}
