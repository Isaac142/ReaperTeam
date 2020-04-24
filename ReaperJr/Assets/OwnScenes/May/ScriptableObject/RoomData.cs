using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RoomDataAsset", menuName = "RoomData")]
public class RoomData : ScriptableObject
{
    public string roomName;
    public string type;
    public bool isARoom;
    public Vector3 roomPosition;
    public Vector3 roomSize;
    public Vector3 doorDirection;
}
