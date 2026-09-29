using TMPro;
using UnityEngine;

public sealed class ExitController : MonoBehaviour
{
    [SerializeField] private KeyCode exitKey = KeyCode.Tab;
    [SerializeField] private TMP_Text exitText;
    [SerializeField] private Canvas initialCanvas;

    private void Start()
    {
        if (exitText != null)
        {
            exitText.text = $"Press '{exitKey}' to Exit";
        }
    }

    private void Update()
{
    if (Input.GetKeyDown(KeyCode.Tab))
    {
        initialCanvas.gameObject.SetActive(true);
        initialCanvas.enabled = true;
    }
}

}