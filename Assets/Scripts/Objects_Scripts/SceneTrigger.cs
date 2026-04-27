using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string nextScene;
    [SerializeField] private string spawnPointID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSpawnManager.nextSpawnID = spawnPointID;
            if (SaveManager.instance != null)
                SaveManager.instance.SaveGame(SlotMenu.CurrentSlot);
            SceneTransition.instance.LoadScene(nextScene);
        }
    }
}