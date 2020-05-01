using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkControl : MonoBehaviour
{
    public GameObject tabSwitch;
    public ParticleSystem flowWater;
    public GameObject waterLevel;
    public GameObject protectFilm;
    public GameObject player;
    public GameObject plugSwitch;
    public GameObject filmSwitch;

    public GameObject soulInTab;
    private Animator soulAnme;

    public float raisingFactor = 0.3f;
    private Vector3 oriWaterLine;
    private Vector3 waterLine;

    private bool tabOn = false;
    private bool tabClickable = false;
    private bool plugIn = true;
    private bool plugClickable = false;
    public bool filmOn = false;
    private bool switchClickable = false;
    private bool switchFilmOn = false;

    public bool fillWater = false;
    public bool playerIn = false;
    public float fillDuration = 5f;
    public float drainDuration = 3f;
    public float clickDist = 3f;
    private float fillTimeRemind = 0f;
    private float drainTimerRemind = 0f;
    public float maxWaterLevel = 3f;


    // Start is called before the first frame update
    void Start()
    {
        flowWater.Stop();
        tabClickable = false;
        tabOn = false;
        oriWaterLine = waterLevel.transform.position;
        waterLine = oriWaterLine;
        waterLevel.SetActive(false);
        fillTimeRemind = fillDuration;
        drainTimerRemind = drainDuration;
        if (soulInTab != null)
        {
            soulAnme = soulInTab.GetComponent<Animator>();
            soulInTab.GetComponent<Rigidbody>().isKinematic = true;
            soulInTab.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        #region ClickEvent
        if (!GameManager.Instance.scytheEquiped && player != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (Vector3.Distance(hit.transform.position, player.transform.position) <= clickDist)
                    {
                        if (tabClickable)
                        {
                            if (hit.transform.tag == "Switch" && hit.transform.name == "Tab")
                            {
                                tabOn = !tabOn;

                                if (soulInTab != null)
                                {
                                    soulInTab.SetActive(true);
                                    soulAnme.SetBool("SoulOut", true);
                                    soulInTab.GetComponent<Rigidbody>().isKinematic = false;
                                    soulInTab.GetComponent<Rigidbody>().AddForce(soulInTab.transform.right * 5, ForceMode.Impulse);
                                }
                            }
                        }

                        if (plugClickable)
                        {
                            if (hit.transform.tag == "Switch" && hit.transform.name == "PlugSwitch")
                                plugIn = !plugIn;
                        }

                        if (switchClickable)
                        {
                            if (hit.transform.tag == "Switch" && hit.transform.name == "FilmSwitch")
                                switchFilmOn = !switchFilmOn;
                        }
                    }
                    else
                        return;
                }
            }
        }
        else
            return;
        #endregion

        waterLevel.transform.position = waterLine;
        if (waterLine.y > oriWaterLine.y)
            waterLevel.SetActive(true);
        else
            waterLevel.SetActive(false);

        #region TabSwitch
        if (tabOn)
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
            tabClickable = false;
            tabOn = false;
            waterLine.y = maxWaterLevel;
        }
        else
            tabClickable = true;

        if (fillTimeRemind <= 0f)
        {
            tabOn = false;
            fillTimeRemind = fillDuration;
        }
        #endregion

        #region Drain
        if (!plugIn)
        {
            StartCoroutine("WaterLevelControl", -2 * raisingFactor);
            drainTimerRemind -= Time.deltaTime; //auto turn off timer

            if (plugSwitch.GetComponent<Renderer>() != null)
                plugSwitch.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(143f / 255f, 185f / 255f, 74f / 255));
        }
        else
        {
            StopCoroutine("WaterLevelControl");
            drainTimerRemind = drainDuration;

            if (plugSwitch.GetComponent<Renderer>() != null)
                plugSwitch.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(101f / 255f, 62f / 255f, 19f / 255));
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
        if (waterLine.y >= protectFilm.transform.GetChild(0).position.y)
        {
            filmOn = false;
        }

        if (waterLine.y >= protectFilm.transform.GetChild(0).position.y)  //auto turn off the protective film if water level is higher that it.
        {
            fillWater = true;
            switchClickable = true;
        }
        else
        {
            fillWater = false;
            switchClickable = false;
        }

        if (filmOn || switchFilmOn)
        {
            protectFilm.transform.GetChild(0).gameObject.SetActive(true);

            if (protectFilm.GetComponent<Renderer>() != null)
                protectFilm.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(130f / 255f, 10f / 255f, 117f / 255));
        }
        else
        {
            protectFilm.transform.GetChild(0).gameObject.SetActive(false);

            if (protectFilm.GetComponent<Renderer>() != null)
                protectFilm.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(101f / 255f, 62f / 255f, 19f / 255));
        }
        #endregion

        if (player != null)
        {
            if (playerIn && waterLine.y >= player.transform.position.y + player.GetComponent<CapsuleCollider>().height / 2f) //player will dead if water level is above it.
                GameManager.Instance.dead = true;
        }

        #region EmissionSwitch
        if (tabSwitch.transform.GetChild(0).GetComponent<Renderer>() != null & tabSwitch.transform.GetChild(1).GetComponent<Renderer>() != null)
        {
            if (tabClickable)
            {
                tabSwitch.transform.GetChild(0).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                tabSwitch.transform.GetChild(1).GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            }
            else
            {
                tabSwitch.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                tabSwitch.transform.GetChild(1).GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            }
        }

        if (plugSwitch.GetComponent<Renderer>() != null)
        {
            if (plugClickable)
                plugSwitch.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            else
                plugSwitch.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }

        if (filmSwitch.GetComponent<Renderer>() != null)
        {
            if (switchClickable)
                filmSwitch.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            else
                filmSwitch.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }

        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (player == null)
                player = other.gameObject;

            tabClickable = true;
            plugClickable = true;
            switchClickable = false;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            tabClickable = false;
            plugClickable = false;
            switchClickable = false;
            player = null;
        }
    }

    IEnumerator WaterLevelControl(float factor)
    {
        waterLine.y += factor * Time.deltaTime;
        yield return null;
    }
}
