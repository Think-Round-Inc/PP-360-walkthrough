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

    [SerializeField] private TTS narration;
    private bool paintingSelected;
    private bool waitingForTitle;

    private void Awake()
    {
        if (narration == null)
            narration = FindFirstObjectByType<TTS>();
    }

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

        EndPaintingPlayback();
        paintingSelected = true;
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
            bgmAudioSource.clip = data.paintingData.paintingClip;
            bgmAudioSource.loop = false;
        }

        narration?.BeginPainting(data.paintingData.paintingName);
        waitingForTitle = true;
    }

    private void LateUpdate()
    {
        if (!paintingSelected) return;
        if (!zoomCanvas.isActiveAndEnabled)
        {
            EndPaintingPlayback();
            return;
        }

        if (!waitingForTitle || (narration != null && narration.PaintingTitleIsSpeaking))
            return;

        waitingForTitle = false;
        if (bgmAudioSource != null && bgmAudioSource.clip != null)
            bgmAudioSource.Play();
    }

    private void EndPaintingPlayback()
    {
        if (!paintingSelected) return;
        paintingSelected = false;
        waitingForTitle = false;
        if (narration != null) narration.EndPainting();
        if (bgmAudioSource != null) bgmAudioSource.Stop();
    }

    private void OnDisable() => EndPaintingPlayback();

    /// Turns on zoom canvas and fires event that it has been opened
    public void OpenCanvas()
    {
        zoomCanvas.gameObject.SetActive(true);
        onCanvasOpened?.Invoke();
    }

    /// Sets painting canvas to null effectively removing it
    public void ClearPaintingCanvas()
    {
        EndPaintingPlayback();
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
        EndPaintingPlayback();
        zoomCanvas.gameObject.SetActive(false);
        onCanvasClosed?.Invoke();
    }
}
