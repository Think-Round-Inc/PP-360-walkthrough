using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public sealed class SpiralTour : MonoBehaviour
{
    [Header("Dome Tour")]
    [SerializeField] private GameObject domeTourPanel;

    private const float DomePanelDelay = 0.5f;
    private const float DomeArrivalDistance = 0.1f;
    private const float DomePanelExitDistance = 1.0f;

    private Transform domeWaypoint;
    private float domePanelOpenTime;
    private bool domePanelOpened;
    
    private static readonly string[] TourPrefixes =
    {
        "Ch", "Je", "Bu", "Hi", "Ta", "In", "Mu", "Ea", "Do"
    };

    [Header("Settings")]
    
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotSpeed = 3f;
    [SerializeField] private float pauseDuration = 2f;

    [Header("Camera Angle Offset (Degrees)")]
    [Tooltip("X = tilt up/down, Y = turn left/right, Z = roll.")]
    [SerializeField] private Vector3 cameraAngleOffset;

    [Header("Playback")]
    private bool viewerControllerActive = true;
    private bool isPlaying = true;

    [Header("Target Lists")]
    [SerializeField] private List<GameObject> targetPositions = new List<GameObject>();
    [SerializeField] private List<GameObject> targetRotations = new List<GameObject>();

    [Header("Spawn Point")]
    [SerializeField] private Transform christiansSpawnPoint;

    private string targetTagViewingPoint = "ViewingPoint";
    private string targetTagPausePoint = "PausePoint";
    private string targetTagScreen = "Display";

    private int currentTargetIndex;
    private float currentTime;

    public void SetViewerControlsActive(bool value)
    {
        viewerControllerActive = value;
    }

    public void InitializeScript()
    {
        ClearDomeVisit();
        // Clear previous targets when starting a different tour.
        targetPositions.Clear();
        targetRotations.Clear();

        currentTargetIndex = 0;
        currentTime = 0f;
        isPlaying = true;

        SetSpawnLocation();

        CollectObjectsWithTag(targetTagViewingPoint, targetPositions);
        CollectObjectsWithTag(targetTagPausePoint, targetPositions);
        CollectObjectsWithTag(targetTagScreen, targetRotations);

        SortLists(targetPositions, targetRotations);

        if (targetPositions.Count == 0)
        {
            Debug.LogWarning("No tour locations were found.", this);
        }

        if (targetPositions.Count != targetRotations.Count)
        {
            Debug.LogWarning(
                "Tour position and rotation lists have different counts. " +
                "Locations without a matching Display will keep their rotation.",
                this
            );
        }
    }

    // ---------- PLAYBACK BUTTONS ---------- //

    public void PreviousLocation()
    {
        if (targetPositions.Count == 0) return;

        int lastIndex = targetPositions.Count - 1;
        int currentIndex = Mathf.Min(currentTargetIndex, lastIndex);
        GoToLocation(currentIndex == 0 ? lastIndex : currentIndex - 1);
    }

    public void NextLocation()
    {
        if (targetPositions.Count == 0) return;

        int lastIndex = targetPositions.Count - 1;
        int currentIndex = Mathf.Min(currentTargetIndex, lastIndex);
        GoToLocation(currentIndex == lastIndex ? 0 : currentIndex + 1);
    }

    private void GoToLocation(int index)
    {
        currentTargetIndex = index;
        currentTime = 0f;

        GameObject target = targetPositions[index];
        if (target == null || target.transform != domeWaypoint) {ClearDomeVisit();}
        if (target == null) return;

        // Back/forward immediately jumps to the requested location.
        transform.position = target.transform.position;

        if (TryGetTargetRotation(out Quaternion rotation))
        {
            transform.rotation = rotation;
        }
        if (domeTourPanel != null && IsDomeLocation(target))
        {
            BeginDomeVisit(target.transform);
        }
    }

    // ---------- TARGET COLLECTION ---------- //

    private void SetSpawnLocation()
    {
        if (christiansSpawnPoint == null)
        {
            Debug.LogWarning("Assign the Christians Spawn Point on SpiralTour.", this);
            return;
        }

        transform.position = christiansSpawnPoint.position;
    }

    private void CollectObjectsWithTag(
        string targetTag,
        List<GameObject> targetList)
    {
        foreach (GameObject target in GameObject.FindGameObjectsWithTag(targetTag))
        {
            if (GetPrefixOrder(target.name) < TourPrefixes.Length)
            {
                targetList.Add(target);
            }
        }
    }

    private void SortLists(
        List<GameObject> positions,
        List<GameObject> rotations)
    {
        positions.Sort((a, b) => CompareNames(a.name, b.name));
        rotations.Sort((a, b) => CompareNames(a.name, b.name));
    }

    private static int GetPrefixOrder(string objectName)
    {
        for (int index = 0; index < TourPrefixes.Length; index++)
        {
            if (objectName.StartsWith(TourPrefixes[index], System.StringComparison.Ordinal))
            {
                return index;
            }
        }

        return TourPrefixes.Length;
    }

    private int CompareNames(string name1, string name2)
    {
        int prefixOrder = GetPrefixOrder(name1).CompareTo(GetPrefixOrder(name2));
        if (prefixOrder != 0)
        {
            return prefixOrder;
        }

        string pattern =
            @"([a-zA-Z]{2})([a-zA-Z]{2})(\d{2})([a-zA-Z]+)(-)(\d+)";

        Match match1 = Regex.Match(name1, pattern);
        Match match2 = Regex.Match(name2, pattern);

        if (match1.Success && match2.Success)
        {
            int number1 = int.Parse(match1.Groups[3].Value);
            int number2 = int.Parse(match2.Groups[3].Value);

            int result = number2.CompareTo(number1);

            if (result == 0)
            {
                result = string.Compare(
                    match1.Groups[4].Value,
                    match2.Groups[4].Value
                );

                if (result == 0)
                {
                    int order1 = int.Parse(match1.Groups[6].Value);
                    int order2 = int.Parse(match2.Groups[6].Value);

                    result = order1.CompareTo(order2);
                }
            }

            return result;
        }

        return string.Compare(name1, name2);
    }

    // ---------- MOVEMENT AND CAMERA ---------- //

    private void FixedUpdate()
    {
        if (viewerControllerActive || !isPlaying) return;

        MoveAndRotateTowardsTargets();
    }

    private bool TryGetTargetRotation(out Quaternion rotation)
    {
        rotation = transform.rotation;

        if (currentTargetIndex < 0 ||
            currentTargetIndex >= targetRotations.Count)
        {
            return false;
        }

        GameObject target = targetRotations[currentTargetIndex];
        if (target == null) return false;

        Vector3 direction = target.transform.position - transform.position;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        rotation =
            Quaternion.LookRotation(direction) *
            Quaternion.Euler(cameraAngleOffset);

        return true;
    }

    private void MoveAndRotateTowardsTargets()
    {
        if (currentTargetIndex >= targetPositions.Count)
        {
            isPlaying = false;
            return;
        }

        GameObject target = targetPositions[currentTargetIndex];

        if (target == null)
        {
            currentTargetIndex++;
            currentTime = 0f;
            return;
        }

        Vector3 targetPosition = target.transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        bool rotationReached = true;

        if (TryGetTargetRotation(out Quaternion targetRotation))
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotSpeed * Time.fixedDeltaTime
            );

            rotationReached =
                Quaternion.Angle(transform.rotation, targetRotation) < 0.1f;
        }

        bool positionReached =
            Vector3.Distance(transform.position, targetPosition) <= 0.1f;

        if (!positionReached || !rotationReached) return;
        if (domeTourPanel != null && IsDomeLocation(target))
        {
            BeginDomeVisit(target.transform);

            if (!domePanelOpened)
                return;
        }

        // Preserves the original behavior:
        // PausePoint advances immediately; other locations wait.
        if (target.CompareTag(targetTagPausePoint))
        {
            AdvanceTarget();
        }
        else
        {
            currentTime += Time.fixedDeltaTime;

            if (currentTime >= pauseDuration)
            {
                AdvanceTarget();
            }
        }
    }

    private void AdvanceTarget()
{
    currentTargetIndex++;
    currentTime = 0f;

    if (currentTargetIndex >= targetPositions.Count)
    {
        isPlaying = false;
        return;
    }

    ClearDomeVisit();
}
    private static bool IsDomeLocation(GameObject target)
{
    return target.name.StartsWith("Do", System.StringComparison.Ordinal);
}

