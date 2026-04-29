using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string nextScene;
    [SerializeField] private string spawnPointID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.nextSpawnID = spawnPointID;
            SaveManager.instance.SaveGame(SlotMenu.CurrentSlot);
        }

        SceneTransition.instance.LoadScene(nextScene);
    }
}