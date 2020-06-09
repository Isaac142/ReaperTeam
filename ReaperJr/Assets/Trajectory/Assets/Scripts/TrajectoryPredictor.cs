using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Requires the LineRenderer component
[RequireComponent (typeof (LineRenderer))]

public class TrajectoryPredictor : MonoBehaviour {
	
	//Velocity that the object will be shot at
	public float velocity;
	//Amount of time between each point
	public float timeBetweenPoints = 0.01f;
	//Max number of points allowed in the trajectory line/Max amount of texture objects allowed
	public int maxNumberOfPoints = 100;
	//Object that will be instantiated
	public GameObject textureObject;
    public GameObject crosshair;
	//If the the point of arc number modulo of this variable equals 0 a texture will be placed on that point
	public int textureObjectDivisor = 1;
	//The z position that the line/textures will reside on
	public float zDepth;
	//The acceleration of gravity. It is automatically set to the acceleration of the earth's gravity.
	public Vector2 gravity = new Vector2(0, -Physics.gravity.y);
	//Layers that will stop the trajectory
	public LayerMask layersToHit;	
	//An internal variable that controlles whether or not the
	internal bool hasFired = false;
	
	//The vertical velocity of the object
	float verticalVelocity;
	//The initial horizontal velocity of the object
	float horizontalVelocity;
	//The y displacement of the object
	float yDisplacement;
	//The x displacement of the object
	float xDisplacement;
	//The angle of launch
	float angle;
	//The line renderer component
	LineRenderer lineRenderer;
	//Vector point of the next point in the trajectory
	Vector3 vector;
    Vector3 lastVector;
	//Array of objectTextures
	GameObject[] objectPoints;
	//Simple integer used for for-loops
	int x;
	//Used for time variable in Kinematic equation
	float i;
	//Holds all of the "Texture Objects"
	GameObject textureObjectsHolder;
    Transform crosshairTransform;
	
	// Use this for initialization
	void Start () {
		
		//Gets the line renderer component
		lineRenderer = GetComponent<LineRenderer>();

        if(crosshair != null)
            crosshairTransform = crosshair.GetComponent<Transform>();

		
		//Creates an "textureObjectHolder" if need be
		if(textureObject != null){
			
			if(GameObject.Find("TextureObjectsHolder(Trajectory)") == null){
			
				textureObjectsHolder = new GameObject("TextureObjectsHolder(Trajectory)");
			
			}else{
				
				textureObjectsHolder = GameObject.Find("TextureObjectsHolder(Trajectory)");
			
			}
		
		//Checks if there is a texture object and if so instatiates them into the objectPoints array				
		if(maxNumberOfPoints >= 0){
			
			objectPoints = new GameObject[maxNumberOfPoints];
			
				for(x = 0; x < maxNumberOfPoints; x++){
					
					objectPoints[x] = (GameObject)Instantiate(textureObject);
					
					objectPoints[x].transform.parent = textureObjectsHolder.transform;
					
					objectPoints[x].active = false;
					
				}
				
		}else{
				
				throw new System.OverflowException("Cannot use a negative number in the 'Max Number Of Points' parameter of the Trajectory Predictor script!");
				
		}
			
		}
		
	}
	
	// Update is called once per frame
	void Update () {

			//Sets the angle to the Euler equivalent of the object's rotation
		if(hasFired == false){
		
			angle = this.transform.rotation.eulerAngles.z;
		
		}
		
		//If there is a texture object set all objects in the objectPoints array to false
		if(textureObject != null){
		
			for(x = 0; x < maxNumberOfPoints; x++){
								
				objectPoints[x].active = false;
				
			}
			
		}
		
		//Gets the vertical velocity of the object
		verticalVelocity = velocity * Mathf.Sin(angle * Mathf.Deg2Rad);
		
		//Gets the horizontal velocity of the object
		horizontalVelocity = velocity * Mathf.Cos(angle * Mathf.Deg2Rad);
		
		//Makes sure that the time between points is not set to 0
		if(timeBetweenPoints != 0){
			
			//An integer that records what line number is currently being operated on
			int lineIndex = 0;
			
			//Sets the line renderer line count to the maxNumberOfPoints
			if(textureObject == null){
			
				lineRenderer.SetVertexCount(maxNumberOfPoints);
			
			}
			
			i = 0;
			
			vector = this.transform.position;		
			
			//Makes sure the line Index does not exceed the maxNumberOfPoints
			while(lineIndex < maxNumberOfPoints){                  
				
				//Makes sure the current vector point is not intersecting an object with one of the layersToHit layer
				if(Physics.CheckSphere(vector, 0, layersToHit) == false){
					
                    if (crosshair != null) {
                    
                        crosshair.GetComponent<Renderer>().enabled = false;
    
                    }

                    lastVector = vector;

					//Iterates I if lineIndex is more than 0 so the
					if(lineIndex > 0){
					
						i += timeBetweenPoints;
					
					//Sets the y displacement to the kinematic equation including the vertical velocity component								
					yDisplacement = (float)(verticalVelocity * i + 0.5f * -gravity.y * (i * i))+this.transform.position.y;
					
					//Sets the x displacement to the kinematic equation including the horizontal velocity component								
					xDisplacement = horizontalVelocity * i + 0.5f * gravity.x * (i * i) + this.transform.position.x;
					
					//Creats a point using the x and y displacement and the zDepth
					vector = new Vector3(xDisplacement, yDisplacement, zDepth);
					
					}

					//Makes sure the texture object isn't null
					if(textureObject != null){
						
						//Checks if lineIndex divided by textureObjectDivisor has a remainder
						if(textureObjectDivisor != 0){
						
							if(lineIndex % textureObjectDivisor == 0){
								
								//Turns one of the objectPoints on
								objectPoints[lineIndex].active = true;
								//Sets the position of the activated to the vector point
								objectPoints[lineIndex].transform.position = vector;
						
							}
						
						}else{
							
							throw new System.DivideByZeroException("The 'Texture Object Divisor' parameter cannot be 0 in the 'Trajectory Predictor' script");
							
						}
							
					}else{
						
						//Sets a line renderer line to the position of the vector point
						lineRenderer.SetPosition(lineIndex, vector);	
																			
					}
				
				//Increments the lineIndex variable by 1
				lineIndex++;
			
				}else{
					
                    if (crosshair != null) {

                        crosshair.GetComponent<Renderer>().enabled = true;

					    RaycastHit hit;
                        Physics.Linecast(lastVector, vector, out hit);
                        crosshairTransform.position = hit.point;
                        crosshairTransform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);;
                    }
                
                    break;
				
				}

				
			}
			
			if(textureObject == null){
				//Sets the number of lines in the line renderer to lineIndex
				lineRenderer.SetVertexCount(lineIndex);
			
			}

		}
			
	}
	
}



