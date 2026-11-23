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
    private KeyCode exitKey = KeyCode.Tab;

    [SerializeField] private SpiralTour tour;

    public void Start()
    {
        CloseControlPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tour.ResetDomeVisit();
            greetingPanel.SetActive(true);
        }
    }

    public void OpenControlPanel() => ControlPanel.SetActive(true);
    public void CloseControlPanel() => ControlPanel.SetActive(false);
    public void OpenMenuUI() => MenuUIPanel.SetActive(true);
    public void CloseMenuUI() => MenuUIPanel.SetActive(false);
    public void ToggleDomeDescription() => domeDescriptionPanel.SetActive(!domeDescriptionPanel.activeInHierarchy);
}
