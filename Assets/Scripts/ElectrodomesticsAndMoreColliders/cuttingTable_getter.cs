using UnityEngine;
using UnityEngine.InputSystem;

public class TagDetector : MonoBehaviour
{
    [SerializeField]
    private string tagToDetect = "canCut";

    [SerializeField]
    private GameObject tomatoToActivate;

    [SerializeField]
    private GameObject onionToActivate;

    public PickUpScript toolUsing;

    [SerializeField]
    private GameObject tomatoSlicePrefab;

    private bool knifeUsing = false;
    private bool withFood = false;

    private void Start()
    {
        if (toolUsing == null)
        {
            toolUsing = FindObjectOfType<PickUpScript>();

            if (toolUsing == null)
            {
                Debug.LogError("No se encontró PickUpScript en la escena.");
            }
        }
    }

    void CurrentTool()
    {
        if (toolUsing.currentToolData != null)
        {
            Debug.Log($"Usando herramienta: {toolUsing.currentToolData}");
            if (toolUsing.currentToolData == "Knife")
            {
                knifeUsing = true;
            }
            else
            {
                knifeUsing = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AssignMultipleTags otherTags = other.GetComponent<AssignMultipleTags>();

        if (otherTags == null)
            return;

        if (otherTags.HasTag("Player"))
        {
            Debug.Log("hola player");

            CurrentTool();
        }

        if (otherTags != null && otherTags.HasTag(tagToDetect))
        {
            if (otherTags.HasTag("tomato"))
            {
                if (tomatoToActivate != null)
                {
                    Debug.Log("Tomato para picar");
                    tomatoToActivate.SetActive(true);
                    withFood = true;
                }
                other.gameObject.SetActive(false);
            }
            else if (otherTags.HasTag("onion"))
            {
                if (onionToActivate != null)
                {
                    onionToActivate.SetActive(true);
                    withFood = true;
                }
                other.gameObject.SetActive(false);
            }
        }

        if (knifeUsing && withFood && (Input.GetKeyDown(KeyCode.F)))
        {
            Debug.Log("picado, nan nan nan");
        }
        else
        {
            Debug.Log(
                "tool "
                    + knifeUsing
                    + " with food"
                    + withFood
                    + "Key "
                    + (Input.GetKeyDown(KeyCode.F))
            );
        }
    }
}
