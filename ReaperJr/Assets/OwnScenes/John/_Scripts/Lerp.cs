using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerp : MonoBehaviour
{
    public Transform target1;
    public Transform target2;
    public float fract;


    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        transform.position = Vector3.Lerp(target1.position, target2.position, fract);
    }
}
