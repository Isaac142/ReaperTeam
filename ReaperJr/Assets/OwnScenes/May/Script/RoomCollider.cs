using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    public RoomData room;
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
        roomPosition = room.roomPosition;
        roomSides = new Vector2(room.roomPosition.x - room.roomSize.x / 2f, room.roomPosition.x + room.roomSize.x / 2f);
        roomHeight = new Vector2(room.roomPosition.y - room.roomSize.y / 2f, room.roomPosition.y + room.roomSize.y / 2f);
        roomDepth = new Vector2(room.roomPosition.z - room.roomSize.z / 2f, room.roomPosition.z + room.roomSize.z / 2f);
    }
    private void Update()
    {
        if (room.type == "Level")
        {
            if (!cameraControl.inRoom)
                StartCoroutine("Disappear", 0f);
            else
                StartCoroutine("Appear", 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (room.type == "Level")
            {
                cameraControl.levelHorBoundaries = new Vector2(roomSides.x, roomSides.y);
                cameraControl.levelVerBoundaries = new Vector2(roomHeight.x, roomHeight.y);
                cameraControl.levelDepthBoundaries = new Vector2(roomDepth.x, + roomDepth.y);
                cameraControl.camDepthBoundaries.x = roomDepth.x - cameraControl.miniCamToEdgeDist;
            }
            else
            {
                cameraControl.roomPosition = room.roomPosition;
                if (room.isARoom)
                {
                    StartCoroutine(cameraControl.RoomSwitch(room));
                    StartCoroutine("Disappear", 0f);
                }
                if (room.type == "Stair")
                {
                    StartCoroutine(cameraControl.StairSwitch(room));
                    cameraControl.onStairs = true;
                }  
                if(room.type == "Coridor" && !cameraControl.inRoom)
                    StartCoroutine(cameraControl.CoridorSwitch(room));
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (room.type == "Coridor")
            {
                if (cameraControl.inRoom != roomSwitch)
                {
                    roomSwitch = cameraControl.inRoom;
                    if (!cameraControl.inRoom)
                    {
                        StartCoroutine(cameraControl.CoridorSwitch(room));
                        StopCoroutine(cameraControl.CoridorSwitch(room));
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine("Appear", 0);
            if (room.type == "Level")
            {
                cameraControl.levelHorBoundaries = Vector2.zero;
                cameraControl.levelVerBoundaries = Vector2.zero;
                cameraControl.levelDepthBoundaries = Vector2.zero;
            }
            if (room.isARoom)
                cameraControl.inRoom = false;
            if (room.type == "Stair")
                cameraControl.onStairs = false;
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
