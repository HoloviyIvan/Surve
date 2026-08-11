using System.Collections.Generic;
using UnityEngine;

public sealed class DamageAreaWeapon : Weapon
{
    private sealed class ActiveArea
    {
        public Vector2 Position;
        public float RemainingLifetime;
        public float DamageTimer;
    }

    [SerializeField, Min(0f)] private float damage = 4f;
    [SerializeField, Min(0.05f)] private float spawnInterval = 3f;
    [SerializeField, Min(0.05f)] private float damageInterval = 0.5f;
    [SerializeField, Min(0f)] private float radius = 1.5f;
    [SerializeField, Min(0.1f)] private float duration = 4f;
    [SerializeField, Min(1)] private int maxActiveAreas = 3;

    private readonly List<ActiveArea> areas = new();
    private readonly List<Collider2D> overlapBuffer = new();
    private readonly HashSet<DamageReceiver> uniqueTargets = new();
    private float spawnTimer;

    protected override void OnInitialized() => spawnTimer = spawnInterval;

    public override void Tick(float deltaTime)
    {
        spawnTimer = Mathf.Min(spawnTimer + deltaTime, spawnInterval);
        if (spawnTimer >= spawnInterval && areas.Count < maxActiveAreas)
        {
            var enemy = FindClosestEnemy?.Invoke(Owner.position);
            if (enemy != null)
            {
                spawnTimer = 0f;
                areas.Add(new ActiveArea
                {
                    Position = enemy.transform.position,
                    RemainingLifetime = duration
                });
            }
        }

        for (var i = areas.Count - 1; i >= 0; i--)
        {
            var area = areas[i];
            area.RemainingLifetime -= deltaTime;
            area.DamageTimer += deltaTime;

            if (area.DamageTimer >= damageInterval)
            {
                area.DamageTimer -= damageInterval;
                uniqueTargets.Clear();
                CombatDamage.ApplyCircle(area.Position, radius, damage, OwnerTeam, overlapBuffer, uniqueTargets);
            }

            if (area.RemainingLifetime <= 0f)
                areas.RemoveAt(i);
        }
    }
}
