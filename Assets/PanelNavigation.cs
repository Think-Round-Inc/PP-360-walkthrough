using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelNavigation : MonoBehaviour
{
    [SerializeField] GameObject ControlPanel;

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
}
