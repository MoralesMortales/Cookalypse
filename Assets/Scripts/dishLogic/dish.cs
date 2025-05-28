using System.Collections.Generic;
using UnityEngine;

public class dish : MonoBehaviour
{
    private bool dishActive;
    public PickUpScript toolUsing;

    [System.Serializable]
    public class DishData
    {
        public GameObject dish;
    }

    public List<DishData> dishDatabase = new List<DishData>();

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

        if (objTags.HasTag("friedEgg") && dishActive)
        {
            Debug.Log("mostrar fried egg");

            for (int i = 0; i < dishDatabase.Count; i++)
            {
                if (dishDatabase[i].dish.GetComponent<AssignMultipleTags>().HasTag("omelette"))
                {
                    Debug.Log("huevito sancochado");
                    dishDatabase[i].dish.SetActive(true);
                    break;
                }
            }
        }
        else if (objTags.HasTag("tomatoSlice") && dishActive)
        {
            Debug.Log("mostrar tomatoSlice");
        }
    }
}
