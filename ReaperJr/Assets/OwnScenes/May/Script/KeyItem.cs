using UnityEngine;

public class KeyItem : ReaperJr
{
    public Sprite itemSprite;

    public Vector3 emiColour = Vector3.zero;
    [HideInInspector]
    public bool isCollected;
    public float rotSpeed = 20f;
    private Vector3 RotCenter;

    public float upDistance = 1f;
    public float floatSpeed = 0.1f;
    private bool movingUp = false;
    [HideInInspector]
    public bool isInPosition = false;

    private void Start()
    {
        GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(emiColour.x / 255f, emiColour.y / 255f, emiColour.z / 255f));
        RotCenter = transform.position;
        isCollected = false;
        isInPosition = false;
    }

    private void Update()
    {
        if (!isCollected)
        {
            transform.RotateAround(RotCenter + new Vector3(0.1f, 0f, 0.1f), Vector3.up, rotSpeed * Time.deltaTime);
            Float();
        }
    }

    private void Float()
    {
        float newY = transform.position.y + (movingUp ? 1 : -1) * 2 * upDistance * floatSpeed * Time.deltaTime;
        if (newY > RotCenter.y + upDistance)
        {
            newY = RotCenter.y + upDistance;
            movingUp = false;
        }
        else if (newY < RotCenter.y)
        {
            newY = RotCenter.y;
            movingUp = true;
        }
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnEnable()
    {
        GameEvents.OnKeyItemCollected += OnKeyItemCollected;
        GameEvents.OnKeyInPosition += OnKeyInPosition;
    }

    private void OnDisable()
    {
        GameEvents.OnKeyItemCollected -= OnKeyItemCollected;
        GameEvents.OnKeyInPosition -= OnKeyInPosition;
    }

    void OnKeyItemCollected(KeyItem keyItem)
    {
        if (keyItem == this)
        {           
            this.gameObject.SetActive(false);
            _PLAYER.keyCollect = false;
            GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
        }
    }

    void OnKeyInPosition(KeyItem key)
    {
        if (key == this)
            _PLAYER.keyCollect = true;
    }
}
