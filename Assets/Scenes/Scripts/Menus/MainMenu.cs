using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene("IntroScene");
    }

    //for open settings, see PauseMenu script

    public void QuitGame()
    {
        Debug.Log ("Quitting game...");
        Application.Quit();
    }
}
