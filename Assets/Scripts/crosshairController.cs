using System;
using UnityEngine;
using UnityEngine.UI;

public class crosshairController : MonoBehaviour
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
bool isLookingAtTarget = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, checkDistance) && hit.collider.CompareTag(interactTag);

crosshair.color = isLookingAtTarget ? interactColor : defaultColor;
    }
}
