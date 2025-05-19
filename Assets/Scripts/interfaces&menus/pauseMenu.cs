using UnityEngine;
using UnityEngine.SceneManagement;
public class pause : MonoBehaviour
{
    public GameObject PauseMenu;
    public bool paused = false;
    void Start()
    {
        
    }
    public void Pause()
    {
        PauseMenu.SetActive(true);
        paused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void Resume()
    {
        PauseMenu.SetActive(false);
        paused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused == false)
            {
                Pause();
            }

            else
            {
                Resume();
            }
        }
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}

