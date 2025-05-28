using UnityEngine;

public class dish : MonoBehaviour
{
    public PickUpScript toolUsing;

    void Start()
    {
        if (toolUsing == null)
        {
            toolUsing = FindObjectOfType<PickUpScript>();
            if (toolUsing == null)
                Debug.LogError("No se encontró PickUpScript en la escena.");
        }
    }

    void Update()
    {
        if (toolUsing.currentToolData != null)
        {
            if (toolUsing.currentToolData == "Plate")
            {
                Debug.Log("true");

            }
        }
    }
}
