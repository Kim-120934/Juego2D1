using UnityEngine;

public class Spawn_Point_Script : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HollowKnightMovement player = other.GetComponent<HollowKnightMovement>();
            if (player != null)
            {
                player.SetRespawnPoint(transform);
                Debug.Log("Checkpoint activado: " + gameObject.name +
                          " POS: " + transform.position);

                if (SaveManager.instance != null)
                    SaveManager.instance.SaveGame(SlotMenu.CurrentSlot);
            }
        }
    }
}