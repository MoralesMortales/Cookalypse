using UnityEngine;

public class stove : MonoBehaviour
{
    // [SerializeField]
    // private string tagToDetect = "canFry";

    [SerializeField]
    private GameObject wellFryedMeat;


    [SerializeField]
    private GameObject eggFried;

    [SerializeField]
    private GameObject overFryedMeat;

    [SerializeField]
    private Vector3 spawnPoint;
    private bool inFrontOfStove;

    public PickUpScript toolUsing;

    private bool fryingPanUsing = false;

    public bool fryingPanOnStove;

    //private bool withFood = false;
    private string food;

    void Start()
    {
        spawnPoint = new Vector3(-14.19f, 0.95f, 5.29f);
        if (toolUsing == null)
        {
            toolUsing = FindObjectOfType<PickUpScript>();
            if (toolUsing == null)
                Debug.LogError("No se encontró PickUpScript en la escena.");
        }
        fryingPanOnStove = false;
    }

    void CurrentTool()
    {
        if (toolUsing.currentToolData != null)
        {
            fryingPanUsing = toolUsing.currentToolData == "fryingPan";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AssignMultipleTags otherTags = other.GetComponent<AssignMultipleTags>();
        if (otherTags == null)
        {
            Debug.Log("ERRRRROR");
            return;
        }
        if (otherTags.HasTag("food"))
        {
            Debug.Log("U have food");
            witchFood(otherTags);
        }
        if (otherTags.HasTag("Player"))
        {
            inFrontOfStove = true;
            CurrentTool();

        }

    }
    private void OnTriggerExit(Collider other)
    {
        AssignMultipleTags otherTags = other.GetComponent<AssignMultipleTags>();

        if (otherTags.HasTag("Player"))
        {
            inFrontOfStove = false;
        }
    }

    void witchFood(AssignMultipleTags foodObject)
    {
        if (foodObject.HasTag("egg"))
        {
            food = "egg";
        }
        else if (foodObject.HasTag("meat"))
        {
            food = "meat";
        }
        else
        {
            food = "air";
        }

    }
    private void Update()
    {
        if (fryingPanOnStove && Input.GetKeyDown(KeyCode.F) && inFrontOfStove)
        {
            if (food == "egg")
            {
                eggFried.SetActive(true);
                eggFried.transform.position = spawnPoint;
            }
            Debug.Log("TRUE IS");
        }
        else
        {
            Debug.Log("Nonono");
        }

        if (fryingPanUsing && Input.GetKeyDown(KeyCode.F))
        {
            GameObject heldObject = toolUsing.GetHeldObject();
            if (heldObject != null)
            {
                fryingPanOnStove = true;
                heldObject.transform.parent = null;
                heldObject.SetActive(true);
                heldObject.transform.position = spawnPoint;
                heldObject.transform.rotation = Quaternion.identity;

                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.isKinematic = false;

                foreach (var tool in toolUsing.toolDatabase)
                {
                    if (tool.toolOnGrab.activeSelf)
                    {
                        tool.toolOnGrab.SetActive(false);
                        break;
                    }
                }

                toolUsing.ClearHeldObject(); // Limpia el heldObj y estado
                fryingPanUsing = false;
            }
        }
    }
}
