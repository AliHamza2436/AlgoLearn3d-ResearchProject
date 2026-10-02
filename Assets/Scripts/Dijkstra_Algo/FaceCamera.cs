using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera targetCamera;

    private void OnEnable()
    {
        AStarCameraSwitch.OnCameraSwitched += SetTargetCamera;
    }

    private void OnDisable()
    {
        AStarCameraSwitch.OnCameraSwitched -= SetTargetCamera;
    }

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    public void SetTargetCamera(Camera newCamera)
    {
        targetCamera = newCamera;
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;
        transform.rotation = targetCamera.transform.rotation;
    }
}