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

    [System.Serializable]
    public class ToolData
    {
        public GameObject toolOnView;
        public GameObject toolOnGrab;
    }

    public List<ToolData> toolDatabase = new List<ToolData>();

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
                                    toolDatabase[i].toolOnView.SetActive(false);
                                    toolDatabase[i].toolOnGrab.SetActive(true);
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
        if (heldObj != null) //if player is holding object
        {
            MoveObject(); //keep object position at holdPos
            RotateObject();
            Debug.Log("q tienes ahi?");
            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop == true) //Mous0 (leftclick) is used to throw, change this if you want another button to be used)
            {

                Debug.Log("Using");
                StopClipping();
                ThrowObject();
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        AssignMultipleTags objTags = pickUpObj.GetComponent<AssignMultipleTags>();

        // if (objTags.HasTag("tool"))
        // {
        //     Debug.Log("--> " + pickUpObj);
        //     heldObj = pickUpObj;
        // }
         if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {
            Debug.Log("It has");
            Debug.Log("-->sd " + pickUpObj);
            heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
            heldObj.layer = LayerNumber; //change the object layer to the holdLayer
            //make sure object doesnt collide with player, it can cause weird bugs
            Physics.IgnoreCollision(
                heldObj.GetComponent<Collider>(),
                player.GetComponent<Collider>(),
                true
            );
        }
        else
        {
            //Debug.Log("It doesnt have RGBD");
        }
    }

    void DropObject()
    {
        bool isTool = false;
        int toolIndex = -1;

        for (int i = 0; i < toolDatabase.Count; i++)
        {
            if (toolDatabase[i].toolOnGrab == heldObj)
            {
                isTool = true;
                toolIndex = i;
                break;
            }
        }

        if (isTool)
        {
            Debug.Log("trying to throw");
            // For tools, we don't actually drop the toolOnGrab, we just switch back
            toolDatabase[toolIndex].toolOnGrab.SetActive(false);
            toolDatabase[toolIndex].toolOnView.SetActive(true);

            // Reset the tool's position/rotation
            toolDatabase[toolIndex].toolOnGrab.transform.localPosition = Vector3.zero;
            toolDatabase[toolIndex].toolOnGrab.transform.localRotation = Quaternion.identity;

            heldObj = null;
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
                Debug.Log("is tool frend");
                toolIndex = i;
                break;
            }
            else
            {
                Debug.Log("not tool");
            }
        }

        if (isTool)
        {
        //     // For tools, we don't throw the toolOnGrab, we switch back and throw the toolOnView
        //     toolDatabase[toolIndex].toolOnGrab.SetActive(false);
        //     GameObject toolToThrow = toolDatabase[toolIndex].toolOnView;
        //     toolToThrow.SetActive(true);

        //     // Position the tool where the held version was
        //     toolToThrow.transform.position = heldObj.transform.position;
        //     toolToThrow.transform.rotation = heldObj.transform.rotation;

        //     // Get its rigidbody and throw it
        //     Rigidbody thrownRb = toolToThrow.GetComponent<Rigidbody>();
        //     thrownRb.isKinematic = false;
        //     thrownRb.AddForce(transform.forward * throwForce);

        //     // Reset the held version
        //     heldObj.transform.localPosition = Vector3.zero;
        //     heldObj.transform.localRotation = Quaternion.identity;
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
