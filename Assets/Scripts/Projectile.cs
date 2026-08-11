using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float lifetime = 5f;

    private Rigidbody2D body;
    private Action<Projectile> release;
    private float remainingLifetime;
    private bool isActive;

    protected ProjectileShotData ShotData { get; private set; }
    protected Rigidbody2D Body => body;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Launch(
        Vector2 direction,
        float speed,
        ProjectileShotData shotData,
        Action<Projectile> releaseAction)
    {
        release = releaseAction;
        ShotData = shotData;
        remainingLifetime = lifetime;
        isActive = true;
        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;
        body.AddForce(direction.normalized * speed);
        OnLaunched();
    }

    public void SetSpawnPosition(Vector3 position)
    {
        body.position = position;
        transform.SetPositionAndRotation(position, Quaternion.identity);
    }

    private void Update()
    {
        if (!isActive)
            return;

        OnTick(Time.deltaTime);
        if (!isActive)
            return;

        remainingLifetime -= Time.deltaTime;
        if (remainingLifetime <= 0f)
            ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || !HandleCollision(other))
            return;

        ReturnToPool();
    }

    public void ResetState()
    {
        isActive = false;
        release = null;
        ShotData = default;

        if (body == null)
            body = GetComponent<Rigidbody2D>();

        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;
        OnResetState();
    }

    protected abstract bool HandleCollision(Collider2D other);

    protected virtual void OnLaunched()
    {
    }

    protected virtual void OnResetState()
    {
    }

    protected virtual void OnTick(float deltaTime)
    {
    }

    protected void ReturnToPool()
    {
        if (!isActive)
            return;

        isActive = false;
        release?.Invoke(this);
    }
}
