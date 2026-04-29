using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuParticles : MonoBehaviour
{
    [Header("Particulas")]
    [SerializeField] private int particleCount = 30;
    [SerializeField] private float minSize = 5f;
    [SerializeField] private float maxSize = 20f;
    [SerializeField] private float minSpeed = 20f;
    [SerializeField] private float maxSpeed = 60f;
    [SerializeField] private Vector2 direction = new Vector2(0.5f, 1f);
    [SerializeField] private Sprite circleSprite;

    private List<RectTransform> _particles = new List<RectTransform>();
    private List<float> _speeds = new List<float>();
    private RectTransform _canvasRect;

    private void Start()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            _canvasRect = canvas.GetComponent<RectTransform>();
            Debug.Log("Canvas encontrado: " + canvas.name);
        }
        else
        {
            Debug.Log("Canvas NO encontrado");
        }

        Debug.Log("Creando " + particleCount + " particulas");
        for (int i = 0; i < particleCount; i++)
            CreateParticle();
        Debug.Log("Particulas creadas: " + _particles.Count);
    }

    private void CreateParticle()
    {
        GameObject obj = new GameObject("Particle");
        obj.transform.SetParent(transform, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        float size = Random.Range(minSize, maxSize);
        rt.sizeDelta = new Vector2(size, size);
        rt.anchoredPosition = new Vector2(
            Random.Range(-960f, 960f),
            Random.Range(-540f, 540f)
        );

        Image img = obj.AddComponent<Image>();
        img.sprite = circleSprite;
        float alpha = Random.Range(0.1f, 0.5f);
        img.color = new Color(0.4f, 0.9f, 0.5f, alpha);

        float speed = Random.Range(minSpeed, maxSpeed);
        _particles.Add(rt);
        _speeds.Add(speed);

        StartCoroutine(PulseParticle(img));
    }

    private IEnumerator PulseParticle(Image img)
    {
        float minAlpha = Random.Range(0.05f, 0.2f);
        float maxAlpha = Random.Range(0.3f, 0.6f);
        float speed = Random.Range(0.5f, 2f);

        while (true)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha,
                (Mathf.Sin(Time.time * speed) + 1f) / 2f);
            Color c = img.color;
            c.a = alpha;
            img.color = c;
            yield return null;
        }
    }

    private void Update()
    {
        Vector2 dir = direction.normalized;
        for (int i = 0; i < _particles.Count; i++)
        {
            _particles[i].anchoredPosition += dir * _speeds[i] * Time.deltaTime;

            Vector2 pos = _particles[i].anchoredPosition;
            if (pos.y > 540f) pos.y = -540f;
            if (pos.y < -540f) pos.y = 540f;
            if (pos.x > 960f) pos.x = -960f;
            if (pos.x < -960f) pos.x = 960f;
            _particles[i].anchoredPosition = pos;
        }
    }
}