private void BeginDomeVisit(Transform waypoint)
{
    if (domeWaypoint == waypoint)
        return;

    ClearDomeVisit();
    domeWaypoint = waypoint;
domePanelOpenTime = Time.unscaledTime + DomePanelDelay;}

private void LateUpdate()
{
    if (domeWaypoint == null ||
        domePanelOpened ||
        domeTourPanel == null ||
        Time.unscaledTime < domePanelOpenTime)
    {
        return;
    }

    domePanelOpened = true;
    domeTourPanel.SetActive(true);
}
private void ClearDomeVisit(
    [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
{
    if (domeWaypoint != null)
    {
        float distance = Vector3.Distance(
            transform.position, domeWaypoint.position);

        Debug.LogWarning(
            $"Dome visit cleared by {caller}. " +
            $"Waypoint: {domeWaypoint.name}, distance: {distance:F4}, " +
            $"panel opened: {domePanelOpened}.",
            this);
    }

    domeWaypoint = null;
    domePanelOpened = false;

    if (domeTourPanel != null)
        domeTourPanel.SetActive(false);
}

private void ClearDomeVisit()
{
    domeWaypoint = null;
    domePanelOpened = false;
    domePanelOpenTime = 0f;

    if (domeTourPanel != null)
        domeTourPanel.SetActive(false);
}
public void ResetDomeVisit()
{
    ClearDomeVisit();
}
}
