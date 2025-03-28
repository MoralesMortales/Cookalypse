using UnityEngine;

public class door : MonoBehaviour
{
    public GameObject doorOpened;
    public GameObject doorClosed;
    public GameObject PickUpText;

    private bool isOpen = false; // Add a boolean to track the door's state

    void Start()
    {
        doorClosed.SetActive(true);
        doorOpened.SetActive(false);
        PickUpText.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PickUpText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Q)) // Changed to GetKeyDown for single press
            {
                isOpen = !isOpen; // Toggle the door's state

                if (isOpen)
                {
                    doorOpened.SetActive(true);
                    doorClosed.SetActive(false);
                }
                else
                {
                    doorOpened.SetActive(false);
                    doorClosed.SetActive(true);
                }

                PickUpText.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PickUpText.SetActive(false);
    }
}