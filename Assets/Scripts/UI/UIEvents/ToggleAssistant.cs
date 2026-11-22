using UnityEngine;
using UnityEngine.Events;

public class ToggleAssistant : MonoBehaviour
{
    [SerializeField] UnityEvent onToggleTurnedOn;
    [SerializeField] UnityEvent onToggleTurnedOff;

    public void ToggleEvent(bool value)
    {
        if (value)
            onToggleTurnedOn.Invoke();
        else
            onToggleTurnedOff?.Invoke();
    }
}
