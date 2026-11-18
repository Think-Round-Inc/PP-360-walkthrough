using System.Collections;
using UnityEngine;

public enum DomeWallState
{
    Art,
    Religion
}

public sealed class DomeWallChanger : MonoBehaviour
{
    public static DomeWallState CurrentDomeWallState = DomeWallState.Art;
    [SerializeField] DomeWallState currentDomeWallState = DomeWallState.Art;
    [SerializeField] bool isActive = true;
    [SerializeField] float timeToChangeWalls;
    [SerializeField] float timeBetweenWallChanges;
    [SerializeField] DomeWall[] faithWalls;
    [SerializeField] bool faithWallsToggle = true;
    [SerializeField] DomeWall[] faunaWalls;
    [SerializeField] bool faunaWallsToggle = true;
    [SerializeField] DomeWall[] infoWalls;
    [SerializeField] bool infoWallsToggle = true;
    private float currentTime;

    private void Start()
    {
        currentTime = Mathf.Max(1, timeToChangeWalls);
    }

    /// <summary>
    /// changes the current dome wal images to art type
    /// </summary>
    public void SetCurrentWallStateToArt()
    {
        StopAllCoroutines();
        currentDomeWallState = DomeWallState.Art;
        CurrentDomeWallState = currentDomeWallState;
        currentTime = 0;
    }

    /// <summary>
    /// changes the current dome wal images to religion type
    /// </summary>
    public void SetCurrentWallStateToReligion()
    {
        StopAllCoroutines();
        currentDomeWallState = DomeWallState.Religion;
        CurrentDomeWallState = currentDomeWallState;
        currentTime = 0;
    }

    /// <summary>
    /// sets the walls actively changing or not (pause/unpause)
    /// </summary>
    /// <param name="isActive">new state for walls changing</param>
    public void SetDomeWallChangerActive(bool isActive) => this.isActive = isActive;

    private void Update()
    {
        if (!isActive)
        {
            StopAllCoroutines();
            return;
        }
        currentTime = currentTime < 0 ? 0 : currentTime -= Time.deltaTime;
        if (currentTime == 0)
        {
            currentTime = Mathf.Max(1, timeToChangeWalls);
            if (faithWallsToggle)
                StartCoroutine(ChangeWalls(faithWalls));
            if (faunaWallsToggle)
                StartCoroutine(ChangeWalls(faunaWalls));
            if (infoWallsToggle)
                StartCoroutine(ChangeWalls(infoWalls));
        }
    }

    /// <summary>
    /// starts changing walls with data from array of dome walls
    /// </summary>
    /// <param name="walls"></param>
    /// <returns>null</returns>
    public IEnumerator ChangeWalls(DomeWall[] walls)
    {
        if (walls.Length.Equals(0)) yield return null;
        for (int i = 0; i < walls.Length; i++)
        {
            yield return new WaitForSeconds(Mathf.Max(0, timeBetweenWallChanges));
            walls[i].SwitchWallSprite();
        }
    }

    /// <summary>
    /// makes sure static field is updated when currentDomeWallState changes in the inspector
    /// </summary>
    private void OnValidate() => CurrentDomeWallState = currentDomeWallState;
}
