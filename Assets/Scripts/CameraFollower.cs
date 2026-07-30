using UnityEngine;

public sealed class CameraFollower : MonoBehaviour
{
    [Header("Dead zone")]
    [SerializeField] private Vector2 deadZoneSize = new(3f, 2f);

    [Header("Movement")]
    [SerializeField, Min(0.01f)] private float smoothTime = 0.2f;
    [SerializeField, Min(0f)] private float maxSpeed = 20f;
    [SerializeField] private bool snapOnInitialize = true;

    private Transform target;
    private Vector3 velocity;

    public void Initialize(Transform newTarget)
    {
        target = newTarget;
        velocity = Vector3.zero;

        if (snapOnInitialize && target != null)
        {
            var position = target.position;
            position.z = transform.position.z;
            transform.position = position;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        var currentPosition = transform.position;
        var desiredPosition = GetDesiredPosition(currentPosition, target.position);

        transform.position = Vector3.SmoothDamp(
            currentPosition,
            desiredPosition,
            ref velocity,
            smoothTime,
            maxSpeed,
            Time.deltaTime);
    }

    private Vector3 GetDesiredPosition(Vector3 cameraPosition, Vector3 targetPosition)
    {
        var halfSize = deadZoneSize * 0.5f;
        var offset = targetPosition - cameraPosition;
        var desiredPosition = cameraPosition;

        if (offset.x > halfSize.x)
            desiredPosition.x += offset.x - halfSize.x;
        else if (offset.x < -halfSize.x)
            desiredPosition.x += offset.x + halfSize.x;

        if (offset.y > halfSize.y)
            desiredPosition.y += offset.y - halfSize.y;
        else if (offset.y < -halfSize.y)
            desiredPosition.y += offset.y + halfSize.y;

        desiredPosition.z = cameraPosition.z;
        return desiredPosition;
    }

    private void OnValidate()
    {
        deadZoneSize.x = Mathf.Max(0f, deadZoneSize.x);
        deadZoneSize.y = Mathf.Max(0f, deadZoneSize.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneSize.x, deadZoneSize.y, 0f));
    }
}
