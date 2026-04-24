using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private string nextScene;

    public void OpenDoor()
    {
        if (!string.IsNullOrEmpty(nextScene))
            SceneTransition.instance.LoadScene(nextScene);
        else
            Destroy(gameObject);
    }
}