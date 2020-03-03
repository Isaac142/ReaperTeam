using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ignor this script

public class ScytheControl : MonoBehaviour
{
    public float timeToMove = 0.2f;

    public GameObject character;
    public GameObject scythe;

    private Vector3 mousePos;
    private Rigidbody rb;

    private bool firstClick = false, secondClick = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = character.transform.GetChild(2).transform.position; //make the scythe follow character's hand
        if(Input.GetMouseButton(0))
        {
            if(firstClick == false && secondClick == false)
            {
                firstClick = true;
            }
        }
        
    }

    IEnumerator Teleport()
    {
        Vector3 playerPos = character.transform.position;
        Vector3 scythePos = scythe.transform.position;
        float timer = 0;
        while(timer < timeToMove)
        {
            character.transform.position = Vector3.Lerp(playerPos, scythePos, timer / timeToMove);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
