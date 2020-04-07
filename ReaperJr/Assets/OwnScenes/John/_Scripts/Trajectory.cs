using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    private bool touchStart = false;
    //public Transform scythe;

    private Vector3 startPos;
    private Vector3 endPos;
    public Vector3 initPos;
    public Rigidbody scytheRigidbody;
    private Vector3 forceAtPlayer;
    public float forceFactor;

    public GameObject trajectoryDot;
    private GameObject[] trajectoryDots;
    public int number;

    // Start is called before the first frame update
    void Start()
    {
        scytheRigidbody = GetComponent<Rigidbody>();
        trajectoryDots = new GameObject[number];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { //click
            startPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
            for (int i = 0; i < number; i++)
            {
                trajectoryDots[i] = Instantiate(trajectoryDot, gameObject.transform);
            }

        }
        if (Input.GetMouseButton(0))
        { //drag
            touchStart = true;
            endPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
            forceAtPlayer = startPos - endPos;
            for (int i = 0; i < number; i++)
            {
                trajectoryDots[i].transform.position = calculatePosition(i * 0.1f);
            }
        }
        else
        {
            touchStart = false;
        }
        if (Input.GetMouseButtonUp(0))
        { //leave
            scytheRigidbody.velocity = new Vector2(-forceAtPlayer.x * forceFactor, -forceAtPlayer.y * forceFactor);
            for (int i = 0; i < number; i++)
            {
                Destroy(trajectoryDots[i]);
            }
        }
    }

    private void FixedUpdate()
    {
        if (touchStart)
        {
            Vector2 offset = endPos = startPos;
            Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);
            //moveScythe(direction * -1);
        }
    }

    /*
    void moveScythe(Vector2 direction)
    {
        scythe.Translate(direction * Time.deltaTime);
    }
    */

    private Vector2 calculatePosition(float elapsedTime)
    {
        return new Vector2(endPos.x, endPos.y) + //X0
                new Vector2(-forceAtPlayer.x * forceFactor, -forceAtPlayer.y * forceFactor) * elapsedTime + //ut
                0.5f * Physics2D.gravity * elapsedTime * elapsedTime;
    }
}
