using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollisionDetector))]
public class PlayerHorizontalMovement : MonoBehaviour
{
    Rigidbody2D rb;
    CircleCollisionDetector coll;
    InputSystem_Actions inputs;

    public float speedGrounded = 5;
    public float speedAir = 10;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollisionDetector>();
        inputs = new InputSystem_Actions();
        inputs.Enable();
    }
    private void FixedUpdate()
    {
        Vector2 move = inputs.Player.Move.ReadValue<Vector2>();

        if (coll.isColliding)
        {
            rb.linearVelocity = new Vector2 (move.x * speedGrounded, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity += new Vector2(move.x * speedAir * Time.fixedDeltaTime, 0);
        }
    }
}
