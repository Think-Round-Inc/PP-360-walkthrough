using UnityEngine;

public static class SectionColorHolder
{
    public static Color EmptyScreenColor;
}

public enum SectionType
{
    Christians,
    Jews,
    Buddhists_Jains,
    Hindus_Sikhs,
    Taoists_Confucians,
    Indigenous,
    Muslims
}

[System.Serializable]
public sealed class Section : MonoBehaviour
{
    [SerializeField] SectionType sectionType;
    public Color missingPaintingScreenColor = Color.red;

    private void Start()
    {
        SectionColorHolder.EmptyScreenColor = missingPaintingScreenColor;
        SetAllPaintingTexturesFromPaintingData();
    }

    public SectionType GetCurrentSectionType() => sectionType;

    public void SetAllPaintingTexturesFromPaintingData()
    {
        PaintingData[] paintings = GetComponentsInChildren<PaintingData>();
        for (int i = 0; i < paintings.Length; i++)
        {
            paintings[i].InitializePaintingData();
        }
    }
}
