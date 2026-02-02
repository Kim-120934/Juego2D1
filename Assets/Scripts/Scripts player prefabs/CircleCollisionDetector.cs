 using UnityEngine;

public class CircleCollisionDetector : MonoBehaviour
{
    public float radius = 0.4f;
    public Vector2 offset = new Vector2(0, -0.7f);
    public LayerMask mask = ~0;
    public bool isColliding;
    bool wasColliding;
    public bool startedCollidingThisFrame
    {
        get { return isColliding && !wasColliding; }
    }
    public bool stoppedCollidingThisFrame
    {
        get { return !isColliding && wasColliding; }
    }

    private void Update()
    {
        wasColliding = isColliding;
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position + (Vector3)offset, radius, mask);
        if (collisions.Length > 0)
        {
            isColliding = true;
        }
        else
        {
            isColliding = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (isColliding)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(transform.position + (Vector3)offset, radius);
    }
}
