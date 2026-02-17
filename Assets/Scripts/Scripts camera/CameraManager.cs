using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using static CameraControlTrigger;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineCamera[] _allVirtualCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;


    private CinemachinePositionComposer _positionComposer;
    private CinemachineCamera _currentCamera;
    private float _normYPanAmount;

    private Vector2 _startingTargetOffset;
    private void Awake()
    {
        if (instance == null)
            instance = this;

        // Buscar la cámara activa
        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].isActiveAndEnabled)
            {
                _currentCamera = _allVirtualCameras[i];
                break;
            }
        }

        // Obtener el Position Composer (nuevo equivalente al FramingTransposer)
        _positionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();

        if (_positionComposer != null)
        {
            _normYPanAmount = _positionComposer.Damping.y;
        }
        _startingTargetOffset = _positionComposer.TargetOffset;
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        if (_lerpYPanCoroutine != null)
            StopCoroutine(_lerpYPanCoroutine);

        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = _positionComposer.Damping.y;
        float endDampAmount;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        float elapsedTime = 0f;

        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(
                startDampAmount,
                endDampAmount,
                elapsedTime / _fallYPanTime
            );

            Vector3 damping = _positionComposer.Damping;
            damping.y = lerpedPanAmount;
            _positionComposer.Damping = damping;

            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion
    #region Pan Camera

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        //set the direction and distance if we are panning in the direction indicated by the trigger object
        if (!panToStartingPos)
        {
            //set the direction and distance
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.left;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;

            startingPos = _startingTargetOffset;

            endPos += startingPos;
        }
        //handle the direction settings when moving back to the starting position
        else
        {
            startingPos = _positionComposer.TargetOffset;
            endPos = _startingTargetOffset;
        }

        //handle the actual panning of the camera
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            _positionComposer.TargetOffset = panLerp;

            yield return null;
        }
    }

    #endregion

    #region Swap Cameras
    public void SwapCamera(CinemachineCamera cameraFromLeft, CinemachineCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        //if the current camera is the camera on the left and our trigger exit direction was on the right
        if (_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            //activate the new camera
            cameraFromRight.enabled = true;

            //deactivate the old camera
            cameraFromLeft.enabled = false;

            //set the new camera as the current camera
            _currentCamera = cameraFromRight;

            //update our composer variable
            _positionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
        }

        //if the current camera is the camera on the right and our trigger hit direction was on the left
        else if (_currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            //activate the new camera
            cameraFromLeft.enabled = true;

            //deactivate the old camera
            cameraFromRight.enabled = false;

            //set the new camera as the current camera
            _currentCamera = cameraFromLeft;

            //update our composer variable
            _positionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
        }
    }

    #endregion
}
