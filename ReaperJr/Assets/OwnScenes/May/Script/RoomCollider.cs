using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    Collider roomCollider;
    [HideInInspector]
    public CameraControlScript cameraControl;
    public List<GameObject> inFront = new List<GameObject>(); //all game objects will not be shown when player is in this collider
    public List<MeshRenderer> frontWall = new List<MeshRenderer>(); //front barriers that will not show in the screen. --> not show their mesh, but still using their collider.
    [HideInInspector]
    public Vector3 roomPosition = Vector3.zero;
    [HideInInspector]
    public Vector2 roomSides = Vector2.zero, roomHeight = Vector2.zero, roomDepth = Vector2.zero;
    private bool roomSwitch;

    private void Awake()
    {
        cameraControl = Camera.main.GetComponent<CameraControlScript>();
    }

    private void Start()
    {
        roomCollider = GetComponent<Collider>();
        roomPosition = roomCollider.transform.position;
        roomSides = new Vector2(roomCollider.transform.position.x - roomCollider.bounds.size.x / 2f, roomCollider.transform.position.x + roomCollider.bounds.size.x / 2f);
        roomHeight = new Vector2(roomCollider.transform.position.y - roomCollider.bounds.size.y / 2f, roomCollider.transform.position.y + roomCollider.bounds.size.y / 2f);
        roomDepth = new Vector2(roomCollider.transform.position.z - roomCollider.bounds.size.z / 2f, roomCollider.transform.position.z + roomCollider.bounds.size.z / 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (transform.tag == "LevelCollider")
            {
                cameraControl.levelHorBoundaries = new Vector2(roomSides.x, roomSides.y);
                cameraControl.levelVerBoundaries = new Vector2(roomHeight.x, roomHeight.y);
                cameraControl.levelDepthBoundaries = new Vector2(roomDepth.x, + roomDepth.y);
            }
            else
            {
                cameraControl.roomPosition = roomPosition;
                if (transform.tag == "RoomCollider")
                {
                    StartCoroutine(cameraControl.RoomSwitch(roomSides, roomHeight, roomDepth));
                    StartCoroutine("Disappear", 0f);
                }
                if (transform.tag == "StairCollider")
                {
                    StartCoroutine(cameraControl.StairSwitch(roomSides, roomHeight, roomDepth));
                    cameraControl.onStairs = true;
                }
                if (transform.tag == "CorridorCollider")
                {
                    cameraControl.inCorridor = true;
                    if(!cameraControl.inRoom)
                    StartCoroutine(cameraControl.CoridorSwitch(roomDepth));                 
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (cameraControl.inRoom != roomSwitch)
            {
                roomSwitch = cameraControl.inRoom;
                if (!cameraControl.inRoom)
                {
                    if (transform.tag == "CorridorCollider")
                    {
                        StartCoroutine(cameraControl.CoridorSwitch(roomDepth));
                        StopCoroutine(cameraControl.CoridorSwitch(roomDepth));
                    }
                }
            }
            if (transform.tag == "LevelCollider")
            {
                if (!cameraControl.inRoom)
                    StartCoroutine("Disappear", 0f);
                else
                    StartCoroutine("Appear", 0);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine("Appear", 0);
            if (transform.tag == "LevelCollider")
            {
                cameraControl.levelHorBoundaries = Vector2.zero;
                cameraControl.levelVerBoundaries = Vector2.zero;
                cameraControl.levelDepthBoundaries = Vector2.zero;
            }

            if (transform.tag == "RoomCollider")
                cameraControl.inRoom = false;
            if (transform.tag == "StairCollider")
                cameraControl.onStairs = false;
            if (transform.tag == "CorridorCollider")
                cameraControl.inCorridor = false;
        }
    }

    IEnumerator Disappear(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        foreach (GameObject barrier in inFront)
            barrier.SetActive(false);

        foreach (MeshRenderer wall in frontWall)
            wall.enabled = false;
    }

    IEnumerator Appear(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        foreach (GameObject barrier in inFront)
            barrier.SetActive(true);

        foreach (MeshRenderer wall in frontWall)
            wall.enabled = true;
    }
}
