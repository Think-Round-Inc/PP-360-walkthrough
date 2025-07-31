using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonAssistant : MonoBehaviour
{
    [SerializeField] UnityEvent onClickedOn;
    [SerializeField] UnityEvent onClickedOff;
    bool currentState = false;
    Button button;

    void Awake() => button = GetComponent<Button>();

    public void ButtonClicked()
    {
        if (currentState)
        {
            currentState = false;
            onClickedOff?.Invoke();
        }
        else
        {
            currentState = true;
            onClickedOn?.Invoke();
        }
    }

    public void DeselectButton()
    {
        if (button == null) return;
        button.OnDeselect(null);
    }
}
