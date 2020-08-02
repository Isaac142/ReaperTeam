using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Requires the LineRenderer component
[RequireComponent(typeof(LineRenderer))]

public class ScytheController : ReaperJr
{
    [Header("Reaper JR")]
    public GameObject scythe;
    public int rateOfRotation;
    [NonSerialized]
    public bool holdingScythe = true;

    [Header("Trajectory")]
    //Velocity that the object will be shot at
    public float velocity;
    //Amount of time between each point
    public float timeBetweenPoints = 0.01f;
    //Max number of points allowed in the trajectory line/Max amount of texture objects allowed
    public int maxNumberOfPoints = 500;
    //Object that will be instantiated
    public GameObject textureObject;
    public GameObject crosshair;
    //If the the point of arc number modulo of this variable equals 0 a texture will be placed on that point
    public int textureObjectDivisor = 1;
    //The acceleration of gravity. It is automatically set to the acceleration of the earth's gravity.
    public Vector3 gravity = new Vector3(0, -Physics.gravity.y, 0);
    //Layers that will stop the trajectory
    public LayerMask layersToHit;
    //Is the projectile being fired out of the xAxis?
    public bool xAxisForward = false;
    //An internal variable that controlles whether or not the
    internal bool hasFired = false;

    //The vertical velocity of the object
    float verticalVelocity;
    //The initial horizontal velocity of the object
    float horizontalVelocity;
    //Velocity in the z direction
    float depthVelocity;

    //The y displacement of the object
    float yDisplacement;
    //The x displacement of the object
    float xDisplacement;
    //Object displacement in the z direction
    float zDisplacement;

    //The angle of launch
    float angle;
    //Another angle of launcher. Two angles are needed for 3D prediction
    float phi;

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

    bool lineOut = false;
    
