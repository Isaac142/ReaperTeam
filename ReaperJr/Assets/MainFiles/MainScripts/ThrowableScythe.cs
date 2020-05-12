using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableScythe : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform forceTransform;

    public PlayerMovement player;

    public float chargeSpeed = 0.1f;
    const float min = 0f, max = 1f;
    float throwPower = 100f;
    //public float chargeValue;

    float normalThrowTimer = 0f, normalThrowThreshold = 0.5f;

    public Rigidbody scythe;                    // The scythe object
    public float throwForce = 50;               // Amount of force to apply when throwing
    public Transform target;                    // the target; which is the player's hand.
    public Transform curve_point;               // The middle point between the scythe and the player's hand, to give it a curve
    private Vector3 old_pos;                    // Last position of the scythe before returning it, to use in the Bezier Quadratic Curve formula
    private bool isReturning = false;           // Is the scythe returning? To update the calculations in the Update method
    private float time = 0.0f;                  // Timer to link to the Bezier formual, Beginnning = 0, End = 1
    bool canThrow;
    bool stopThrow;
    bool throwLeft;
    bool isPlayerHolding;

    public bool isThrown = false;

    public SpriteMask chargeBar;
    #endregion

    private void Start()
    {
        canThrow = true;
        isPlayerHolding = true;

        chargeBar.alphaCutoff = 1;
    }

    #region Update
    // Update is called once per frame
    void Update()
    {
        if (!isThrown)
            scythe.transform.position = target.position;

        if (GameManager.Instance.Energy >= GameManager.Instance.throwEngery)
            canThrow = true;

        if (Input.GetMouseButton(0) && GameManager.Instance.scytheEquiped && !GameManager.Instance.onCD)
        {
            ParabolaController controller = scythe.GetComponent<ParabolaController>();
            controller.parabolaScale = 1 + normalThrowTimer * 7f;
            normalThrowTimer += Time.deltaTime;
            normalThrowTimer = Mathf.Clamp(normalThrowTimer, min, max);
            //if (chargeValue < max)
            {
                /*
                chargeValue += chargeSpeed * Time.deltaTime;
                if (chargeValue > max)
                {
                    chargeValue = max;
                }
                */
                float percentage = normalThrowTimer / max;
                chargeBar.alphaCutoff = 1 - percentage;
                Debug.Log(percentage);
                
            }
        }
        if (Input.GetMouseButtonUp(0) && canThrow && GameManager.Instance.scytheEquiped && !GameManager.Instance.onCD)
        {
            if (transform.position.x < Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
            {
                throwLeft = true;
            }
            else
            {
                throwLeft = false;
            }
            canThrow = false;

            //if (normalThrowTimer < normalThrowThreshold)
            //{
            //    ThrowScythe();
            //    ParabolaController controller = scythe.GetComponent<ParabolaController>();

            //    controller.FollowParabola();
            //    Debug.Log(normalThrowTimer);

            //}

            //if (normalThrowTimer > 0.2f)
            //{
            //    Fire(normalThrowTimer * throwPower);
            //}
            ThrowScythe();
            Fire(normalThrowTimer * throwPower);
            normalThrowTimer = min;
            
            GameManager.Instance.Energy -= GameManager.Instance.throwEngery;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ReturnScythe();
        }

        // If the scythe is returning
        if (isReturning)
        {
            Debug.Log("Is Returning");
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

        if(Vector3.Distance(scythe.velocity, Vector3.zero) < 0.5f && !isReturning)
        {
            //if coming to a stop do this
            scythe.velocity = Vector3.zero;
            scythe.angularVelocity = Vector3.zero;
            StartCoroutine(ResetRotation());
        }
    }
    #endregion

    #region Charge
    void Fire(float power)
    {
        Debug.Log("This much power: " + power);
        ThrowScythe();
        ParabolaController controller = scythe.GetComponent<ParabolaController>();
        controller.RefreshTransforms(1);
        controller.FollowParabola();
        canThrow = false;
    }
    #endregion

    #region Throw scythe
    void ThrowScythe()
    {
        if (GameManager.Instance.Energy >= GameManager.Instance.throwEngery && isPlayerHolding)
        {
            isPlayerHolding = false;
            // The scythe isn't returning
            isReturning = false;
            // Deatach it form its parent
            scythe.transform.parent = null;
            // Set isKinematic to false, so we can apply force to it
            scythe.isKinematic = false;
            // Add force to the forward axis of the camera
            // We used TransformDirection to conver the axis from local to world
            if (player.facingDirection == PlayerMovement.FacingDirection.LEFT)
            {
                scythe.AddForce(Camera.main.transform.TransformDirection(Vector3.left) * throwForce, ForceMode.Impulse);
                scythe.AddTorque(scythe.transform.TransformDirection(Vector3.back) * 100, ForceMode.Impulse);
            }
            else if (player.facingDirection == PlayerMovement.FacingDirection.RIGHT)
            {
                scythe.AddForce(Camera.main.transform.TransformDirection(Vector3.right) * throwForce, ForceMode.Impulse);
                scythe.AddTorque(scythe.transform.TransformDirection(Vector3.back) * 100, ForceMode.Impulse);
            }
            else if (player.facingDirection == PlayerMovement.FacingDirection.FRONT || player.facingDirection == PlayerMovement.FacingDirection.BACK)
            {
                canThrow = false;
                stopThrow = true;
            }
            else
            {
                canThrow = true;
                stopThrow = false;
            }
            //canThrow = false;
            isThrown = true;
            chargeBar.alphaCutoff = 1;
        }
            
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
    public void ResetScythe()
    {
        scythe.velocity = Vector3.zero;
        scythe.angularVelocity = Vector3.zero;
        scythe.GetComponent<ParabolaController>().StopFollow();
        // Scythe has reached, so it is not returning anymore
        isReturning = false;
        isPlayerHolding = true;
        // Set its position to the target's
        scythe.transform.position = target.position;
        // Set its rotation to the target's
        scythe.transform.rotation = target.rotation;
        canThrow = true;
        isThrown = false;
        // Attach back to its parent, in this case it will attach it to the player (or where you attached the script to)
        scythe.transform.parent = transform;
        //Debug.Break();
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

    #region Reset Rotation
    IEnumerator ResetRotation()
    {
        Vector3 startRotation = scythe.transform.eulerAngles;
        float timer = 0f, time = 0.2f;
        while(timer < time)
        {
            scythe.transform.eulerAngles = Vector3.Lerp(startRotation,target.eulerAngles, (timer/time));
            timer += Time.deltaTime;
            yield return null;
        }
    }
    #endregion
}
