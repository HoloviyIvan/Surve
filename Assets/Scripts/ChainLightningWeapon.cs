using System.Collections.Generic;
using UnityEngine;

public sealed class ChainLightningWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 18f;
    [SerializeField, Min(0.05f)] private float attackInterval = 2f;
    [SerializeField, Min(1)] private int maxTargets = 4;
    [SerializeField, Min(0f)] private float searchRadius = 5f;
    [SerializeField, Min(0f)] private float jumpRadius = 3.5f;
    [SerializeField, Range(0f, 1f)] private float damageMultiplierPerJump = 0.8f;

    [Header("Visual")]
    [SerializeField] private Material lightningMaterial;
    [SerializeField] private Color lightningColor = new(0.45f, 0.85f, 1f, 1f);
    [SerializeField, Min(0.01f)] private float lightningWidth = 0.12f;
    [SerializeField, Min(0.01f)] private float visualDuration = 0.22f;
    [SerializeField, Min(0f)] private float deadTargetFlashDuration = 0.06f;
    [SerializeField, Min(0.01f)] private float flickerInterval = 0.035f;
    [SerializeField, Min(0f)] private float jitter = 0.18f;
    [SerializeField, Min(1)] private int subdivisionsPerJump = 5;
    [SerializeField] private int sortingOrder = 115;

    private readonly List<Collider2D> overlapBuffer = new();
    private readonly HashSet<DamageReceiver> hitTargets = new();
    private readonly List<DamageReceiver> lightningTargets = new();
    private readonly List<Vector3> lightningAnchors = new();
    private readonly List<Vector3> lightningPoints = new();
    private readonly ContactFilter2D enemyFilter = new()
    {
        useLayerMask = true,
        layerMask = 1 << 7,
        useTriggers = true
    };
    private LineRenderer lightningRenderer;
    private float timer;
    private float visualTimer;
    private float visualElapsed;
    private float flickerTimer;

    protected override void OnInitialized()
    {
        if (lightningMaterial == null)
            throw new MissingReferenceException("ChainLightningWeapon: Lightning Material is not assigned.");

        lightningRenderer = gameObject.AddComponent<LineRenderer>();
        lightningRenderer.sharedMaterial = lightningMaterial;
        lightningRenderer.useWorldSpace = true;
        lightningRenderer.startWidth = lightningWidth;
        lightningRenderer.endWidth = lightningWidth * 0.65f;
        lightningRenderer.numCapVertices = 2;
        lightningRenderer.numCornerVertices = 2;
        lightningRenderer.sortingOrder = sortingOrder;
        lightningRenderer.enabled = false;
        timer = attackInterval;
    }

    public override void Tick(float deltaTime)
    {
        UpdateLightningVisual(deltaTime);
        timer = Mathf.Min(timer + deltaTime, attackInterval);
        if (timer < attackInterval)
            return;

        var enemy = FindClosestEnemy?.Invoke(Owner.position);
        if (enemy == null)
            return;
        if (((Vector2)(enemy.transform.position - Owner.position)).sqrMagnitude > searchRadius * searchRadius)
            return;

        timer = 0f;
        hitTargets.Clear();
        lightningTargets.Clear();

        if (!enemy.TryGetComponent<DamageReceiver>(out var currentTarget))
            return;

        for (var jump = 0; jump < maxTargets; jump++)
        {
            if (currentTarget == null || !currentTarget.IsAlive ||
                currentTarget.Team == OwnerTeam || hitTargets.Contains(currentTarget))
                break;

            hitTargets.Add(currentTarget);
            lightningTargets.Add(currentTarget);

            var currentPosition = (Vector2)currentTarget.transform.position;
            overlapBuffer.Clear();
            Physics2D.OverlapCircle(currentPosition, jumpRadius, enemyFilter, overlapBuffer);

            DamageReceiver closest = null;
            var closestDistance = float.MaxValue;
            for (var i = 0; i < overlapBuffer.Count; i++)
            {
                if (!overlapBuffer[i].TryGetComponent<DamageReceiver>(out var candidate))
                    continue;
                if (!candidate.IsAlive || candidate.Team == OwnerTeam || hitTargets.Contains(candidate))
                    continue;
                var distance = ((Vector2)candidate.transform.position - currentPosition).sqrMagnitude;
                if (distance >= closestDistance)
                    continue;
                closest = candidate;
                closestDistance = distance;
            }

            if (closest == null)
                break;

            currentTarget = closest;
        }

        var currentDamage = damage;
        for (var i = 0; i < lightningTargets.Count; i++)
        {
            lightningTargets[i].TakeDamage(currentDamage);
            currentDamage *= damageMultiplierPerJump;
        }

        ShowLightning();
    }

    private void ShowLightning()
    {
        lightningAnchors.Clear();
        lightningAnchors.Add(Owner.position);
        for (var i = 0; i < lightningTargets.Count; i++)
        {
            if (lightningTargets[i] != null)
                lightningAnchors.Add(lightningTargets[i].transform.position);
        }

        if (lightningAnchors.Count < 2)
            return;

        visualTimer = visualDuration;
        visualElapsed = 0f;
        flickerTimer = 0f;
        lightningRenderer.enabled = true;
        RebuildLightningPath();
    }

    private void UpdateLightningVisual(float deltaTime)
    {
        if (lightningRenderer == null || !lightningRenderer.enabled)
            return;

        visualTimer -= deltaTime;
        visualElapsed += deltaTime;
        if (visualTimer <= 0f)
        {
            lightningRenderer.enabled = false;
            return;
        }

        var previousAnchorCount = lightningAnchors.Count;
        if (visualElapsed >= deadTargetFlashDuration && !RefreshLightningAnchors())
        {
            lightningRenderer.enabled = false;
            return;
        }

        if (lightningAnchors.Count != previousAnchorCount)
        {
            flickerTimer = flickerInterval;
            RebuildLightningPath();
        }

        flickerTimer -= deltaTime;
        if (flickerTimer <= 0f)
        {
            flickerTimer = flickerInterval;
            RebuildLightningPath();
        }

        var color = lightningColor;
        color.a *= Mathf.Clamp01(visualTimer / visualDuration);
        lightningRenderer.startColor = color;
        lightningRenderer.endColor = color;
    }

    private bool RefreshLightningAnchors()
    {
        lightningAnchors.Clear();
        lightningAnchors.Add(Owner.position);

        for (var i = 0; i < lightningTargets.Count; i++)
        {
            var target = lightningTargets[i];
            if (target == null || !target.IsAlive)
                break;

            lightningAnchors.Add(target.transform.position);
        }

        return lightningAnchors.Count >= 2;
    }

    private void RebuildLightningPath()
    {
        lightningPoints.Clear();
        lightningPoints.Add(lightningAnchors[0]);

        var subdivisions = Mathf.Max(1, subdivisionsPerJump);
        for (var anchorIndex = 0; anchorIndex < lightningAnchors.Count - 1; anchorIndex++)
        {
            var start = lightningAnchors[anchorIndex];
            var end = lightningAnchors[anchorIndex + 1];
            var direction = end - start;
            var perpendicular = new Vector3(-direction.y, direction.x).normalized;

            for (var pointIndex = 1; pointIndex <= subdivisions; pointIndex++)
            {
                var progress = pointIndex / (float)subdivisions;
                var point = Vector3.Lerp(start, end, progress);
                if (pointIndex < subdivisions)
                {
                    var edgeFade = Mathf.Sin(progress * Mathf.PI);
                    point += perpendicular * Random.Range(-jitter, jitter) * edgeFade;
                }

                lightningPoints.Add(point);
            }
        }

        lightningRenderer.positionCount = lightningPoints.Count;
        for (var i = 0; i < lightningPoints.Count; i++)
            lightningRenderer.SetPosition(i, lightningPoints[i]);
    }
}
