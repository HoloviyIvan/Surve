using System.Collections.Generic;
using UnityEngine;

public sealed class PiercingBeamWeapon : Weapon
{
    [Header("Damage")]
    [SerializeField, Min(0f)] private float damage = 14f;
    [SerializeField, Min(0.05f)] private float attackInterval = 1.8f;
    [SerializeField, Min(0.1f)] private float beamLength = 12f;
    [SerializeField, Min(0.01f)] private float beamHitWidth = 0.4f;

    [Header("Visual")]
    [SerializeField] private Material beamMaterial;
    [SerializeField] private Color beamColor = new(0.35f, 0.9f, 1f, 0.9f);
    [SerializeField, Min(0.01f)] private float beamVisualWidth = 0.18f;
    [SerializeField, Min(0.01f)] private float beamVisualDuration = 0.12f;
    [SerializeField] private int sortingOrder = 110;

    private readonly List<Collider2D> overlapBuffer = new();
    private readonly HashSet<DamageReceiver> uniqueTargets = new();
    private LineRenderer beamRenderer;
    private float attackTimer;
    private float visualTimer;

    protected override void OnInitialized()
    {
        if (beamMaterial == null)
            throw new MissingReferenceException("PiercingBeamWeapon: Beam Material is not assigned.");

        beamRenderer = gameObject.AddComponent<LineRenderer>();
        beamRenderer.sharedMaterial = beamMaterial;
        beamRenderer.useWorldSpace = true;
        beamRenderer.positionCount = 2;
        beamRenderer.startWidth = beamVisualWidth;
        beamRenderer.endWidth = beamVisualWidth;
        beamRenderer.startColor = beamColor;
        beamRenderer.endColor = beamColor;
        beamRenderer.numCapVertices = 4;
        beamRenderer.sortingOrder = sortingOrder;
        beamRenderer.enabled = false;
        attackTimer = attackInterval;
    }

    public override void Tick(float deltaTime)
    {
        UpdateVisual(deltaTime);
        attackTimer = Mathf.Min(attackTimer + deltaTime, attackInterval);
        if (attackTimer < attackInterval)
            return;

        var enemy = FindClosestEnemy?.Invoke(Owner.position);
        if (enemy == null)
            return;

        attackTimer = 0f;
        Fire((enemy.transform.position - Owner.position).normalized);
    }

    private void Fire(Vector2 direction)
    {
        var origin = (Vector2)Owner.position;
        var end = origin + direction * beamLength;
        var center = (origin + end) * 0.5f;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        uniqueTargets.Clear();
        CombatDamage.ApplyBox(
            center,
            new Vector2(beamLength, beamHitWidth),
            angle,
            damage,
            OwnerTeam,
            overlapBuffer,
            uniqueTargets);

        beamRenderer.SetPosition(0, origin);
        beamRenderer.SetPosition(1, end);
        beamRenderer.enabled = true;
        visualTimer = beamVisualDuration;
    }

    private void UpdateVisual(float deltaTime)
    {
        if (beamRenderer == null || !beamRenderer.enabled)
            return;

        visualTimer -= deltaTime;
        if (visualTimer <= 0f)
            beamRenderer.enabled = false;
    }
}
