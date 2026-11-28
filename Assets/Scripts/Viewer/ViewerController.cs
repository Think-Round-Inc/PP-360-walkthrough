/*
*   Name: ViewerController.cs
*   Description: this script is repsonsible for the viewer contorls the user uses
*                in the self guided tour 
*/

using UnityEngine;

public sealed class ViewerController : MonoBehaviour
{
    public bool viewerControllerActive = true;
    private bool settingsOpen;
    [SerializeField] Transform headTransform;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField, Min(0f), Tooltip("Turn speed at the default mouse sensitivity of 20.")]
    float rotationSpeed = 5f;
    [SerializeField, Range(0f, 0.1f), Tooltip("Seconds used to smooth mouse look during a drag. Zero applies movement immediately.")]
    float lookSmoothing = 0.035f;

    private const float DefaultSensitivity = 20f;
    private float sensitivityMultiplier = 1f;
    private Vector2 pendingLook;
    [SerializeField, Range(0, 90)] int maxHeadRotateAngle = 80;

    // Mouse deltas belong to the rendered frame and should be consumed once.
    private void Update()
    {
        if (!viewerControllerActive || settingsOpen) return;
        RotateViewer();
        MoveCharacter();
    }

    private void RotateViewer()
    {
        bool dragging = Input.GetMouseButton(0);
        Cursor.visible = !dragging;
        if (!dragging || Input.GetMouseButtonDown(0))
        {
            pendingLook = Vector2.zero;
            return;
        }

        Vector2 delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        pendingLook += delta * (rotationSpeed * sensitivityMultiplier);
        float fraction = lookSmoothing > 0f
            ? 1f - Mathf.Exp(-Time.unscaledDeltaTime / lookSmoothing)
            : 1f;
        Vector2 turn = pendingLook * fraction;
        pendingLook -= turn;

        Vector3 angles = transform.localEulerAngles;
        float pitch = Mathf.DeltaAngle(0f, angles.x) - turn.y;
        transform.localRotation = Quaternion.Euler(
            Mathf.Clamp(pitch, -maxHeadRotateAngle, maxHeadRotateAngle), angles.y + turn.x, 0f);
    }

    public void SetSettingsOpen(bool value)
    {
        settingsOpen = value;
        if (value) pendingLook = Vector2.zero;
    }

    public void SetViewerControlsActive(bool value)
    {
        viewerControllerActive = value;
        if (!value)
        {
            pendingLook = Vector2.zero;
            Cursor.visible = true;
        }
    }

    private void MoveCharacter()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput == 0f && verticalInput == 0f) return;
        var camera = Camera.main;
        if (camera == null) return;
        Transform cameraTransform = camera.transform;
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 movementDirection = cameraForward * verticalInput + cameraTransform.right * horizontalInput;
        movementDirection.y = 0f;

        if (movementDirection.magnitude > 1f)
            movementDirection.Normalize();

        Vector3 movement = movementSpeed * Time.deltaTime * movementDirection;
        movement.y = 0f;

        transform.Translate(movement, Space.World);
    }

    public void SetMouseSensitivity(float value)
    {
        sensitivityMultiplier = value / DefaultSensitivity;
    }
}