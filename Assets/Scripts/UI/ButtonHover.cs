using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private GameObject descriptionPanel;

    private Image buttonImage;
    private Color normalColor;
    private bool isHovered;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        normalColor = buttonImage.color;
        descriptionPanel?.SetActive(false);
    }

    public void SetNormalColor(Color color)
    {
        normalColor = color;
        UpdateColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateColor();
        descriptionPanel?.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        descriptionPanel?.SetActive(false);
    }

    private void OnDisable()
    {
        ResetHover();
    }

    private void ResetHover()
    {
        isHovered = false;
        UpdateColor();
        descriptionPanel?.SetActive(false);
    }

    private void UpdateColor()
    {
        if (buttonImage != null)
            buttonImage.color = isHovered ? hoverColor : normalColor;
    }
}