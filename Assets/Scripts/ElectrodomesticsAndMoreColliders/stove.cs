using UnityEngine;

public class stove : MonoBehaviour
{
    [SerializeField]
    private string tagToDetect = "canFry";

    [SerializeField]
    private GameObject wellFryedMeat;

    [SerializeField]
    private GameObject overFryedMeat;

    [SerializeField]
    private Vector3 spawnPoint;

    public PickUpScript toolUsing;

    private bool fryingPanUsing = false;
    public bool fryingPanOnStove = false;

    //private bool withFood = false;
    //private string food;

    void Start()
    {
        spawnPoint = new Vector3(-14.19f, 0.95f, 5.29f);
        if (toolUsing == null)
        {
            toolUsing = FindObjectOfType<PickUpScript>();
            if (toolUsing == null)
                Debug.LogError("No se encontró PickUpScript en la escena.");
        }
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

        if (otherTags.HasTag("Player"))
        {
            CurrentTool();
        }
    }

    private void Update()
    {
        Debug.Log("Using? " + fryingPanOnStove);

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
