using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public Image[] segments; // 15 bloques
    public HollowKnightMovement player;

    void Update()
    {
        int totalHits = player.currentLives * player.maxHitsPerLife
                        + player.currentHits - player.maxHitsPerLife;

        int maxHits = player.maxLives * player.maxHitsPerLife;

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].enabled = i < totalHits;
        }
    }
}