    // Use this for initialization
    void Start()
    {
        //Gets the line renderer component
        lineRenderer = GetComponent<LineRenderer>();
        lineOut = false;
        GameEvents.RepoartCrossHairOut(lineOut);

        if (crosshair != null)
            crosshairTransform = crosshair.GetComponent<Transform>();

        //Generates an empty game Object to hold the "Texture Objects" if need be
        if (textureObject != null)
        {

            if (GameObject.Find("TextureObjectsHolder(Trajectory)") == null)
            {

                textureObjectsHolder = new GameObject("TextureObjectsHolder(Trajectory)");

            }
            else
            {

                textureObjectsHolder = GameObject.Find("TextureObjectsHolder(Trajectory)");

            }

            //Checks if there is a texture object and if so instantiates them into the objectPoints array
            if (maxNumberOfPoints >= 0)
            {

                objectPoints = new GameObject[maxNumberOfPoints];

                for (x = 0; x < maxNumberOfPoints; x++)
                {

                    objectPoints[x] = (GameObject)Instantiate(textureObject);

                    objectPoints[x].transform.parent = textureObjectsHolder.transform;

                    objectPoints[x].active = false;

                }

            }
            else
            {

                throw new System.OverflowException("Cannot use a negative number in the 'Max Number Of Points' parameter of the Trajectory Predictor script!");

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Sets "angle" and "phi" to the Euler equivalent of the object's rotation
        if (hasFired == false)
        {
            if (xAxisForward == true)
            {
                angle = transform.rotation.eulerAngles.z;
                phi = transform.rotation.eulerAngles.y + 90;
            }
            else
            {
                angle = -transform.rotation.eulerAngles.x;
                phi = transform.rotation.eulerAngles.y;
            }
        }

        //If there is a texture object set all objects in the objectPoints array to false
        if (textureObject != null)
        {
            for (x = 0; x < maxNumberOfPoints; x++)
            {
                objectPoints[x].active = false;
            }
        }

        //Gets the vertical velocity of the object		
        verticalVelocity = velocity * Mathf.Sin(angle * Mathf.Deg2Rad);

        //Gets the horizontal velocity of the object
        horizontalVelocity = velocity * Mathf.Cos(angle * Mathf.Deg2Rad) * Mathf.Sin(phi * Mathf.Deg2Rad);

        //Uses Pythagorean's theorem to get the velocity in the z direction
        if (((phi - 90 > 180 && xAxisForward == true) || ((phi < 90 || phi > 270) && xAxisForward == false)) && velocity != verticalVelocity)
        {
            depthVelocity = Mathf.Sqrt((velocity * velocity) - (horizontalVelocity * horizontalVelocity) - (verticalVelocity * verticalVelocity));
        }
        else if (velocity != verticalVelocity)
        {
            depthVelocity = -Mathf.Sqrt((velocity * velocity) - (horizontalVelocity * horizontalVelocity) - (verticalVelocity * verticalVelocity));
        }
        else
        {
            depthVelocity = 0;
        }

        //If "depthVelocity" is an imaginary number (as sometimes happens when used in 2D/near 2D situations) it sets "depthVelocity" to a new equation
        if (float.IsNaN(depthVelocity))
        {
            depthVelocity = velocity * Mathf.Cos(phi * Mathf.Deg2Rad);
        }

        //Makes sure that the time between points is not set to 0
        if (timeBetweenPoints != 0)
        {
            //An integer that records what line number is currently being operated on
            int lineIndex = 0;

            //Sets the line renderer line count to the maxNumberOfPoints
            if (textureObject == null)
            {
                lineRenderer.SetVertexCount(maxNumberOfPoints);
            }

            i = 0;

            vector = this.transform.position;

            //Makes sure the line Index does not exceed the maxNumberOfPoints
            while (lineIndex < maxNumberOfPoints)
            {
                //Makes sure the current vector point is not intersecting an object with one of the layersToHit layer
                if (Physics.CheckSphere(vector, 0, layersToHit) == false)
                {
                    if (crosshair != null)
                    {
                        crosshair.GetComponent<Renderer>().enabled = false;
                    }

                    lastVector = vector;

                    //Iterates i if lineIndex is more than 0 so the
                    if (lineIndex > 0)
                    {
                        i += timeBetweenPoints;

                        //Sets the y displacement to the kinematic equation including the vertical velocity component								
                        yDisplacement = (float)(verticalVelocity * i + 0.5f * -gravity.y * (i * i)) + this.transform.position.y;

                        //Sets the x displacement to the kinematic equation including the horizontal velocity component								
                        xDisplacement = horizontalVelocity * i + 0.5f * gravity.x * (i * i) + this.transform.position.x;

                        if (angle >= 90 && angle < 270 || velocity < 0)
                        {

                            zDisplacement = -(depthVelocity * i + 0.5f * -gravity.z * (i * i)) + this.transform.position.z;

                        }
                        else
                        {

                            zDisplacement = depthVelocity * i + 0.5f * gravity.z * (i * i) + this.transform.position.z;

                        }

                        //Creats a point using the x and y displacement and the zDepth
                        vector = new Vector3(xDisplacement, yDisplacement, zDisplacement);

                    }

                    //Makes sure the texture object isn't null
                    if (textureObject != null)
                    {

                        //Checks if lineIndex divided by textureObjectDivisor has a remainder
                        if (textureObjectDivisor != 0)
                        {

                            if (lineIndex % textureObjectDivisor == 0)
                            {

                                //Turns one of the objectPoints on
                                objectPoints[lineIndex].active = true;
                                //Sets the position of the activated to the vector point
                                objectPoints[lineIndex].transform.position = vector;

                            }

                        }
                        else
                        {

                            throw new System.DivideByZeroException("The 'Texture Object Divisor' parameter cannot be 0 in the 'Trajectory Predictor' script");

                        }

                    }
                    else
                    {

                        //Sets a line renderer line to the position of the vector point
                        lineRenderer.SetPosition(lineIndex, vector);

                    }

                    //Increments the lineIndex variable by 1
                    lineIndex++;

                }
                else
                {

                    if (crosshair != null && holdingScythe)
                    {
                        crosshair.GetComponent<Renderer>().enabled = true;

                        RaycastHit hit;
                        Physics.Linecast(lastVector, vector, out hit);
                        crosshairTransform.position = hit.point;
                        crosshairTransform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                        crosshair.transform.position = hit.point;
                        crosshair.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    }

                    else if(crosshair != null && !holdingScythe)
                        crosshair.GetComponent<Renderer>().enabled = true;

                    break;
                }
            }

            if (textureObject == null)
            {
                //Sets the number of lines in the line renderer to lineIndex
                lineRenderer.SetVertexCount(lineIndex);

            }

        }


        #region Reaper JR
        //Increases the rotation when the left arrow key is pressed
        if (Input.GetKey("left"))
        {

            this.transform.localRotation = Quaternion.Euler(0, 0, this.transform.rotation.eulerAngles.z + rateOfRotation);

            //Decreases rotation when the right arrow key is pressed
        }
        else if (Input.GetKey("right"))
        {

            this.transform.localRotation = Quaternion.Euler(0, 0, this.transform.rotation.eulerAngles.z - rateOfRotation);

        }

        if (Input.GetKeyDown(KeyCode.Q) && _GAME.playerActive)
        {
            lineOut = !lineOut;
            GameEvents.RepoartCrossHairOut(lineOut);
        }

        if (lineOut)
        {
            lineRenderer.enabled = true;
            crosshair.SetActive(true);
            _CAMERA.scythe = crosshair.transform;   // when showing the renderline, camera stays at the middle between character and crosshair.
        }
        else
        {
            lineRenderer.enabled = false;
            crosshair.SetActive(false);
            _CAMERA.scythe = scythe.transform;
        }

        if (!holdingScythe)
            lineRenderer.enabled = false;

        if (Input.GetMouseButtonDown(0) && _GAME.playerActive && _GAME.gameState == GameState.INGAME)
        {
            if (holdingScythe && !_GAME.onCD)
            {
                _AUDIO.Play("ScytheThrow");
                _PLAYER.anim.SetTrigger("ScytheThrow");
                scythe.transform.parent = null;
                Physics.gravity = new Vector3(0, -gravity.y, 0);
                scythe.GetComponent<Scythe>().Launch(velocity);
                holdingScythe = false;

                GameEvents.ReportScytheThrown(true);
            }

            else if (!holdingScythe)
            {
                StartCoroutine(ResetScythe());
            }
        }
        #endregion

        //float newF = Map(velocity, 10, 50, 0, 1);
        //float 
        //Debug.Log(newF);
    }

    IEnumerator ResetScythe()
    {
        yield return new WaitForSeconds(_PLAYER.timeToMove);
        //Physics.gravity.Set(0, -9.81f, 0);
        scythe.transform.SetParent(this.transform);
        scythe.transform.localPosition = Vector3.zero;
        scythe.transform.localEulerAngles = Vector3.zero;
        scythe.GetComponent<Rigidbody>().isKinematic = true;
        scythe.GetComponent<Rigidbody>().velocity.Set(0, 0, 0);
        holdingScythe = true;
        GameEvents.ReportScytheThrown(false);
    }
    /// <summary>
    /// Maps a value from a range to 0f..1f
    /// </summary>
    /// <returns>The mapped value</returns>
    /// <param name="value">The input Value.</param>
    /// <param name="inMin">Input min</param>
    /// <param name="inMax">Input max</param>
    /// <param name="clamp">Clamp output value to 0f..1f</param>
    float MapTo01(float value, float inMin, float inMax, bool clamp = true)
    {
        return Map(value, inMin, inMax, 0f, 1f, clamp);
    }

    /// <summary>
    /// Maps a value from one range to another
    /// </summary>
    /// <returns>The mapped value</returns>
    /// <param name="value">The input Value.</param>
    /// <param name="inMin">Input min</param>
    /// <param name="inMax">Input max</param>
    /// <param name="outMin">Output min</param>
    /// <param name="outMax">Output max</param>
    /// <param name="clamp">Clamp output value to outMin..outMax</param>
    float Map(float value, float inMin, float inMax, float outMin, float outMax, bool clamp = true)
    {
        float f = ((value - inMin) / (inMax - inMin));
        float d = (outMin <= outMax ? (outMax - outMin) : -(outMin - outMax));
        float v = (outMin + d * f);
        if (clamp) v = ClampSmart(v, outMin, outMax);
        return v;
    }

    static float ClampSmart(float value, float min, float max)
    {
        if (min < max)
            return Mathf.Clamp(value, min, max);
        if (max < min)
            return Mathf.Clamp(value, max, min);
        return value;
    }

}