/*  Name: HotspotController.cs
*  Description: Finds the nearest hotspot in range, positions the icon, tints it (if visited),
*               dispatches the selected hotspot's click event and updates the plainting's title             
*/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HotspotController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameObject viewer; // player or camera parent

    [SerializeField] GameObject hotspotIcon;

    [SerializeField, Tooltip("Tint of hotspot icon after first visit")] Color hotspotVisitedColor = Color.gray;

    [Header("Detection")]
    [SerializeField, Tooltip("World-space height at which the overhead detection radius applies.")] float distanceAbovePlayer = 2f;

    [SerializeField, Tooltip("Max distance from hotspot the player needs to be for it to show up")] float maxDistanceFromHotspot = 20f;

    [SerializeField, Tooltip("Max distance the hotspots above player can be before they are visible")] float maxDistanceFromHotspotAbovePlayer = 100f;

    [SerializeField, Range(-1f, 1f), Tooltip("Dot product when facing screen, 1 completely facing, 0 facing away")] float visibilityThreshold = 0.5f;

    [SerializeField, Tooltip("Info Text GameObject that will hold the hotspot info")] TMP_Text infoText;

    [SerializeField] Camera mainCamera;

    private HotSpot[] hotSpots;
    private Image iconImage;
    private PaintingData closestPainting;

    private GameObject closestHotspot;

    private void Start()
    {
        // finds all GameObjects with HotSpot script attached
        hotSpots = FindObjectsOfType<HotSpot>();
        foreach (var hotspot in hotSpots)
            hotspot.gameObject.tag = "Screen";
        iconImage = hotspotIcon.GetComponent<Image>();

        // if main camera is not set, sets it here to main camera
        if (mainCamera == null)
            mainCamera = Camera.main;

        // hides hotspot icon at start in case the player is not near a hotspot
        HideHotspotIcon();
    }

    /// Method to set the painting data information to the infoText
    /// <param name="screen">screen that the painting is shown on</param>
   public void SetPaintingNameAndInfoToInfoText(GameObject screen)
    {
        if (infoText == null || screen == null) return;

        infoText.text = "";

        if (screen.TryGetComponent(out PaintingData data) &&
            data.paintingData != null)
        {
            infoText.enableAutoSizing = true;
            infoText.text = data.paintingData.paintingName;
        }
    }

    /// Clears infoText
    public void ClearInfoPanelText() => infoText.text = string.Empty;

    /// Returns closest hotspot GameObject
    /// <returns>closest hotspot GameObject</returns>
    public GameObject GetClosestHotspotGameObject() => closestHotspot;

    /// Triggers the current hotspots clicked events
    public void HotSpotClickedEventTriggered()
    {
        if (closestHotspot == null) return;

        // Keep the existing painting setup events.
        if (closestHotspot.TryGetComponent(out HotSpot hotSpot))
        {
            hotSpot.onClickedHotspot?.Invoke(closestHotspot);
        }

        // Update the title above the zoomed painting.
        SetPaintingNameAndInfoToInfoText(closestHotspot);
    }

    private void Update()
    {
        GameObject next = FindClosestHotspot();
        if (next != closestHotspot)
        {
            if (closestHotspot != null)
                closestHotspot.tag = "Screen";
            closestHotspot = next;
            closestPainting = null;
            if (next != null)
            {
                next.tag = "ClosestHotspot";
                next.TryGetComponent(out closestPainting);
            }
        }

        if (next == null)
        {
            HideHotspotIcon();
            return;
        }
        if (closestPainting != null && closestPainting.isActiveAndEnabled)
            closestPainting.EnsureImageLoaded();
        MoveIconToHotspot(next);
    }

    private GameObject FindClosestHotspot()
    {
        Vector3 viewerPosition = viewer.transform.position;
        Transform cameraTransform = mainCamera.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 forward = cameraTransform.forward;
        float nearestSquared = float.PositiveInfinity;
        GameObject nearest = null;
        foreach (var hotspot in hotSpots)
        {
            if (hotspot == null) continue;
            Vector3 point = hotspot.transform.position + hotspot.hotspotIconOffset;
            float radius = point.y > distanceAbovePlayer ? maxDistanceFromHotspotAbovePlayer : maxDistanceFromHotspot;
            float dx = point.x - viewerPosition.x;
            float dz = point.z - viewerPosition.z;
            float distanceSquared = dx * dx + dz * dz;
            if (radius <= 0 || distanceSquared >= radius * radius || distanceSquared >= nearestSquared)
                continue;
            if (Vector3.Dot(forward, (point - cameraPosition).normalized) <= visibilityThreshold)
                continue;
            nearestSquared = distanceSquared;
            nearest = hotspot.gameObject;
        }
        return nearest;
    }

    /// Sets hotspot icon GameObject inactive
    public void HideHotspotIcon() => hotspotIcon.SetActive(false);

    /// Sets the hotspot as seen by viewer
    public void SetHotspotAsSeen()
    {
        if (closestHotspot == null) return;
        // checks closets hotspot GameObject for hotspot script then sets it as 'has been seen'
        if (closestHotspot.TryGetComponent(out HotSpot hotspot))
            hotspot.hasSeenBefore = true;
    }

    /// Moves the hotspot icon to hotspot location
    /// <param name="targetHotspot">Hotspot target GameObject</param>
    public void MoveIconToHotspot(GameObject targetHotspot)
    {
        if (hotspotIcon == null || targetHotspot == null) return;

        if (targetHotspot.TryGetComponent(out HotSpot hotSpot))
        {
            if (iconImage != null)
                iconImage.color = hotSpot.hasSeenBefore ? hotspotVisitedColor : Color.white;

            // get world space location of hotspot + offset and converts it to screen space and set icon to that position
            Vector3 hotspotScreenPos = mainCamera.WorldToScreenPoint(targetHotspot.transform.position + hotSpot.hotspotIconOffset);
            hotspotIcon.transform.position = hotspotScreenPos;
            hotspotIcon.SetActive(true);
        }
    }

    /// Used for debugging, can be added to event list for a custom message to fire
    /// <param name="message">Message to print to the console</param>
    public void PrintMessage(string message) => print($"{message}");
}
