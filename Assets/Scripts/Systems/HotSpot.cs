using UnityEngine;
using UnityEngine.Events;

public sealed class HotSpot : MonoBehaviour
{
    public bool hasSeenBefore;
    public Vector3 hotspotIconOffset;
    public UnityEvent<GameObject> onClickedHotspot;
    [Header("Debug")]
    [SerializeField] float debugSphereRadius = .2f;
    [SerializeField] Color debugSphereColor = Color.red;

    private void OnDrawGizmos()
    {
        Gizmos.color = debugSphereColor;
        Gizmos.DrawSphere(transform.position + hotspotIconOffset, debugSphereRadius);
    }
}
