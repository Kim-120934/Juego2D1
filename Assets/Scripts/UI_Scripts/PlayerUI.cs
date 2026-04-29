using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Corazones")]
    public Image[] hearts;
    public Sprite heartFull;
    public Sprite heartEmpty;

    [Header("Frasco de alma")]
    public Image soulFlask;
    public Sprite[] soulSprites; // 0 = vacío, 4 = lleno

    private HollowKnightMovement _player;

    private void Start()
    {
        _player = FindFirstObjectByType<HollowKnightMovement>();
    }

    private void Update()
    {
        UpdateHearts();
        UpdateSoul();
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < _player.currentHits ? heartFull : heartEmpty;
        }
    }

    private void UpdateSoul()
    {
        soulFlask.sprite = soulSprites[_player.currentSoul];
    }
}