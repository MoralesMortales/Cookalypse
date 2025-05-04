using System;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;

    public float throwForce = 500f;
    public float pickUpRange = 7f;
    private float rotationSensitivity = 50f;
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    private GameObject originalToolObj;

    [System.Serializable]
    public class ToolData
    {
        public GameObject toolOnView;
        public GameObject toolOnGrab;
    }

    public String currentToolData; 

    public List<ToolData> toolDatabase = new List<ToolData>();

    void usingTool(AssignMultipleTags tool)
    {
        if (tool.HasTag("Knife"))
        {
            currentToolData = "Knife";
        }
        else if (tool.HasTag("plate"))
        {
            currentToolData = "Plate";
        }
        else
        {
            currentToolData = "Aire";
        }
    }

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (
                    Physics.Raycast(
                        transform.position,
                        transform.TransformDirection(Vector3.forward),
                        out hit,
                        pickUpRange
                    )
                )
                {
                    AssignMultipleTags objTags = hit.transform.GetComponent<AssignMultipleTags>();

                    if (objTags != null)
                    {
                        if (objTags.HasTag("tool"))
                        {
                            GameObject currentTool = hit.transform.gameObject;

                            for (int i = 0; i < toolDatabase.Count; i++)
                            {
                                if (toolDatabase[i].toolOnView == currentTool)
                                {
                                    usingTool(objTags);
                                    
                                    toolDatabase[i].toolOnView.SetActive(false);
                                    toolDatabase[i].toolOnGrab.SetActive(true);
                                    originalToolObj = toolDatabase[i].toolOnView;

                                    PickUpObject(toolDatabase[i].toolOnView);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            PickUpObject(hit.transform.gameObject);
                        }
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
            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop == true) //Mous0 (leftclick) is used to throw, change this if you want another button to be used)
            {
                StopClipping();
                ThrowObject();
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        AssignMultipleTags objTags = pickUpObj.GetComponent<AssignMultipleTags>();

        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            if (objTags.HasTag("tool")) { }
            else
            {
                heldObjRb.transform.parent = holdPos.transform;
                heldObj.layer = LayerNumber;
            }

            Physics.IgnoreCollision(
                heldObj.GetComponent<Collider>(),
                player.GetComponent<Collider>(),
                true
            );
        }
    }

    void DropObject()
    {
        bool isTool = false;
        int toolIndex = -1;

        for (int i = 0; i < toolDatabase.Count; i++)
        {
            if (toolDatabase[i].toolOnView == heldObj)
            {
                isTool = true;
                toolIndex = i;
                break;
            }
        }

        if (isTool && originalToolObj != null)
        {
            GameObject toolToHide = toolDatabase[toolIndex].toolOnGrab;

            toolToHide.SetActive(false);

            originalToolObj.transform.position = holdPos.position;
            originalToolObj.transform.rotation = holdPos.rotation;
            originalToolObj.SetActive(true);

            Rigidbody originalRb = originalToolObj.GetComponent<Rigidbody>();
            originalRb.isKinematic = false;

            heldObj = null;
            originalToolObj = null;
        }
        else
        {
            Physics.IgnoreCollision(
                heldObj.GetComponent<Collider>(),
                player.GetComponent<Collider>(),
                false
            );
            heldObj.layer = 0;
            heldObjRb.isKinematic = false;
            heldObj.transform.parent = null;
            heldObj = null;
        }
    }

    void MoveObject()
    {
        //keep object position the same as the holdPosition position
        heldObj.transform.position = holdPos.transform.position;
    }

    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))
        {
            canDrop = false;

            //disable player being able to look around
            //mouseLookScript.verticalSensitivity = 0f;
            //mouseLookScript.lateralSensitivity = 0f;

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
            //rotate the object depending on mouse X-Y Axis
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            //re-enable player being able to look around
            //mouseLookScript.verticalSensitivity = originalvalue;
            //mouseLookScript.lateralSensitivity = originalvalue;
            canDrop = true;
        }
    }

    void ThrowObject()
    {
        bool isTool = false;
        int toolIndex = -1;

        for (int i = 0; i < toolDatabase.Count; i++)
        {
            if (toolDatabase[i].toolOnView == heldObj)
            {
                isTool = true;
                toolIndex = i;
                break;
            }
        }

        if (isTool && originalToolObj != null)
        {
            GameObject toolToHide = toolDatabase[toolIndex].toolOnGrab;

            toolToHide.SetActive(false);

            // Reposicionar y activar el original
            originalToolObj.transform.position = holdPos.position;
            originalToolObj.transform.rotation = holdPos.rotation;
            originalToolObj.SetActive(true);

            Rigidbody thrownRb = originalToolObj.GetComponent<Rigidbody>();
            thrownRb.isKinematic = false;
            thrownRb.AddForce(transform.forward * throwForce);

            heldObj = null;
            originalToolObj = null; // Limpiar ref
        }
        else
        {
            Physics.IgnoreCollision(
                heldObj.GetComponent<Collider>(),
                player.GetComponent<Collider>(),
                false
            );
            heldObj.layer = 0;
            heldObjRb.isKinematic = false;
            heldObj.transform.parent = null;
            heldObjRb.AddForce(transform.forward * throwForce);
            heldObj = null;
        }
    }

    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(
            transform.position,
            transform.TransformDirection(Vector3.forward),
            clipRange
        );
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }
}
