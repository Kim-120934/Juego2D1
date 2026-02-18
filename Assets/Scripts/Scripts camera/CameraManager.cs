using System.Collections;
using System.Collections.Generic;
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

    // Diccionario para guardar el offset inicial de cada cámara
    private Dictionary<CinemachineCamera, Vector3> _cameraStartingOffsets = new Dictionary<CinemachineCamera, Vector3>();

    private void Awake()
    {
        if (instance == null)
            instance = this;

        // Guardar el offset inicial de TODAS las cámaras
        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            var cam = _allVirtualCameras[i];
            var composer = cam.GetComponent<CinemachinePositionComposer>();

            if (composer != null)
            {
                _cameraStartingOffsets[cam] = composer.TargetOffset;
            }

            if (cam.isActiveAndEnabled)
            {
                _currentCamera = cam;
                _positionComposer = composer;

                if (_positionComposer != null)
                {
                    _normYPanAmount = _positionComposer.Damping.y;
                }
            }
        }
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
        Vector3 endPos = Vector3.zero;
        Vector3 startingPos = Vector3.zero;

        //set the direction and distance if we are panning in the direction indicated by the trigger object
        if (!panToStartingPos)
        {
            //set the direction and distance
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector3.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector3.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector3.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector3.right;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;

            // Usar el offset inicial de la cámara actual
            startingPos = _cameraStartingOffsets[_currentCamera];

            endPos += startingPos;
        }
        //handle the direction settings when moving back to the starting position
        else
        {
            startingPos = _positionComposer.TargetOffset;
            endPos = _cameraStartingOffsets[_currentCamera];
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