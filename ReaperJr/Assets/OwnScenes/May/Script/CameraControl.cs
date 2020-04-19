using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    public Transform target;
    public Transform scythe;
    public BoxCollider roomCollider;

    public Vector3 cameraPos;
    public Vector3 cameraRot;

    public Vector3 offset;
    public float movingDis = 2f;
    private float betweenDis;

    private Vector3 tarPos;
    private Vector3 scythePos;

    public float followSpeed = 2.5f;
    public float rotationSpeed = 4f;

    public Vector2 camClampX = Vector2.zero;
    public Vector2 camClampY = Vector2.zero;
    public float norRot = 17f;
    public float topRot = 24f;
    public float offScreenZ = -3f;
    //public Vector2 camClampZ = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = cameraPos;
        transform.eulerAngles = cameraRot;
    }

    // Update is called once per frame
    void Update()
    {
        tarPos = target.position;
        scythePos = scythe.position;
        betweenDis = Vector3.Distance(target.position, scythe.position); //need a trigger later to prevent lag, might be when scythe throws.

        if (betweenDis > movingDis)
        {
            offset = new Vector3((scythePos.x - tarPos.x) / 2f, (scythePos.y - tarPos.y) / 2f, 0f);
        }
        else
            offset = Vector3.zero;

        transform.position = Vector3.Lerp(transform.position, tarPos + cameraPos + offset, followSpeed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, camClampX.x, camClampX.y), Mathf.Clamp(transform.position.y, camClampY.x, camClampY.y), cameraPos.z);

        if (tarPos.z <= offScreenZ)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler (norRot, 0f, 0f), rotationSpeed * Time.deltaTime);
        else if (transform.position.y >= camClampY.y)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(topRot, 0f, 0f), rotationSpeed * Time.deltaTime);
        else if(transform.eulerAngles != cameraRot)
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(cameraRot), rotationSpeed * Time.deltaTime);
    }
}
