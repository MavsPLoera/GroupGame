using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{
    public void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        if(Time.timeScale == 0) Time.timeScale = 1.0f; // ExitGame button from PauseMenu.
    }
    
    public void startGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void quit()
    {
        Debug.Log("Called application quit");
        Application.Quit();
    }
}
