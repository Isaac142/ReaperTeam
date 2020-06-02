using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkControl : ReaperJr
{
    public GameObject tapSwitch;
    public ParticleSystem flowWater;
    public GameObject waterLevel;
    public GameObject protectFilm;
    public Transform player;
    public GameObject plugSwitch;
    public GameObject filmSwitch;
    public List<GameObject> bubbles = new List<GameObject>();

    public GameObject soulInTab;
    [VectorLabels("Xaxis", "Yaxis", "Zaxis")]
    public Vector3 soulMoveDir = Vector3.left;
    public float soulOutForce = 5f;
    private Animator soulAnme;

    public float raisingFactor = 0.3f;
    private Vector3 oriWaterLine;
    private Vector3 waterLine;

    public bool tapOn = false;
    private bool tapClickable = false;
    private bool plugIn = true;
    private bool plugClickable = false;
    [HideInInspector]
    public bool filmOn = false, fillWater = false, playerIn = false;

    private bool switchClickable = false;
    private bool switchFilmOn = false;
    public float fillDuration = 5f;
    public float drainDuration = 3f;
    public float clickDist = 3f;
    private float fillTimeRemind = 0f;
    private float drainTimerRemind = 0f;
    public float maxWaterLevel = 3f;


    // Start is called before the first frame update
    void Awake()
    {
        flowWater.Stop();
        if (soulInTab != null)
        {
            soulAnme = soulInTab.GetComponent<Animator>();
            soulInTab.GetComponent<Rigidbody>().isKinematic = true;
            soulInTab.SetActive(false);
        }
    }

    private void Start()
    {
        oriWaterLine = waterLevel.transform.position;
        waterLine = oriWaterLine;
        protectFilm.SetActive(false);
        fillTimeRemind = fillDuration;
        drainTimerRemind = drainDuration;

        if (bubbles.Count > 0)
        {
            foreach (GameObject bubble in bubbles)
                bubble.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        #region EmissionSwitch
        if (tapClickable)
        {
            tapSwitch.transform.GetChild(0).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            tapSwitch.transform.GetChild(1).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
        else
        {
            tapSwitch.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            tapSwitch.transform.GetChild(1).GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }

        if (plugClickable)
            plugSwitch.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        else
            plugSwitch.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

        if (plugIn)
            plugSwitch.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(101f / 255f, 62f / 255f, 19f / 255));
        else
            plugSwitch.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(143f / 255f, 185f / 255f, 74f / 255));

        if (switchClickable)
            filmSwitch.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        else
            filmSwitch.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");

        if (switchFilmOn)
            filmSwitch.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(130f / 255f, 10f / 255f, 117f / 255));
        else
            filmSwitch.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(101f / 255f, 62f / 255f, 19f / 255));
        #endregion

        if (player != null)
        {
            tapClickable = true;
            plugClickable = true;
        }
        else
        {
            tapClickable = false;
            plugClickable = false;
            switchClickable = false;
            return;
        }

        if (playerIn && waterLine.y >= player.position.y + player.GetComponent<CapsuleCollider>().height) //player will dead if water level is above it.
            _GAME.dead = true;

        #region ClickEvent
        if (!_GAME.scytheEquiped)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (Vector3.Distance(hit.transform.position, player.transform.position) <= clickDist)
                    {
                        if (hit.transform.tag == "Switch")
                        {
                            if (tapClickable )
                            {
                                if(hit.transform.name == tapSwitch.name)
                                tapOn = !tapOn;

                                if (soulInTab != null)
                                {
                                    soulInTab.SetActive(true);
                                    soulAnme.SetBool("SoulOut", true);
                                    soulInTab.GetComponent<Rigidbody>().isKinematic = false;
                                    soulInTab.GetComponent<Rigidbody>().AddForce(soulMoveDir * soulOutForce, ForceMode.Impulse);
                                }

                                if (bubbles.Count > 0)
                                {
                                    foreach (GameObject bubble in bubbles)
                                        bubble.SetActive(true);
                                }
                            }

                            if (plugClickable && hit.transform.name == plugSwitch.name)
                                plugIn = !plugIn;

                            if (switchClickable && hit.transform.name == filmSwitch.name)
                                switchFilmOn = !switchFilmOn;
                        }
                    }
                }
            }
        }
        #endregion

        waterLevel.transform.position = waterLine;
        if (waterLine.y > oriWaterLine.y)
            waterLevel.SetActive(true);
        else
            waterLevel.SetActive(false);

        #region TapSwitch
        if (tapOn)
        {
            flowWater.Play();
            StartCoroutine("WaterLevelControl", raisingFactor);
            fillTimeRemind -= Time.deltaTime; //auto turn off timer
        }
        else
        {
            flowWater.Stop();
            StopCoroutine("WaterLevelControl");
            fillTimeRemind = fillDuration;
        }

        if (waterLine.y >= maxWaterLevel) //water level clamp
        {
            tapClickable = false;
            tapOn = false;
            waterLine.y = maxWaterLevel;
        }
        else
            tapClickable = true;

        if (fillTimeRemind <= 0f)
        {
            tapOn = false;
            fillTimeRemind = fillDuration;
        }
        #endregion

        #region Drain
        if (!plugIn)
        {
            StartCoroutine("WaterLevelControl", -2 * raisingFactor);
            drainTimerRemind -= Time.deltaTime; //auto turn off timer
        }
        else
        {
            StopCoroutine("WaterLevelControl");
            drainTimerRemind = drainDuration;          
        }

        if (waterLine.y <= oriWaterLine.y) //water level clamp
        {
            plugClickable = false;
            plugIn = true;
            waterLine.y = oriWaterLine.y;
        }
        else
            plugClickable = true;

        if (fillTimeRemind <= 0f)
        {
            plugIn = true;
            drainTimerRemind = drainDuration;
        }
        #endregion

        #region ProtectiveFilmSwitch

        if (playerIn && !fillWater)
            filmOn = true;
        else
            filmOn = false;

        if (filmOn || switchFilmOn)
            protectFilm.SetActive(true);
        else
            protectFilm.SetActive(false);

        if (waterLine.y >= protectFilm.transform.position.y)
        {
            filmOn = false;
            fillWater = true;
            switchClickable = true;
        }
        else
        {
            fillWater = false;
            switchClickable = false;
        }
        #endregion    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (player == null)
                player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player = null;
        }
    }

    IEnumerator WaterLevelControl(float factor)
    {
        waterLine.y += factor * Time.deltaTime;
        yield return null;
    }
}
