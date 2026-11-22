using UnityEngine;
using UnityEngine.Events;

public sealed class EarthView : MonoBehaviour
{
    [SerializeField] private Canvas earthViewCanvas;
    [SerializeField] private EarthViewPanel[] earthViewPanels = new EarthViewPanel[4];
    [SerializeField] private GameObject viewerHead;
    [SerializeField] private UnityEvent onEarthViewCanvasOpened;
    [SerializeField] private UnityEvent onEarthViewCanvasClosed;
    [SerializeField] private UnityEvent<EarthViewPanel> onPanelOpened;
    private int currentPanelIndex;

    private void Start()
    {
        HideAllPanels();
        if (earthViewCanvas == null) return;
        earthViewCanvas.gameObject.SetActive(false);
    }

    public void HideAllPanels()
    {
        foreach (EarthViewPanel panel in earthViewPanels)
        {
            if (panel != null) panel.gameObject.SetActive(false);
        }
    }

    public void MoveToPanel(EarthViewPanel panelToMoveTo)
    {
        foreach (EarthViewPanel panel in earthViewPanels)
        {
            if (panel == null) continue;

            bool isRequestedPanel = panel == panelToMoveTo;
            panel.gameObject.SetActive(isRequestedPanel);
            if (isRequestedPanel) onPanelOpened?.Invoke(panel);
        }
    }

    public void MoveToPreviousPanel() => MoveBy(-1);

    public void MoveToNextPanel() => MoveBy(1);

    private void MoveBy(int step)
    {
        int panelCount = earthViewPanels.Length;
        if (panelCount == 0) return;

        currentPanelIndex = (currentPanelIndex + step + panelCount) % panelCount;
        MoveToPanel(earthViewPanels[currentPanelIndex]);
    }

    public void SetEarthViewCanvasActive(bool isActive)
    {
        if (earthViewCanvas == null) return;

        earthViewCanvas.gameObject.SetActive(isActive);
        UnityEvent canvasEvent = isActive ? onEarthViewCanvasOpened : onEarthViewCanvasClosed;
        canvasEvent?.Invoke();
    }
}
