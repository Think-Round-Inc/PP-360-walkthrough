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
