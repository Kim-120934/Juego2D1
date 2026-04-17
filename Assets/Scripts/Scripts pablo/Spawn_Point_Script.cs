using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HollowKnightMovement player = other.GetComponent<HollowKnightMovement>();

            if (player != null)
            {
                player.SetRespawnPoint(transform);

                Debug.Log(" Checkpoint activado: " + gameObject.name +
                          " POS: " + transform.position);

            }
        }
    }
}