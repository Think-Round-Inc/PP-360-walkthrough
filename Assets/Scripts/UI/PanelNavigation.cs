/*
*   Name: PanelNavigation.cs
*   Description: This script is responsible for openning/closing panels
*/

using UnityEngine;

public class PanelNavigation : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject ControlPanel;
    [SerializeField] private GameObject MenuUIPanel;
    [SerializeField] private GameObject greetingPanel;
    [SerializeField] private GameObject domeDescriptionPanel;

    [Header("Action Keys")]
    private const KeyCode ExitKey = KeyCode.Tab;

    [SerializeField] private SpiralTour tour;
    [SerializeField] private UnityEngine.UI.Button locationButton;
    [SerializeField] private UnityEngine.UI.Button domeWallsButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject panelClickSensor;

    public bool SpiralTourActive { get; private set; }
    public bool SelfGuidedTourActive { get; private set; }
    public event System.Action TourModeChanged;

    private Canvas menuCanvas;
    private int menuSortingOrder;

    public void Start()
    {
        if (MenuUIPanel.TryGetComponent(out menuCanvas))
            menuSortingOrder = menuCanvas.sortingOrder;
        SetTourMode(false, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(ExitKey))
        {
            SetTourMode(false, false);
            tour.SetViewerControlsActive(true);
            tour.ResetDomeVisit();
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (panelClickSensor != null) panelClickSensor.SetActive(false);
            greetingPanel.SetActive(true);
        }
    }

    public void StartSpiralTour() => SetTourMode(true, false);
    public void StartSelfGuidedTour() => SetTourMode(false, true);

    private void SetTourMode(bool spiral, bool selfGuided)
    {
        SpiralTourActive = spiral;
        SelfGuidedTourActive = selfGuided;
        ControlPanel.SetActive(spiral);
        if (spiral) OpenMenuUI();
        if (menuCanvas != null)
            menuCanvas.sortingOrder = spiral ? menuSortingOrder + 1 : menuSortingOrder;
        SetNavigationButtonActive(locationButton, selfGuided);
        SetNavigationButtonActive(domeWallsButton, selfGuided);
        TourModeChanged?.Invoke();
    }

    private static void SetNavigationButtonActive(UnityEngine.UI.Button button, bool enabled)
    {
        if (button == null) return;

        button.interactable = enabled;
        if (button.TryGetComponent<HoverButton>(out var hover))
            hover.enabled = enabled;
    }

    public void OpenControlPanel() => ControlPanel.SetActive(true);
    public void CloseControlPanel() => ControlPanel.SetActive(false);

    public void ReturnFromPainting()
    {
        OpenMenuUI();
        if (SelfGuidedTourActive && tour.TryGetComponent<ViewerController>(out var viewer))
            viewer.SetViewerControlsActive(true);
    }

    public void OpenMenuUI() => MenuUIPanel.SetActive(true);
    public void CloseMenuUI()
    {
        if (!SpiralTourActive)
            MenuUIPanel.SetActive(false);
    }
    public void ToggleDomeDescription() => domeDescriptionPanel.SetActive(!domeDescriptionPanel.activeInHierarchy);
}
