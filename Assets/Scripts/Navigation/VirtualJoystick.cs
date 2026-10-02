using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joystickContainer;
    public RectTransform joystickHandle;

    [Header("Joystick Settings")]
    [Range(20f, 150f)]
    public float handleRange = 75f;
    [Range(0f, 0.5f)]
    public float deadZone = 0.1f;
    public float inputSmoothSpeed = 20f;

    private Vector2 rawInput = Vector2.zero;
    private Vector2 smoothedInput = Vector2.zero;
    private Canvas rootCanvas;

    public Vector2 InputVector => smoothedInput;
    public float Horizontal => smoothedInput.x;
    public float Vertical => smoothedInput.y;

    private void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        if (joystickContainer == null)
        {
            joystickContainer = GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        smoothedInput = Vector2.Lerp(smoothedInput, rawInput, Time.deltaTime * inputSmoothSpeed);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ProcessJoystickMovement(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ProcessJoystickMovement(eventData);
    }

    private void ProcessJoystickMovement(PointerEventData eventData)
    {
        Camera uiCamera = (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? rootCanvas.worldCamera
            : null;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickContainer,
            eventData.position,
            uiCamera,
            out Vector2 localPoint))
        {
            Vector2 clampedPosition = Vector2.ClampMagnitude(localPoint, handleRange);

            if (joystickHandle != null)
            {
                joystickHandle.anchoredPosition = clampedPosition;
            }

            Vector2 normalizedVector = clampedPosition / handleRange;

            if (normalizedVector.magnitude < deadZone)
            {
                rawInput = Vector2.zero;
            }
            else
            {
                float factor = (normalizedVector.magnitude - deadZone) / (1f - deadZone);
                rawInput = Vector2.ClampMagnitude(normalizedVector.normalized * factor, 1.0f);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rawInput = Vector2.zero;
        if (joystickHandle != null)
        {
            joystickHandle.anchoredPosition = Vector2.zero;
        }
    }
}