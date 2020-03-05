using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    //Target Player
    public Transform target;

    Vector3 velocity = Vector3.zero;

    public float smoothTimer = .15f;

    //Setting Max Y Value
    public bool YMaxEnabled = false;
    public float YMaxValue = 0;

    //Setting Min Y Value
    public bool YMinEnabled = false;
    public float YMinValue = 0;

    //Setting Max X Value
    public bool XMaxEnabled = false;
    public float XMaxValue = 0;

    //Setting Min X Value
    public bool XMinEnabled = false;
    public float XMinValue = 0;

    void FixedUpdate()
    {
        //Tarets Position
        Vector3 targetPos = target.position;

        //Vertical
        if (YMinEnabled && YMaxEnabled)
            targetPos.y = Mathf.Clamp(target.position.y, YMinValue, YMaxValue);

        else if (YMinEnabled)
            targetPos.y = Mathf.Clamp(target.position.y, YMinValue, target.position.y);

        else if (YMaxEnabled)
            targetPos.y = Mathf.Clamp(target.position.y, target.position.y, YMaxValue);

        //Horizontal
        if (XMinEnabled && XMaxEnabled)
            targetPos.x = Mathf.Clamp(target.position.x, XMinValue, XMaxValue);

        else if (XMinEnabled)
            targetPos.x = Mathf.Clamp(target.position.x, XMinValue, target.position.x);

        else if (XMaxEnabled)
            targetPos.x = Mathf.Clamp(target.position.x, target.position.x, XMaxValue);




        //Camera and Target Z position
        targetPos.z = transform.position.z;

        //Camera smoothing
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTimer);

    }
         
    
}
