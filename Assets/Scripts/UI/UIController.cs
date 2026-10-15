using UnityEngine;

public sealed class UIController : MonoBehaviour
{
    [SerializeField, Tooltip("Labels shown when hovering over navigation buttons")]
    private GameObject[] navigationInfoLabels;

    private void Start()
    {
        SetAllNavigationLabelsInactive();
    }

    // Kept temporarily for existing Inspector event references.
    public void SetPaintingNameAndInfoToInfoText(GameObject screen) { }

    public void ClearInfoPanelText() { }

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

    public void SetAllNavigationLabelsInactive()
    {
        for (int i = 0; i < navigationInfoLabels.Length; i++)
        {
            if (navigationInfoLabels[i] != null)
            {
                navigationInfoLabels[i].SetActive(false);
            }
        }
    }
}