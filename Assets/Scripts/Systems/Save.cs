using UnityEngine;

[DefaultExecutionOrder(-100)]
public sealed class Save : MonoBehaviour
{
    private const string Prefix = "Exhibit.Settings.";

    [Header("Narration")]
    [SerializeField] private TTS tts;
    [SerializeField] private UnityEngine.UI.Toggle voiceOver;
    [SerializeField] private UnityEngine.UI.Slider ttsVolume;
    [Header("Exhibit Preferences")]
    [SerializeField] private UnityEngine.UI.Toggle colorblindness;
    [SerializeField] private UnityEngine.UI.Slider musicVolume;
    [SerializeField] private UnityEngine.UI.Slider brightness;
    [SerializeField] private UnityEngine.UI.Slider mouseSensitivity;

    private void Awake()
    {
        BindToggle("VoiceOver", voiceOver);
        BindToggle("Colorblindness", colorblindness);
        BindSlider("TTSVolume", ttsVolume);
        BindSlider("MusicVolume", musicVolume);
        BindSlider("Brightness", brightness);
        BindSlider("MouseSensitivity", mouseSensitivity);
        tts.SetSpeed(PlayerPrefs.GetFloat(Prefix + "TTSSpeed", tts.Speed));
        tts.SpeedChanged += SaveSpeed;
    }

    private void OnDestroy()
    {
        tts.SpeedChanged -= SaveSpeed;
    }

    private static void BindToggle(string name, UnityEngine.UI.Toggle control)
    {
        string key = Prefix + name;
        control.SetIsOnWithoutNotify(PlayerPrefs.GetInt(key, control.isOn ? 1 : 0) != 0);
        control.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        });
    }

    private static void BindSlider(string name, UnityEngine.UI.Slider control)
    {
        string key = Prefix + name;
        control.SetValueWithoutNotify(PlayerPrefs.GetFloat(key, control.value));
        control.onValueChanged.AddListener(value => SaveFloat(key, value));
    }

    private static void SaveSpeed(float value) => SaveFloat(Prefix + "TTSSpeed", value);

    private static void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }
}
