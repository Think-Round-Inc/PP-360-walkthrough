using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerAssistant : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UnityEvent onPointerClicked;
    public void OnPointerClick(PointerEventData eventData) => onPointerClicked?.Invoke();
}
