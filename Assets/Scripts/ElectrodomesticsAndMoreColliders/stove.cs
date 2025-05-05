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
    private bool withFood = false;
    private string food;

    void Start()
    {
        spawnPoint = new Vector3(-14.15f, 0.95f, 6.7f);
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
            fryingPanUsing = (toolUsing.currentToolData == "fryingPan");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AssignMultipleTags otherTags = other.GetComponent<AssignMultipleTags>();
        if (otherTags == null)
        {
            return;
        }

        if (otherTags.HasTag("Player"))
        {
            CurrentTool();
        }
    }

    private void Update()
    {
        if (fryingPanUsing && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("usando");

            // Encuentra el objeto sartén que se está usando actualmente
            GameObject heldObject = toolUsing.GetHeldObject();
            if (heldObject != null)
            {
                // Quita el objeto de la mano
                heldObject.transform.parent = null;

                // Lo mueve a la posición deseada
                heldObject.transform.position = spawnPoint;
                heldObject.transform.rotation = Quaternion.identity; // o como prefieras

                // Reactiva física si era necesario
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.isKinematic = false;

                // Limpia el estado del pickup
                toolUsing.ClearHeldObject();

                // Ya no está usando sartén
                fryingPanUsing = false;
            }
        }
    }
}
