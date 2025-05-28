using UnityEngine;

public class dish : MonoBehaviour
{
private bool dishActive;
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
                dishActive = true;
            }
            else
            {
                dishActive = false;
            }
        }
    }

    public void addIngredientPlusToDish(GameObject ingredientPlus)
    {
        Debug.Log("holas, tienes ", ingredientPlus);
        AssignMultipleTags objTags = ingredientPlus.GetComponent<AssignMultipleTags>();
        if (objTags.HasTag("friedEgg")  && dishActive)
        {
            Debug.Log("mostrar fried egg");
        }
        else if (objTags.HasTag("tomatoSlice") && dishActive)
        {
            Debug.Log("mostrar tomatoSlice");
        }
    }
}
