using TMPro;
using UnityEngine;
using UnityEngine.Events;

// sealed as its not intended to be inherited
public sealed class ExitController : MonoBehaviour
{
    [SerializeField] KeyCode exitKey = KeyCode.Escape;
    [SerializeField] TMP_Text exitText;
    [SerializeField, Tooltip("Fired when exit key is pressed")] UnityEvent onExitKeyPressed;
    bool exitEventFired;

    private void Start()
    {
        if (exitText == null) return;
        exitText.text = $"Press '{exitKey}' to Exit";
    }

    private void Update()
    {
        // checks for key press then fires exit event once
        if (!Input.GetKeyDown(exitKey)) return;
        if (!exitEventFired)
        {
            exitEventFired = true;
            onExitKeyPressed?.Invoke();
        }
    }

    /// <summary>
    /// Resets exit event in case you ever want to, this makes sure its not fired multiple times per frame
    /// </summary>
    public void ResetEventFired() => exitEventFired = false;

    /// <summary>
    /// Quits the application, called in event, but can be changed so a confirmation panel pops up then gets called by button
    /// </summary>
    public void QuitApplication() => Application.Quit();
}
