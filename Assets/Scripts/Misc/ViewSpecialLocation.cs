using UnityEngine;
using UnityEngine.Events;
// was being used for launch, but currently leaving in as a "feature" to be able to see from above
public sealed class ViewSpecialLocation : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] KeyCode specialKey;
    [SerializeField] Transform specialTransform;
    [SerializeField] UnityEvent onPressedKey;
    [SerializeField] UnityEvent onReleasedKey;
    private bool atSpecialLocation;
    private Vector3 _currentPosition;
    private Vector3 _currentRotation;

    void Update()
    {
        if (!atSpecialLocation)
        {
            _currentPosition = targetTransform.position;
            _currentRotation = targetTransform.eulerAngles;
        }
        if (Input.GetKeyDown(specialKey))
        {
            onPressedKey?.Invoke();
        }
        else if (Input.GetKeyUp(specialKey))
        {
            onReleasedKey?.Invoke();
        }
    }

    public void MoveToSpecialLocation()
    {
        atSpecialLocation = true;
        if (targetTransform.TryGetComponent(out Rigidbody rigid))
            rigid.useGravity = false;
        targetTransform.position = specialTransform.position;
        targetTransform.eulerAngles = new(specialTransform.eulerAngles.x, specialTransform.eulerAngles.y, 0);
    }

    public void MoveToLastKnownLocation()
    {
        targetTransform.position = _currentPosition;
        targetTransform.eulerAngles = _currentRotation;
        if (targetTransform.TryGetComponent(out Rigidbody rigid))
            rigid.useGravity = true;
        atSpecialLocation = false;
    }
}
