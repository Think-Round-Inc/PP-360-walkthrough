using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DomeTourManager : MonoBehaviour
{

    //There's a script somewhere that auto populates the dome panel according to the info required
    // I need to locate it and use it as a reference here so that I can auto populate descriptions, and panels.
    // I do believe the titles and contient images may need to be set up mannually sitll though.
    private readonly Dictionary<string, string> religiousSlides = new()
    {
        ["Christians"] = "Christianity, a major monotheistic religion with over 2 billion adherents, centers on Jesus Christ as the Son of God and Savior, with the Bible as its sacred text, and diverse denominations and practices.",
        ["Jews"] = "Judaism is an ancient monotheistic religion followed by Jews, emphasizing God's covenant with the Jewish people, as found in the Hebrew Bible (Tanakh), Talmud, and diverse traditions.",
        ["Buddhists/Jains"] = "Buddhism, followed by Buddhists, teaches the path to enlightenment through meditation and ethical living, based on the teachings of Siddhartha Gautama (Buddha). Jainism, practiced by Jains, emphasizes non-violence, truth, and asceticism, guided by the teachings of Mahavira.",
        ["Hindus"] = "Hinduism, followed by Hindus, is a diverse religion rooted in ancient Indian traditions with beliefs in karma, reincarnation, and a wide array of deities. Sikhism, practiced by Sikhs, teaches monotheism, equality, and devotion to one God, as per Guru Nanak's teachings.",
        ["Taoists"] = "Taoism, followed by Taoists, seeks harmony with the Tao (the Way) through simplicity and spontaneity. Confucianism, adhered to by Confucians, emphasizes ethics, filial piety, and social harmony, as taught by Confucius.",
        ["Indigenous"] = "Indigenous religions are diverse, rooted in native cultures worldwide, often emphasizing a spiritual connection to nature, ancestors, and unique cosmologies, with rituals and beliefs varying among different Indigenous communities.",
        ["Muslims"] = "Islam, followed by Muslims, is a monotheistic faith centered on the Quran, the final revelation from God to Prophet Muhammad. It emphasizes prayer, fasting, charity, and the pilgrimage to Mecca. Muslims number over a billion worldwide."
    };

    private readonly Dictionary<string, string> artSlides = new()
    {
        ["Africa"] = "Africa, the world's second-largest continent, is a cradle of human civilization and biodiversity. It features diverse cultures, languages, and landscapes, ranging from the Sahara Desert to the lush rainforests. Africa's rich history, music, art, and wildlife make it a continent of immense cultural and ecological significance.",
        ["Europe"] = "Europe is a continent known for its diverse cultures and peoples. It has a rich history of art, music, literature, and philosophy. From the Mediterranean to the Arctic, Europe's heritage influences global culture and politics.",
        ["Asia"] = "Asia, the world's largest continent, is a tapestry of cultures, religions, and languages. Its diversity ranges from the nomadic tribes of Central Asia to the bustling metropolises of East Asia. Asian heritage has profoundly shaped global history, philosophy, cuisine, and arts.",
        ["North America"] = "North America's culture is a mosaic, reflecting its diverse population, including Indigenous peoples, immigrants, and their descendants. It embraces a variety of traditions, languages, and cuisines, contributing to a rich and dynamic cultural landscape.",
        ["South America"] = "South America, a continent of stunning landscapes and cultures, showcases the Amazon rainforest, the Andes Mountains, and diverse ecosystems. Its people encompass indigenous communities and descendants of European, African, and Asian immigrants, contributing to a vibrant cultural tapestry.",
        ["Australia"] = "Australia, a continent and country, boasts a unique blend of Indigenous and immigrant cultures. Its multicultural society values outdoor living, sports, arts, and a laid-back lifestyle. The land Down Under offers diverse landscapes, from arid deserts to lush coastlines.",
        ["Antarctica"] = "Antarctica, Earth's southernmost continent, is an inhospitable, frozen wilderness with no native human population. It is primarily characterized by its vast ice sheets, extreme cold, and unique scientific research stations, making it a hub for international scientific cooperation."
    };

    [Header("Religious Section")]
    [SerializeField] private GameObject[] religiousPanels;

    [Header("Art History Section")]
    [SerializeField] private GameObject[] continentImages;
    [SerializeField] private GameObject[] artHistoryPanels;

    [Header("Display")]
    [SerializeField] private TextMeshProUGUI DisplayTitle;
    [SerializeField] private TextMeshProUGUI DisplayDescription;

    private IEnumerator slideshow;
    private GameObject visibleSlideObject;

    public bool SlideshowComplete { get; private set; }

    public void DomeSlideshow()
    {
        SlideshowComplete = false;
        slideshow = PlaySlides();
        NextSlide();
    }

    public void NextSlide()
    {
        if (slideshow == null)
            return;

        ClearSlide();

        if (!slideshow.MoveNext())
        {
            slideshow = null;
            SlideshowComplete = true;
        }
    }

    private IEnumerator PlaySlides()
    {
        int religiousIndex = 0;

        foreach (var slide in religiousSlides)
        {
            DisplayTitle.text = slide.Key;
            DisplayDescription.text = slide.Value;
            yield return null;

            ShowSlideObject(religiousPanels[religiousIndex]);
            yield return null;

            religiousIndex++;
        }

        int artIndex = 0;

        foreach (var slide in artSlides)
        {
            DisplayTitle.text = slide.Key;
            ShowSlideObject(continentImages[artIndex]);
            yield return null;

            DisplayDescription.text = slide.Value;
            yield return null;

            ShowSlideObject(artHistoryPanels[artIndex]);
            yield return null;

            artIndex++;
        }
    }

    private void ShowSlideObject(GameObject slideObject)
    {
        visibleSlideObject = slideObject;

        if (visibleSlideObject != null)
            visibleSlideObject.SetActive(true);
    }

    private void ClearSlide()
    {
        DisplayTitle.text = string.Empty;
        DisplayDescription.text = string.Empty;

        if (visibleSlideObject != null)
            visibleSlideObject.SetActive(false);

        visibleSlideObject = null;
    }
}