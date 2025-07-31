using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HotspotIcon : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    public UnityEvent onPointerClicked;
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;

    public void OnPointerClick(PointerEventData eventData) => onPointerClicked?.Invoke();

    public void OnPointerEnter(PointerEventData eventData) => onPointerEnter?.Invoke();

    public void OnPointerExit(PointerEventData eventData) => onPointerExit?.Invoke();
}