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
    [SerializeField] float rotationSpeed;
    [SerializeField, Range(0, 90)] int maxHeadRotateAngle = 80;

    void FixedUpdate()
    {
        if (!viewerControllerActive || settingsOpen) return;
        RotateViewer();
        MoveCharacter();
    }

    private void RotateViewer()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            Cursor.visible = false;

            Vector3 currentRotation = transform.localRotation.eulerAngles;
            float newAngleX = currentRotation.x - mouseY * rotationSpeed;
            newAngleX = WrapAngle(newAngleX);
            newAngleX = Mathf.Clamp(newAngleX, -maxHeadRotateAngle, maxHeadRotateAngle);

            transform.localRotation = Quaternion.Euler(newAngleX, currentRotation.y, 0f);

            transform.Rotate(Vector3.up, mouseX * rotationSpeed, Space.World);
        }
        else
        {
            Cursor.visible = true;
        }
    }

    private float WrapAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f)
            return angle - 360f;
        if (angle < -180f)
            return angle + 360f;
        return angle;
    }

    public void SetSettingsOpen(bool value) => settingsOpen = value;

    public void SetViewerControlsActive(bool value)
    {
        viewerControllerActive = value;
        if (!value)
            Cursor.visible = true;
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
        rotationSpeed = value;
    }
}