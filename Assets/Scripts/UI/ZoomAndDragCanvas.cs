/*
*   Name: ZoomAndDragCanvas.cs
*   Description: this script is reponsible for the drag and zoom configuration and settings.
*/

using UnityEngine;

public sealed class ZoomAndDragCanvas : MonoBehaviour
{
    public enum MouseButton { Left, Right, Middle }

    public MouseButton dragButton;
    public float zoomSpeed = 1f;
    public float minZoom = 1f;
    public float maxZoom = 3f;
    public float dragSpeed = 1f;

    private RectTransform canvasRectTransform;
    private Vector3 lastMousePosition;

    private void Start()
    {
        canvasRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        UpdateZoom();
        UpdateDrag();
    }

    private void UpdateZoom()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        float newScale = Mathf.Clamp(canvasRectTransform.localScale.x + zoomDelta, minZoom, maxZoom);
        canvasRectTransform.localScale = Vector3.one * newScale;
    }

    private void UpdateDrag()
    {
        if (Input.GetMouseButtonDown((int)dragButton))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton((int)dragButton))
        {
            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
            canvasRectTransform.anchoredPosition += (Vector2)deltaMouse * dragSpeed;
            lastMousePosition = Input.mousePosition;
        }
    }
}