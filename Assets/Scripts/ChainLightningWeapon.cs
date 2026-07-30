using System.Collections.Generic;
using UnityEngine;

public sealed class ChainLightningWeapon : Weapon
{
    [SerializeField, Min(0f)] private float damage = 18f;
    [SerializeField, Min(0.05f)] private float attackInterval = 2f;
    [SerializeField, Min(1)] private int maxTargets = 4;
    [SerializeField, Min(0f)] private float searchRadius = 5f;
    [SerializeField, Range(0f, 1f)] private float damageMultiplierPerJump = 0.8f;

    private readonly List<Collider2D> overlapBuffer = new();
    private readonly HashSet<DamageReceiver> hitTargets = new();
    private readonly ContactFilter2D enemyFilter = new()
    {
        useLayerMask = true,
        layerMask = 1 << 7,
        useTriggers = true
    };
    private float timer;

    protected override void OnInitialized() => timer = attackInterval;

    public override void Tick(float deltaTime)
    {
        timer = Mathf.Min(timer + deltaTime, attackInterval);
        if (timer < attackInterval)
            return;

        var enemy = FindClosestEnemy?.Invoke(Owner.position);
        if (enemy == null)
            return;

        timer = 0f;
        hitTargets.Clear();
        var currentPosition = (Vector2)enemy.transform.position;
        var currentDamage = damage;

        for (var jump = 0; jump < maxTargets; jump++)
        {
            overlapBuffer.Clear();
            Physics2D.OverlapCircle(currentPosition, searchRadius, enemyFilter, overlapBuffer);

            DamageReceiver closest = null;
            var closestDistance = float.MaxValue;
            for (var i = 0; i < overlapBuffer.Count; i++)
            {
                if (!overlapBuffer[i].TryGetComponent<DamageReceiver>(out var candidate))
                    continue;
                if (candidate.Team == OwnerTeam || hitTargets.Contains(candidate))
                    continue;
                var distance = ((Vector2)candidate.transform.position - currentPosition).sqrMagnitude;
                if (distance >= closestDistance)
                    continue;
                closest = candidate;
                closestDistance = distance;
            }

            if (closest == null)
                break;

            closest.TakeDamage(currentDamage);
            hitTargets.Add(closest);
            currentPosition = closest.transform.position;
            currentDamage *= damageMultiplierPerJump;
        }
    }
}
