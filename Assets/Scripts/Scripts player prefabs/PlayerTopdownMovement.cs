using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerTopdownMovement : MonoBehaviour
{
    Rigidbody2D rb;
    InputSystem_Actions inputs;

    public float speedGrounded = 5;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputs = new InputSystem_Actions();
        inputs.Enable();
    }
    private void FixedUpdate()
    {
        Vector2 move = inputs.Player.Move.ReadValue<Vector2>();

        rb.linearVelocity = move * speedGrounded;
    }
}
