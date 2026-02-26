using UnityEngine;

public class EnemyPatrol : MonoBehaviour

{
    [Header("Patrol")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitTimeAtPoint = 1f;
    
    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Combat")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float damageInterval = 1f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth enemyHealth;
    
    // State
    private enum EnemyState { Patrol, Chase, Attack }
    private EnemyState currentState = EnemyState.Patrol;
    
    // Patrol
    private Transform currentTarget;
    private float waitTimer;
    private bool isWaiting;
    
    // Chase
    private Transform player;
    private float lastDamageTime;
    
    // Movement
    private bool facingRight = true;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
    }
    
    private void Start()
    {
        // Empezar patrullando hacia el punto A
        currentTarget = pointA;
        
        // Buscar al jugador en la escena
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }
    
    private void Update()
    {
        // Si el enemigo está muerto, no hacer nada
        if (enemyHealth != null && enemyHealth.IsDead)
            return;
        
        // Detectar al jugador
        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;
        
        // Máquina de estados
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                
                // Si el jugador está cerca, cambiar a Chase
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = EnemyState.Chase;
                    isWaiting = false;
                }
                break;
                
            case EnemyState.Chase:
                Chase();
                
                // Si el jugador está en rango de ataque
                if (distanceToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attack;
                }
                // Si el jugador se aleja mucho, volver a patrullar
                else if (distanceToPlayer > detectionRange * 1.5f)
                {
                    currentState = EnemyState.Patrol;
                }
                break;
                
            case EnemyState.Attack:
                Attack();
                
                // Si el jugador se aleja, volver a perseguir
                if (distanceToPlayer > attackRange * 1.2f)
                {
                    currentState = EnemyState.Chase;
                }
                break;
        }
    }

    private void Patrol()
    {
        if (isWaiting)
        {
            // Asegurar que está completamente detenido mientras espera
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                // Cambiar al otro punto
                currentTarget = (currentTarget == pointA) ? pointB : pointA;
            }
            return;
        }

        // Calcular distancia al objetivo
        float distance = Vector2.Distance(transform.position, currentTarget.position);

        // Si está muy cerca, detenerse completamente
        if (distance < 0.3f)  // Aumentado de 0.2f a 0.3f
        {
            isWaiting = true;
            waitTimer = waitTimeAtPoint;
            rb.linearVelocity = Vector2.zero;  // Detener completamente

            // Opcional: Snapear a la posición exacta del punto
            Vector3 targetPos = currentTarget.position;
            transform.position = new Vector3(targetPos.x, transform.position.y, transform.position.z);

            return;
        }

        // Moverse hacia el punto objetivo
        MoveTowards(currentTarget.position, patrolSpeed);
    }

    private void Chase()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }
        
        // Perseguir al jugador
        MoveTowards(player.position, chaseSpeed);
    }
    
    private void Attack()
    {
        // Detenerse cuando está atacando
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        
        // Mirar hacia el jugador
        if (player != null)
        {
            bool shouldFaceRight = player.position.x > transform.position.x;
            if (shouldFaceRight != facingRight)
                Flip();
        }
    }
    
    private void MoveTowards(Vector2 targetPosition, float speed)
    {
        // Verificar si hay suelo delante (para no caer de plataformas)
        if (!IsGroundAhead())
        {
            // Si no hay suelo, detenerse o dar la vuelta
            if (currentState == EnemyState.Patrol)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }
        }
        
        // Calcular dirección
        float direction = Mathf.Sign(targetPosition.x - transform.position.x);

        // Aplicar velocidad
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        
        // Flip del sprite según dirección
        bool shouldFaceRight = direction > 0;
        if (shouldFaceRight != facingRight)
            Flip();
    }
    
    private bool IsGroundAhead()
    {
        // Raycast hacia abajo y adelante para detectar suelo
        Vector2 rayOrigin = (Vector2)groundCheck.position + Vector2.right * (facingRight ? 0.5f : -0.5f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
        
        return hit.collider != null;
    }
    
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Detectar colisión con el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            DamagePlayer(collision.gameObject);
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Daño continuo mientras está tocando al jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastDamageTime + damageInterval)
            {
                DamagePlayer(collision.gameObject);
            }
        }
    }
    
    private void DamagePlayer(GameObject playerObj)
    {
        HollowKnightMovement playerMovement = playerObj.GetComponent<HollowKnightMovement>();
        if (playerMovement != null)
        {
            // Aplicar daño al jugador
            playerMovement.TakeDamage(contactDamage, transform.position);
            lastDamageTime = Time.time;
            
            Debug.Log($"{gameObject.name} hizo {contactDamage} de daño al jugador");
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualizar rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Visualizar rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Visualizar puntos de patrullaje
        if (pointA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pointA.position, 0.3f);
        }
        
        if (pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(pointB.position, 0.3f);
        }
        
        // Línea entre puntos de patrullaje
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
        
        // Ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.magenta;
            Vector2 rayOrigin = (Vector2)groundCheck.position + Vector2.right * (facingRight ? 0.5f : -0.5f);
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * groundCheckDistance);
        }
    }
}