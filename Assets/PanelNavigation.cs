using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelNavigation : MonoBehaviour
{
    [SerializeField] GameObject ControlPanel;
    [SerializeField] GameObject MenuUIPanel;

    public void Start()
    {
        CloseControlPanel();
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
}
