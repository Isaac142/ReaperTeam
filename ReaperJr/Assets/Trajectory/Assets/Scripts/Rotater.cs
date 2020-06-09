using UnityEngine;
using System.Collections;

public class Rotater : MonoBehaviour {
	
	//The amount rotation the object rotates per frame
	public int rateOfRotation;
	public bool _3D = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(_3D){
			
			if(Input.GetKey("left")){
			
				transform.Rotate(Vector3.up, -rateOfRotation, Space.World);
			
			}else if(Input.GetKey("right")){
				
				transform.Rotate(Vector3.up, rateOfRotation, Space.World);
			
			}else if(Input.GetKey("up") && (transform.rotation.eulerAngles.z > 270 || transform.rotation.eulerAngles.z < 95)){
				
				transform.Rotate(Vector3.forward, -rateOfRotation);
				
			}else if(Input.GetKey("down") && (transform.rotation.eulerAngles.z < 90 || transform.rotation.eulerAngles.z > 265)){
				
				transform.Rotate(Vector3.forward, rateOfRotation);	
				
			}
				
		}else{
		
			//Increases the rotation when the left arrow key is pressed
			if(Input.GetKey("left")){
				
				this.transform.localRotation = Quaternion.Euler(0,0,this.transform.rotation.eulerAngles.z + rateOfRotation);
			
			//Decreases rotation when the right arrow key is pressed
			}else if(Input.GetKey("right")){
				
				this.transform.localRotation = Quaternion.Euler(0,0,this.transform.rotation.eulerAngles.z - rateOfRotation);
						
			}
			
		}
		
	}
}
