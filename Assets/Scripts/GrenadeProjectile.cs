using System.Collections.Generic;
using UnityEngine;

public sealed class GrenadeProjectile : Projectile
{
    [SerializeField] private SpriteRenderer grenadeRenderer;
    [SerializeField] private SpriteRenderer explosionRenderer;
    [SerializeField] private Sprite[] explosionFrames;
    [SerializeField, Min(0.01f)] private float explosionFrameDuration = 0.08f;
    [SerializeField] private float spinSpeed = -540f;

    private readonly List<Collider2D> overlapBuffer = new();
    private readonly HashSet<DamageReceiver> uniqueTargets = new();
    private Vector2 startPosition;
    private Vector3 grenadeBaseScale;
    private Vector3 explosionTargetScale;
    private float elapsed;
    private State state;

    protected override void Awake()
    {
        base.Awake();
        if (grenadeRenderer == null || explosionRenderer == null)
            throw new MissingReferenceException("GrenadeProjectile: renderers are not assigned.");

        grenadeBaseScale = grenadeRenderer.transform.localScale;
    }

    protected override void OnLaunched()
    {
        if (explosionFrames == null || explosionFrames.Length == 0)
            throw new MissingReferenceException("GrenadeProjectile: explosion frames are not assigned.");

        Body.linearVelocity = Vector2.zero;
        Body.angularVelocity = 0f;
        startPosition = Body.position;
        elapsed = 0f;
        state = State.Flying;
        grenadeRenderer.enabled = true;
        grenadeRenderer.transform.localPosition = Vector3.zero;
        grenadeRenderer.transform.localScale = grenadeBaseScale;
        explosionRenderer.enabled = false;
        explosionRenderer.sprite = explosionFrames[0];

        var spriteDiameter = Mathf.Max(
            explosionFrames[0].bounds.size.x,
            explosionFrames[0].bounds.size.y);
        var scale = ShotData.ExplosionRadius * 2f / spriteDiameter;
        explosionTargetScale = Vector3.one * scale;
    }

    protected override void OnTick(float deltaTime)
    {
        if (state == State.Flying)
            TickFlight(deltaTime);
        else if (state == State.Exploding)
            TickExplosion(deltaTime);
    }

    protected override bool HandleCollision(Collider2D other) => false;

    protected override void OnResetState()
    {
        state = State.Inactive;
        elapsed = 0f;
        overlapBuffer.Clear();
        uniqueTargets.Clear();

        if (grenadeRenderer != null)
        {
            grenadeRenderer.enabled = true;
            grenadeRenderer.transform.localPosition = Vector3.zero;
            grenadeRenderer.transform.localScale = grenadeBaseScale;
        }

        if (explosionRenderer != null)
        {
            explosionRenderer.enabled = false;
            if (explosionFrames is { Length: > 0 })
                explosionRenderer.sprite = explosionFrames[0];
        }
    }

    private void TickFlight(float deltaTime)
    {
        elapsed += deltaTime;
        var duration = Mathf.Max(ShotData.FlightDuration, 0.05f);
        var progress = Mathf.Clamp01(elapsed / duration);
        Body.position = Vector2.Lerp(startPosition, ShotData.TargetPosition, progress);

        var arc = 4f * ShotData.ArcHeight * progress * (1f - progress);
        grenadeRenderer.transform.localPosition = Vector3.up * arc;
        grenadeRenderer.transform.Rotate(0f, 0f, spinSpeed * deltaTime, Space.Self);
        grenadeRenderer.transform.localScale = grenadeBaseScale * (1f + 0.2f * arc / Mathf.Max(ShotData.ArcHeight, 0.01f));

        if (progress >= 1f)
            Explode();
    }

    private void Explode()
    {
        state = State.Exploding;
        elapsed = 0f;
        Body.position = ShotData.TargetPosition;
        grenadeRenderer.enabled = false;
        explosionRenderer.enabled = true;
        explosionRenderer.sprite = explosionFrames[0];
        explosionRenderer.transform.localScale = explosionTargetScale;

        uniqueTargets.Clear();
        CombatDamage.ApplyCircle(
            ShotData.TargetPosition,
            ShotData.ExplosionRadius,
            ShotData.Damage,
            ShotData.SourceTeam,
            overlapBuffer,
            uniqueTargets);
    }

    private void TickExplosion(float deltaTime)
    {
        elapsed += deltaTime;
        var frameIndex = Mathf.Min(
            Mathf.FloorToInt(elapsed / explosionFrameDuration),
            explosionFrames.Length - 1);
        explosionRenderer.sprite = explosionFrames[frameIndex];

        if (elapsed >= explosionFrameDuration * explosionFrames.Length)
            ReturnToPool();
    }

    private enum State
    {
        Inactive,
        Flying,
        Exploding
    }
}
