using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerTopdownCardinalMovement : MonoBehaviour
{
    Rigidbody2D rb;
    InputSystem_Actions inputs;

    public float speedGrounded = 5;
    public bool allowDiagonal = true;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputs = new InputSystem_Actions();
        inputs.Enable();
    }
    private void FixedUpdate()
    {
        Vector2 move = inputs.Player.Move.ReadValue<Vector2>().normalized;
        if (allowDiagonal)
        {
            if(move.x > 0.5 && move.y > 0.5)
            {
                move = (Vector2.up + Vector2.right);
            }
            else if (move.x < -0.5 && move.y > 0.5)
            {
                move = (Vector2.up + Vector2.left);
            }
            else if (move.x > 0.5 && move.y < -0.5)
            {
                move = (Vector2.down + Vector2.right);
            }
            else if (move.x < -0.5 && move.y < -0.5)
            {
                move = (Vector2.down + Vector2.left);
            }
            else if (Mathf.Abs(move.x) < Mathf.Abs(move.y))
            {
                move.x = 0;
            }
            else
            {
                move.y = 0;
            }
        }
        else
        {
            if(Mathf.Abs(move.x) < Mathf.Abs(move.y))
            {
                move.x = 0;
            }
            else
            {
                move.y = 0;
            }
        }
        move.Normalize();
        rb.linearVelocity = move * speedGrounded;
    }
}
