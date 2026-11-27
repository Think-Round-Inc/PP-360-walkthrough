using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public sealed class TTS : MonoBehaviour
{
    [Header("Narration Controls")]
    [SerializeField] private UnityEngine.UI.Toggle speechToggle;
    [SerializeField] private UnityEngine.UI.Slider ttsSlider;
    [SerializeField] private Transform sliderTitles;

    [SerializeField, Range(0.5f, 3f), Tooltip("Speech speed multiplier. 1 uses the device voice’s normal speed.")]
    private float speed = 1f;

    [SerializeField, Min(0.05f), Tooltip("Seconds between visibility checks for new captions.")]
    private float scanInterval = 0.2f;

    [Header("Narration Scope")]
    [SerializeField] private RectTransform[] fullScreenIntroPanels = System.Array.Empty<RectTransform>();
    [SerializeField] private Transform domeWallPreviews;
    [SerializeField] private PanelNavigation tourNavigation;
    [SerializeField] private TMP_Text exitHint;
    [SerializeField] private UnityEngine.UI.Button speedButton;
    [SerializeField] private TMP_Text speedButtonLabel;

    private static readonly float[] SpeedSteps = { 1f, 1.5f, 2f, 2.5f, 3f };
    public float Speed => speed;
    public event System.Action<float> SpeedChanged;

    private bool wasSelfGuidedTourActive;
    private bool exitHintAnnounced;

    private static readonly Regex RichText = new Regex(@"<br\s*/?>|<[^>]+>", RegexOptions.IgnoreCase);
    private readonly Dictionary<int, string> visibleText = new Dictionary<int, string>();
    private readonly Dictionary<int, string> announcedText = new Dictionary<int, string>();
    private readonly List<Caption> captions = new List<Caption>();
    private readonly List<int> hiddenIds = new List<int>();
    private readonly Vector3[] corners = new Vector3[4];
    private readonly List<TMP_Text> greetingText = new List<TMP_Text>();
    private readonly List<UnityEngine.UI.Text> legacyGreetingText = new List<UnityEngine.UI.Text>();
    private Caption speaking;
    private Transform narrationPanel;
    private float nextScan;
    private float volume;

    private struct Caption
    {
        public int Id;
        public string Text;
        public Vector2 Position;
        public bool IsExitHint;
    }

    private void OnEnable()
    {
        speechToggle.onValueChanged.AddListener(SetSpeechEnabled);
        ttsSlider.onValueChanged.AddListener(SetVolume);
        speedButton.onClick.AddListener(CycleSpeed);
        RefreshSpeedLabel();
        SetVolume(ttsSlider.value);
        nextScan = 0;
    }

    private void OnDisable()
    {
        speechToggle.onValueChanged.RemoveListener(SetSpeechEnabled);
        ttsSlider.onValueChanged.RemoveListener(SetVolume);
        speedButton.onClick.RemoveListener(CycleSpeed);
        StopNarration();
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        if (synthesizer != System.IntPtr.Zero)
        {
            SendVoid(synthesizer, sel_registerName("release"));
            synthesizer = System.IntPtr.Zero;
        }
#endif
    }

    public void SetSpeed(float value)
    {
        speed = Mathf.Clamp(value, 0.5f, 3f);
        RefreshSpeedLabel();
        SpeedChanged?.Invoke(speed);
    }

    private void CycleSpeed()
    {
        foreach (float step in SpeedSteps)
        {
            if (step > speed)
            {
                SetSpeed(step);
                return;
            }
        }

        SetSpeed(SpeedSteps[0]);
    }

    private void RefreshSpeedLabel()
    {
        speedButtonLabel.text = "TTS Speed " +
            speed.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + "x";
    }

    private void SetSpeechEnabled(bool enabled)
    {
        if (!enabled)
            StopNarration();

        nextScan = 0;
    }

    private void SetVolume(float value)
    {
        volume = Mathf.InverseLerp(ttsSlider.minValue, ttsSlider.maxValue, value);
        SetDeviceVolume(volume);
        if (volume == 0)
            StopNarration();
        nextScan = 0;
    }

    private void StopNarration()
    {
        StopDeviceSpeech();
        speaking = default;
        announcedText.Clear();
    }

    private void LateUpdate()
    {
        bool selfGuidedTourActive = tourNavigation.SelfGuidedTourActive;
        Transform greetingPanel = GetOpenGreetingPanel();
        if (selfGuidedTourActive != wasSelfGuidedTourActive || greetingPanel != narrationPanel)
        {
            StopNarration();
            wasSelfGuidedTourActive = selfGuidedTourActive;
            narrationPanel = greetingPanel;
            nextScan = 0;
        }

        if (selfGuidedTourActive || !speechToggle.isOn ||
            volume == 0 || Time.unscaledTime < nextScan)
            return;

        nextScan = Time.unscaledTime + scanInterval;
        visibleText.Clear();
        captions.Clear();
        var mainCamera = Camera.main;
        if (greetingPanel != null)
        {
            greetingPanel.GetComponentsInChildren(false, greetingText);
            greetingPanel.GetComponentsInChildren(false, legacyGreetingText);
            foreach (var label in greetingText)
                Collect(label, label.text, label.richText, mainCamera);
            foreach (var label in legacyGreetingText)
                Collect(label, label.text, label.supportRichText, mainCamera);
        }
        else
        {
            foreach (var label in FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))
                Collect(label, label.text, label.richText, mainCamera);
            foreach (var label in FindObjectsByType<UnityEngine.UI.Text>(FindObjectsSortMode.None))
                Collect(label, label.text, label.supportRichText, mainCamera);
        }

        if (speaking.Text != null && !IsStillVisible(speaking.Id, speaking.Text))
        {
            StopDeviceSpeech();
            speaking = default;
        }

        RemoveHiddenAnnouncements();

        if (DeviceIsSpeaking())
            return;

        speaking = default;
        captions.Sort((a, b) =>
        {
            int priority = b.IsExitHint.CompareTo(a.IsExitHint);
            if (priority != 0) return priority;
            int row = b.Position.y.CompareTo(a.Position.y);
            return row != 0 ? row : a.Position.x.CompareTo(b.Position.x);
        });

        foreach (var caption in captions)
        {
            if (announcedText.ContainsKey(caption.Id))
                continue;

            announcedText[caption.Id] = caption.Text;
            speaking = caption;
            exitHintAnnounced |= caption.IsExitHint;
            SpeakOnDevice(caption.Text, Mathf.Clamp(speed, 0.5f, 3f));
            break;
        }
    }

    private Transform GetOpenGreetingPanel()
    {
        for (int i = fullScreenIntroPanels.Length - 1; i >= 0; i--)
        {
            var panel = fullScreenIntroPanels[i];
            if (panel != null && panel.gameObject.activeInHierarchy)
                return panel;
        }

        return null;
    }

    private void RemoveHiddenAnnouncements()
    {
        hiddenIds.Clear();
        foreach (var previous in announcedText)
        {
            if (!IsStillVisible(previous.Key, previous.Value))
                hiddenIds.Add(previous.Key);
        }
        foreach (int id in hiddenIds)
            announcedText.Remove(id);
    }

    private bool IsStillVisible(int id, string text)
    {
        return visibleText.TryGetValue(id, out string current) && current == text;
    }

    private void Collect(UnityEngine.UI.Graphic label, string text, bool richText, Camera mainCamera)
    {
        bool isExitHint = label == exitHint;
        if (isExitHint && (!tourNavigation.SpiralTourActive ||
            (exitHintAnnounced && speaking.Id != label.GetInstanceID())))
            return;

        if (!label.isActiveAndEnabled || string.IsNullOrWhiteSpace(text) ||
            label.color.a <= 0 ||
            (narrationPanel != null && !label.transform.IsChildOf(narrationPanel)) ||
            (domeWallPreviews != null && label.transform.IsChildOf(domeWallPreviews)) ||
            label.GetComponentInParent<UnityEngine.UI.Button>() != null ||
            label.GetComponentInParent<UnityEngine.UI.Toggle>() != null ||
            label.GetComponentInParent<UnityEngine.UI.Slider>() != null ||
            (sliderTitles != null && label.transform.IsChildOf(sliderTitles)))
            return;

        var canvas = label.canvas;
        Camera camera = mainCamera;
        if (canvas != null)
        {
            var root = canvas.rootCanvas;
            if (!canvas.isActiveAndEnabled || !root.isActiveAndEnabled ||
                label.canvasRenderer.cull || label.canvasRenderer.GetInheritedAlpha() <= 0)
                return;
            camera = root.renderMode == RenderMode.ScreenSpaceOverlay ? null : root.worldCamera ?? mainCamera;
            if (root.renderMode != RenderMode.ScreenSpaceOverlay && camera == null)
                return;
        }
        else if (camera == null || !label.TryGetComponent<Renderer>(out var renderer) || !renderer.enabled)
        {
            return;
        }

        label.rectTransform.GetWorldCorners(corners);
        Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        bool inFront = camera == null;
        foreach (Vector3 corner in corners)
        {
            Vector3 point = camera == null ? corner : camera.WorldToScreenPoint(corner);
            inFront |= camera != null && point.z >= camera.nearClipPlane && point.z <= camera.farClipPlane;
            min = Vector2.Min(min, point);
            max = Vector2.Max(max, point);
        }

        Rect viewport = camera == null ? new Rect(0, 0, Screen.width, Screen.height) : camera.pixelRect;
        if (!inFront || min.x >= max.x || min.y >= max.y || !viewport.Overlaps(Rect.MinMaxRect(min.x, min.y, max.x, max.y)))
            return;

        if (richText && text.IndexOf('<') >= 0)
            text = RichText.Replace(text, match => match.Value.StartsWith("<br", System.StringComparison.OrdinalIgnoreCase) ? " " : "");
        text = text.Trim();
        if (text.Length == 0)
            return;

        int id = label.GetInstanceID();
        visibleText[id] = text;
        captions.Add(new Caption { Id = id, Text = text, Position = new Vector2(min.x, max.y), IsExitHint = isExitHint });
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void SpeakOnDevice(string text, float rateMultiplier);
    [DllImport("__Internal")] private static extern void StopDeviceSpeech();
    [DllImport("__Internal")] private static extern int TTSDeviceBusy();
    [DllImport("__Internal")] private static extern void SetDeviceVolume(float value);

    private static bool DeviceIsSpeaking() => TTSDeviceBusy() != 0;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    private const string ObjC = "/usr/lib/libobjc.A.dylib";
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    private System.IntPtr synthesizer;
    private float defaultDeviceRate;

    [DllImport(ObjC)] private static extern System.IntPtr objc_getClass(string name);
    [DllImport(ObjC)] private static extern System.IntPtr sel_registerName(string name);
    [DllImport(ObjC, EntryPoint = "objc_msgSend")] private static extern System.IntPtr Send(System.IntPtr target, System.IntPtr selector);
    [DllImport(ObjC, EntryPoint = "objc_msgSend")] private static extern void SendVoid(System.IntPtr target, System.IntPtr selector);
    [DllImport(ObjC, EntryPoint = "objc_msgSend")] private static extern void SendFloat(System.IntPtr target, System.IntPtr selector, float value);
    [DllImport(ObjC, EntryPoint = "objc_msgSend")] private static extern float SendRate(System.IntPtr target, System.IntPtr selector);
    [DllImport(ObjC, EntryPoint = "objc_msgSend")] private static extern byte SendBool(System.IntPtr target, System.IntPtr selector);
    [DllImport(ObjC, EntryPoint = "objc_msgSend")] private static extern byte SendString(System.IntPtr target, System.IntPtr selector, System.IntPtr value);
    [DllImport(CoreFoundation)] private static extern System.IntPtr CFStringCreateWithCString(System.IntPtr allocator, [MarshalAs(UnmanagedType.LPUTF8Str)] string text, uint encoding);
    [DllImport(CoreFoundation)] private static extern void CFRelease(System.IntPtr value);

    private void SpeakOnDevice(string text, float rateMultiplier)
    {
        if (synthesizer == System.IntPtr.Zero)
            return;
        System.IntPtr value = CFStringCreateWithCString(System.IntPtr.Zero, text, 0x08000100);
        if (value == System.IntPtr.Zero)
            return;
        try
        {
            SendFloat(synthesizer, sel_registerName("setRate:"), defaultDeviceRate * rateMultiplier);
            SendString(synthesizer, sel_registerName("startSpeakingString:"), value);
        }
        finally
        {
            CFRelease(value);
        }
    }

    private void StopDeviceSpeech()
    {
        if (synthesizer != System.IntPtr.Zero)
            SendVoid(synthesizer, sel_registerName("stopSpeaking"));
    }

    private bool DeviceIsSpeaking()
    {
        return synthesizer != System.IntPtr.Zero && SendBool(synthesizer, sel_registerName("isSpeaking")) != 0;
    }

    private void SetDeviceVolume(float value)
    {
        if (synthesizer == System.IntPtr.Zero)
        {
            var speechClass = objc_getClass("NSSpeechSynthesizer");
            synthesizer = Send(Send(speechClass, sel_registerName("alloc")), sel_registerName("init"));
            if (synthesizer != System.IntPtr.Zero)
                defaultDeviceRate = SendRate(synthesizer, sel_registerName("rate"));
        }
        if (synthesizer != System.IntPtr.Zero)
            SendFloat(synthesizer, sel_registerName("setVolume:"), value);
    }
#else
    private static void SpeakOnDevice(string text, float rateMultiplier) { }
    private static void StopDeviceSpeech() { }
    private static bool DeviceIsSpeaking() => false;
    private static void SetDeviceVolume(float value) { }
#endif
}
