/*
*   Name: DomeTourManager.cs
*   Description: Gets DomeTour Information from DomeWall.cs
*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DomeTourManager : MonoBehaviour
{
    internal sealed class TourSlide
    {
        public readonly string Title;
        public readonly string Description;
        public readonly Sprite Image;
        public readonly string ImageTitle;
        public readonly string ImageDescription;

        public TourSlide(
            string title,
            string description,
            Sprite image,
            string imageTitle = "",
            string imageDescription = "")
        {
            Title = title;
            Description = description;
            Image = image;
            ImageTitle = imageTitle;
            ImageDescription = imageDescription;
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

    [Tooltip("Location Order: Africa, Europe, Asia, North America, South America, Australia, Antarctica.")]
    [SerializeField] private Sprite[] continentSprites = new Sprite[ArtTitles.Length];

    private DomeTourPanel Panel => GetComponent<DomeTourPanel>();

    public bool SlideshowComplete => Panel != null && Panel.SlideshowComplete;

    // Preserve existing button bindings and the slideshow start on activation.
    private void OnEnable() => Panel?.OpenOnEnable();
    public void DomeSlideshow() => Panel?.Open();
    public void NextSlide() => Panel?.NextSlide();
    public void PreviousSlide() => Panel?.PreviousSlide();
    public void EndTour() => Panel?.Close();

    internal void OpenForVisit(GameObject visitPanel) => Panel?.OpenForVisit(visitPanel);

    internal List<TourSlide> BuildSlides()
    {
        var slides = new List<TourSlide>();

        DomeWall[] walls = FindObjectsOfType<DomeWall>(true);

        var religionEntries = ReadArtwork(walls, DomeWallState.Religion, "Group");
        foreach (string title in ReligionTitles)
        {
            AddSection(slides, religionEntries, DomeWallState.Religion, "Group", title, null);
        }

        var artEntries = ReadArtwork(walls, DomeWallState.Art, "Continent");

        for (int index = 0; index < ArtTitles.Length; index++)
        {
            Sprite map = continentSprites != null && index < continentSprites.Length
                ? continentSprites[index]
                : null;

            AddSection(slides, artEntries, DomeWallState.Art, "Continent", ArtTitles[index], map);
        }

        return slides;
    }

    private static List<(Sprite Sprite, string Section, string Title, string Info, string ShortInfo)>
        ReadArtwork(DomeWall[] walls, DomeWallState state, string metadataLabel)
    {
        var entries = new List<(Sprite Sprite, string Section, string Title, string Info, string ShortInfo)>();
        foreach (DomeWall wall in walls)
        {
            DomeWallSpriteGroup group = wall.GetSpriteGroup(state);
            if (group?.groupSprites == null) { continue; }

            for (int index = 0; index < group.groupSprites.Length; index++)
            {
                Sprite sprite = group.groupSprites[index];
                if (sprite == null) { continue; }

                string section = ReadMetadata(group.groupInfo, index, metadataLabel);
                if (string.IsNullOrWhiteSpace(section))
                {
                    section = ReadMetadata(group.groupShortInfo, index, metadataLabel);
                }

                if (string.IsNullOrWhiteSpace(section)) { continue; }

                entries.Add((sprite, section,
                    GetEntryText(group.groupTitles, index),
                    GetEntryText(group.groupInfo, index),
                    GetEntryText(group.groupShortInfo, index)));
            }
        }

        return entries;
    }

    private void AddSection(
        List<TourSlide> slides,
        List<(Sprite Sprite, string Section, string Title, string Info, string ShortInfo)> entries,
        DomeWallState state,
        string metadataLabel,
        string title,
        Sprite introImage)
    {
        string[] aliases = GetTitleAliases(title);
        var artwork = new List<(Sprite Sprite, string Title, string Info)>();
        var addedSprites = new HashSet<Sprite>();
        string description = string.Empty;

        foreach (var entry in entries)
        {
            if (!MatchesSection(entry.Section, title, aliases)) { continue; }

            if (string.IsNullOrWhiteSpace(description))
            {
                description = entry.ShortInfo.Trim();
            }

            if (entry.Sprite != introImage && addedSprites.Add(entry.Sprite))
            {
                artwork.Add((entry.Sprite, entry.Title, entry.Info));
            }
        }

        if (introImage != null) { slides.Add(new TourSlide(title, description, introImage)); }

        foreach (var entry in artwork)
        {
            slides.Add(new TourSlide(title, description, entry.Sprite, entry.Title, entry.Info));
        }

        if (artwork.Count > 0) { return; }

        Debug.LogWarning(
            $"No {state} artwork matched '{title}' using {metadataLabel} metadata.",
            this);

        if (introImage == null) { slides.Add(new TourSlide(title, description, null)); }
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
        if (string.Equals(value, title, StringComparison.OrdinalIgnoreCase)) { return true; }

        foreach (string alias in aliases)
        {
            if (string.Equals(value, alias, StringComparison.OrdinalIgnoreCase)) { return true; }
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
                return new[] { "Buddhism", "Jainism", "Buddhism/Jainism", "Buddhism and Jainism", "Tibetan Buddhism" };
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
                return new[] { "Antartica/Polar Region" };
            default:
                return Array.Empty<string>();
        }
    }

    private static string GetEntryText(string[] entries, int index)
    {
        return entries != null && index >= 0 && index < entries.Length
            ? entries[index] ?? string.Empty
            : string.Empty;
    }
}
