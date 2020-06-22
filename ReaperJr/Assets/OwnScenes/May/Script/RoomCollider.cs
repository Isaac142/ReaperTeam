using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomCollider : ReaperJr
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

    public List<GameObject> soul = new List<GameObject>();
    [HideInInspector]
    public List<Sprite> souls = new List<Sprite>();
    [HideInInspector]
    public List<Sprite> soulMasks = new List<Sprite>();

    private enum RoomType { LEVEL, ROOM, STAIR, CORRIDOR }
    private RoomType roomType;

    private void Awake()
    {
        transform.GetComponent<Collider>().isTrigger = true;
        cameraControl = Camera.main.GetComponent<CameraControlScript>();
        if (transform.tag == "LevelCollider")
            roomType = RoomType.LEVEL;
        if (transform.tag == "RoomCollider")
            roomType = RoomType.ROOM;
        if (transform.tag == "StairCollider")
            roomType = RoomType.STAIR;
        if (transform.tag == "CorridorCollider")
            roomType = RoomType.CORRIDOR;
    }

    private void Start()
    {
        roomCollider = GetComponent<Collider>();
        roomPosition = roomCollider.transform.position;
        roomSides = new Vector2(roomCollider.transform.position.x - roomCollider.bounds.size.x / 2f, roomCollider.transform.position.x + roomCollider.bounds.size.x / 2f);
        roomHeight = new Vector2(roomCollider.transform.position.y - roomCollider.bounds.size.y / 2f, roomCollider.transform.position.y + roomCollider.bounds.size.y / 2f);
        roomDepth = new Vector2(roomCollider.transform.position.z - roomCollider.bounds.size.z / 2f, roomCollider.transform.position.z + roomCollider.bounds.size.z / 2f);
        for (int i = 0; i <soul.Count; i ++)
        {
            souls.Add(soul[i].GetComponent<SoulType>().soulIcon);
            soulMasks.Add(soul[i].GetComponent<SoulType>().soulMask);
        }
        _GAME.totalSoulNo += soul.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (roomType)
            {
                case RoomType.ROOM:
                    foreach (Image soul in _UI.souls)
                    {
                        soul.sprite = null;
                        soul.enabled = false;
                    }
                    foreach (Image mask in _UI.soulMasks)
                    {
                        mask.sprite = null;
                        mask.enabled = false;
                    }

                    for (int i = 0; i < souls.Count; i++)
                    {
                        _UI.souls[i].enabled = true;
                        _UI.souls[i].sprite = souls[i];
                        _UI.soulMasks[i].sprite = soulMasks[i];
                        _UI.soulMasks[i].enabled = false;
                    }
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (roomType)
            {
                case RoomType.LEVEL:
                    cameraControl.levelHorBoundaries = new Vector2(roomSides.x, roomSides.y);
                    cameraControl.levelVerBoundaries = new Vector2(roomHeight.x, roomHeight.y);
                    cameraControl.levelDepthBoundaries = new Vector2(roomDepth.x, +roomDepth.y);
                    if (!cameraControl.inRoom)
                        StartCoroutine("Disappear", 0f);
                    else
                        StartCoroutine("Appear", 0);
                    break;

                case RoomType.ROOM:
                    cameraControl.roomPosition = roomPosition;
                    StartCoroutine(cameraControl.RoomSwitch(roomSides, roomHeight, roomDepth));
                    StartCoroutine("Disappear", 0f);

                    for (int i = 0; i < souls.Count; i++)
                    {
                        if (soul[i] == null)
                            _UI.soulMasks[i].enabled = true;
                        if (!_UI.souls[i].IsActive())
                            _UI.soulMasks[i].enabled = false;
                    }
                    break;

                case RoomType.STAIR:
                    cameraControl.roomPosition = roomPosition;
                    StartCoroutine(cameraControl.StairSwitch(roomSides, roomHeight, roomDepth));
                    cameraControl.onStairs = true;
                    break;
                case RoomType.CORRIDOR:
                    cameraControl.roomPosition = roomPosition;
                    cameraControl.inCorridor = true;
                    if (!cameraControl.inRoom)
                        StartCoroutine(cameraControl.CorridorSwitch(roomDepth));
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine("Appear", 0);
            switch(roomType)
            {
                case RoomType.ROOM:
                    foreach (Image soul in _UI.souls)
                    {
                        soul.sprite = null;
                        soul.enabled = false;
                    }
                    foreach (Image mask in _UI.soulMasks)
                    {
                        mask.sprite = null;
                        mask.enabled = false;
                    }
                    cameraControl.inRoom = false;
                    break;
                case RoomType.STAIR:
                    cameraControl.onStairs = false;
                    break;
                case RoomType.CORRIDOR:
                    cameraControl.inCorridor = false;
                    break;
            }
        }
    }

    IEnumerator Disappear(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        foreach (GameObject barrier in inFront)
            barrier.SetActive(false);

        foreach (MeshRenderer wall in frontWall)
        {
            wall.enabled = false;
            wall.gameObject.layer = 2;
        }
    }

    IEnumerator Appear(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        foreach (GameObject barrier in inFront)
            barrier.SetActive(true);

        foreach (MeshRenderer wall in frontWall)
        {
            wall.enabled = true;
            wall.gameObject.layer = 0;
        }
    }
}
