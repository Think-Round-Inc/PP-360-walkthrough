/*
*   Name: DomeWall.cs
*   Description: Placed on each DomeWall and manages that specific wall
*/

using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public sealed class DomeWallSpriteGroup
{
    public Sprite[] groupSprites;
    public string[] groupTitles;
    [TextArea(0, 20)] public string[] groupInfo;
    [TextArea(0, 10)] public string[] groupShortInfo;
}

public sealed class DomeWall : MonoBehaviour
{
    [SerializeField] private DomeWallSpriteGroup artGroup;
    [SerializeField] private DomeWallSpriteGroup religionGroup;

    [SerializeField] private TMP_Text infoText;

    [SerializeField] private UnityEvent onDomeWallSwitched;

    private Sprite currentSprite;
    private string currentTitle;
    private string currentInfo;
    private Renderer wallRenderer;
    private int currentIndex = -1;

    private void Awake()
    {
        wallRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        ResetIndex();
        SwitchWallSprite();
    }

    public string GetCurrentTitle() => currentTitle;
    public string GetCurrentInfo() => currentInfo;
    public Sprite GetCurrentSprite() => currentSprite;
    public void ResetIndex() => currentIndex = -1;
    public void SwitchWallSprite() => SwitchWallSprite(1);
    public void PreviousImage() => SwitchWallSprite(-1);
    
    private void SwitchWallSprite(int direction)
    {
        DomeWallState state = DomeWallChanger.CurrentDomeWallState;
        if (state == DomeWallState.Off)
        {
            ClearWallImage();
            return;
        }

        if (SwitchFromGroup(GetSpriteGroup(state), direction)) { onDomeWallSwitched?.Invoke(); }
    }

    private bool SwitchFromGroup(DomeWallSpriteGroup group, int direction)
    {
        if (wallRenderer == null || group == null || group.groupSprites == null || group.groupSprites.Length == 0)
        {
            return false;
        }

        int count = group.groupSprites.Length;

        if (currentIndex < 0 || currentIndex >= count)
        {
            currentIndex = direction > 0 ? 0 : count - 1;
        }
        else
        {
            currentIndex = (currentIndex + direction + count) % count;
        }

        currentSprite = group.groupSprites[currentIndex];
        currentTitle = GetText(group.groupTitles, currentIndex);
        currentInfo = GetText(group.groupInfo, currentIndex);

        if (infoText != null){ infoText.text = GetText(group.groupShortInfo, currentIndex); }

        wallRenderer.material.mainTexture = currentSprite != null ? currentSprite.texture : null;

        return true;
    }

    private static string GetText(string[] values, int index)
    {
        return values != null && index >= 0 && index < values.Length
            ? values[index] ?? string.Empty
            : string.Empty;
    }

    internal void ClearWallImage()
    {
        currentSprite = null;
        currentTitle = string.Empty;
        currentInfo = string.Empty;

        if (infoText != null) { infoText.text = string.Empty; }
        if (wallRenderer != null) { wallRenderer.material.mainTexture = Texture2D.blackTexture; }
    }

    public DomeWallSpriteGroup GetSpriteGroup(DomeWallState state)
    {
        switch (state)
        {
            case DomeWallState.Religion: return religionGroup;
            case DomeWallState.Art: return artGroup;
            default: return null;
        }
    }
}
