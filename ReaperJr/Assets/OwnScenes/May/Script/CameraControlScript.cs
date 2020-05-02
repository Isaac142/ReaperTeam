using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlScript : MonoBehaviour
{
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
    private Vector2 camDepthBoundaries = Vector2.zero;
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
       
    //[HideInInspector]
    public bool inRoom = false, onStairs = false, inCorridor = false;
    private bool checkEntre;

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
        inRoom = false;
        onStairs = false;
    }

    private void FixedUpdate()
    {
        playerPos = player.position;
        scythePos = scythe.position;
        betweenDist = Vector3.Distance(playerPos, scythePos); //calculate difference between player and scythe

       offset = (betweenDist > chaseThreshold)? //camera chasing scythe if distance over threshold
            new Vector3((scythePos.x - playerPos.x) / 2f, (scythePos.y - playerPos.y) / 2f, 0f) : Vector3.zero;

        if (inRoom) //if player is in room, camera follow rules
        {
            //if the distance between camera and player over max value, camera chasing player at the max distance
            toPlayerDist = (playerPos.z - transform.position.z > camToPlayerDist.y)? 
                new Vector3(0f, transform.position.y, -camToPlayerDist.y) : new Vector3(0f, camHeight.x, camDepthBoundaries.x - playerPos.z); //camera stays at the z- minimun clamp vale.
        }
        if (onStairs)
        {
            toPlayerDist = (playerPos.y >= roomPosition.y) ?
               new Vector3(0f, camHeight.y, -toPlayerDist.x) 
             : Vector3.Lerp(new Vector3(0f, camHeight.y, -camToPlayerDist.x), new Vector3(0, camHeight.x, -camToPlayerDist.x), followSpeed.x * Time.deltaTime);
        }
        else
            toPlayerDist = new Vector3(0f, camHeight.x, -camToPlayerDist.x); // if player is out room, camera chasing player at minimun camera to player distance

        if (!inRoom && !onStairs)
        {
            //set up camera boundaries based on the level boundaries. 
            if (!inCorridor)
                camDepthBoundaries.x = levelDepthBoundaries.x - miniCamToEdgeDist;
            camHorBoundaries = new Vector2(levelHorBoundaries.x + worldSpaceScreenSize.x / 2f + camHorClampFactor.x, levelHorBoundaries.y - worldSpaceScreenSize.x / 2f - camHorClampFactor.y);
            camVerBoundaries = new Vector2(levelVerBoundaries.x + camHeight.x, levelVerBoundaries.x + camHeight.y);
            camDepthBoundaries = new Vector2(camDepthBoundaries.x, levelDepthBoundaries.y - miniCamToEdgeDist);           
        }

        transform.position = Vector3.Lerp(transform.position, playerPos + toPlayerDist + offset, followSpeed.x * Time.deltaTime); //camera movement
        //set up camera boundary clamps
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, camHorBoundaries.x, camHorBoundaries.y), Mathf.Clamp(transform.position.y, camVerBoundaries.x, camVerBoundaries.y), Mathf.Clamp(transform.position.z, camDepthBoundaries.x, camDepthBoundaries.y));

        if (onStairs)
        {
            transform.rotation = (playerPos.y <= roomPosition.y) ?
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(bottomRot), followSpeed.y * Time.deltaTime) :
                Quaternion.Lerp(transform.rotation, Quaternion.Euler(camRotation), followSpeed.y * Time.deltaTime);
        }
        else
        {
            transform.rotation = (playerPos.z - transform.position.z <= camToPlayerDist.x) ? //when player closer to camera than minimum value, tilt down camera for better view.
                Quaternion.Lerp(transform.rotation, Quaternion.Euler(bottomRot), followSpeed.y * Time.deltaTime) :
                (playerPos.y >= camHeight.x) ? //if player higher than minimum camera height, camera tilt down to view ground.
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(topRot), followSpeed.y * Time.deltaTime) :
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(camRotation), followSpeed.y * Time.deltaTime);
        }
    }

    public IEnumerator RoomSwitch(Vector2 roomSides, Vector2 roomHeight, Vector2 roomDepth) //set up new boundary clamps when switch rooms.
    {
        inRoom = true;

        camHorBoundaries = new Vector2(roomSides.x +  worldSpaceScreenSize.x/2f + camHorClampFactor.x, roomSides.y - worldSpaceScreenSize.x / 2f - camHorClampFactor.y);
        if (camHorBoundaries.x > camHorBoundaries.y) //if the room is too narrow, camera stays at the middle of the room
        {
            camHorBoundaries.x = camHorBoundaries.y;
            camHorBoundaries.y = roomPosition.x;
        }
        camVerBoundaries = new Vector2(roomHeight.x + camHeight.x, roomHeight.x + camHeight.y);

        camDepthBoundaries = new Vector2(roomDepth.x - miniCamToEdgeDist, roomDepth.y - camToPlayerDist.y);
        if (camDepthBoundaries.x > camDepthBoundaries.y) //if room is not deep enough, camera stays at the edge of wall.
            camDepthBoundaries.y = camDepthBoundaries.x;
        yield return null;
    }

    public IEnumerator CorridorSwitch(Vector2 roomDepth)
    {
        //set up minimun camera z value, others will be follow the level boundary rules.
        //this set up only require room colliders when z-clamp changes.
        camDepthBoundaries.x = roomDepth.x - miniCamToEdgeDist;
        yield return null;
    }

    public IEnumerator StairSwitch(Vector2 roomSides, Vector2 roomHeight, Vector2 roomDepth)
    {
        camHorBoundaries = new Vector2(roomSides.x + worldSpaceScreenSize.x / 2f + camHorClampFactor.x, roomSides.y - worldSpaceScreenSize.x / 2f - camHorClampFactor.y);
        if (camHorBoundaries.x > camHorBoundaries.y) //if the room is too narrow, camera stays at the middle of the room
        {
            camHorBoundaries.x = roomSides.x;
            camHorBoundaries.y = roomSides.y;
        }

        camVerBoundaries = new Vector2(roomHeight.x + camHeight.x, roomHeight.y - camHeight.x);
        camDepthBoundaries = new Vector2(roomDepth.x - miniCamToEdgeDist, roomDepth.y - camToPlayerDist.y);
        yield return null;
    }
}
