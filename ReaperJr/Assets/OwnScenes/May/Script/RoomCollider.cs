using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomCollider : ReaperJr
{
    Collider roomCollider;
    [HideInInspector]
    public Vector3 roomPosition = Vector3.zero;
    [HideInInspector]
    public Vector2 roomSides = Vector2.zero, roomHeight = Vector2.zero, roomDepth = Vector2.zero;
    
    [HideInInspector]
    public List<Sprite> soulSprite = new List<Sprite>();

    public List<SoulType> souls = new List<SoulType>();
    public List<GameObject> frontWalls;

    private enum RoomType { LEVEL, ROOM }
    private RoomType roomType;

    private void Awake()
    {
        transform.GetComponent<Collider>().isTrigger = true;
        this.gameObject.layer = 2; // set room collider to ignore raycast layer.
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
        for (int i = 0; i <souls.Count; i ++)
        {
            soulSprite.Add(souls[i].soulIcon);
        }
        _GAME.totalSoulNo += souls.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            WallDisappear();
            switch (roomType)
            {
                case RoomType.ROOM:
                    _CAMERA.SetCameraState(CameraControlScript.CameraState.INROOM);
                    break;
            }
            _UI.SetSouls(souls);
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
                    break;

                case RoomType.ROOM:
                    _CAMERA.roomPosition = roomPosition;
                    StartCoroutine(_CAMERA.RoomSwitch(roomSides, roomHeight, roomDepth));
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
           WallAppear();
            switch(roomType)
            {
                case RoomType.ROOM:
                    _CAMERA.SetCameraState(CameraControlScript.CameraState.OUTROOM);
                    break;

                case RoomType.LEVEL:
                    break;
            }
            _UI.FadeOutPanel(_UI.soulPanel);
        }
    }

    void WallDisappear()
    {
        foreach (GameObject go in frontWalls)
        {
            go.gameObject.layer = 2;
            Renderer rend = go.GetComponent<Renderer>();
            rend.material.SetFloat("_Mode", 2);
            rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            rend.material.SetInt("_ZWrite", 0);
            rend.material.DisableKeyword("_ALPHATEST_ON");
            rend.material.EnableKeyword("_ALPHABLEND_ON");
            rend.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            rend.material.renderQueue = 3000;
            go.GetComponent<Renderer>().material.DOFade(0, "_BaseColor", 1).SetEase(Ease.OutQuart);
        }
    }
    void WallAppear()
    {
        foreach (GameObject go in frontWalls)
        {
            go.gameObject.layer = 0;
            go.GetComponent<Renderer>().material.DOFade(1, "_BaseColor", 1).SetEase(Ease.OutQuart);
            Renderer rend = go.GetComponent<Renderer>();
            rend.material.SetFloat("_Mode", 0);
            rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            rend.material.SetInt("_ZWrite", 1);
            rend.material.DisableKeyword("_ALPHATEST_ON");
            rend.material.DisableKeyword("_ALPHABLEND_ON");
            rend.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            rend.material.renderQueue = -1;
        }
    }
}
