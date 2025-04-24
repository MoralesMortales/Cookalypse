using UnityEngine;

public class TagDetector : MonoBehaviour
{
    [SerializeField] private string tagToDetect = "canCut"; // Tag que queremos detectar

    private void OnTriggerEnter(Collider other) 
    {
        AssignMultipleTags otherTags = other.GetComponent<AssignMultipleTags>();
        if (otherTags != null && otherTags.HasTag(tagToDetect))
        {
            Debug.Log($"{other.name} entró en la zona. ¡Tiene el tag '{tagToDetect}'!");
        }
    }
}