using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
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

    public void SetBrightness(float value)
    {
        // 1 = normal brightness; 0 = darkest.
        float darkness = (1f - Mathf.Clamp01(value)) * 0.8f;
        brightnessOverlay.color = new Color(0f, 0f, 0f, darkness);
    }

    public void SetFontSize(float offset)
    {
        foreach (var entry in originalFontSizes)
        {
            if (entry.Key != null)
            {
                entry.Key.fontSize =
                    Mathf.Max(1f, entry.Value + offset);
            }
        }
    }

    public void SetMouseSensitivity(float value)
    {
        if (viewerController != null)
        {
            viewerController.SetMouseSensitivity(value);
        }
    }

    public void SetVolume(float value)
    {
        // Pending audio implementation.
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