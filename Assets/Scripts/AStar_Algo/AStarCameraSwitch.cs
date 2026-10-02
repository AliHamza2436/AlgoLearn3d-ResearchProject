using System;
using UnityEngine;

public class AStarCameraSwitch : MonoBehaviour
{
    public Camera mainPlayerCamera;
    public Camera topDownCamera;
    public static event Action<Camera> OnCameraSwitched;
    private bool isTopDown = true;

    private void Start()
    {
        if (topDownCamera != null) topDownCamera.gameObject.SetActive(false);
        if (mainPlayerCamera != null) mainPlayerCamera.gameObject.SetActive(true);

        OnCameraSwitched?.Invoke(mainPlayerCamera);
    }

    public void ToggleCameraView()
    {
        isTopDown = !isTopDown;

        if (mainPlayerCamera != null) mainPlayerCamera.gameObject.SetActive(!isTopDown);
        if (topDownCamera != null) topDownCamera.gameObject.SetActive(isTopDown);

        Camera activeCam = isTopDown ? topDownCamera : mainPlayerCamera;
        OnCameraSwitched?.Invoke(activeCam);
    }

    public void TriggerCameraSwitchEvent(Camera newCam)
    {
        OnCameraSwitched?.Invoke(newCam);
    }
}