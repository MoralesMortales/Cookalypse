using UnityEngine;

public class PickUp_tool : MonoBehaviour
{

public GameObject ToolOnPlayer;
public GameObject PickUpText;

    void Start()
    {
    ToolOnPlayer.SetActive(false);
    PickUpText.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PickUpText.SetActive(true);

            if (Input.GetKey(KeyCode.E))
            {
                this.gameObject.SetActive(false);
                ToolOnPlayer.SetActive(true);
                PickUpText.SetActive(false);

            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        PickUpText.SetActive(false);
    }

}
