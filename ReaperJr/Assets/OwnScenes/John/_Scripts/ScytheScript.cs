using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class ScytheScript : MonoBehaviour
{
    public bool activated;

    [SerializeField] private Transform forceTransform;
    private SpriteMask forceSpriteMask;

    private void Awake()
    {
        forceSpriteMask = forceTransform.Find("mask").GetComponent<SpriteMask>();
        HideForce();
    }

    private void Update()
    {
        forceTransform.position = transform.position;
        Vector2 dir = (UtilsClass.GetMouseWorldPosition() - transform.position).normalized;
        forceTransform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
    }

    private void HideForce()
    {
        forceSpriteMask.alphaCutoff = 1;
    }

    #region Heavy Throw

    public const float MAX_FORCE = 500f;

    public void Launch(float force)
    {
        Vector2 dir = (UtilsClass.GetMouseWorldPosition() - transform.position).normalized * -1f;
        transform.GetComponent<Rigidbody>().velocity = dir * force;
        HideForce();
    }

    public void ShowForce(float force)
    {
        forceSpriteMask.alphaCutoff = 1 - force / MAX_FORCE;
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        activated = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
