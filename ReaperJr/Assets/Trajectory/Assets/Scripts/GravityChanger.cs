using UnityEngine;
using System.Collections;

public class GravityChanger : MonoBehaviour {
	
	//Variable that represents the acceleration of gravity
	public Vector3 accelerationOfGravity;
	
	// Use this for initialization
	void Start () {
		
		//Disables the object's use of gravity
		this.GetComponent<Rigidbody>().useGravity = false;
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Adds the downward force of the accelerationOfGravity times the mass of the object in order to give the object the proper acceleration
		GetComponent<Rigidbody>().AddForce(accelerationOfGravity.x * GetComponent<Rigidbody>().mass, -accelerationOfGravity.y * GetComponent<Rigidbody>().mass, accelerationOfGravity.z * GetComponent<Rigidbody>().mass);
		
	}
}
