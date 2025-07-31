using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ViewerController : MonoBehaviour
{
    public bool viewerControllerActive = true;
    [SerializeField] Transform headTransform;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float rotationSpeed;
    [SerializeField, Range(0, 90)] int maxHeadRotateAngle = 80;

    void FixedUpdate()
    {
        if (!viewerControllerActive)return;
        RotateViewer();
        MoveCharacter();
    }

    private void RotateViewer()
    {
        if (Input.GetMouseButton(1))
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

    public void SetViewerControlsActive(bool value) => viewerControllerActive = value;

    private void MoveCharacter()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 movementDirection = cameraForward * verticalInput + Camera.main.transform.right * horizontalInput;
        movementDirection.y = 0f;

        if (movementDirection.magnitude > 1f)
            movementDirection.Normalize();

        Vector3 movement = movementSpeed * Time.deltaTime * movementDirection;
        movement.y = 0f;

        transform.Translate(movement, Space.World);
    }
}