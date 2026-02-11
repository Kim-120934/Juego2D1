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
    
    private void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto!");
        
        // Aquí puedes añadir:
        // - Animación de muerte
        // - Partículas
        // - Sonido
        // - Drop de items/alma
        
        Destroy(gameObject);
    }
}

