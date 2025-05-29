using UnityEngine;
using UnityEngine.InputSystem;

public class TagDetector : MonoBehaviour
{
    [SerializeField]
    private string tagToDetect = "canCut";

    [SerializeField]
    private GameObject tomatoToActivate;

    [SerializeField]
    private GameObject tomatoSlicePrefab;

    [SerializeField]
    private GameObject lettuceToActivate;

    [SerializeField]
    private GameObject lettuceSlicePrefab;

    [SerializeField]
    private Vector3 spawnPoint;

    public PickUpScript toolUsing;

    private bool knifeUsing = false;
    private bool withFood = false;
    private string food;

    private void Start()
    {
        spawnPoint = new Vector3(-16.171f, 1.059f, 22.966f);
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
            knifeUsing = (toolUsing.currentToolData == "Knife");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AssignMultipleTags otherTags = other.GetComponent<AssignMultipleTags>();
        if (otherTags == null)
            return;

        if (otherTags.HasTag("Player"))
        {
            CurrentTool();
        }

        if (otherTags.HasTag(tagToDetect))
        {
            if (otherTags.HasTag("tomato") && tomatoToActivate != null)
            {
                tomatoToActivate.SetActive(true);
                withFood = true;
                food = "tomato";
                other.gameObject.SetActive(false);
            }
            else if (otherTags.HasTag("lettuce") && lettuceToActivate != null)
            {
                lettuceToActivate.SetActive(true);
                withFood = true;
                food = "lettuce";
                other.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (knifeUsing && withFood && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("picado, nan nan nan");
            if (tomatoSlicePrefab != null)
            {
                if (food == "tomato")
                {
                    SpawnTomatoCopy();
                }
                if (food == "lettuce")
                {
                    SpawnLettuceCopy();
                }
            }
            withFood = false;
            if (tomatoToActivate != null)
                tomatoToActivate.SetActive(false);
            if (lettuceToActivate != null)
                lettuceToActivate.SetActive(false);
        }

    }

    void SpawnTomatoCopy()
    {
        if (tomatoSlicePrefab != null)
        {
            Debug.Log("creadp");
            Debug.Log("spawnPoint = " + spawnPoint);
            GameObject newTomato = Instantiate(tomatoSlicePrefab, spawnPoint, Quaternion.identity);
            newTomato.SetActive(true);
        }
    }
    void SpawnLettuceCopy()
    {
        if (lettuceSlicePrefab != null)
        {
            Debug.Log("creadp");
            Debug.Log("spawnPoint = " + spawnPoint);
            GameObject newTomato = Instantiate(lettuceSlicePrefab, spawnPoint, Quaternion.identity);
            newTomato.SetActive(true);
        }
    }
}
