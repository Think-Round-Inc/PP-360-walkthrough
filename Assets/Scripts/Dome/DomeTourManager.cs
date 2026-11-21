using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DomeTourManager : MonoBehaviour
{
    [System.Serializable]
    private sealed class TourEntry
    {
        public DomeWall wall;
        [Min(0)] public int entryIndex;
        public Sprite continentImage;
    }

    private static readonly string[] ReligionTitles =
    {
        "Christians",
        "Jews",
        "Buddhists/Jains",
        "Hindus",
        "Taoists",
        "Indigenous",
        "Muslims"
    };

    private static readonly string[] ArtTitles =
    {
        "Africa",
        "Europe",
        "Asia",
        "North America",
        "South America",
        "Australia",
        "Antarctica"
    };

    [Header("Exhibits in tour order")]
    [SerializeField] private TourEntry[] religiousEntries =
        new TourEntry[ReligionTitles.Length];

    [SerializeField] private TourEntry[] artEntries =
        new TourEntry[ArtTitles.Length];

    [Header("Tour panel")]
    [SerializeField] private GameObject tourPanel;
    [SerializeField] private TMP_Text DisplayTitle;
    [SerializeField] private TMP_Text DisplayDescription;
    [SerializeField] private Image exhibitImage;
    [SerializeField] private Image continentImage;

    private int nextSlideIndex;
    private bool tourRunning;

    public bool SlideshowComplete { get; private set; }

    public void DomeSlideshow()
    {
        nextSlideIndex = 0;
        tourRunning = true;
        SlideshowComplete = false;

        NextSlide();

        if (tourPanel != null)
            tourPanel.SetActive(true);
    }

    public void NextSlide()
    {
        if (!tourRunning)
            return;

        if (nextSlideIndex >= ReligionTitles.Length + ArtTitles.Length)
        {
            EndTour();
            return;
        }

        int slideIndex = nextSlideIndex++;

        if (slideIndex < ReligionTitles.Length)
        {
            PopulatePanel(
                ReligionTitles[slideIndex],
                GetEntry(religiousEntries, slideIndex),
                DomeWallState.Religion);
            return;
        }

        int artIndex = slideIndex - ReligionTitles.Length;

        PopulatePanel(
            ArtTitles[artIndex],
            GetEntry(artEntries, artIndex),
            DomeWallState.Art);
    }

    public void EndTour()
    {
        tourRunning = false;
        SlideshowComplete = true;

        if (tourPanel != null)
            tourPanel.SetActive(false);
    }

    private void PopulatePanel(
        string title,
        TourEntry entry,
        DomeWallState state)
    {
        Sprite sprite = null;
        string description = string.Empty;

        if (entry != null && entry.wall != null)
        {
            DomeWallSpriteGroup group = entry.wall.GetSpriteGroup(state);
            int index = entry.entryIndex;

            if (group?.groupSprites != null &&
                index >= 0 &&
                index < group.groupSprites.Length)
            {
                sprite = group.groupSprites[index];

                if (group.groupInfo != null && index < group.groupInfo.Length)
                    description = group.groupInfo[index] ?? string.Empty;
            }
            else
            {
                Debug.LogWarning(
                    $"Invalid {state} exhibit index {index} for '{title}'.", this);
            }
        }
        else
        {
            Debug.LogWarning($"Assign a DomeWall for '{title}'.", this);
        }

        if (DisplayTitle != null)
            DisplayTitle.text = title;

        if (DisplayDescription != null)
            DisplayDescription.text = description;

        SetImage(exhibitImage, sprite);
        SetImage(
            continentImage,
            state == DomeWallState.Art ? entry?.continentImage : null);
    }

    private static TourEntry GetEntry(TourEntry[] entries, int index)
    {
        return entries != null && index < entries.Length ? entries[index] : null;
    }

    private static void SetImage(Image image, Sprite sprite)
    {
        if (image == null)
            return;

        image.sprite = sprite;
        image.enabled = sprite != null;
    }
}