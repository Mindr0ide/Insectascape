using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene("AfterBridge");
    }

    public void OpenSettings()
    {
        Debug.Log("Opening settings...");
        //TODO settings
    }

    public void QuitGame()
    {
        Debug.Log ("Quitting game...");
        Application.Quit();
    }
}
