using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollisionDetector))]
public class PlayerJump : MonoBehaviour
{
    public float force = 20;
    public int jumpCount;
    Rigidbody2D rb;
    CircleCollisionDetector coll;
    InputSystem_Actions inputs;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollisionDetector>();
        inputs = new InputSystem_Actions();
        inputs.Enable();
    }

    void Update()
    {
        if (coll.startedCollidingThisFrame)
        {
            jumpCount = 0;
        }
        if (coll.isColliding && inputs.Player.Jump.WasPressedThisFrame())
        {
            Jump();
        }
    }
    public void Jump()
    {
        jumpCount++;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force );
    }
}
