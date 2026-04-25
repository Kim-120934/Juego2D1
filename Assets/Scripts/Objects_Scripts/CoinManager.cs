using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Image coinIcon;

    private int _totalCoins = 0;

    private void Awake()
    {
        instance = this;
    }

    public void AddCoins(int amount)
    {
        _totalCoins += amount;
        coinText.text = _totalCoins.ToString();
        Debug.Log($"Monedas: {_totalCoins}");
    }

    public int GetCoins() => _totalCoins;

    public bool SpendCoins(int amount)
    {
        if (_totalCoins < amount) return false;
        _totalCoins -= amount;
        coinText.text = _totalCoins.ToString();
        return true;
    }
}