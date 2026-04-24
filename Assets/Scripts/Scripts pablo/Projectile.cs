using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    private Vector2 _direction;

    public void Init(Vector2 direction)
    {
        _direction = direction;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(_direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Door"))
        {
            Door door = other.GetComponent<Door>();
            if (door != null)
                door.OpenDoor();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Destructible"))
        {
            DestructibleWall wall = other.GetComponent<DestructibleWall>();
            if (wall != null)
                wall.DestroyWall();
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(1, _direction);
            Destroy(gameObject);
        }
    }
}