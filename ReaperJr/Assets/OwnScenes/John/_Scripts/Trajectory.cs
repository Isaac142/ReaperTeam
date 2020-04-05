using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    public Vector3 initPos;
    public Rigidbody scythe;
    private Vector3 forceAtPlayer;
    public float forceFactor;

    public GameObject trajectoryDot;
    private GameObject[] trajectoryDots;
    public int number;

    // Start is called before the first frame update
    void Start()
    {
        scythe = GetComponent<Rigidbody>();
        trajectoryDots = new GameObject[number];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { //click
            startPos = gameObject.transform.position;
            for (int i = 0; i < number; i++)
            {
                trajectoryDots[i] = Instantiate(trajectoryDot, gameObject.transform);
            }

        }
        if (Input.GetMouseButton(0))
        { //drag
            endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
            gameObject.transform.position = endPos;
            forceAtPlayer = endPos - startPos;
            for (int i = 0; i < number; i++)
            {
                trajectoryDots[i].transform.position = calculatePosition(i * 0.1f);
            }
        }
        if (Input.GetMouseButtonUp(0))
        { //leave
            scythe.velocity = new Vector2(-forceAtPlayer.x * forceFactor, -forceAtPlayer.y * forceFactor);
            for (int i = 0; i < number; i++)
            {
                Destroy(trajectoryDots[i]);
            }
        }
    }

    private Vector2 calculatePosition(float elapsedTime)
    {
        return new Vector2(endPos.x, endPos.y) + //X0
                new Vector2(-forceAtPlayer.x * forceFactor, -forceAtPlayer.y * forceFactor) * elapsedTime + //ut
                0.5f * Physics2D.gravity * elapsedTime * elapsedTime;
    }
}
