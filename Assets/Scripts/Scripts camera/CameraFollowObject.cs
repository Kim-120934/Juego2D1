using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYrotationTime = 0.5f;

    private Coroutine _turnCoroutine;

    private HollowKnightMovement _player;

    private bool _isFacingRight;

    public void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<HollowKnightMovement>();
        _isFacingRight = _player.IsFacingRight;

    }
    private void Update()
    {
        transform.position = _playerTransform.position;

    }

    public void CallTurn()
    {
        LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYrotationTime).setEaseInOutSine();
    }
    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < _flipYrotationTime)
        {
            elapsedTime += Time.deltaTime;
            yRotation = Mathf.Lerp(startRotation, endRotationAmount, (elapsedTime / _flipYrotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            yield return null;

        }
    }

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;
        if (_isFacingRight)
        {
            return 180f;
        }

        else
        {
            return 180f;
        }
    }
}