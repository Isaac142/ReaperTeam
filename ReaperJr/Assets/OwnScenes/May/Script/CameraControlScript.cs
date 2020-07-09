using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraControlScript : Singleton<CameraControlScript>
{
    [Header("Zooming")]
    public bool isViewingAll;

    public Vector3 zoomOutPos;
    public float zoomSpeed = 0.5f;
    public Ease zoomEase;

    [Header("Player")]
    public Transform player;
    public Transform scythe;
    public float chaseThreshold = 0.5f;
    private Vector3 playerPos = Vector3.zero;
    private Vector3 scythePos = Vector3.zero;
    private Vector3 offset = Vector3.zero; //offset between player and scythe
    private float betweenDist = 0f; //distance between player and scythe

    private Vector3 toPlayerDist = Vector3.zero; //intended distance between player can camera.
    private Vector3[] screenConcersInWorld = new Vector3[4]; // 4 screen corners in order: topL, BottomL, TopR, BottomR
    private Vector2 worldSpaceScreenSize = Vector2.zero; //screen boarders.
    private Vector3[] screenConstrains = new Vector3[4]; //boundaries: top, bottom, left, right

    [VectorLabels ("min" , "max")]
    public Vector2 camToPlayerDist = Vector2.zero; // x = minimun distanc between player and camera, less than this, camera will tile down, y = maximun distance between player and camera, camera will follow on z-axis if it's to far to camera
    public float miniCamToEdgeDist = 0f; //minimun distance to room's wall infront of camera

    [VectorLabels("Left", "Right")]
    public Vector2 camHorClampFactor = Vector2.zero; //side clamps factor: how much on the side can be shown.
    [VectorLabels("min", "max")]
    public Vector2 camHeight = Vector2.zero;
    private Vector2 camHorBoundaries = Vector2.zero; //min, max
    private Vector2 camVerBoundaries = Vector2.zero;
    public Vector2 camDepthBoundaries = Vector2.zero;
    [HideInInspector]
    public Vector2 levelHorBoundaries = Vector2.zero, levelVerBoundaries = Vector2.zero, levelDepthBoundaries = Vector2.zero;
    [HideInInspector]
    public Vector3 roomPosition = Vector3.zero;

    public Vector3 camRotation = Vector3.zero; //default rotation
    public Vector3 topRot = Vector3.zero;  //rotation when camera reach the y max.
    public Vector3 bottomRot = Vector3.zero; //rotation when camera reach the y min.
    public float stairRotFactor = 0f; //adjust rotation on stairs
    
    [VectorLabels("Move" , "Tilt")]
    public Vector2 followSpeed = new Vector2(2.5f, 2f); // how fast camera moves to follow player. x = movement speed, y = tilting speed.
       

    private List<GameObject> objInFront = new List<GameObject>(), objInFrontLast = new List<GameObject>();
    private RaycastHit[] hits;
    public LayerMask layerMask = 0;
    private float rendererMode = 2f;
    public float transparentFactor = 0.5f;
    private Color color;

    public enum CameraState { INROOM, OUTROOM}
    public CameraState camState;

    private void Awake()
    {
        screenConcersInWorld[0] = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, miniCamToEdgeDist));
        screenConcersInWorld[1] = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, miniCamToEdgeDist));
        screenConcersInWorld[2] = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, miniCamToEdgeDist));
        screenConcersInWorld[3] = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0f, miniCamToEdgeDist));

        worldSpaceScreenSize = new Vector2(Mathf.Abs(screenConcersInWorld[0].x - screenConcersInWorld[2].x), Mathf.Abs(screenConcersInWorld[0].y - screenConcersInWorld[1].y));
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = camRotation;
        levelHorBoundaries = Vector2.zero;
        levelVerBoundaries = Vector2.zero;
        levelDepthBoundaries = Vector2.zero;

        player = _PLAYER.gameObject.transform;
        scythe = _PLAYER.scythe.transform;
    }
    private void Update()
    {
        #region TurnToTransparent
        objInFront = new List<GameObject>();
        ObjectInFront(objInFront);
        foreach (GameObject obj in objInFront)
        {
            Fade(obj);
        }

        foreach (GameObject obj in objInFrontLast)
        {
            if (!objInFront.Contains(obj))
                ReturnColor(obj);
        }
        objInFrontLast = objInFront;
        #endregion

        if (Input.GetKeyDown(KeyCode.V))
        {
            isViewingAll = !isViewingAll;

            if (isViewingAll)
            {
                transform.DOMove(zoomOutPos, zoomSpeed).SetEase(zoomEase);
            }
            else
            {
                transform.DOMove(playerPos + toPlayerDist + offset, zoomSpeed).SetEase(zoomEase);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isViewingAll)
        {
            playerPos = _PLAYER.gameObject.transform.position;
            scythePos = scythe.position;
            betweenDist = Vector3.Distance(playerPos, scythePos); //calculate difference between player and scythe

            offset = (betweenDist > chaseThreshold) ? //camera chasing scythe if distance over threshold
                 new Vector3((scythePos.x - playerPos.x) / 2f, (scythePos.y - playerPos.y) / 2f, 0f) : Vector3.zero;

            

            switch(camState)
            {
                case CameraState.INROOM:
                    //if the distance between camera and player over max value, camera chasing player at the max distance
                    toPlayerDist = (playerPos.z - transform.position.z > camToPlayerDist.y) ?
                        new Vector3(0f, camHeight.x, -camToPlayerDist.y) : new Vector3(0f, camHeight.x, camDepthBoundaries.x - playerPos.z); //camera stays at the z- minimun clamp vale.
                    break;

                case CameraState.OUTROOM:
                    // if player is out room, camera chasing player at minimun camera to player distance
                    toPlayerDist = new Vector3(0f, camHeight.x, camDepthBoundaries.x);

                    //set up camera boundaries based on the level boundaries. 
                    camDepthBoundaries.x = levelDepthBoundaries.x - miniCamToEdgeDist;
                    camHorBoundaries = new Vector2(levelHorBoundaries.x + worldSpaceScreenSize.x / 2f + camHorClampFactor.x, levelHorBoundaries.y - worldSpaceScreenSize.x / 2f - camHorClampFactor.y);
                    camVerBoundaries = new Vector2(levelVerBoundaries.x + camHeight.x, levelVerBoundaries.x + camHeight.y);
                    camDepthBoundaries = new Vector2(camDepthBoundaries.x, levelDepthBoundaries.y - miniCamToEdgeDist);
                    break;
            }
            
            //camera movement
            transform.position = Vector3.Lerp(transform.position, playerPos + toPlayerDist + offset, followSpeed.x * Time.deltaTime);
            //set up camera boundary clamps
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, camHorBoundaries.x, camHorBoundaries.y), Mathf.Clamp(transform.position.y, camVerBoundaries.x, camVerBoundaries.y), Mathf.Clamp(transform.position.z, camDepthBoundaries.x, camDepthBoundaries.y));
            //camera rotation
            transform.rotation = (playerPos.z - transform.position.z <= camToPlayerDist.x) ? //when player closer to camera than minimum value, tilt down camera for better view.
                    Quaternion.Lerp(transform.rotation, Quaternion.Euler(bottomRot), followSpeed.y * Time.deltaTime) :
                    (playerPos.y >= camHeight.x) ? //if player higher than minimum camera height, camera tilt down to view ground.
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(topRot), followSpeed.y * Time.deltaTime) :
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(camRotation), followSpeed.y * Time.deltaTime); //normal
        }
    }

    public IEnumerator RoomSwitch(Vector2 roomSides, Vector2 roomHeight, Vector2 roomDepth) //set up new boundary clamps when switch rooms.
    {
        camHorBoundaries = new Vector2(roomSides.x +  worldSpaceScreenSize.x/2f + camHorClampFactor.x, roomSides.y - worldSpaceScreenSize.x / 2f - camHorClampFactor.y);
        if (camHorBoundaries.x > camHorBoundaries.y) //if the room is too narrow, camera stays at the middle of the room
        {
            camHorBoundaries.x = roomSides.x;
            camHorBoundaries.y = roomSides.y;
            toPlayerDist = new Vector3(roomPosition.x - _PLAYER.gameObject.transform.position.x, transform.position.y, -camToPlayerDist.y);
        }
        camVerBoundaries = new Vector2(roomHeight.x + camHeight.x, roomHeight.x + camHeight.y);

        camDepthBoundaries = new Vector2(roomDepth.x - miniCamToEdgeDist, roomDepth.y - camToPlayerDist.y);
        if (camDepthBoundaries.x > camDepthBoundaries.y) //if room is not deep enough, camera stays at the edge of wall.
            camDepthBoundaries.y = camDepthBoundaries.x;
        yield return null;
    }

    void ObjectInFront(List<GameObject> obj)
    {
        Vector3 direction = new Vector3(_PLAYER.gameObject.transform.position.x, _PLAYER.gameObject.transform.position.y + player.GetComponent<CapsuleCollider>().center.y, _PLAYER.gameObject.transform.position.z) - transform.position;
        float distance = Vector3.Distance(transform.position, _PLAYER.gameObject.transform.position);

        hits = Physics.RaycastAll(transform.position, direction, distance, layerMask);
        Debug.DrawRay(transform.position, direction);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.GetComponent<Renderer>() != null)
            {
                if (!obj.Contains(hit.transform.gameObject))
                    obj.Add(hit.transform.gameObject);
            }
        }
    }

    void Fade(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        color = rend.material.color;
        rend.material.SetFloat("_Mode", rendererMode);
        rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        rend.material.SetInt("_ZWrite", 0);
        rend.material.DisableKeyword("_ALPHATEST_ON");
        rend.material.EnableKeyword("_ALPHABLEND_ON");
        rend.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        rend.material.renderQueue = 3000;
        rend.material.SetColor("_BaseColor", new Color(color.r, color.g, color.b, transparentFactor));
    }

    void ReturnColor(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        color = rend.material.color;
        rend.material.SetFloat("_Mode", 0);
        rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        rend.material.SetInt("_ZWrite", 1);
        rend.material.DisableKeyword("_ALPHATEST_ON");
        rend.material.DisableKeyword("_ALPHABLEND_ON");
        rend.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        rend.material.renderQueue = -1;
        rend.material.SetColor("_BaseColor", new Color(color.r, color.g, color.b, 1f));
    }

    public void SetCameraState(CameraState state)
    {
        camState = state;
    }
}
