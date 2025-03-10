using UnityEngine;

public class Camera_Holder : MonoBehaviour
{

    public Transform cameraPosition;


    // Update is called once per frame
    private void Update()
    {
        transform.position = cameraPosition.position;
    }
}
