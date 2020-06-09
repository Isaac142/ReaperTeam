using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]


public class Launch : MonoBehaviour {
	
	//Velocity that you will be launching the object at in the x-axis direction
	public float velocity;
	//The amount rotation the object rotates per frame
	public int rateOfRotation;
	
	// Use this for initialization
	void Start () {
		
		//Sets the rigidbody to isKinematic state so I doesn't fall out of place while aiming
		GetComponent<Rigidbody>().isKinematic = true;
				
	}
	
	// Update is called once per frame
	void Update () {
		
		//Increases the rotation when the left arrow key is pressed
		if(Input.GetKey("left")){
			
			this.transform.rotation = Quaternion.Euler(0,0,this.transform.rotation.eulerAngles.z + rateOfRotation);
		
		//Decreases rotation when the right arrow key is pressed
		}else if(Input.GetKey("right")){
			
			this.transform.rotation = Quaternion.Euler(0,0,this.transform.rotation.eulerAngles.z - rateOfRotation);
					
		}
		
		//Gives the object a velocity of the velocity variable and turns off
		//the trajectory predictor script so the line doesn't follow the object
		if(Input.GetKeyDown("space")){
			
			GetComponent<Rigidbody>().isKinematic = false;
			
			GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.right * velocity);
			
			this.GetComponent<TrajectoryPredictor>().enabled = false;
			
			this.enabled = false;

			
		}
		
	}

}

