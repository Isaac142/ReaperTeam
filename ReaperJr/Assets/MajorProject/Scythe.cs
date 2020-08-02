
using UnityEngine;

public class Scythe : ReaperJr
{
    public void Launch(float _velocity)
    {
        

        GetComponent<Rigidbody>().isKinematic = false;

        GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.right * _velocity);
    }
    
}
