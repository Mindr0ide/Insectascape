using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour

{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        SceneManager.LoadScene("AfterBridge");
    }
}
