using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : MonoBehaviour
{
    [SerializeField] private ScytheScript scythe;

    private float holdDownStartTime;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Mouse Down, start holding
            holdDownStartTime = Time.time;
        }

        if (Input.GetMouseButton(1))
        {
            // Mouse still down, show force
            float holdDownTime = Time.time - holdDownStartTime;
            scythe.ShowForce(CalculateHoldDownForce(holdDownTime));
        }

        if (Input.GetMouseButtonUp(1))
        {
            // Mouse Up, Launch!
            float holdDownTime = Time.time - holdDownStartTime;
            scythe.Launch(CalculateHoldDownForce(holdDownTime));
        }
    }

    private float CalculateHoldDownForce(float holdTime)
    {
        float maxForceHoldDownTime = 2f;
        float holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        float force = holdTimeNormalized * ScytheScript.MAX_FORCE;
        return force;
    }
}
