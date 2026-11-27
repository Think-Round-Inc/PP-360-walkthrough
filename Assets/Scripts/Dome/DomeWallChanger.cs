/*
*   Name: DomeWallChanger.cs
*   Description: This script toggles the Dome from Art to Relgion and
                iterates through the dome images, displaying them over time.
                Unlike DomeWall, this script handles ALL domewalls, not just one.
*/

using System.Collections;
using UnityEngine;

public enum DomeWallState { Art, Religion, Off }

public sealed class DomeWallChanger : MonoBehaviour
{
    public static DomeWallState CurrentDomeWallState = DomeWallState.Art;
    [SerializeField] DomeWallState currentDomeWallState = DomeWallState.Art;
    [SerializeField] bool isActive = true;
    [Header("Wall Timing")]
    [SerializeField, Min(1f), Tooltip("Seconds between starts of wall change sequences.")] float timeToChangeWalls;
    [SerializeField, Min(0f), Tooltip("Seconds between consecutive walls in a group.")] float timeBetweenWallChanges;
    [Header("Wall Groups")]
    [SerializeField] DomeWall[] faithWalls;
    [SerializeField] bool faithWallsToggle = true;
    [SerializeField] DomeWall[] faunaWalls;
    [SerializeField] bool faunaWallsToggle = true;
    [SerializeField] DomeWall[] infoWalls;
    [SerializeField] bool infoWallsToggle = true;
    private float currentTime;
    private bool wasCycling;

    private void Start()
    {
        currentTime = Mathf.Max(1, timeToChangeWalls);
    }

    // changes the current dome wal images to art type
    public void SetCurrentWallStateToArt()
    {
        StopAllCoroutines();
        currentDomeWallState = DomeWallState.Art;
        CurrentDomeWallState = currentDomeWallState;
        currentTime = 0;
    }

    /// changes the current dome wal images to religion type
    public void SetCurrentWallStateToReligion()
    {
        StopAllCoroutines();
        currentDomeWallState = DomeWallState.Religion;
        CurrentDomeWallState = currentDomeWallState;
        currentTime = 0;
    }

    // sets the walls actively changing or not (pause/unpause)
    // <param name="isActive">new state for walls changing</param>
    public void SetDomeWallChangerActive(bool isActive) => this.isActive = isActive;

    private void Update()
    {
        if (!isActive || CurrentDomeWallState == DomeWallState.Off)
        {
            if (wasCycling) StopAllCoroutines();
            wasCycling = false;
            return;
        }
        wasCycling = true;
        currentTime = Mathf.Max(0f, currentTime - Time.deltaTime);
        if (currentTime == 0)
        {
            currentTime = Mathf.Max(1, timeToChangeWalls);
            if (faithWallsToggle) { StartCoroutine(ChangeWalls(faithWalls)); }
            if (faunaWallsToggle) { StartCoroutine(ChangeWalls(faunaWalls)); }
            if (infoWallsToggle) { StartCoroutine(ChangeWalls(infoWalls)); }
        }
    }

    // starts changing walls with data from array of dome walls
    // <param name="walls"></param>
    // <returns>null</returns>
    public IEnumerator ChangeWalls(DomeWall[] walls)
    {
        if (walls == null) yield break;
        for (int i = 0; i < walls.Length; i++)
        {
            yield return new WaitForSeconds(Mathf.Max(0, timeBetweenWallChanges));
            if (walls[i] != null) walls[i].SwitchWallSprite();
        }
    }

    // makes sure static field is updated when currentDomeWallState changes in the inspector
    private void OnValidate() => CurrentDomeWallState = currentDomeWallState;

    public void DomeWallOff()
    {
        StopAllCoroutines();
        currentDomeWallState = DomeWallState.Off;
        CurrentDomeWallState = currentDomeWallState;

        ClearWallImages(faithWalls);
        ClearWallImages(faunaWalls);
        ClearWallImages(infoWalls);
    }

    private static void ClearWallImages(DomeWall[] walls)
    {
        if (walls == null) { return; }

        foreach (DomeWall wall in walls)
        {
            if (wall != null) { wall.ClearWallImage(); }
        }
    }
}
