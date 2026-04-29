using UnityEngine;

public class Spawn_Trans : MonoBehaviour
{
    [SerializeField] private string spawnID;

    private void Start()
    {
        Invoke("ApplySpawn", 0.1f);
    }

    void ApplySpawn()
    {
        if (SaveManager.instance != null &&
            !string.IsNullOrEmpty(SaveManager.instance.nextSpawnID) &&
            SaveManager.instance.nextSpawnID == spawnID)
        {
            var player = FindFirstObjectByType<HollowKnightMovement>();

            if (player != null)
            {
                player.transform.position = transform.position;


                player.SetRespawnPoint(transform);

                SaveManager.instance.nextSpawnID = "";
            }
        }
    }
}
