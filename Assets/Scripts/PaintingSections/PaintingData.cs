using UnityEngine;

[System.Serializable]
public sealed class PaintingDataHolder
{
    public Sprite paintingImage;
    public string paintingName;
    [TextArea(0, 10)] public string extraPaintingInfo;
    public AudioClip paintingClip;
    [HideInInspector] public Color paintingTextureColor = Color.white;
}

[System.Serializable]
public sealed class PaintingData : MonoBehaviour
{
    public PaintingDataHolder paintingData;

    public void InitializePaintingData()
    {
        if (!TryGetComponent<Renderer>(out var screenRenderer)) return;
        var sprite = paintingData?.paintingImage;
        var material = screenRenderer.material;
        material.mainTexture = sprite != null ? sprite.texture : null;
        material.color = sprite != null ? paintingData.paintingTextureColor : SectionColorHolder.EmptyScreenColor;
    }
}
