/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Bird : MonoBehaviour {
    
    #region private

    [SerializeField] private Transform forceTransform;
    private SpriteMask forceSpriteMask;

    private void Awake() {
        forceSpriteMask = forceTransform.Find("mask").GetComponent<SpriteMask>();
        HideForce();
    }

    private void Update() {
        forceTransform.position = transform.position;
        Vector2 dir = (UtilsClass.GetMouseWorldPosition() - transform.position).normalized;
        forceTransform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
        
        /*
        if (Input.GetMouseButtonDown(0)) {
            testForce = 0;
        }

        if (Input.GetMouseButton(0)) {
            testForce += MAX_FORCE * Time.deltaTime;
            if (testForce > MAX_FORCE) {
                testForce = MAX_FORCE;
            }
            ShowForce(testForce);
        }
        if (Input.GetMouseButtonUp(0)) {
            Launch(testForce);
        }
        */
    }

    private void HideForce() {
        forceSpriteMask.alphaCutoff = 1;
    }

    #endregion

    public const float MAX_FORCE = 500f;
    
    public void Launch(float force) {
        Vector2 dir = (UtilsClass.GetMouseWorldPosition() - transform.position).normalized * -1f;
        transform.GetComponent<Rigidbody2D>().velocity = dir * force;
        HideForce();
    }

    public void ShowForce(float force) {
        forceSpriteMask.alphaCutoff = 1 - force / MAX_FORCE;
    }

}
