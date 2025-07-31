using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// sealed class as its not intended to be inherited
public sealed class ZoomCanvas : MonoBehaviour
{
    public Canvas zoomCanvas;
    [SerializeField] Image paintingZoomableCanvas;
    [SerializeField] UnityEvent onCanvasOpened;
    [SerializeField] UnityEvent onCanvasClosed;

    private void Start()
    {
        // turn canvas off at load of scene
        zoomCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets current painting to the zoombale canvas for zooming
    /// </summary>
    /// <param name="screen">Current close screen</param>
    public void DisplayPaintingToZoomableCanvas(GameObject screen)
    {
        // checks screen for painting data then if found, adds painting sprite to canvas
        if (screen.TryGetComponent(out PaintingData data))
        {
            if (data.paintingData.paintingImage != null)
                paintingZoomableCanvas.sprite = data.paintingData.paintingImage;
        }
    }

    /// <summary>
    /// Turns on zoom canvas and fires event that it has been opened
    /// </summary>
    public void OpenCanvas()
    {
        zoomCanvas.gameObject.SetActive(true);
        onCanvasOpened?.Invoke();
    }

    /// <summary>
    /// Sets painting canvas to null effectively removing it
    /// </summary>
    public void ClearPaintingCanvas()
    {
        if (paintingZoomableCanvas == null) return;
        paintingZoomableCanvas.sprite = null;
    }

    /// <summary>
    /// Switch the visibility of zoom canvas
    /// </summary>
    public void SwitchCanvasVisibility()
    {
        if (zoomCanvas.gameObject.activeSelf)
            CloseCanvas();
        else
            OpenCanvas();
    }

    /// <summary>
    /// Turns canvas off and fired event that it has been closed
    /// </summary>
    public void CloseCanvas()
    {
        zoomCanvas.gameObject.SetActive(false);
        onCanvasClosed?.Invoke();
    }
}
