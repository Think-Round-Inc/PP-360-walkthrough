using TMPro;
using UnityEngine;

public sealed class UIController : MonoBehaviour
{
    [SerializeField, Tooltip("Labels that pop up when hovering over navigation buttons")] GameObject[] navigationInfoLabels;
    [SerializeField] TMP_Text hotspotInfoText;
    [SerializeField] float paintingNameTextSize;
    [SerializeField] float paintingInfoTextSize;

    private void Start()
    {
        SetAllNavigationLabelsInactive();
    }

    /// <summary>
    /// sets painting name and info to UI text from painting screen, intended to be used dynamically
    /// </summary>
    /// <param name="screen">screen that is displaying target painting</param>
    public void SetPaintingNameAndInfoToInfoText(GameObject screen)
    {
        if (screen.TryGetComponent(out PaintingData data))
            hotspotInfoText.text = $"<size={paintingNameTextSize}>{data.paintingData.paintingName}</size>\n\n<size={paintingInfoTextSize}>{data.paintingData.extraPaintingInfo}</size>";
    }

    /// <summary>
    /// clear hotspot info text from UI
    /// </summary>
    public void ClearInfoPanelText() => hotspotInfoText.text = string.Empty;

    /// <summary>
    /// sets target navigation label active and all other inactive based on index from label array
    /// </summary>
    /// <param name="index">target index in array for label to be active</param>
    public void SetNavigationInfoLabelActive(int index)
    {
        for (int i = 0; i < navigationInfoLabels.Length; i++)
        {
            if (i == index && navigationInfoLabels[i] != null)
                navigationInfoLabels[i].SetActive(true);
            else
                navigationInfoLabels[i].SetActive(false);
        }
    }

    /// <summary>
    /// turns off all navigation labels individually
    /// </summary>
    public void SetAllNavigationLabelsInactive()
    {
        for (int i = 0; i < navigationInfoLabels.Length; i++)
        {
            if (navigationInfoLabels[i] != null)
                navigationInfoLabels[i].SetActive(false);
        }
    }
}
