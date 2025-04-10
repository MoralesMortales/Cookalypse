using UnityEngine;

public class PickUpScript : MonoBehaviour
{
public GameObject player;
public Transform holdPos;

public float throwForce = 500f;
public float pickUpRange = 5f;
private float rotationSensitivity = 1f;
private GameObject heldObj;
private Rigidbody heldObjRb;
private bool canDrop = true;
private int LayerNumber;

void Start() {
LayerNumber = LayerMask.NameToLayer("holdLayer");

}

void Update() {
if (Input.GetKeyDown(KeyCode.F))
{
    if (heldObj == null)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
        {
            if (hit.transform.gameObject.tag == "canPickUp")
            {
                PickUpObject(hit.transform.gameObject);
            }
        }
    }

    else
    {
        if (canDrop == true)
        {
            StopClipping();
            DropObject();
        }
    }
}    
if (heldObj != null)
{
    MoveObject();
    RotateObject();
    if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop == true)
    {
        stopClipping();
        ThrowObject();
        
    }
}
}

void

}
