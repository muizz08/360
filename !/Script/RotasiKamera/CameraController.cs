using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("Rotation Targets")]
    public Transform RotatableH;
    public Transform RotatableV;

    [Header("Manual Rotation Settings")]
    public float RotationSpeed = 0.2f;

    [Header("Auto Rotate Settings")]
    public float AutoRotateSpeed = 20f;
    public bool AutoRotateClockwise = true;
    private bool autoRotate = false;

    [Header("Vertical Clamp")]
    public bool LimitedV = true;
    public float minVertical = -85f;
    public float maxVertical = 85f;

    [Header("Inertia / Damping")]
    [Tooltip("Minimum waktu berhenti (gerakan pelan).")]
    public float minDamping = 0.1f;
    [Tooltip("Maximum waktu berhenti (gerakan cepat).")]
    public float maxDamping = 1.0f;
    [Tooltip("Patokan seberapa cepat gerakan dianggap maksimum.")]
    public float maxSpeedForDamping = 200f;

    private Vector2 smoothVelocity;
    private Vector2 inertiaVelocity;
    private float dynamicDamping;

    // Sensitivitas mouse
    public float mouseSensitivity;

    [Header("Zoom Settings")]
    public Camera targetCamera;
    public Slider zoomSlider;
    public float minFOV = 20f;
    public float maxFOV = 60f;
    public float zoomSmoothSpeed = 5f;
    private float targetFOV;

    private float rotationX = 0f;
    private Vector3 lastMousePosition = Vector3.zero;
    private Vector2 lastDelta = Vector2.zero;
    private Coroutine inertiaCoroutine;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (zoomSlider != null)
        {
            zoomSlider.minValue = 0f;
            zoomSlider.maxValue = 1f;
            targetFOV = Mathf.Lerp(minFOV, maxFOV, zoomSlider.value);
            zoomSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void Update()
    {
        if (autoRotate && RotatableH != null)
        {
            float direction = AutoRotateClockwise ? -1f : 1f;
            RotatableH.Rotate(0f, AutoRotateSpeed * Time.deltaTime * direction, 0f);
        }

        // --- Manual rotation logic ---
        if (Input.GetMouseButtonDown(0))
        {
            autoRotate = false;
            lastMousePosition = Input.mousePosition;
            if (inertiaCoroutine != null) StopCoroutine(inertiaCoroutine);
            smoothVelocity = Vector2.zero;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector2 delta = (currentMousePosition - lastMousePosition) * RotationSpeed;
            lastMousePosition = currentMousePosition;

            lastDelta = delta;

            if (RotatableH != null)
                RotatableH.Rotate(0f, -delta.x, 0f);

            if (RotatableV != null)
            {
                rotationX += delta.y;
                if (LimitedV) rotationX = Mathf.Clamp(rotationX, minVertical, maxVertical);
                RotatableV.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (lastDelta.sqrMagnitude > 0.01f)
            {
                if (inertiaCoroutine != null) StopCoroutine(inertiaCoroutine);
                smoothVelocity = lastDelta;
                inertiaVelocity = Vector2.zero;

                float speed = lastDelta.magnitude;
                float t = Mathf.Clamp01(speed / maxSpeedForDamping);
                dynamicDamping = Mathf.Lerp(minDamping, maxDamping, t);

                inertiaCoroutine = StartCoroutine(ApplyInertia());
            }
            lastDelta = Vector2.zero;
        }

        // --- Scroll Wheel Zoom ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetFOV -= scroll * 20f;
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);

            if (zoomSlider != null)
            {
                zoomSlider.SetValueWithoutNotify(
                    Mathf.InverseLerp(minFOV, maxFOV, targetFOV)
                );
            }
        }

        // --- Selalu update kamera tiap frame ---
        if (targetCamera != null)
        {
            targetCamera.fieldOfView = Mathf.Lerp(
                targetCamera.fieldOfView,
                targetFOV,
                Time.deltaTime * zoomSmoothSpeed
            );
        }
    }


    private IEnumerator ApplyInertia()
    {
        while (smoothVelocity.sqrMagnitude > 0.01f)
        {
            if (RotatableH != null)
                RotatableH.Rotate(0f, -smoothVelocity.x, 0f, Space.Self);

            if (RotatableV != null)
            {
                rotationX += smoothVelocity.y;
                if (LimitedV) rotationX = Mathf.Clamp(rotationX, minVertical, maxVertical);
                RotatableV.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            }

            // Pakai damping dinamis
            smoothVelocity = Vector2.SmoothDamp(
                smoothVelocity,
                Vector2.zero,
                ref inertiaVelocity,
                dynamicDamping
            );

            yield return null;
        }
    }

    public void ToggleAutoRotate()
    {
        autoRotate = !autoRotate;
        Debug.Log("Auto-rotate is now: " + autoRotate);

        if (autoRotate && inertiaCoroutine != null)
        {
            StopCoroutine(inertiaCoroutine);
        }
    }

    public void SetAutoRotateDirection(bool clockwise)
    {
        AutoRotateClockwise = clockwise;
    }

    public void OnSliderChanged(float value)
    {
        targetFOV = Mathf.Lerp(minFOV, maxFOV, value);
        autoRotate = false;
    }

}
