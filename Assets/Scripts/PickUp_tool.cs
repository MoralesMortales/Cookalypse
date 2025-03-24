using UnityEngine;

public class PickUp_tool : MonoBehaviour
{

public GameObject ToolOnPlayer;

    void Start()
    {
    ToolOnPlayer.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKey(KeyCode.E))
            {
                this.gameObject.SetActive(false);
                ToolOnPlayer.SetActive(true);
            }
        }
    }

}
