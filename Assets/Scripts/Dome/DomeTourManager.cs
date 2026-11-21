using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DomeTourManager : MonoBehaviour
{
    private sealed class TourSlide
    {
        public readonly string Title;
        public readonly string Description;
        public readonly Sprite Image;

        public TourSlide(string title, string description, Sprite image)
        {
            Title = title;
            Description = description;
            Image = image;
        }
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

    [Header("Tour panel")]
    [SerializeField] private GameObject tourPanel;
    [SerializeField] private TMP_Text DisplayTitle;
    [SerializeField] private TMP_Text DisplayDescription;
    [SerializeField] private Image exhibitImage;

    [Tooltip("Africa, Europe, Asia, North America, South America, Australia, Antarctica.")]
    [SerializeField] private Sprite[] continentSprites = new Sprite[ArtTitles.Length];

    private readonly List<TourSlide> slides = new();
    private int currentSlideIndex;
    private bool tourRunning;

    public bool SlideshowComplete { get; private set; }

    private void OnEnable()
    {
        DomeSlideshow();
    }

    public void DomeSlideshow()
    {
        BuildSlides();

        currentSlideIndex = 0;
        tourRunning = slides.Count > 0;
        SlideshowComplete = false;

        if (!tourRunning)
            return;

        ShowCurrentSlide();

        if (tourPanel != null)
            tourPanel.SetActive(true);
    }

    public void NextSlide()
    {
        if (!tourRunning)
            return;

        if (currentSlideIndex == slides.Count - 1)
        {
            EndTour();
            return;
        }

        currentSlideIndex++;
        ShowCurrentSlide();
    }

    public void PreviousSlide()
    {
        if (!tourRunning || currentSlideIndex == 0)
            return;

        currentSlideIndex--;
        ShowCurrentSlide();
    }

    public void EndTour()
    {
        tourRunning = false;
        SlideshowComplete = true;

        if (tourPanel != null)
            tourPanel.SetActive(false);
    }

    private void ShowCurrentSlide()
    {
        TourSlide slide = slides[currentSlideIndex];

        if (DisplayTitle != null)
            DisplayTitle.text = slide.Title;

        if (DisplayDescription != null)
            DisplayDescription.text = slide.Description;

        if (exhibitImage != null)
        {
            exhibitImage.sprite = slide.Image;
            exhibitImage.enabled = slide.Image != null;
        }
    }

  private void BuildSlides()
{
    slides.Clear();

    DomeWall[] walls = FindObjectsOfType<DomeWall>(true);

    foreach (string title in ReligionTitles)
        AddSection(walls, DomeWallState.Religion, "Group", title, null);

    for (int index = 0; index < ArtTitles.Length; index++)
    {
        Sprite map = continentSprites != null && index < continentSprites.Length
            ? continentSprites[index]
            : null;

        AddSection(walls, DomeWallState.Art, "Continent", ArtTitles[index], map);
    }
}

private void AddSection(
    DomeWall[] walls,
    DomeWallState state,
    string metadataLabel,
    string title,
    Sprite introImage)
{
    string[] aliases = GetTitleAliases(title);
    var artwork = new List<Sprite>();
    var addedSprites = new HashSet<Sprite>();
    string description = string.Empty;

    foreach (DomeWall wall in walls)
    {
        DomeWallSpriteGroup group = wall.GetSpriteGroup(state);
        if (group?.groupSprites == null)
            continue;

        for (int index = 0; index < group.groupSprites.Length; index++)
        {
            Sprite sprite = group.groupSprites[index];
            if (sprite == null)
                continue;

            string section = ReadMetadata(group.groupInfo, index, metadataLabel);
            if (string.IsNullOrWhiteSpace(section))
                section = ReadMetadata(group.groupShortInfo, index, metadataLabel);

            if (!MatchesSection(section, title, aliases))
                continue;

            if (string.IsNullOrWhiteSpace(description) &&
                group.groupShortInfo != null &&
                index < group.groupShortInfo.Length)
            {
                description = group.groupShortInfo[index]?.Trim() ?? string.Empty;
            }

            if (sprite != introImage && addedSprites.Add(sprite))
                artwork.Add(sprite);
        }
    }

    if (introImage != null)
        slides.Add(new TourSlide(title, description, introImage));

    foreach (Sprite sprite in artwork)
        slides.Add(new TourSlide(title, description, sprite));

    if (artwork.Count == 0)
    {
        Debug.LogWarning(
            $"No {state} artwork matched '{title}' using {metadataLabel} metadata.",
            this);

        if (introImage == null)
            slides.Add(new TourSlide(title, description, null));
    }
}
    private void AddSection(
        DomeWall[] walls,
        DomeWallState state,
        string metadataLabel,
        string title,
        string description,
        Sprite introImage)
    {
        int firstSlideIndex = slides.Count;

        if (introImage != null)
            slides.Add(new TourSlide(title, description, introImage));

        string[] aliases = GetTitleAliases(title);
        var addedSprites = new HashSet<Sprite>();

        foreach (DomeWall wall in walls)
        {
            DomeWallSpriteGroup group = wall.GetSpriteGroup(state);
            if (group?.groupSprites == null)
                continue;

            for (int index = 0; index < group.groupSprites.Length; index++)
{
    Sprite sprite = group.groupSprites[index];

    string section = ReadMetadata(group.groupInfo, index, metadataLabel);
    if (string.IsNullOrEmpty(section))
        section = ReadMetadata(group.groupShortInfo, index, metadataLabel);

    if (!MatchesSection(section, title, aliases))
        continue;

    string artworkTitle = group.groupTitles != null &&
                          index < group.groupTitles.Length
        ? group.groupTitles[index]
        : "(untitled)";

    if (sprite == null)
    {
        Debug.LogWarning(
            $"{wall.name} [{index}] '{artworkTitle}' matches '{title}' " +
            "but has an empty sprite slot.",
            wall);
        continue;
    }

    if (sprite == introImage || !addedSprites.Add(sprite))
    {
        Debug.LogWarning(
            $"{wall.name} [{index}] '{artworkTitle}' reuses sprite '{sprite.name}' " +
            $"in '{title}'.",
            wall);
        continue;
    }

    slides.Add(new TourSlide(title, description, sprite));
}
        }

        if (addedSprites.Count == 0)
        {
            Debug.LogWarning(
                $"No {state} artwork matched '{title}' using {metadataLabel} metadata.",
                this);
        }

        if (slides.Count == firstSlideIndex)
            slides.Add(new TourSlide(title, description, null));
    }

    private static string ReadMetadata(string[] entries, int index, string label)
    {
        if (entries == null || index >= entries.Length ||
            string.IsNullOrWhiteSpace(entries[index]))
        {
            return string.Empty;
        }

        string text = Regex.Replace(
            entries[index], @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"<[^>]+>", string.Empty);

        Match match = Regex.Match(
            text,
            $@"\b{Regex.Escape(label)}[ \t]*:[ \t]*([^\r\n]+)",
            RegexOptions.IgnoreCase);

        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }

    private static bool MatchesSection(string value, string title, string[] aliases)
    {
        if (string.Equals(value, title, StringComparison.OrdinalIgnoreCase))
            return true;

        foreach (string alias in aliases)
        {
            if (string.Equals(value, alias, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static string[] GetTitleAliases(string title)
    {
        switch (title)
        {
            case "Christians":
                return new[] { "Christianity", "Christian" };
            case "Jews":
                return new[] { "Judaism", "Jewish" };
            case "Buddhists/Jains":
                return new[] { "Buddhism", "Jainism", "Buddhism/Jainism", "Buddhism and Jainism",  "Tibetan Buddhism" };
            case "Hindus":
                return new[] { "Hinduism", "Hindu", "Sikhism", "Hinduism/Sikhism" };
            case "Taoists":
                return new[] { "Taoism", "Daoism", "Confucianism", "Confucianism/Daoism", "Confucianism/Taoism" };
            case "Indigenous":
                return new[] { "Indigenous Religions", "Aboriginal" };
            case "Muslims":
                return new[] { "Islam", "Islamic" };
            case "North America":
                return new[] { "NorthAmerica" };
            case "South America":
                return new[] { "SouthAmerica" };
            case "Antarctica":
    return new[]
    {
        "Antartica",
        "Antartica/Polar",
        "Antarctica/Polar Region",
        "Antartica/Polar Regions",
        "Antarctica/Polar Regions"
    };
            default:
                return Array.Empty<string>();
        }
    }
}