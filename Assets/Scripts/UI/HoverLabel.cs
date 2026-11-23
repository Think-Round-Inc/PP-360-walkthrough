/*
*   Name: HoverLabel.cs
*   Description: This script is responsible for displaying the labels 
*                   shown when hovering over navigation buttons.
*/

using UnityEngine;

public sealed class HoverLabel : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private GameObject[] navigationInfoLabels;

    private void Start()
    {
        SetAllNavigationLabelsInactive();
    }

    public void SetNavigationInfoLabelActive(int index)
    {
        for (int i = 0; i < navigationInfoLabels.Length; i++)
        {
            if (navigationInfoLabels[i] != null)
            {
                navigationInfoLabels[i].SetActive(i == index);
            }
        }
    }

    public void SetAllNavigationLabelsInactive() => SetNavigationInfoLabelActive(-1);
}
