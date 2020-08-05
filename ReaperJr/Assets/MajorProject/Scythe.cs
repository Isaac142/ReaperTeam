
using UnityEngine;

public class Scythe : ReaperJr
{
    private void Start()
    {
        transform.position = transform.position;
    }
    public void Launch(float _velocity)
    {
        GetComponent<Rigidbody>().isKinematic = false;

        GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.right * _velocity);

        this.transform.parent = null;
    }

}
