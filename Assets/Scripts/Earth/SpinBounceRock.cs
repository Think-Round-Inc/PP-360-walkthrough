using UnityEngine;

public class SpinBounceRock : MonoBehaviour
{
    public float bounceSpeed = 1f;
    public float bounceFrequency = 1f;
    public float bounceHeight = 0.5f;
    public float rockSpeed = 1f;
    public float rockFrequency = 1f;
    public float rockAmount = 0.1f;

    private Vector3 initialPosition;
    private Vector3 rockingOffset;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        float verticalOffset = Mathf.Sin(Time.time * bounceFrequency) * bounceHeight;
        float rockingAmount = Mathf.Sin(Time.time * rockFrequency) * rockSpeed;
        float t = (rockingAmount + 1f) / 2f;

        Vector3 localForward = transform.TransformDirection(Vector3.forward);
        rockingOffset = -localForward * rockAmount;

        Vector3 newPosition = Vector3.Lerp(initialPosition, initialPosition + rockingOffset, t);
        newPosition += new Vector3(0f, verticalOffset, 0f);
        transform.position = newPosition;
    }
}
