using UnityEngine;
using System.Collections;

public class Reloader : MonoBehaviour {
	
	//Object to be instantiated as a prefab
	public GameObject projectile;
	//Time it takes for the cannon to reload
	public float reloadTime;
	//The object where the projectile will instantiate to. Required in order to avoid child to parent scaling
	public Transform reloadPosObject;
	
	//Used to hold instances of the projectile variable
	GameObject instanceOfProjectile;
	
	//Start function with co-routine inside
	IEnumerator Start () {
		
		//Loop that runs forever
		while(true){
			
			//Waits for a single frame. Required to stop freezing from infinite loop
			yield return new WaitForSeconds(0);
			
			
			
		}
		
	}

	private void Update()
	{
		//Waits for the spacebar to be pressed
		if (Input.GetKeyDown(KeyCode.F))
		{

			//Waits for reloadTime in seconds
			
			//Instantiates a projectile to the instanceOfProjectile variable
			instanceOfProjectile = (GameObject)Instantiate(projectile, reloadPosObject.position, transform.rotation);
			//Sets the parent of the instance to the object this script is on
			instanceOfProjectile.transform.parent = this.transform;

		}
	}

}
