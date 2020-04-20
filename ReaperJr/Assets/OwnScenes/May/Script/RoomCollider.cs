using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    public CameraControl cameraControl;
    public List<GameObject> inFront = new List<GameObject>();
    public List<MeshRenderer> frontWall = new List<MeshRenderer>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            GameManager.Instance.changedRoom = false;
        cameraControl.roomCollider = transform.GetComponent<Collider>();
        foreach (GameObject barrier in inFront)
        {
            barrier.SetActive(false);
        }

        foreach (MeshRenderer wall in frontWall)
        {
            wall.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            GameManager.Instance.changedRoom = true;

        foreach (GameObject barrier in inFront)
        {
            barrier.SetActive(true);
        }

        foreach (MeshRenderer wall in frontWall)
        {
            wall.enabled = true;
        }
    }
}
