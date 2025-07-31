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

    [Header("Target Lists")]
    [SerializeField] private List<GameObject> targetPositions = new List<GameObject>();
    [SerializeField] private List<GameObject> targetRotations = new List<GameObject>();

    [Header("Spiral Tour Options")]
    [SerializeField] private bool Christians = false;
    [SerializeField] private bool Jews = false;
    [SerializeField] private bool Buddhists = false;
    [SerializeField] private bool Hindus = false;
    [SerializeField] private bool Taoists = false;
    [SerializeField] private bool Indigenous = false;
    [SerializeField] private bool Muslims = false;

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
    private int currentTargetIndex = 0;
    private float currentTime = 0f;

    public void SetViewerControlsActive(bool value) => viewerControllerActive = value;

    public void SetChristiansActive(bool value) => Christians = value;
    public void SetJewsActive(bool value) => Jews = value;
    public void SetBuddhistsActive(bool value) => Buddhists = value;
    public void SetHindusActive(bool value) => Hindus = value;
    public void SetTaoistsActive(bool value) => Taoists = value;
    public void SetIndigenousActive(bool value) => Indigenous = value;
    public void SetMuslimsActive(bool value) => Muslims = value;

    public void InitializeScript()
    {
        SetSpawnLocation();
        CollectObjectsWithTag(targetTagViewingPoint, targetPositions);
        CollectObjectsWithTag(targetTagPausePoint, targetPositions);
        CollectObjectsWithTag(targetTagScreen, targetRotations);

        SortLists(targetPositions, targetRotations);
    }

    private void SetSpawnLocation()
    {
        // Set the spawn location based on the selected option
        Vector3 spawnPosition = GetSpawnPosition();
        transform.position = spawnPosition;
    }

    private Vector3 GetSpawnPosition()
    {
        // Return the position of the corresponding spawn point based on the selected option
        if (Christians && christiansSpawnPoint != null) return christiansSpawnPoint.position;
        if (Jews && jewsSpawnPoint != null) return jewsSpawnPoint.position;
        if (Buddhists && buddhistsSpawnPoint != null) return buddhistsSpawnPoint.position;
        if (Hindus && hindusSpawnPoint != null) return hindusSpawnPoint.position;
        if (Taoists && taoistsSpawnPoint != null) return taoistsSpawnPoint.position;
        if (Indigenous && indigenousSpawnPoint != null) return indigenousSpawnPoint.position;
        if (Muslims && muslimsSpawnPoint != null) return muslimsSpawnPoint.position;

        // Default spawn position if no option is selected or if the corresponding spawn point is not set
        return Vector3.zero;
    }

    private void CollectObjectsWithTag(string targetTag, List<GameObject> targetList)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(targetTag);

        // Filter objects based on the selected option
        if (Christians || Jews || Buddhists || Hindus || Taoists || Indigenous || Muslims)
        {
            string optionPrefix = GetSelectedOptionPrefix();
            objectsWithTag = objectsWithTag.Where(obj => obj.name.StartsWith(optionPrefix)).ToArray();
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

        // Default prefix if no option is selected
        return "";
    }

    private void SortLists(List<GameObject> targetPositionsList, List<GameObject> targetRotationsList)
    {
        targetPositionsList.Sort((obj1, obj2) => CompareNames(obj1.name, obj2.name));
        targetRotationsList.Sort((obj1, obj2) => CompareNames(obj1.name, obj2.name));
    }

    private int CompareNames(string name1, string name2)
    {
        // Define the regular expression pattern
        string pattern = @"([a-zA-Z]{2})([a-zA-Z]{2})(\d{2})([a-zA-Z]+)(-)(\d+)";

        // Match the patterns for both names
        Match match1 = Regex.Match(name1, pattern);
        Match match2 = Regex.Match(name2, pattern);

        // Check if both matches were successful
        if (match1.Success && match2.Success)
        {
            int part3_1 = int.Parse(match1.Groups[3].Value);
            int part3_2 = int.Parse(match2.Groups[3].Value);

            // Sort first by Part 3 in descending order
            int result = part3_2.CompareTo(part3_1);

            if (result == 0) // If Part 3 values are equal, sort by Part 4 in ascending order
            {
                string part4_1 = match1.Groups[4].Value;
                string part4_2 = match2.Groups[4].Value;

                result = part4_1.CompareTo(part4_2);

                if (result == 0) // If Part 4 values are equal, sort by Part 5 in ascending order
                {
                    int part5_1 = int.Parse(match1.Groups[6].Value);
                    int part5_2 = int.Parse(match2.Groups[6].Value);

                    result = part5_1.CompareTo(part5_2);
                }
            }

            return result;
        }

        // If the pattern doesn't match, consider them equal
        return string.Compare(name1, name2);
    }

    private void FixedUpdate()
    {
        if (viewerControllerActive) return;
        MoveAndRotateTowardsTargets();
    }

    private void MoveAndRotateTowardsTargets()
    {
        if (targetPositions.Count == 0 || currentTargetIndex == targetPositions.Count)
            return;

        Transform currentTargetPosition = targetPositions[currentTargetIndex].transform;
        Transform currentTargetRotation = targetRotations[currentTargetIndex].transform;

        transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition.position, moveSpeed * Time.fixedDeltaTime);

        Vector3 directionToFace = currentTargetRotation.position - transform.position;

        if (directionToFace != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(directionToFace);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotSpeed * Time.fixedDeltaTime);
        }

        float distanceToTarget = Vector3.Distance(transform.position - new Vector3(0f, 0.985f, 0f), currentTargetPosition.position);

        if (distanceToTarget <= 0.1f && Quaternion.Angle(transform.rotation, Quaternion.LookRotation(directionToFace)) < 0.1f)
        {
            if (targetPositions[currentTargetIndex].CompareTag(targetTagPausePoint))
            {
                currentTargetIndex++;
                currentTime = 0f;
            }
            else
            {
                currentTime += 0.01f;
                if (currentTime > pauseDuration)
                {
                    currentTargetIndex++;
                    currentTime = 0f;
                }
            }
        }
    }
}
