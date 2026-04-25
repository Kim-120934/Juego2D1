using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenOptions()
    {
        Debug.Log("Options");
    }

    public void OpenCredits()
    {
        Debug.Log("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}