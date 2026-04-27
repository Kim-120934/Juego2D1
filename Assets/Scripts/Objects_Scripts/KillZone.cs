using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HollowKnightMovement player = other.GetComponent<HollowKnightMovement>();
            if (player != null)
            {
                player.currentHits--;
                if (player.currentHits <= 0)
                    player.LoseLife();
                else
                    player.Respawn();
            }
        }
    }
}