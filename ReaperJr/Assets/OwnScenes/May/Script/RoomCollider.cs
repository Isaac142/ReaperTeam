using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomCollider : ReaperJr
{
    Collider roomCollider;
    [HideInInspector]
    public List<GameObject> inFront = new List<GameObject>(); //all game objects will not be shown when player is in this collider
    public List<MeshRenderer> frontWall = new List<MeshRenderer>(); //front barriers that will not show in the screen. --> not show their mesh, but still using their collider.
    [HideInInspector]
    public Vector3 roomPosition = Vector3.zero;
    [HideInInspector]
    public Vector2 roomSides = Vector2.zero, roomHeight = Vector2.zero, roomDepth = Vector2.zero;

    public List<GameObject> soul = new List<GameObject>();
    [HideInInspector]
    public List<Sprite> souls = new List<Sprite>();
    [HideInInspector]
    public List<Sprite> soulMasks = new List<Sprite>();

    public List<GameObject> frontWalls;

    private enum RoomType { LEVEL, ROOM }
    private RoomType roomType;

    private void Awake()
    {
        transform.GetComponent<Collider>().isTrigger = true;
        if (transform.tag == "LevelCollider")
            roomType = RoomType.LEVEL;
        if (transform.tag == "RoomCollider")
            roomType = RoomType.ROOM;
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
            WallDisappear();
            switch (roomType)
            {
                case RoomType.ROOM:
                    _UI.DisableSoulIcons();
                    _CAMERA.SetCameraState(CameraControlScript.CameraState.INROOM);
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
                    //set up level boundaries
                    _CAMERA.levelHorBoundaries = new Vector2(roomSides.x, roomSides.y);
                    _CAMERA.levelVerBoundaries = new Vector2(roomHeight.x, roomHeight.y);
                    _CAMERA.levelDepthBoundaries = new Vector2(roomDepth.x, +roomDepth.y);

                    //making all object in between camera and player invisible
                    //StartCoroutine("Disappear", 0f);
                    //if (_GAME.gameState == GameManager.GameState.WON)
                    //    StartCoroutine("Appear", 0);
                    break;

                case RoomType.ROOM:
                    _CAMERA.roomPosition = roomPosition;
                    StartCoroutine(_CAMERA.RoomSwitch(roomSides, roomHeight, roomDepth));
                    //StartCoroutine("Disappear", 0f);

                    if (souls.Count > 0)
                    {
                        for (int i = 0; i < souls.Count; i++)
                        {
                            if (soul[i] == null)
                                _UI.soulMasks[i].enabled = true;
                            if (!_UI.souls[i].IsActive())
                                _UI.soulMasks[i].enabled = false;
                        }
                    }
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //StartCoroutine("Appear", 0);
            WallAppear();
            switch(roomType)
            {
                case RoomType.ROOM:
                    _UI.DisableSoulIcons();
                    _CAMERA.SetCameraState(CameraControlScript.CameraState.OUTROOM);
                    break;

                case RoomType.LEVEL:
                    //StartCoroutine("Appear", 0);
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

    void WallDisappear()
    {
        foreach (GameObject go in frontWalls)
        {
            go.gameObject.layer = 2;
            go.GetComponent<Renderer>().material.DOFade(0, "_BaseColor", 1).SetEase(Ease.OutQuart);
        }
    }
    void WallAppear()
    {
        foreach (GameObject go in frontWalls)
        {
            go.gameObject.layer = 0;
            go.GetComponent<Renderer>().material.DOFade(1, "_BaseColor", 1).SetEase(Ease.OutQuart);
        }
    }
}
