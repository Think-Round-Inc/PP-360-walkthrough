using UnityEngine;
using UnityEngine.Events;

public sealed class EarthView : MonoBehaviour
{
    [SerializeField] Canvas earthViewCanvas;
    [SerializeField] EarthViewPanel[] earthViewPanels = new EarthViewPanel[4];
    [SerializeField] GameObject viewerHead;
    [SerializeField] UnityEvent onEarthViewCanvasOpened;
    [SerializeField] UnityEvent onEarthViewCanvasClosed;
    [SerializeField] UnityEvent<EarthViewPanel> onPanelOpened;
    [Range(0, 3)] int _currentPanelOpen = 0;

    void Start()
    {
        HideAllPanels();
        if (earthViewCanvas == null) return;
        earthViewCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// hides all earth panels
    /// </summary>
    public void HideAllPanels()
    {
        for (int i = 0; i < earthViewPanels.Length; i++)
        {
            if (earthViewPanels[i] != null)
                earthViewPanels[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// moves to a specific Earth panel
    /// </summary>
    /// <param name="panelToMoveTo">view panel to move to</param>
    public void MoveToPanel(EarthViewPanel panelToMoveTo)
    {
        for (int i = 0; i < earthViewPanels.Length; i++)
        {
            if (earthViewPanels[i] == null) continue;
            if (earthViewPanels[i] != panelToMoveTo)
                earthViewPanels[i].gameObject.SetActive(false);
            else
            {
                earthViewPanels[i].gameObject.SetActive(true);
                onPanelOpened?.Invoke(earthViewPanels[i]);
            }
        }
    }

    /// <summary>
    /// Move to previous Earth panel
    /// </summary>
    public void MoveToPreviousPanel()
    {
        _currentPanelOpen--;
        if (_currentPanelOpen < 0)
            _currentPanelOpen = earthViewPanels.Length - 1;
        MoveToPanel(earthViewPanels[_currentPanelOpen]);
    }

    /// <summary>
    /// move to next earth panel
    /// </summary>
    public void MoveToNextPanel()
    {
        _currentPanelOpen++;
        if (_currentPanelOpen >= earthViewPanels.Length)
            _currentPanelOpen = 0;
        MoveToPanel(earthViewPanels[_currentPanelOpen]);
    }

    /// <summary>
    /// set earth view canvas active
    /// </summary>
    /// <param name="isActive">new state of panel</param>
    public void SetEarthViewCanvasActive(bool isActive)
    {
        if (earthViewCanvas == null) return;
        if (isActive)
        {
            earthViewCanvas.gameObject.SetActive(true);
            onEarthViewCanvasOpened.Invoke();
        }
        else
        {
            earthViewCanvas.gameObject.SetActive(false);
            onEarthViewCanvasClosed.Invoke();
        }
    }
}
