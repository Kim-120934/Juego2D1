using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float lifetime = 8f;
    [SerializeField] private float attractSpeed = 5f;
    [SerializeField] private GameObject coinEffectPrefab;
    private bool _collected = false;
    private Transform _player;

    private void Start()
    {
        _player = FindFirstObjectByType<HollowKnightMovement>().transform;
        Destroy(gameObject, lifetime);

        // Pequeño impulso aleatorio al aparecer
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomForce = new Vector2(
                Random.Range(-3f, 3f),
                Random.Range(3f, 6f)
            );
            rb.AddForce(randomForce, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (other.CompareTag("Player"))
        {
            _collected = true;
            CoinManager.instance.AddCoins(value);
            AudioManager.instance.PlaySFX(AudioManager.instance.coinPickupSFX);
            if (coinEffectPrefab != null)
            {
                GameObject effect = Instantiate(coinEffectPrefab, transform.position, Quaternion.identity);
                effect.GetComponent<CoinEffect>().Play();
            }
            Destroy(gameObject);
        }
    }
}
