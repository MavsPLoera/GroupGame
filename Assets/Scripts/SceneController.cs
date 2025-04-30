using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{
    public void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1.0f;
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
