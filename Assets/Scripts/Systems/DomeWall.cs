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
    [SerializeField] DomeWallSpriteGroup artGroup;
    [SerializeField] DomeWallSpriteGroup religionGroup;
    [SerializeField] TMP_Text infoText;
    [SerializeField] UnityEvent onDomeWallSwitched;
    private Sprite currentSprite;
    private string currentTitle;
    private string currentInfo;
    private Renderer wallRenderer;
    private int currentIndex;

    private void Awake()
    {
        wallRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        ResetIndex();
        SwitchWallSprite();
        if (infoText != null)
            infoText.text = string.Empty;
    }

    public string GetCurrentTitle() => currentTitle;

    public string GetCurrentInfo() => currentInfo;

    public Sprite GetCurrentSprite() => currentSprite;

    public void ResetIndex() => currentIndex = -1;

    public void SwitchWallSprite()
    {
        DomeWallState currentWallState = DomeWallChanger.CurrentDomeWallState;
        switch (currentWallState)
        {
            case DomeWallState.Art:
                SwitchFromGroup(artGroup);
                break;
            case DomeWallState.Religion:
                SwitchFromGroup(religionGroup);
                break;
        }
        onDomeWallSwitched?.Invoke();
    }

    void SwitchFromGroup(DomeWallSpriteGroup spriteGroup)
    {
        if (wallRenderer == null) return;
        currentIndex++;
        if (currentIndex >= spriteGroup.groupSprites.Length)
            currentIndex = 0;
        if (spriteGroup.groupSprites.Length > 0)
            currentSprite = spriteGroup.groupSprites[currentIndex];
        if (spriteGroup.groupTitles.Length > 0 && currentIndex < spriteGroup.groupTitles.Length)
            currentTitle = spriteGroup.groupTitles[currentIndex];
        if (spriteGroup.groupInfo.Length > 0 && currentIndex < spriteGroup.groupInfo.Length)
            currentInfo = spriteGroup.groupInfo[currentIndex];
        if (spriteGroup.groupShortInfo.Length > 0 && currentIndex < spriteGroup.groupShortInfo.Length)
            if (infoText != null)
                infoText.text = spriteGroup.groupShortInfo[currentIndex];
        if (currentSprite != null)
            wallRenderer.material.mainTexture = currentSprite.texture;
    }
}
