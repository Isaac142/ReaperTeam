using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Public variables
    public float MaxSpeed = 20;
    public float Acceleration = 64;
    public float JumpSpeed = 8;
    public float JumpDuration = 150;

    //Input variables
    private float horizontal;
    private float vertical;
    private float jumpInput;

    //Internal variables
    private bool onTheGround;
    private float jmpDuration;
    private bool jumpKeyDown = false;
    private bool canVariableJump = false;

    Rigidbody rigid;
    LayerMask layerMask;
    Transform modelTrans;

    public Transform shoulderTrans;
    public Transform rightShoulder;
    public Vector3 lookPos;
    GameObject rsp;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        layerMask = ~(1 << 9);

        rsp = new GameObject();
        rsp.name = transform.root.name + "Right Shoulder IK Helper";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputHandler();
        UpdateRigidBodyValues();
        MovementHandler();
        HandleRotation();
        HandleAimingPos();
        HandleShoulder();
    }

    void InputHandler()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        jumpInput = Input.GetAxis("Fire2");
    }

    void MovementHandler()
    {
        onTheGround = isOnGround(); 

        if(horizontal < -0.1f)
        {
            if(rigid.velocity.x > -this.MaxSpeed)
            {
                rigid.AddForce(new Vector3(-this.Acceleration, 0, 0));
            }
            else
            {
                rigid.velocity = new Vector3(-this.MaxSpeed, rigid.velocity.y, 0);
            }
        }
        else if (horizontal > 0.1f)
        {
            if(rigid.velocity.x < this.MaxSpeed)
            {
                rigid.AddForce(new Vector3(this.Acceleration, 0, 0));
            }
            else
            {
                rigid.velocity = new Vector3(this.MaxSpeed, rigid.velocity.y, 0);
            }
        }

        if(jumpInput > 0.1f)
        {
            if (!jumpKeyDown)
            {
                jumpKeyDown = true;

                if (onTheGround)
                {
                    rigid.velocity = new Vector3(rigid.velocity.y, this.JumpSpeed, 0);
                    jmpDuration = 0.0f;
                }
            }
            else if (canVariableJump)
            {
                jmpDuration += Time.deltaTime;

                if(jmpDuration < this.JumpDuration / 1000)
                {
                    rigid.velocity = new Vector3(rigid.velocity.x, this.JumpSpeed, 0);
                }
            }
        }
        else
        {
            jumpKeyDown = false;
        }
    }

    void HandleRotation()
    {
        Vector3 directionToLook = lookPos - transform.position;
        directionToLook.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToLook);

        Debug.Log(lookPos.x + " " + transform.position.x);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15);
    }

    void HandleAimingPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 lookP = hit.point;
            lookP.z = transform.position.z;
            lookPos = lookP;
        }
    }

    void HandleShoulder()
    {
        shoulderTrans.LookAt(lookPos);

        Vector3 rightShoulderPos = rightShoulder.TransformPoint(Vector3.zero);
        rsp.transform.position = rightShoulderPos;
        rsp.transform.parent = transform;

        shoulderTrans.position = rsp.transform.position;
    }

    void UpdateRigidBodyValues()
    {
        if (onTheGround)
        {
            rigid.drag = 4;
        }
        else
        {
            rigid.drag = 0;
        }
    }

    private bool isOnGround()
    {
        bool retVal = false;
        float lengthToSearch = 1.5f;

        Vector3 lineStart = transform.position + Vector3.up;
        Vector3 vectorToSeacrh = -Vector3.up;

        RaycastHit hit;
        
        if(Physics.Raycast(lineStart, vectorToSeacrh, out hit, lengthToSearch, layerMask))
        {
            retVal = true;
        }

        return retVal;
    }
}
