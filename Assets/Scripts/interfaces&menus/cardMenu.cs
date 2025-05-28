using UnityEngine;
using System.Collections;
public class cardMenu : MonoBehaviour
{
    public GameObject CardMenu;
    public bool shown = false;
    void Start()
    {

    }
    public void Show()
    {
        CardMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        shown = true;
    }
    public void Hide()
    {
        CardMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        shown = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (shown == false)
            {
                Show();
            }

            else
            {
                Hide();
            }
        }
    }

    public void UseCard()
    {
    
    }

}
