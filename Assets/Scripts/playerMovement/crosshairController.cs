using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("Settings")]
    public float checkDistance = 7f;
    public string interactTag = "canPickUp";

    public Image crosshair;
    private Color defaultColor = Color.white; 
    private Color interactColor = Color.red; 

    void Update()
    {
        RaycastHit hit;
        bool isLookingAtTarget = false;

        // Lanza el raycast
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, checkDistance))
        {
            AssignMultipleTags objTags = hit.collider.GetComponent<AssignMultipleTags>();
            if (objTags != null && objTags.HasTag(interactTag))
            {
                isLookingAtTarget = true;
            }
        }

        crosshair.color = isLookingAtTarget ? interactColor : defaultColor;
    }
}