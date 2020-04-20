using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    public Transform target;
    public Transform scythe;
    public Collider roomCollider;

    public Vector3 colliderSize = Vector3.zero;
    public Vector3 colliderPos = Vector3.zero;
    public float camField;

    public Vector3 cameraPos;
    public Vector3 cameraRot;

    private Vector3 offset;
    public float movingDis = 2f;
    public float camDistFactor = 10.5f;
    private float betweenDis;

    private Vector3 tarPos;
    private Vector3 scythePos;

    public float followSpeed = 2.5f;
    public float rotateSpeed = 2f;

    public Vector2 camClampX = Vector2.zero;
    public Vector2 camClampY = Vector2.zero;
    //public Vector2 camClampZ = Vector2.zero;
    private float offScreenZ = -3f; //character out of camera below this number
    public float distToEdgeWhenDisppear = 4.5f; 
    public Vector3 norRot = new Vector3(18f, 0, 0);
    public Vector3 topRot = new Vector3(24f, 0, 0);

    public float cameraDistRef = 18f;
    private bool roomChangeCheck;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = cameraPos;
        transform.eulerAngles = cameraRot;
        camField = Camera.main.fieldOfView;

        if(roomCollider != null)
        {
            SetCamera();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.changedRoom != roomChangeCheck)
        {
            roomChangeCheck = GameManager.Instance.changedRoom;
            if (!GameManager.Instance.changedRoom)
                SetCamera();
        }


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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(norRot), rotateSpeed * Time.deltaTime);
        else if (transform.position.y >= camClampY.y)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(topRot), rotateSpeed * Time.deltaTime);
       else
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(cameraRot), rotateSpeed* Time.deltaTime);
    }

    void SetCamera()
    {
        colliderSize = roomCollider.bounds.size;
        colliderPos = roomCollider.transform.position;
        camClampX = new Vector2(colliderPos.x - (colliderSize.x / 2f) + (cameraDistRef * Camera.main.fieldOfView / 100f), colliderPos.x + (colliderSize.x / 2f) - (cameraDistRef * Camera.main.fieldOfView / 100f));
        if (camClampX.x > camClampX.y)
        {
            camClampX.x = colliderPos.x;
            camClampX.y = colliderPos.x;
        }
        camClampY = new Vector2(colliderPos.y - colliderSize.y / 2f + cameraPos.y, colliderSize.y);
        cameraPos.z = colliderPos.z - (colliderSize.z / 2f) - camDistFactor;
        offScreenZ = colliderPos.z - (colliderSize.z / 2f) + distToEdgeWhenDisppear;
    }
}
