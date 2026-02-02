using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    CircleCollisionDetector coll;
    Animator anim;
    SpriteRenderer sr;

    public enum PlayerDirection { Right, Left, Up, Down, RightUp, LeftUp, RightDown, LeftDown }
    public Vector2 playerSpeed;
    public float playerSpeedAmount;
    public Vector2 playerSpeedNormalized;
    [Min(0.01f)]
    public float playerSpeedAmountThreshold = 0.1f;
    Vector3 lastPosition;
    public bool allowDiagonals;
    public PlayerDirection direction;

    private void Start()
    {
        coll = GetComponent<CircleCollisionDetector>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        playerSpeed = transform.position - lastPosition;

        playerSpeedAmount = playerSpeed.magnitude;

        playerSpeedNormalized = playerSpeed.normalized;

        if (playerSpeedAmount > playerSpeedAmountThreshold)
        {
            ComputeDirection();
            sr.flipX = playerSpeed.x < 0;
            if (anim)
            {
                anim.SetBool("isRunning", true);
            }
        }
        else
        {
            if (anim)
            {
                anim?.SetBool("isRunning", false);
            }
        }

        if (anim && coll)
        {
            anim?.SetBool("isGrounded", coll.isColliding);
        }

        lastPosition = transform.position;
    }

    void ComputeDirection()
    {
        if (allowDiagonals)
        {
            if (playerSpeedNormalized.x > 0.5 && playerSpeedNormalized.y > 0.5)
            {
                direction = PlayerDirection.RightUp;
            }
            else if (playerSpeedNormalized.x < -0.5 && playerSpeedNormalized.y > 0.5)
            {
                direction = PlayerDirection.LeftUp;
            }
            else if (playerSpeedNormalized.x > 0.5 && playerSpeedNormalized.y < -0.5)
            {
                direction = PlayerDirection.RightDown;
            }
            else if (playerSpeedNormalized.x < -0.5 && playerSpeedNormalized.y < -0.5)
            {
                direction = PlayerDirection.LeftDown;
            }
            else if (Mathf.Abs(playerSpeedNormalized.x) < Mathf.Abs(playerSpeedNormalized.y))
            {
                if(playerSpeedNormalized.y > 0)
                {
                    direction = PlayerDirection.Up;
                }
                else
                {
                    direction = PlayerDirection.Down;
                }
            }
            else
            {
                if (playerSpeedNormalized.x > 0)
                {
                    direction = PlayerDirection.Right;
                }
                else
                {
                    direction = PlayerDirection.Left;
                }
            }
        }
        else
        {
            if (playerSpeedNormalized.y > 0)
            {
                if (playerSpeedNormalized.x > 0)
                {
                    if (playerSpeedNormalized.y > playerSpeedNormalized.x)
                    {
                        direction = PlayerDirection.Up;
                    }
                    else
                    {
                        direction = PlayerDirection.Right;
                    }
                }
                else
                {
                    if (playerSpeedNormalized.y > -playerSpeedNormalized.x)
                    {
                        direction = PlayerDirection.Up;
                    }
                    else
                    {
                        direction = PlayerDirection.Left;
                    }
                }
            }
            else
            {
                if (playerSpeedNormalized.x > 0)
                {
                    if (-playerSpeedNormalized.y > playerSpeedNormalized.x)
                    {
                        direction = PlayerDirection.Down;
                    }
                    else
                    {
                        direction = PlayerDirection.Right;
                    }
                }
                else
                {
                    if (-playerSpeedNormalized.y > -playerSpeedNormalized.x)
                    {
                        direction = PlayerDirection.Down;
                    }
                    else
                    {
                        direction = PlayerDirection.Left;
                    }
                }
            }
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, playerSpeed * 10, Color.red);
        if (allowDiagonals)
        {
            switch (direction)
            {
                case PlayerDirection.Right:
                    Debug.DrawRay(transform.position, Vector3.right, Color.yellow);
                    break;
                case PlayerDirection.Left:
                    Debug.DrawRay(transform.position, Vector3.left, Color.yellow);
                    break;
                case PlayerDirection.Up:
                    Debug.DrawRay(transform.position, Vector3.up, Color.yellow);
                    break;
                case PlayerDirection.Down:
                    Debug.DrawRay(transform.position, Vector3.down, Color.yellow);
                    break;
                case PlayerDirection.RightUp:
                    Debug.DrawRay(transform.position, (Vector3.right + Vector3.up).normalized, Color.yellow);
                    break;
                case PlayerDirection.LeftUp:
                    Debug.DrawRay(transform.position, (Vector3.left + Vector3.up).normalized, Color.yellow);
                    break;
                case PlayerDirection.RightDown:
                    Debug.DrawRay(transform.position, (Vector3.right + Vector3.down).normalized, Color.yellow);
                    break;
                case PlayerDirection.LeftDown:
                    Debug.DrawRay(transform.position, (Vector3.left + Vector3.down).normalized, Color.yellow);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (direction)
            {
                case PlayerDirection.Right:
                    Debug.DrawRay(transform.position, Vector3.right, Color.yellow);
                    break;
                case PlayerDirection.Left:
                    Debug.DrawRay(transform.position, Vector3.left, Color.yellow);
                    break;
                case PlayerDirection.Up:
                    Debug.DrawRay(transform.position, Vector3.up, Color.yellow);
                    break;
                case PlayerDirection.Down:
                    Debug.DrawRay(transform.position, Vector3.down, Color.yellow);
                    break;
            }
        }
    }
}
