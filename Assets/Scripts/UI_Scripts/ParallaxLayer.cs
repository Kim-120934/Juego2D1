using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float parallaxSpeed = 0.5f;
    [SerializeField] private bool infiniteHorizontal = true;

    private Transform _cam;
    private Vector3 _lastCamPos;
    private float _spriteWidth;

    private void Start()
    {
        _cam = Camera.main.transform;
        _lastCamPos = _cam.position;
        _spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        Vector3 delta = _cam.position - _lastCamPos;
        transform.position += new Vector3(delta.x * parallaxSpeed, delta.y * parallaxSpeed, 0);
        _lastCamPos = _cam.position;

        if (infiniteHorizontal)
        {
            float distX = _cam.position.x - transform.position.x;
            if (Mathf.Abs(distX) >= _spriteWidth)
                transform.position += new Vector3(Mathf.Sign(distX) * _spriteWidth, 0, 0);
        }
    }
}