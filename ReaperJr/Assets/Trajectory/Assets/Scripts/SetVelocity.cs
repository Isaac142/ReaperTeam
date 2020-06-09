using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]

public class SetVelocity : MonoBehaviour {
	
	//Velocity that you will be launching the object at in the x-axis direction
	public float velocity;
	
	// Use this for initialization
	void Start () {
		//Sets the rigidbody to isKinematic state so I doesn't fall out of place while aiming	
		GetComponent<Rigidbody>().isKinematic = true;

	}
	
	// Update is called once per frame
	void Update () {
			
		//Gives the object a velocity of the velocity variable and turns off
		//the trajectory predictor script so the line doesn't follow the object
		if(Input.GetKeyDown("f")){
		
			GetComponent<Rigidbody>().isKinematic = false;
			
			GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.right * velocity);
			
			this.transform.parent = null;
			
			this.enabled = false;
			
		}
		
	}
}
