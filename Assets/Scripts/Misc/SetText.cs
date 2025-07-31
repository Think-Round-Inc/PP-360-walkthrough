using TMPro;
using UnityEngine;

public class SetText : MonoBehaviour
{
    [SerializeField] TMP_Text targetText;

    public void SetTargetText(string text)
    {
        if (targetText == null) return;
        targetText.text = text;
    }
}
