using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DomeWallController : MonoBehaviour
{
    [SerializeField] Sprite missingImageSprite;
    [SerializeField] Image currentDomeImage;
    [SerializeField] TMP_Text currentDomeTitleText;
    [SerializeField] TMP_Text currentDomeInfoText;
    [SerializeField] CanvasRenderer bulletIcon;

    private void Start()
    {
        bulletIcon.gameObject.SetActive(false);
    }

    public void UpdateDomeWallViewingPanel(GameObject hotSpotObject)
    {
        currentDomeInfoText.text = "";
        currentDomeImage.sprite = null;
        currentDomeTitleText.text = "";

        if (DomeWallChanger.CurrentDomeWallState == DomeWallState.Religion)
            bulletIcon.gameObject.SetActive(true);
        else if (DomeWallChanger.CurrentDomeWallState == DomeWallState.Art)
            bulletIcon.gameObject.SetActive(false);

        if (hotSpotObject.TryGetComponent(out DomeWall domeWall))
        {
            SetInfoText();

            Sprite domeSprite = domeWall.GetCurrentSprite();

            if (domeSprite != null)
                currentDomeImage.sprite = domeSprite;
            else if (missingImageSprite != null)
                currentDomeImage.sprite = missingImageSprite;

            string title = domeWall.GetCurrentTitle();

            if (title != "")
                currentDomeTitleText.text = title;
            else
                currentDomeTitleText.text = "Data coming soon!";

            string info = domeWall.GetCurrentInfo();

            if (info != "")
                currentDomeInfoText.text = info;
        }
    }

    void SetInfoText()
    {
        switch (DomeWallChanger.CurrentDomeWallState)
        {
            case DomeWallState.Art:
                currentDomeInfoText.text = "Data\n\nbeing\n\nadded!\n\nPlease\n\ncome back\n\nlater!";
                break;
            case DomeWallState.Religion:
                currentDomeInfoText.text = "Data\n\nbeing\n\nadded!\n\nPlease\n\ncome\n\nback\n\nlater!";
                break;
        }
    }
}
