/*
*   Name: ZoomCanvas.cs
*   Description: Displays selected painting in the zoom canvas and plays
*         its associated audio. Manages canvas visibility.
*/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// sealed class as its not intended to be inherited
public sealed class ZoomCanvas : MonoBehaviour
{
    public Canvas zoomCanvas;
    [SerializeField] Image paintingZoomableCanvas;
    [SerializeField] Text Title;
    [SerializeField] UnityEvent onCanvasOpened;
    [SerializeField] UnityEvent onCanvasClosed;

    //Get audio
    [SerializeField] private AudioSource bgmAudioSource;

    private void Start()
    {
        // turn canvas off at load of scene
        zoomCanvas.gameObject.SetActive(false);
    }

    // Sets current painting to the zoombale canvas for zooming
    // <param name="screen">Current close screen</param>
    public void DisplayPaintingToZoomableCanvas(GameObject screen)
    {
        if (screen == null || !screen.TryGetComponent(out PaintingData data) || data.paintingData == null)
        {
            return;
        }

        // Display this painting.
        paintingZoomableCanvas.sprite = data.paintingData.paintingImage;

        // Play this painting's paired audio.
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
            bgmAudioSource.clip = data.paintingData.paintingClip;
            bgmAudioSource.loop = false;

            if (bgmAudioSource.clip != null)
            {
                bgmAudioSource.Play();
            }
        }
    }

    /// Turns on zoom canvas and fires event that it has been opened
    public void OpenCanvas()
    {
        zoomCanvas.gameObject.SetActive(true);
        onCanvasOpened?.Invoke();
    }

    /// Sets painting canvas to null effectively removing it
    public void ClearPaintingCanvas()
    {
        if (paintingZoomableCanvas == null) { return; }
        paintingZoomableCanvas.sprite = null;
    }

    /// Switch the visibility of zoom canvas
    public void SwitchCanvasVisibility()
    {
        if (zoomCanvas.gameObject.activeSelf) { CloseCanvas(); }
        else { OpenCanvas(); }
    }

    // Turns canvas off and fired event that it has been closed
    public void CloseCanvas()
    {
        zoomCanvas.gameObject.SetActive(false);
        onCanvasClosed?.Invoke();
    }
}
