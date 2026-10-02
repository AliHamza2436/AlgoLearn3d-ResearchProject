using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SmoothDragCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset = new Vector3(0f, 1.4f, 0f);
    public float defaultDistance = 3.0f;
    public float minDistance = 1.0f;

    [Range(0.05f, 1.0f)]
    public float touchRotationSpeed = 0.18f;

    [Range(0.05f, 1.0f)]
    public float mouseRotationSpeed = 0.15f;
    public float rotationDamping = 12.0f;


    public float minPitchAngle = -5f;
    public float maxPitchAngle = 55f;
    public float defaultPitchAngle = 20f;

    public LayerMask collisionLayers = ~0;
    public float cameraCollisionRadius = 0.25f;
    public float collisionDamping = 15.0f;

    private const string SensitivityPrefKey = "TouchSensitivity";

    private float currentYaw;
    private float currentPitch;
    private float targetYaw;
    private float targetPitch;
    private float currentDistance;
    private float targetDistance;

    private Vector2 previousPointerPosition;
    private bool isInteracting;
    private int activeTouchId = -1;

    private void OnEnable() => EnhancedTouchSupport.Enable();
    private void OnDisable() => EnhancedTouchSupport.Disable();

    private void Start()
    {
        if (target != null) targetYaw = target.eulerAngles.y;
        targetPitch = defaultPitchAngle;
        currentYaw = targetYaw;
        currentPitch = targetPitch;

        currentDistance = defaultDistance;
        targetDistance = defaultDistance;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        ProcessInput();

        currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime * rotationDamping);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * rotationDamping);

        Quaternion currentRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 focusPoint = target.position + targetOffset;

        ResolveCameraCollision(focusPoint, currentRotation);
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * collisionDamping);

        transform.rotation = currentRotation;
        transform.position = focusPoint - (currentRotation * Vector3.forward * currentDistance);
    }

    private void ProcessInput()
    {
        float sensitivity = PlayerPrefs.GetFloat(SensitivityPrefKey, 0.18f) / 0.18f;

        if (Touch.activeTouches.Count > 0)
        {
            bool touchFound = false;

            foreach (var touch in Touch.activeTouches)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.touchId))
                    continue;

                if (!isInteracting && touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    activeTouchId = touch.touchId;
                    previousPointerPosition = touch.screenPosition;
                    isInteracting = true;
                    touchFound = true;
                    break;
                }
                else if (isInteracting && touch.touchId == activeTouchId)
                {
                    touchFound = true;

                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
                    {
                        Vector2 delta = touch.screenPosition - previousPointerPosition;
                        ApplyRotationDelta(delta, touchRotationSpeed * sensitivity);
                        previousPointerPosition = touch.screenPosition;
                    }
                    else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended || touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                    {
                        isInteracting = false;
                        activeTouchId = -1;
                    }
                    break;
                }
            }

            if (isInteracting && !touchFound)
            {
                isInteracting = false;
                activeTouchId = -1;
            }
            return;
        }
        else if (isInteracting && Mouse.current == null)
        {
            isInteracting = false;
            activeTouchId = -1;
        }


        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
                {
                    isInteracting = true;
                    previousPointerPosition = Mouse.current.position.ReadValue();
                }
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isInteracting = false;
            }

            if (isInteracting && Mouse.current.leftButton.isPressed)
            {
                Vector2 currentMousePos = Mouse.current.position.ReadValue();
                Vector2 delta = currentMousePos - previousPointerPosition;
                ApplyRotationDelta(delta, mouseRotationSpeed * sensitivity);
                previousPointerPosition = currentMousePos;
            }
        }
    }

    private void ApplyRotationDelta(Vector2 delta, float speedMultiplier)
    {
        targetYaw += delta.x * speedMultiplier;
        targetPitch -= delta.y * speedMultiplier;
        targetPitch = Mathf.Clamp(targetPitch, minPitchAngle, maxPitchAngle);
    }

    private void ResolveCameraCollision(Vector3 focusPoint, Quaternion rotation)
    {
        Vector3 desiredDirection = -(rotation * Vector3.forward);
        Ray ray = new Ray(focusPoint, desiredDirection);

        if (Physics.SphereCast(ray, cameraCollisionRadius, out RaycastHit hit, defaultDistance, collisionLayers, QueryTriggerInteraction.Ignore))
        {
            targetDistance = Mathf.Clamp(hit.distance, minDistance, defaultDistance);
        }
        else
        {
            targetDistance = defaultDistance;
        }
    }
}