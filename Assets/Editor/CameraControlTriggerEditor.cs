using UnityEditor;
using UnityEngine;
using Unity.Cinemachine;

[CustomEditor(typeof(CameraControlTrigger))]
public class CameraControlTriggerEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;

    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (cameraControlTrigger.customInspectorObjects.swapCameras)
        {
            cameraControlTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField(
                "Camera on Left",
                cameraControlTrigger.customInspectorObjects.cameraOnLeft,
                typeof(CinemachineCamera), true) as CinemachineCamera;

            cameraControlTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField(
                "Camera on Right",
                cameraControlTrigger.customInspectorObjects.cameraOnRight,
                typeof(CinemachineCamera), true) as CinemachineCamera;
        }

        if (cameraControlTrigger.customInspectorObjects.panCameraOnContact)
        {
            cameraControlTrigger.customInspectorObjects.panDirection = (CameraControlTrigger.PanDirection)EditorGUILayout.EnumPopup(
                "Camera Pan Direction",
                cameraControlTrigger.customInspectorObjects.panDirection);

            cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField(
                "Pan Distance",
                cameraControlTrigger.customInspectorObjects.panDistance);

            cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField(
                "Pan Time",
                cameraControlTrigger.customInspectorObjects.panTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}