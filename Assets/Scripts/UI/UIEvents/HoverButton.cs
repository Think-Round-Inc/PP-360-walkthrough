using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;

    public void OnPointerEnter(PointerEventData eventData) => onPointerEnter?.Invoke();

    public void OnPointerExit(PointerEventData eventData) => onPointerExit?.Invoke();
}
