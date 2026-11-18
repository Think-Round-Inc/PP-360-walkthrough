using System.Collections;
using UnityEngine;

// this is currently not being implemented
public class AutoMover : MonoBehaviour
{
    public bool autoMoverActive = false;
    public GameObject viewer; // Reference to the viewer object
    public GameObject head; // Reference to the head object
    public Vector3 targetOffset;
    public GameObject[] targetObjects; // Array of target GameObjects
    public bool liftViewer = true; // Flag to determine if the viewer should be lifted up
    public float liftHeight = 3f; // Height to lift the viewer above its current position
    public float floorLevel = 0f; // Y-value for the floor level
    public float moveSpeed = 3f; // Speed at which the viewer moves
    public Vector3 headRotation = Vector3.zero; // Desired rotation for the head

    private int currentIndex = 0; // Current index of the target object in the array
    private bool isMoving = false; // Flag to check if the viewer is currently moving
    bool waitingForTargetLocation = true;

    private void Update()
    {
        if (!autoMoverActive) return;
        if (isMoving)
        {
            RotateViewerTowardsTarget();
            return;
        }
    }

    public void MoveToPreviousTarget()
    {
        if (!autoMoverActive) return;
        if (!Application.isPlaying) return;
        if (!waitingForTargetLocation) return;
        if (targetObjects.Length == 0)
        {
            Debug.LogWarning("No target objects found!");
            return;
        }

        currentIndex = (currentIndex - 1 + targetObjects.Length) % targetObjects.Length;
        MoveToTarget(currentIndex);
    }

    public void MoveToNextTarget()
    {
        if (!Application.isPlaying) return;
        if (!waitingForTargetLocation) return;
        if (targetObjects.Length == 0)
        {
            Debug.LogWarning("No target objects found!");
            return;
        }

        currentIndex = (currentIndex + 1) % targetObjects.Length; // Increment the current index in a loop
        MoveToTarget(currentIndex);
    }

    private void MoveToTarget(int index)
    {
        StartCoroutine(MoveViewer(targetObjects[index].transform.position, index));
    }

    private IEnumerator MoveViewer(Vector3 targetPosition, int index)
    {
        waitingForTargetLocation = false;
        isMoving = true;
        Vector3 offsetPosition = targetPosition + targetObjects[index].transform.forward + targetOffset;

        // Lift the viewer to the desired height if the liftViewer flag is set to true
        if (liftViewer)
        {
            Vector3 liftPosition = viewer.transform.position + Vector3.up * liftHeight;
            while (Vector3.Distance(viewer.transform.position, liftPosition) > 0.1f)
            {
                viewer.transform.position = Vector3.MoveTowards(viewer.transform.position, liftPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // Move the viewer above the target object
        Vector3 targetPos = offsetPosition;
        if (liftViewer)
            targetPos = offsetPosition + Vector3.up * liftHeight;

        while (Vector3.Distance(viewer.transform.position, targetPos) > 0.1f)
        {
            viewer.transform.position = Vector3.MoveTowards(viewer.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Move the viewer back to the floor level
        while (Vector3.Distance(viewer.transform.position, new Vector3(offsetPosition.x, floorLevel, offsetPosition.z)) > 0.1f)
        {
            viewer.transform.position = Vector3.MoveTowards(viewer.transform.position, new Vector3(offsetPosition.x, floorLevel, offsetPosition.z), moveSpeed * Time.deltaTime);
            if (head != null)
            {
                head.transform.localRotation = Quaternion.Euler(headRotation);
            }
            yield return null;
        }
        isMoving = false;
        waitingForTargetLocation = true;
    }

    private void RotateViewerTowardsTarget()
    {
        Vector3 targetPosition = targetObjects[currentIndex].transform.position;
        Vector3 directionToTarget = targetPosition - viewer.transform.position;
        directionToTarget.y = 0f; // Ignore the y-axis component

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
        viewer.transform.rotation = Quaternion.Lerp(viewer.transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
    }


    public void ResetHeadRotation()
    {
        if (!Application.isPlaying) return;
        head.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
