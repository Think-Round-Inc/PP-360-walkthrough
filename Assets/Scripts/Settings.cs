using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    [Header("Sliders")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider fontSizeSlider;
    [SerializeField] private Slider mouseSensitivitySlider;

    [Header("References")]
    [SerializeField] private Image brightnessOverlay;
    [SerializeField] private ViewerController viewerController;

    private readonly Dictionary<TextMeshProUGUI, float> originalFontSizes =
        new Dictionary<TextMeshProUGUI, float>();

    private void Start()
    {
        // Include text on inactive panels.
        TextMeshProUGUI[] texts =
            FindObjectsOfType<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI text in texts)
        {
            originalFontSizes[text] = text.fontSize;
            text.enableAutoSizing = false;
        }

        // The overlay must allow clicks through it.
        brightnessOverlay.raycastTarget = false;

        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        fontSizeSlider.onValueChanged.AddListener(SetFontSize);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);

        SetBrightness(brightnessSlider.value);
        SetFontSize(fontSizeSlider.value);
        SetMouseSensitivity(mouseSensitivitySlider.value);
    }

    private void Update()
    {
        bool settingsOpen = settingsPanel.activeInHierarchy;

        viewerController.SetViewerControlsActive(!settingsOpen);

        if (settingsOpen)
        {
            Cursor.visible = true;
        }
    }

  public void SetBrightness(float value)
    {
        // 100 = normal brightness; 1 = darkest.
        float brightness = Mathf.Clamp(value, 1f, 100f) / 100f;
        float darkness = (1f - brightness) * 0.8f;

        brightnessOverlay.color = new Color(0f, 0f, 0f, darkness);
    }

    public void SetFontSize(float value)
    {
        // 1 = original sizes; each step adds 1.
        float offset = Mathf.Clamp(value, 1f, 100f) - 1f;

        foreach (var entry in originalFontSizes)
        {
            if (entry.Key != null)
            {
                entry.Key.fontSize = entry.Value + offset;
            }
        }
    }

    public void SetMouseSensitivity(float value)
    {
        if (viewerController != null)
        {
            viewerController.SetMouseSensitivity(
                Mathf.Clamp(value, 1f, 100f));
        }
    }

    public void SetVolume(float value)
    {
        // Pending audio implementation.
        // Convert to 0–1 when connecting audio:
        // float volume = Mathf.Clamp(value, 1f, 100f) / 100f;
    }

    public void SetColorblindMode(bool enabled)
    {
        // Pending artist feedback.
    }

    public void SetVoiceOver(bool enabled)
    {
        // Pending audio implementation.
    }

    private void OnDestroy()
    {
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.RemoveListener(SetBrightness);

        if (fontSizeSlider != null)
            fontSizeSlider.onValueChanged.RemoveListener(SetFontSize);

        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.RemoveListener(SetMouseSensitivity);
    }
}