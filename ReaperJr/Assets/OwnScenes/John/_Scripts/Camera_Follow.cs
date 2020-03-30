using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    public Transform player;
    public float cameraDistance;
    public float yModifier;

    private void FixedUpdate()
    {
        transform.position = new Vector3(player.position.x, player.position.y + yModifier, cameraDistance);
    }
}
