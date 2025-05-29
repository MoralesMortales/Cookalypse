using System.Collections.Generic;
using UnityEngine;

public class dish : MonoBehaviour
{
    private bool dishActive;
    public PickUpScript toolUsing;
    public string currentDish;

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
        AssignMultipleTags objTags = ingredientPlus.GetComponent<AssignMultipleTags>();

        if (objTags.HasTag("friedEgg") && dishActive)
        {
            Debug.Log("mostrar fried egg");
            currentDish = "omelette";

            for (int i = 0; i < dishDatabase.Count; i++)
            {
                if (dishDatabase[i].dish.GetComponent<AssignMultipleTags>().HasTag("omelette"))
                {
                    Debug.Log("huevito sancochado");
                    dishDatabase[i].dish.SetActive(true);

                    // Desactivar ambos: toolOnGrab y toolOnView del plato
                    if (toolUsing != null)
                    {
                        // Buscar el plato en la toolDatabase
                        foreach (var tool in toolUsing.toolDatabase)
                        {
                            if (tool.toolOnView.GetComponent<AssignMultipleTags>().HasTag("plate"))
                            {
                                tool.toolOnGrab.SetActive(false);
                                tool.toolOnView.SetActive(false);
                                break;
                            }
                        }

                        toolUsing.currentToolData = "Aire"; // Resetear la herramienta actual
                        dishActive = false;
                    }

                    break;
                }
            }
        }
        else if (objTags.HasTag("tomatoSlice") && dishActive)
        {
            Debug.Log("mostrar tomatoSlice");
            currentDish = "tomato";
        }
    }
}
