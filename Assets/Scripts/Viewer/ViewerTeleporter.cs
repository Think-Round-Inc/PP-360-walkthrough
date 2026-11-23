/*
*   Name: ViewerTeleporter.cs
*   Description: this script goes on a location and gets the transform coordinates
*                 so they may be used to transport the viewer to that location later
*/

using UnityEngine;

public class ViewerTeleporter : MonoBehaviour
{
    [SerializeField] Transform[] viewerTeleportLocations;
    [SerializeField] GameObject viewer;
    [SerializeField] Vector3 viewerTeleportOffset;

    public void TeleportViewerToLocation(int targetLocation)
    {
        if (targetLocation < viewerTeleportLocations.Length && viewerTeleportLocations[targetLocation] != null)
        {
            viewer.transform.position = viewerTeleportLocations[targetLocation].position + viewerTeleportOffset;
            viewer.transform.forward = viewerTeleportLocations[targetLocation].forward;
        }
    }
}
