using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HotspotIcon : MonoBehaviour,
    IPointerExitHandler,
    IPointerEnterHandler,
    IPointerClickHandler
{
    public UnityEvent onPointerClicked;
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;

    [Header("Panels")]
    [SerializeField] private GameObject domePanel;
    [SerializeField] private GameObject zoomablePaintingPanel;
    [SerializeField] private GameObject earthPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClicked?.Invoke();

        if (domePanel != null && domePanel.activeSelf)
        {
            if (zoomablePaintingPanel != null)
                zoomablePaintingPanel.SetActive(false);

            if (earthPanel != null)
                earthPanel.SetActive(false);
        }
        else if (earthPanel != null && earthPanel.activeSelf)
        {
            if (zoomablePaintingPanel != null)
                zoomablePaintingPanel.SetActive(false);

            if (domePanel != null)
                domePanel.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke();
    }
}