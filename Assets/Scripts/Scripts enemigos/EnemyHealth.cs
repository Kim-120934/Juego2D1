using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    
    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 8f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public bool IsDead { get; internal set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        currentHealth -= damage;
        
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Vida restante: {currentHealth}/{maxHealth}");
        
        // Aplicar knockback
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
        
        // Efecto visual de daño (parpadeo rápido)
        StartCoroutine(DamageFlash());
        AudioManager.instance.PlaySFX(AudioManager.instance.hitEnemySFX);

        // Verificar muerte
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private System.Collections.IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    [Header("Coins")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins = 2;
    [SerializeField] private int maxCoins = 10;

    private void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto!");

        HollowKnightMovement player = FindFirstObjectByType<HollowKnightMovement>();
        AudioManager.instance.PlaySFX(AudioManager.instance.killEnemySFX);
        if (player != null)
            player.GainSoul(1);

        // Soltar monedas
        int coinAmount = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < coinAmount; i++)
        {
            if (coinPrefab != null)
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

