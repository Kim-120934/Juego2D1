using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Plataform_Script : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;

    private Vector2 targetPos;
    private Rigidbody2D rb;

    private Vector2 lastPosition;
    private Vector2 platformVelocity;

    private HollowKnightMovement playerMovement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPos = posB.position;
        lastPosition = rb.position;
    }

    private void FixedUpdate()
    {
        // Movimiento entre puntos con física
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Cambio de objetivo
        if (Vector2.Distance(rb.position, posA.position) < 0.05f)
            targetPos = posB.position;

        if (Vector2.Distance(rb.position, posB.position) < 0.05f)
            targetPos = posA.position;

        // Velocidad real de la plataforma
        platformVelocity = (rb.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = rb.position;

        // Pasar velocidad al jugador
        if (playerMovement != null)
            playerMovement.platformVelocity = platformVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                playerMovement = collision.gameObject.GetComponent<HollowKnightMovement>();
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerMovement != null)
                playerMovement.platformVelocity = Vector2.zero;

            playerMovement = null;
        }
    }
}