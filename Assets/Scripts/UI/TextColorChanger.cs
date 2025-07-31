using TMPro;
using UnityEngine;

public class TextColorChanger : MonoBehaviour
{
    [SerializeField] TMP_Text targetText;
    [SerializeField] Color targetColor;
    private Color originalColor;

    private void Start()
    {
        if (targetText == null) return;
        originalColor = targetText.color;
    }

    public void SetColorToTargetColor()
    {
        if (targetText == null) return;
        targetText.color = targetColor;
    }

    public void SetColorToOriginalColor()
    {
        if (targetText == null) return;
        targetText.color = originalColor;
    }
}
