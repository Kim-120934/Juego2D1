using UnityEngine;
using UnityEngine.UI;

public class GlowPulse : MonoBehaviour
{
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 0.6f;
    [SerializeField] private float speed = 1.5f;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
        Color c = _image.color;
        c.a = alpha;
        _image.color = c;
    }
}