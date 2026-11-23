/*
*   Name: DomeWallController.cs
*   Description: This script manages the dome wall informaiton that displays
*                when the hotspot button is clicked during self guided tours
*/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DomeWallController : MonoBehaviour
{
    [SerializeField] private Sprite missingImageSprite;
    [SerializeField] private Image currentDomeImage;
    [SerializeField] private TMP_Text currentDomeTitleText;
    [SerializeField] private TMP_Text currentDomeInfoText;
    [SerializeField] private CanvasRenderer bulletIcon;

    private DomeWall selectedDomeWall;

    private void Start()
    {
        bulletIcon.gameObject.SetActive(false);
    }

    public void UpdateDomeWallViewingPanel(GameObject hotSpotObject)
    {
        currentDomeInfoText.text = "";
        currentDomeImage.sprite = null;
        currentDomeTitleText.text = "";

        DomeWallState state = DomeWallChanger.CurrentDomeWallState;
        if (state == DomeWallState.Religion || state == DomeWallState.Art)
            bulletIcon.gameObject.SetActive(state == DomeWallState.Religion);

        if (!hotSpotObject.TryGetComponent(out DomeWall domeWall))
            return;

        selectedDomeWall = domeWall;
        ShowWall(domeWall, state);
    }

    private void ShowWall(DomeWall wall, DomeWallState state)
    {
        Sprite sprite = wall.GetCurrentSprite();
        currentDomeImage.sprite = sprite != null ? sprite : missingImageSprite;

        string title = wall.GetCurrentTitle();
        currentDomeTitleText.text = title == "" ? "Data coming soon!" : title;

        string info = wall.GetCurrentInfo();
        if (info != "")
        {
            currentDomeInfoText.text = info;
            return;
        }

        switch (state)
        {
            case DomeWallState.Art:
                currentDomeInfoText.text = "Data\n\nbeing\n\nadded!\n\nPlease\n\ncome back\n\nlater!";
                break;
            case DomeWallState.Religion:
                currentDomeInfoText.text = "Data\n\nbeing\n\nadded!\n\nPlease\n\ncome\n\nback\n\nlater!";
                break;
        }
    }

    public void NextImage()
    {
        if (selectedDomeWall == null) { return; }

        selectedDomeWall.SwitchWallSprite();
        UpdateDomeWallViewingPanel(selectedDomeWall.gameObject);
    }

    public void PreviousImage()
    {
        if (selectedDomeWall == null) { return; }

        selectedDomeWall.PreviousImage();
        UpdateDomeWallViewingPanel(selectedDomeWall.gameObject);
    }
}
