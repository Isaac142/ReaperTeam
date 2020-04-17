using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class FloatingObject : MonoBehaviour
{
    public Transform water;

    private float waterLevel = 0f;
    public float floatThreshold = 2f;
    public float waterDensity = 0.125f;
    public float downForce = 0f;

    public float forceFactor;
    private Vector3 floatForce;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        waterLevel = water.position.y;

        forceFactor = 1f - ((transform.position.y - waterLevel) / floatThreshold);
        if (forceFactor > 0)
        {
            floatForce = -Physics.gravity * rb.mass * (forceFactor - rb.velocity.y * waterDensity);
            floatForce += new Vector3(0f, -downForce * rb.mass, 0f);
            rb.AddForceAtPosition(floatForce, transform.position);
        }
    }
    void FixedUpdate()
    {
        
    }
}
