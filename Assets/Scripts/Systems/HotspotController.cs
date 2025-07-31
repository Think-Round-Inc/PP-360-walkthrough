using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// sealed as its not intended to be inherited
[System.Serializable]
public sealed class HotspotController : MonoBehaviour
{
    [SerializeField] GameObject viewer; // player or camera parent

    [SerializeField] GameObject hotspotIcon;

    [SerializeField, Tooltip("Tint of hotspot icon after first visit")] Color hotspotVisitedColor = Color.gray;

    [SerializeField, Tooltip("Anything above this height will then ignore its height and just use height of floor, this way things above viewer will show up, or else they will usually be too far away to show up")] float distanceAbovePlayer = 2f;

    [SerializeField, Tooltip("Max distance from hotspot the player needs to be for it to show up")] float maxDistanceFromHotspot = 20f;

    [SerializeField, Tooltip("Max distance the hotspots above player can be before they are visible")] float maxDistanceFromHotspotAbovePlayer = 100f;

    [SerializeField, Tooltip("Dot product when facing screen, 1 completely facing, 0 facing away")] float visibilityThreshold = 0.5f;

    [SerializeField, Tooltip("Info Text GameObject that will hold the hotspot info")] TMP_Text infoText;

    [SerializeField] Camera mainCamera;

    private List<HotSpot> hotSpots;

    private GameObject closestHotspot;

    private void Start()
    {
        // finds all GameObjects with HotSpot script attached
        hotSpots = FindObjectsOfType<HotSpot>().ToList();

        // if main camera is not set, sets it here to main camera
        if (mainCamera == null)
            mainCamera = Camera.main;

        // hides hotspot icon at start in case the player is not near a hotspot
        HideHotspotIcon();
    }

    /// <summary>
    /// Method to set the painting data information to the infoText
    /// </summary>
    /// <param name="screen">screen that the painting is shown on</param>
    public void SetPaintingNameAndInfoToInfoText(GameObject screen)
    {
        if (screen.TryGetComponent(out PaintingData data))
            infoText.text = $"{data.paintingData.paintingName}\n{data.paintingData.extraPaintingInfo}";
    }

    /// <summary>
    /// Clears infoText
    /// </summary>
    public void ClearInfoPanelText() => infoText.text = string.Empty;

    /// <summary>
    /// Returns closest hotspot GameObject
    /// </summary>
    /// <returns>closest hotspot GameObject</returns>
    public GameObject GetClosestHotspotGameObject() => closestHotspot;

    /// <summary>
    /// Triggers the current hotspots clicked events
    /// </summary>
    public void HotSpotClickedEventTriggered()
    {
        if (closestHotspot == null) return;
        // checks currentHotspot GameObject for hotspot script then activates its clicked events
        if (closestHotspot.TryGetComponent(out HotSpot hotSpot))
            hotSpot.onClickedHotspot?.Invoke(closestHotspot);
    }

    private void Update()
    {
        float closestDistance = float.MaxValue;
        closestHotspot = null;

        // store forward direction of main camera
        Vector3 cameraForward = mainCamera.transform.forward;

        // iterate through all found hotspots (found at start)
        for (int i = 0; i < hotSpots.Count; i++)
        {
            if (hotSpots[i] == null) continue;
            float currentMaxDistance;

            // check if hotspot above player or not and uses that max distance for hotspot icon distance check
            if (hotSpots[i].transform.position.y + hotSpots[i].hotspotIconOffset.y > distanceAbovePlayer)
                currentMaxDistance = maxDistanceFromHotspotAbovePlayer;
            else
                currentMaxDistance = maxDistanceFromHotspot;

            // store hotspot location and offset
            Vector3 hotspotPoint = hotSpots[i].transform.position + hotSpots[i].hotspotIconOffset;

            // store direction from viewer to hotspot
            Vector3 cameraToHotspot = (hotspotPoint - mainCamera.transform.position).normalized;

            // store dot prodect to see if camera forward direction is similar to screen direction
            float dotProduct = Vector3.Dot(cameraForward, cameraToHotspot);

            // store distance from hotspot setting hotspot height to same as viewers (removing height check)
            float distance = Vector3.Distance(new Vector3(hotspotPoint.x, viewer.transform.position.y, hotspotPoint.z), viewer.transform.position);

            // use all stored data to set closest hotspot GameObject for use
            if (dotProduct > visibilityThreshold && distance < currentMaxDistance && distance < closestDistance)
            {
                closestDistance = distance;
                closestHotspot = hotSpots[i].gameObject;
            }
        }

        for (int i = 0; i < hotSpots.Count; i++)
        {
            if (hotSpots[i] != null)
                hotSpots[i].gameObject.tag = "Screen";
        }

        // if there is a closest hotspot, move icon to that location or else hide it
        if (closestHotspot != null)
        {
            MoveIconToHotspot(closestHotspot);
            closestHotspot.tag = "ClosestHotspot";
        }
        else
        {
            HideHotspotIcon();
        }
    }

    /// <summary>
    /// Sets hotspot icon GameObject inactive
    /// </summary>
    public void HideHotspotIcon() => hotspotIcon.SetActive(false);

    /// <summary>
    /// Sets the hotspot as seen by viewer
    /// </summary>
    public void SetHotspotAsSeen()
    {
        if (closestHotspot == null) return;
        // checks closets hotspot GameObject for hotspot script then sets it as 'has been seen'
        if (closestHotspot.TryGetComponent(out HotSpot hotspot))
            hotspot.hasSeenBefore = true;
    }

    /// <summary>
    /// Moves the hotspot icon to hotspot location
    /// </summary>
    /// <param name="targetHotspot">Hotspot target GameObject</param>
    public void MoveIconToHotspot(GameObject targetHotspot)
    {
        if (hotspotIcon == null || targetHotspot == null) return;

        if (targetHotspot.TryGetComponent(out HotSpot hotSpot))
        {
            // gets image of icon
            Image hotspotIconImage = hotspotIcon.GetComponent<Image>();

            // sets icon to color if it has been visited upon reaching target hotspot
            if (hotSpot.hasSeenBefore && hotspotIconImage != null)
                hotspotIconImage.color = hotspotVisitedColor;
            else
                hotspotIconImage.color = Color.white;

            // get world space location of hotspot + offset and converts it to screen space and set icon to that position
            Vector3 hotspotScreenPos = mainCamera.WorldToScreenPoint(targetHotspot.transform.position + hotSpot.hotspotIconOffset);
            hotspotIcon.transform.position = hotspotScreenPos;
            hotspotIcon.SetActive(true);
        }
    }

    /// <summary>
    /// Used for debugging, can be added to event list for a custom message to fire
    /// </summary>
    /// <param name="message">Message to print to the console</param>
    public void PrintMessage(string message) => print($"{message}");
}
