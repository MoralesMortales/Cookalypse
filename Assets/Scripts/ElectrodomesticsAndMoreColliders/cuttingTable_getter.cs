using UnityEngine;

public class TagDetector : MonoBehaviour
{
    [SerializeField]
    private string tagToDetect = "canCut";

    [SerializeField]
    private GameObject tomatoToActivate;

    [SerializeField]
    private GameObject onionToActivate;

    public PickUpScript toolUsing;

    void CurrentTool()
    {
        if (toolUsing.currentToolData != null)
        {
            Debug.Log($"Usando herramienta: {toolUsing.currentToolData}");
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
            //Debug.Log($"{other.name} entró en la zona. ¡Tiene el tag '{tagToDetect}'!");

            if (otherTags.HasTag("tomato"))
            {
                if (tomatoToActivate != null)
                {
                    Debug.Log("es tomato");
                    tomatoToActivate.SetActive(true);
                }
                other.gameObject.SetActive(false);
            }
            else if (otherTags.HasTag("onion"))
            {
                if (onionToActivate != null)
                {
                    onionToActivate.SetActive(true);
                }
                other.gameObject.SetActive(false);
            }
        }
    }
}
