using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] Vector3 rotateVector;
    [SerializeField] float rotateSpeed;

    void FixedUpdate() => transform.Rotate(rotateSpeed * Time.fixedDeltaTime * rotateVector.normalized, Space.Self);
}
