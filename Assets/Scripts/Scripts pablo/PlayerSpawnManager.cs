using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static string nextSpawnID = "";

    [SerializeField] private string spawnID;
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        if (nextSpawnID == spawnID && nextSpawnID != "")
        {
            HollowKnightMovement player = FindObjectOfType<HollowKnightMovement>();
            if (player != null)
                player.transform.position = spawnPoint.position;

            nextSpawnID = "";
        }
    }
}