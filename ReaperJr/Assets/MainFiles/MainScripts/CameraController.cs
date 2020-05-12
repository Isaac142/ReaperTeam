using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Transform scythe;

    public Vector3 cameraPos;
    public Vector3 cameraRot;

    public Vector3 offset;
    public float movingDis = 2f;
    public float betweenDis;

    private Vector3 tarPos;
    private Vector3 scythePos;

    public float speed = 1f;

    public bool isViewingAll;

    public Camera main;
    public Camera secondary;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = cameraPos;
        transform.eulerAngles = cameraRot;
        isViewingAll = false;
    }

    // Update is called once per frame
    void Update()
    {
        tarPos = target.position;
        scythePos = scythe.position;
        betweenDis = Vector3.Distance(target.position, scythe.position);

        if (betweenDis > movingDis)
        {
            offset = new Vector3((scythePos.x - tarPos.x) / 2f, (scythePos.y - tarPos.y) / 2f, 0f);
        }
        else
            offset = Vector3.zero;


        transform.position = Vector3.Lerp(transform.position, new Vector3(tarPos.x, tarPos.y, 0) + cameraPos + offset, speed * Time.deltaTime);

        //if(Input.GetKeyDown(KeyCode.V))
        //    {
        //    isViewingAll = true;
        //    if(isViewingAll)
        //    {
        //        cameraPos = new Vector3(-15, 12, -30);
        //        main.gameObject.SetActive(false);
        //        secondary.gameObject.SetActive(true);
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    isViewingAll = false;
        //    if (!isViewingAll)
        //    {
        //        cameraPos = new Vector3(0, 5, -12);
        //        main.gameObject.SetActive(true);
        //        secondary.gameObject.SetActive(false);
        //    }
        //    //transform.position = Vector3.Lerp(transform.position, new Vector3(tarPos.x, tarPos.y, 0) + cameraPos + offset, speed * Time.deltaTime);
        //}
        //if (Input.GetKeyDown(KeyCode.V) && !isViewingAll)
        //{
        //    isViewingAll = true;
        //}
        //if (Input.GetKeyUp(KeyCode.V) && isViewingAll)
        //{
        //    isViewingAll = false;
        //}
        //if(isViewingAll)
        //{
        //    cameraPos = new Vector3(-15, 12, -30);
        //    main.gameObject.SetActive(false);
        //    secondary.gameObject.SetActive(true);
        //}
        //if(!isViewingAll)
        //{
        //    cameraPos = new Vector3(0, 5, -12);
        //    main.gameObject.SetActive(true);
        //    secondary.gameObject.SetActive(false);
        //}
    }
}
