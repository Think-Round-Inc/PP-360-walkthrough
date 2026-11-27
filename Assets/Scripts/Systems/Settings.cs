/*
*   Name: Settings.cs
*   Description: this script is reponsible for managing the setting panel
*                slides and toggles.
*/

using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    [Header("Sliders")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider mouseSensitivitySlider;

    [Header("References")]
    [SerializeField] private Image brightnessOverlay;
    [SerializeField] private ViewerController viewerController;

    private void Start()
    {
        // The overlay must allow clicks through it.
        brightnessOverlay.raycastTarget = false;

        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);

        SetBrightness(brightnessSlider.value);
        if (!TryGetComponent<Save>(out _))
            mouseSensitivitySlider.SetValueWithoutNotify(20f);
        SetMouseSensitivity(mouseSensitivitySlider.value);
    }

    private void Update()
    {
        bool settingsOpen = settingsPanel.activeInHierarchy;

        viewerController.SetSettingsOpen(settingsOpen);

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

    public void SetMouseSensitivity(float value)
    {
        
        if (viewerController != null)
        {
            viewerController.SetMouseSensitivity(Mathf.Clamp(value, 1f, 100f));
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

        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.RemoveListener(SetMouseSensitivity);
    }
}