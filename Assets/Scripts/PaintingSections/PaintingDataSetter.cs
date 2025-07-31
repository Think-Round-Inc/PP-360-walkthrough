using UnityEditor;
using UnityEngine;

[System.Serializable]
public sealed class PaintingDataSetter : MonoBehaviour
{
    [SerializeField, Tooltip("When TRUE, each change will update all data to all paintings while in the Editor, or else use button on bottom of script to set data to paintings when done.")]
    bool setAllDataWhenAValueChanges;
    [SerializeField] PaintingDataHolder[] paintingData;
    [SerializeField] GameObject[] screens;
    [SerializeField] Color paintingTextureColor;
    [SerializeField] Sprite missingPaintingTexture;

    private void Start()
    {
        SetPaintingData();
    }

    /// <summary>
    /// sets painting data from entered info to individual paintings in scene
    /// </summary>
    [ContextMenu(nameof(SetPaintingData))]
    public void SetPaintingData()
    {
        for (int i = 0; i < paintingData.Length; i++)
        {
            if (i >= screens.Length) return;
            if (paintingData[i].paintingImage != null && screens[i] != null)
            {
                if (screens[i].TryGetComponent(out PaintingData data))
                {
                    data.paintingData.paintingImage = paintingData[i].paintingImage;
                    if (paintingData[i].paintingName != string.Empty)
                    {
                        data.paintingData.paintingTextureColor = paintingTextureColor;
                        data.paintingData.paintingName = paintingData[i].paintingName;
                        data.paintingData.paintingClip = paintingData[i].paintingClip;
                        data.paintingData.extraPaintingInfo = paintingData[i].extraPaintingInfo;
                    }
                    else
                    {
                        if (missingPaintingTexture != null)
                            data.paintingData.paintingImage = missingPaintingTexture;
                        data.paintingData.paintingName = string.Empty;
                        data.paintingData.paintingTextureColor = paintingTextureColor;
                    }
#if UNITY_EDITOR
                    EditorUtility.SetDirty(screens[i]);
#endif
                }
            }
        }
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    private void OnValidate()
    {
        if (setAllDataWhenAValueChanges)
            SetPaintingData();
    }
}

