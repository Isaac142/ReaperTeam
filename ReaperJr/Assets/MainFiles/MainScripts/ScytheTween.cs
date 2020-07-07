using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScytheTween : MonoBehaviour
{
    public float scythePower;
    public GameObject projectile;
    public float throwDistance;
    public Ease ease;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Throw();
        }
    }

    void Throw()
    {
        projectile.transform.position = this.transform.position;
        projectile.transform.DOJump(transform.position + Vector3.right * throwDistance, scythePower, 1, 1).SetEase(ease);
    }
}
