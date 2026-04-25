using UnityEngine;

public class CoinEffect : MonoBehaviour
{
    [SerializeField] private int particleCount = 8;
    [SerializeField] private float particleSize = 0.1f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float lifetime = 0.4f;
    [SerializeField] private Color particleColor = new Color(1f, 0.85f, 0f, 1f);
    [SerializeField] private Sprite particleSprite;

    public void Play()
    {
        for (int i = 0; i < particleCount; i++)
        {
            GameObject p = new GameObject("CoinParticle");
            p.transform.position = transform.position;

            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = particleSprite;
            sr.color = particleColor;

            Rigidbody2D rb = p.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0.5f;

            float angle = (360f / particleCount) * i;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            rb.AddForce(dir * speed, ForceMode2D.Impulse);

            p.transform.localScale = Vector3.one * particleSize;
            Destroy(p, lifetime);
        }
        Destroy(gameObject, lifetime);
    }
}