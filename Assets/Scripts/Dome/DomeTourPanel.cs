/*
*   Name: DomeTourPanel.cs
*   Description: Populates the DomeTourPanel in the guided tour.
*/

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(DomeTourManager))]
public sealed class DomeTourPanel : MonoBehaviour
{
    [SerializeField] private GameObject tourPanel;
    [SerializeField] private TMP_Text sectionTitle;
    [SerializeField] private TMP_Text sectionDescription;
    [SerializeField] private Image exhibitImage;
    [SerializeField] private TMP_Text imageTitle;
    [SerializeField] private TMP_Text imageDescription;

    private List<DomeTourManager.TourSlide> slides;
    private int currentSlideIndex;
    private bool tourRunning;
    private bool openingForVisit;

    public bool SlideshowComplete { get; private set; }

    internal void OpenOnEnable()
    {
        if (!openingForVisit) { Open(); }
    }

    internal void OpenForVisit(GameObject visitPanel)
    {
        openingForVisit = true;
        try
        {
            Open();
            visitPanel.SetActive(true);
        }
        finally
        {
            openingForVisit = false;
        }
    }

    public void Open()
    {
        slides = GetComponent<DomeTourManager>().BuildSlides();
        currentSlideIndex = 0;
        tourRunning = slides.Count > 0;
        SlideshowComplete = false;

        if (!tourRunning) { return; }

        ShowCurrentSlide();

        if (tourPanel != null) { tourPanel.SetActive(true); }
    }

    public void NextSlide() => MoveBy(1);
    public void PreviousSlide() => MoveBy(-1);

    private void MoveBy(int step)
    {
        if (!tourRunning) { return; }

        int count = slides.Count;
        currentSlideIndex = (currentSlideIndex + step + count) % count;
        ShowCurrentSlide();
    }

    public void Close()
    {
        tourRunning = false;
        SlideshowComplete = true;

        if (tourPanel != null) { tourPanel.SetActive(false); }
    }

    private void ShowCurrentSlide()
    {
        DomeTourManager.TourSlide slide = slides[currentSlideIndex];

        if (sectionTitle != null) { sectionTitle.text = slide.Title; }

        if (sectionDescription != null) { sectionDescription.text = slide.Description; }

        if (exhibitImage != null)
        {
            exhibitImage.sprite = slide.Image;
            exhibitImage.enabled = slide.Image != null;
        }

        if (imageTitle != null) { imageTitle.text = slide.ImageTitle; }

        if (imageDescription != null) { imageDescription.text = slide.ImageDescription; }
    }
}
