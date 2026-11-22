using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelNavigation : MonoBehaviour
{
   [Header("Panels")]
    [SerializeField] GameObject ControlPanel;
    [SerializeField] GameObject MenuUIPanel;
    [SerializeField] private GameObject greetingPanel;
    [SerializeField] private GameObject domeDescriptionPanel;
    
    [Header("Action Keys")]
    [SerializeField] private KeyCode exitKey = KeyCode.Tab;

    [SerializeField] SpiralTour tour;

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
            // Note canvas is getting disabled somewhere but I don't know where.
        }
    }

    public void OpenControlPanel()
    {
        ControlPanel.SetActive(true);
    }
    public void CloseControlPanel()
    {
        ControlPanel.SetActive(false);
    }
    public void OpenMenuUI()
    {
        MenuUIPanel.SetActive(true);
    }

    public void CloseMenuUI()
    {
        MenuUIPanel.SetActive(false);
    }

    public void ToggleDomeDescription()
    {
        if (domeDescriptionPanel.activeInHierarchy)
        domeDescriptionPanel.SetActive(false);
        else
        {
            domeDescriptionPanel.SetActive(true);
        }
    }
}

