using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public sealed class SpiralTour : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool viewerControllerActive = true;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotSpeed = 3f;
    [SerializeField] private float pauseDuration = 2f;

    [Header("Camera Angle Offset (Degrees)")]
    [Tooltip("X = tilt up/down, Y = turn left/right, Z = roll.")]
    [SerializeField] private Vector3 cameraAngleOffset;

    [Header("Playback")]
    [SerializeField] private bool isPlaying = true;

    [Header("Target Lists")]
    [SerializeField] private List<GameObject> targetPositions =
        new List<GameObject>();

    [SerializeField] private List<GameObject> targetRotations =
        new List<GameObject>();

    [Header("Spiral Tour Options")]
    [SerializeField] private bool Christians;
    [SerializeField] private bool Jews;
    [SerializeField] private bool Buddhists;
    [SerializeField] private bool Hindus;
    [SerializeField] private bool Taoists;
    [SerializeField] private bool Indigenous;
    [SerializeField] private bool Muslims;

    [Header("Spawn Points")]
    [SerializeField] private Transform christiansSpawnPoint;
    [SerializeField] private Transform jewsSpawnPoint;
    [SerializeField] private Transform buddhistsSpawnPoint;
    [SerializeField] private Transform hindusSpawnPoint;
    [SerializeField] private Transform taoistsSpawnPoint;
    [SerializeField] private Transform indigenousSpawnPoint;
    [SerializeField] private Transform muslimsSpawnPoint;

    private string targetTagViewingPoint = "ViewingPoint";
    private string targetTagPausePoint = "PausePoint";
    private string targetTagScreen = "Display";

    private int currentTargetIndex;
    private float currentTime;

    public void SetViewerControlsActive(bool value)
    {
        viewerControllerActive = value;
    }

    public void SetChristiansActive(bool value) => Christians = value;
    public void SetJewsActive(bool value) => Jews = value;
    public void SetBuddhistsActive(bool value) => Buddhists = value;
    public void SetHindusActive(bool value) => Hindus = value;
    public void SetTaoistsActive(bool value) => Taoists = value;
    public void SetIndigenousActive(bool value) => Indigenous = value;
    public void SetMuslimsActive(bool value) => Muslims = value;

    public void InitializeScript()
    {
        // Clear previous targets when starting a different tour.
        targetPositions.Clear();
        targetRotations.Clear();

        currentTargetIndex = 0;
        currentTime = 0f;

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

        int index = Mathf.Clamp(
            currentTargetIndex - 1,
            0,
            targetPositions.Count - 1
        );

        GoToLocation(index);
    }

    public void NextLocation()
    {
        if (targetPositions.Count == 0) return;

        int index = Mathf.Clamp(
            currentTargetIndex + 1,
            0,
            targetPositions.Count - 1
        );

        GoToLocation(index);
    }

    private void GoToLocation(int index)
    {
        currentTargetIndex = index;
        currentTime = 0f;

        GameObject target = targetPositions[index];
        if (target == null) return;

        // Back/forward immediately jumps to the requested location.
        transform.position = target.transform.position;

        if (TryGetTargetRotation(out Quaternion rotation))
        {
            transform.rotation = rotation;
        }
    }

    // ---------- TARGET COLLECTION ---------- //

    private void SetSpawnLocation()
    {
        transform.position = GetSpawnPosition();
    }

    private Vector3 GetSpawnPosition()
    {
        if (Christians && christiansSpawnPoint != null)
            return christiansSpawnPoint.position;

        if (Jews && jewsSpawnPoint != null)
            return jewsSpawnPoint.position;

        if (Buddhists && buddhistsSpawnPoint != null)
            return buddhistsSpawnPoint.position;

        if (Hindus && hindusSpawnPoint != null)
            return hindusSpawnPoint.position;

        if (Taoists && taoistsSpawnPoint != null)
            return taoistsSpawnPoint.position;

        if (Indigenous && indigenousSpawnPoint != null)
            return indigenousSpawnPoint.position;

        if (Muslims && muslimsSpawnPoint != null)
            return muslimsSpawnPoint.position;

        return Vector3.zero;
    }

    private void CollectObjectsWithTag(
        string targetTag,
        List<GameObject> targetList)
    {
        GameObject[] objectsWithTag =
            GameObject.FindGameObjectsWithTag(targetTag);

        if (Christians || Jews || Buddhists || Hindus ||
            Taoists || Indigenous || Muslims)
        {
            string prefix = GetSelectedOptionPrefix();

            objectsWithTag = objectsWithTag
                .Where(obj => obj.name.StartsWith(prefix))
                .ToArray();
        }

        targetList.AddRange(objectsWithTag);
    }

    private string GetSelectedOptionPrefix()
    {
        if (Christians) return "Ch";
        if (Jews) return "Je";
        if (Buddhists) return "Bu";
        if (Hindus) return "Hi";
        if (Taoists) return "Ta";
        if (Indigenous) return "In";
        if (Muslims) return "Mu";

        return "";
    }

    private void SortLists(
        List<GameObject> positions,
        List<GameObject> rotations)
    {
        positions.Sort((a, b) => CompareNames(a.name, b.name));
        rotations.Sort((a, b) => CompareNames(a.name, b.name));
    }

    private int CompareNames(string name1, string name2)
    {
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
        }
    }
}