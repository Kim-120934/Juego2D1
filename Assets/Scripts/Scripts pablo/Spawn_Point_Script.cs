using UnityEngine;

public class Spawn_Point_Script: MonoBehaviour
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

                // Guardar automaticamente al activar checkpoint
                // El slot se elegira desde el menu, por ahora guardamos en slot 0
                if (SaveManager.instance != null)
                    Debug.Log("Guardando en slot: " + SlotMenu.CurrentSlot);
                SaveManager.instance.SaveGame(SlotMenu.CurrentSlot);
            }
        }
    }
}