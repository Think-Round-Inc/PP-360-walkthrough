using UnityEngine;
using UnityEngine.Events;

public class SwitchEvent : MonoBehaviour
{
    [SerializeField] bool currentState;
    [SerializeField] UnityEvent onSwitchedOn;
    [SerializeField] UnityEvent onSwitchedOff;

    /// <summary>
    /// switch state, on or off depending on last state and throws UnityEvent based on state switched to
    /// </summary>
    public void Switch()
    {
        if (currentState)
        {
            currentState = false;
            onSwitchedOff.Invoke();
        }
        else
        {
            currentState = true;
            onSwitchedOn.Invoke();
        }
    }
}
